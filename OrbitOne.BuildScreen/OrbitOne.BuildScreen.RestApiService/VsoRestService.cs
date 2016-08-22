using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using OrbitOne.BuildScreen.Configuration;
using OrbitOne.BuildScreen.Models;
using OrbitOne.BuildScreen.Services;

namespace OrbitOne.BuildScreen.RestApiService
{
    public class VsoRestService : IService
    {
        private readonly IConfigurationRestService _configurationRestService;
        private readonly IHelperClass _helperClass;

        /* This contrains the amount of parallel tasks, because there is nested
         * parallelism, there is a need to constrain this number. Testing has pointed out
         * that 4 is the best solution */
        private const int DegreeOfParallelism = 1;

        public VsoRestService(IConfigurationRestService configurationRestService, IHelperClass helperClass)
        {
            _configurationRestService = configurationRestService;
            _helperClass = helperClass;
        }

        public List<BuildInfoDto> GetBuildInfoDtosPolling(String finishTimePoll)
        {
            var dtoPollList = new List<BuildInfoDto>();
            try
            {
                var sinceDateTime = DateTime.Now.Subtract(new TimeSpan(int.Parse(finishTimePoll), 0, 0)).ToUniversalTime();

                var teamProjects = _helperClass.RetrieveTask<TeamProject>(_configurationRestService.RetrieveProjectsAsyncUrl).Result;
                Parallel.ForEach(teamProjects, new ParallelOptions { MaxDegreeOfParallelism = DegreeOfParallelism }, teamProject =>
                {
                    var tempListOfBuildsPerTeamProject = GetBuildsForPollingSince(teamProject.Name, teamProject.Id, sinceDateTime).ToList();

                    tempListOfBuildsPerTeamProject = tempListOfBuildsPerTeamProject.GroupBy(b => b.Id)
                        .Select(b => b.OrderByDescending(d => d.StartBuildDateTime).FirstOrDefault())
                        .ToList();

                    if (!tempListOfBuildsPerTeamProject.Any()) return;

                    lock (dtoPollList)
                    {
                        dtoPollList.AddRange(tempListOfBuildsPerTeamProject);
                    }
                });
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }

            return dtoPollList;
        }

        private IEnumerable<BuildInfoDto> GetBuildsForPollingSince(string teamProjectName, string teamProjectId, DateTime finishTime)
        {
            List<BuildInfoDto> dtos = new List<BuildInfoDto>();
            try
            {
                var polledBuilds = GetPolledBuilds(teamProjectName, finishTime);
                Parallel.ForEach(polledBuilds, new ParallelOptions { MaxDegreeOfParallelism = DegreeOfParallelism }, build =>
                {
                    var buildInfoDto = new BuildInfoDto
                    {
                        TeamProject = teamProjectName,
                        Status = build.Result ?? build.Status,
                        Builddefinition = build.Definition.Name,
                        StartBuildDateTime = build.StartTime,
                        FinishBuildDateTime = build.FinishTime,
                        //RequestedByName = build.RequestedFor.DisplayName,
                        //RequestedByPictureUrl = build.RequestedFor.ImageUrl + "&size=2",
                        TotalNumberOfTests = 0,
                        PassedNumberOfTests = 0,
                        BuildReportUrl = _helperClass.ConvertReportUrl(teamProjectName, build.Uri, true),
                        Id = "VSO" + teamProjectId + build.Definition.Id
                    };

                    if (string.IsNullOrEmpty(buildInfoDto.RequestedByName))
                    {
                        buildInfoDto.RequestedByName = "No RequestedBy";
                    }
                    else if (buildInfoDto.RequestedByName.StartsWith("[DefaultCollection]"))
                    {
                        buildInfoDto.RequestedByName = "Service Account";
                    }

                    if (build.Status.Equals(Enum.GetName(typeof(StatusEnum.Statuses), StatusEnum.Statuses.inProgress)))
                    {
                        buildInfoDto.BuildReportUrl = _helperClass.ConvertReportUrl(teamProjectName, build.Uri, false);
                        var lastBuildTime = GetLastBuildTime(teamProjectName, build);
                        buildInfoDto.Status = StatusEnum.Statuses.inProgress.ToString();
                        if (lastBuildTime != null)
                        {
                            buildInfoDto.LastBuildTime = lastBuildTime.FinishTime - lastBuildTime.StartTime;
                        }
                    }
                    //if ( build.Result != null && build.Result.Equals(Enum.GetName(typeof(StatusEnum.Statuses), StatusEnum.Statuses.partiallySucceeded)))
                    if (build.Result != null)
                    {
                        var results = GetTestResults(teamProjectName, build.Uri);
                        
                        if (results != null)
                        {
                            buildInfoDto.TotalNumberOfTests = results.Count - results.Count(r => r.Outcome.ToLower().Equals("notexecuted"));
                            buildInfoDto.PassedNumberOfTests = results.Count(r => r.Outcome.ToLower().Equals("passed"));
                        }
                    }
                    lock (dtos)
                    {
                        dtos.Add(buildInfoDto);
                    }
                });
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }
            return dtos;
        }

        private IEnumerable<Build> GetPolledBuilds(string teamProjectName, DateTime finishTime)
        {
            var polledBuilds = new List<Build>();
            try
            {
                var onFinishTimeBuilds = _helperClass.RetrieveTask<Build>(String.Format(_configurationRestService.RetrieveBuildsOnFinishtime,
                teamProjectName, String.Format(_configurationRestService.HourFormatRest, finishTime.Year, finishTime.Month, finishTime.Day, finishTime.Hour, finishTime.Minute))).Result;
                var inProgressBuilds =
                    _helperClass.RetrieveTask<Build>(String.Format(_configurationRestService.RetrieveBuildsInProgress,
                        teamProjectName)).Result;
                polledBuilds.AddRange(onFinishTimeBuilds);
                polledBuilds.AddRange(inProgressBuilds);
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }
            return polledBuilds;
        }

        private Build GetLastBuildTime(string teamProjectName, Build build)
        {
            Build secondLastBuild = null;
            try
            {
                secondLastBuild = _helperClass.RetrieveTask<Build>(
                    String.Format(
                        _configurationRestService.RetrieveLastSuccessfulBuildUrl,
                        teamProjectName,
                        build.Definition.Id)
                        ).Result.FirstOrDefault()
                  ??
                  _helperClass.RetrieveTask<Build>(
                      String.Format(
                          _configurationRestService.RetriveLastPartiallyOrFailedUrl,
                          teamProjectName,
                          build.Definition.Id))
                      .Result.FirstOrDefault();
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }

            return secondLastBuild;
        }


        public List<BuildInfoDto> GetBuildInfoDtos()
        {
            var dtoList = new List<BuildInfoDto>();
            try
            {
                var teamProjects = _helperClass.RetrieveTask<TeamProject>(_configurationRestService.RetrieveProjectsAsyncUrl).Result;
                Parallel.ForEach(teamProjects, new ParallelOptions { MaxDegreeOfParallelism = DegreeOfParallelism }, teamProject =>
                {
                    lock (dtoList)
                    {
                        dtoList.AddRange(GetBuildDefinitions(teamProject.Name, teamProject.Id).ToList());
                    }
                });
                if (!dtoList.Any())
                {
                    throw new ObjectNotFoundException("VSO did not return any results.");
                }
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }

            return dtoList;
        }

        private IEnumerable<BuildInfoDto> GetBuildDefinitions(string teamProjectName, string teamProjectId)
        {
            var dtoList = new List<BuildInfoDto> { };
            try
            {
                var builddefinitions = _helperClass.RetrieveTask<BuildDefinition>(
                String.Format(_configurationRestService.RetrieveBuildDefinitionsUrl, teamProjectName))
                .Result
                .Where(b => b.QueueStatus == null || !b.QueueStatus.Equals("disabled")) //it only returns a status when it's disabled (not tested for paused yet)
                .ToList();

                Parallel.ForEach(builddefinitions, new ParallelOptions { MaxDegreeOfParallelism = DegreeOfParallelism }, bd =>
                {
                    var dto = GetLatestBuild(teamProjectName, bd.Id, bd.Uri, bd.Name, teamProjectId);
                    if (dto != null)
                    {
                        lock (dtoList)
                        {
                            dtoList.Add(dto);
                        }
                    }
                });
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }


            return dtoList;
        }
        private BuildInfoDto GetLatestBuild(string teamProjectName, string bdId, string bdUri, string bdName, string teamProjectId)
        {
            BuildInfoDto buildInfoDto = null;
            Build latestBuild = null;
            try
            {
                 latestBuild =
                    _helperClass
                        .RetrieveTask<Build>(
                        (String.Format(_configurationRestService.RetrieveLastBuildAsyncUrl, teamProjectName, bdId)))
                        .Result
                        .FirstOrDefault();
                if (latestBuild == null) return null;
                buildInfoDto = new BuildInfoDto
                {
                    TeamProject = teamProjectName,
                    Status = latestBuild.Result ?? latestBuild.Status,
                    Builddefinition = bdName,
                    StartBuildDateTime = latestBuild.StartTime,
                    FinishBuildDateTime = latestBuild.FinishTime,
                    //RequestedByName = latestBuild.RequestedFor.DisplayName,
                    //RequestedByPictureUrl = latestBuild.RequestedFor.ImageUrl + "&size=2",
                    TotalNumberOfTests = 0,
                    PassedNumberOfTests = 0,
                    BuildReportUrl = _helperClass.ConvertReportUrl(teamProjectName, latestBuild.Uri, true),
                    Id = "VSO" + teamProjectId + bdId
                };

                if (string.IsNullOrEmpty(buildInfoDto.RequestedByName))
                {
                    buildInfoDto.RequestedByName = "No RequestedBy";
                }
                else if (buildInfoDto.RequestedByName.StartsWith("[DefaultCollection]"))
                {
                    buildInfoDto.RequestedByName = "Service Account";
                }

                if (latestBuild.Status.Equals(Enum.GetName(typeof(StatusEnum.Statuses), StatusEnum.Statuses.inProgress)))
                {
                    buildInfoDto.BuildReportUrl = _helperClass.ConvertReportUrl(teamProjectName, latestBuild.Uri, false);
                    buildInfoDto.Status = StatusEnum.Statuses.inProgress.ToString();
                    var secondLastBuildList =
                         _helperClass.RetrieveTask<Build>(
                        (String.Format(_configurationRestService.RetrieveLastSuccessfulBuildUrl, teamProjectName, bdId))).Result;

                    var secondLastBuild = secondLastBuildList.FirstOrDefault();
                    if (secondLastBuild != null)
                    {
                        buildInfoDto.LastBuildTime = secondLastBuild.FinishTime - secondLastBuild.StartTime;
                    }

                }
                //if (latestBuild.Result != null &&
                //    latestBuild.Result.Equals(Enum.GetName(typeof(StatusEnum.Statuses), StatusEnum.Statuses.partiallySucceeded)))
                if (latestBuild.Result != null)
                {
                    var results = GetTestResults(teamProjectName, latestBuild.Uri);
                    if (results != null)
                    {
                        buildInfoDto.TotalNumberOfTests = results.Count - results.Count(r => r.Outcome.ToLower().Equals("notexecuted"));
                        buildInfoDto.PassedNumberOfTests = results.Count(r => r.Outcome.ToLower().Equals("passed"));
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(buildInfoDto);
                Debug.WriteLine(latestBuild);

                LogService.WriteError(e);
                throw;
            }

            return buildInfoDto;
        }

        private IReadOnlyCollection<TestResult> GetTestResults(string teamProjectName, string buildUri)
        {
            TestResult[] result = { };
            try
            {
                var runs =
                _helperClass.RetrieveTask<Test>(String.Format(_configurationRestService.RetrieveRunsAsyncUrl, teamProjectName, buildUri))
                .Result;
                if (!runs.Any()) return null;
                var runResult = runs.Max(t => t.Id);
                result = _helperClass.RetrieveTask<TestResult>(String.Format(_configurationRestService.RetrieveTestsAsyncUrl,
                    teamProjectName, runResult)).Result;
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }
            return result;
        }
    }
}