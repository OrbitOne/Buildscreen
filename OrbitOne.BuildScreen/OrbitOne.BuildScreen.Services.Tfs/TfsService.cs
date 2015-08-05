using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.TestManagement.Client;
using OrbitOne.BuildScreen.Models;

namespace OrbitOne.BuildScreen.Services.Tfs
{
    public class TfsService : IService
    {
        private readonly ITfsHelperClass _helperClass;

        public TfsService(ITfsHelperClass tfsHelperClass)
        {
           _helperClass = tfsHelperClass;
        }

        public List<BuildInfoDto> GetBuildInfoDtos()
        {
            var buildInfoDtos = new List<BuildInfoDto>();
            try
            {
                
                var tfsServer = _helperClass.GetTfsServer();
                // Get the catalog of team project collections
                var teamProjectCollectionNodes = tfsServer.CatalogNode.QueryChildren(
                    new[] { CatalogResourceTypes.ProjectCollection }, false, CatalogQueryOptions.None);
                Parallel.ForEach(teamProjectCollectionNodes, teamProjectCollectionNode =>
                {
                    lock (buildInfoDtos)
                    {
                        buildInfoDtos.AddRange(GetBuildInfoDtosPerTeamProject(teamProjectCollectionNode, tfsServer, DateTime.MinValue));
                    }
                });
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }

            return buildInfoDtos;
        }

        public List<BuildInfoDto> GetBuildInfoDtosPolling(String filterDate)
        {
            var buildInfoDtos = new List<BuildInfoDto>();
            try
            {
                var sinceDateTime = DateTime.Now.Subtract(new TimeSpan(int.Parse(filterDate), 0, 0));


                var tfsServer = _helperClass.GetTfsServer();

                // Get the catalog of team project collections
                var teamProjectCollectionNodes = tfsServer.CatalogNode.QueryChildren(
                    new[] { CatalogResourceTypes.ProjectCollection }, false, CatalogQueryOptions.None);

                Parallel.ForEach(teamProjectCollectionNodes, teamProjectCollectionNode =>
                {
                    lock (buildInfoDtos)
                    {
                        buildInfoDtos.AddRange(GetBuildInfoDtosPerTeamProject(teamProjectCollectionNode, tfsServer, sinceDateTime));
                    }
                });
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }

            return buildInfoDtos;
        }

        private IEnumerable<BuildInfoDto> GetBuildInfoDtosPerTeamProject(CatalogNode teamProjectCollectionNode,
            TfsConfigurationServer tfsServer, DateTime filterDate)
        {
            var buildInfoDtos = new List<BuildInfoDto>();
            try
            {
                // Use the InstanceId property to get the team project collection
                var collectionId = new Guid(teamProjectCollectionNode.Resource.Properties["InstanceId"]);
                var teamProjectCollection = tfsServer.GetTeamProjectCollection(collectionId);

                var buildServer = (IBuildServer)teamProjectCollection.GetService(typeof(IBuildServer));
                var testService = teamProjectCollection.GetService<ITestManagementService>();

                // Get a catalog of team projects for the collection
                var teamProjectNodes = teamProjectCollectionNode.QueryChildren(
                    new[] { CatalogResourceTypes.TeamProject },
                    false, CatalogQueryOptions.None);

                // List the team projects in the collection
                Parallel.ForEach(teamProjectNodes, teamProjectNode =>
                {
                    var buildDefinitionList =
                        new List<IBuildDefinition>(buildServer.QueryBuildDefinitions(teamProjectNode.Resource.DisplayName));
                    lock (buildInfoDtos)
                    {
                        buildInfoDtos.AddRange(GetBuildInfoDtosPerBuildDefinition(buildDefinitionList, buildServer,
                            teamProjectNode,
                            teamProjectCollection, testService, filterDate));
                    }
                });
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }


            return buildInfoDtos;
        }

        private IEnumerable<BuildInfoDto> GetBuildInfoDtosPerBuildDefinition(List<IBuildDefinition> buildDefinitionList,
            IBuildServer buildServer, CatalogNode teamProjectNode,
            TfsTeamProjectCollection teamProjectCollection, ITestManagementService testService, DateTime filterDate)
        {
            var buildDtos = new List<BuildInfoDto>();
            try
            {
                Parallel.ForEach(buildDefinitionList, def =>
                {
                    var build = GetBuild(buildServer, teamProjectNode, def, filterDate);

                    if (build == null) return;

                    var buildInfoDto = new BuildInfoDto
                    {
                        Builddefinition = def.Name,
                        FinishBuildDateTime = build.FinishTime,
                        LastBuildTime = new TimeSpan(),
                        PassedNumberOfTests = 0,
                        RequestedByName = build.RequestedFor,
                        RequestedByPictureUrl = "",
                        StartBuildDateTime = build.StartTime,
                        Status = Char.ToLowerInvariant(build.Status.ToString()[0]) + build.Status.ToString().Substring(1),
                        TeamProject = teamProjectNode.Resource.DisplayName,
                        TeamProjectCollection = teamProjectCollection.Name,
                        TotalNumberOfTests = 0,
                        Id = "TFS" + teamProjectNode.Resource.Identifier + def.Id,
                        BuildReportUrl = _helperClass.GetReportUrl(teamProjectCollection.Uri.ToString(), teamProjectNode.Resource.DisplayName, build.Uri.OriginalString)
                    };
                    //Retrieve testruns
                    var testResults = GetTestResults(teamProjectNode, testService, build);

                    if (testResults.ContainsKey("PassedTests"))
                    {
                        buildInfoDto.PassedNumberOfTests = testResults["PassedTests"];
                        buildInfoDto.TotalNumberOfTests = testResults["TotalTests"];
                    }
                    //Add last succeeded build if in progress
                    if (build.Status == BuildStatus.InProgress)
                    {
                        buildInfoDto.LastBuildTime = GetLastSuccesfulBuildTime(buildServer, teamProjectNode, def);
                    }
                    lock (buildDtos)
                    {
                        buildDtos.Add(buildInfoDto);
                    }
                });
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }

            return buildDtos;
        }
        private IBuildDetail GetBuild(IBuildServer buildServer, CatalogNode teamProjectNode, IBuildDefinition def, DateTime filterDate)
        {
            IBuildDetail build = null;
            try
            {
                var buildDetailSpec = buildServer.CreateBuildDetailSpec(teamProjectNode.Resource.DisplayName, def.Name);

                buildDetailSpec.MaxBuildsPerDefinition = 1;
                buildDetailSpec.QueryOrder = BuildQueryOrder.FinishTimeDescending;
                buildDetailSpec.MinFinishTime = filterDate;

                build = buildServer.QueryBuilds(buildDetailSpec).Builds.FirstOrDefault();
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }
            return build;
        }

        private TimeSpan GetLastSuccesfulBuildTime(IBuildServer buildServer, CatalogNode teamProjectNode,
            IBuildDefinition def)
        {
            var buildTime = new TimeSpan();
            try
            {
                var inProgressBuildDetailSpec = buildServer.CreateBuildDetailSpec(teamProjectNode.Resource.DisplayName, def.Name);

                inProgressBuildDetailSpec.Status = BuildStatus.Succeeded;
                inProgressBuildDetailSpec.MaxBuildsPerDefinition = 1;
                inProgressBuildDetailSpec.QueryOrder = BuildQueryOrder.FinishTimeDescending;

                var lastSuccesfulBuild = buildServer.QueryBuilds(inProgressBuildDetailSpec).Builds.FirstOrDefault();

                if (lastSuccesfulBuild != null)
                {
                    buildTime = lastSuccesfulBuild.FinishTime - lastSuccesfulBuild.StartTime;
                }
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }
            return buildTime;
        }

        private Dictionary<String, int> GetTestResults(CatalogNode teamProjectNode, ITestManagementService testService,
            IBuildDetail build)
        {
            var testResults = new Dictionary<string, int>();
            try
            {
                var testProject = testService.GetTeamProject(teamProjectNode.Resource.DisplayName);
                var testRun = testProject.TestRuns.ByBuild(build.Uri).FirstOrDefault();

                if (testRun != null)
                {
                    testResults.Add("PassedTests", testRun.PassedTests);
                    testResults.Add("TotalTests", testRun.TotalTests);
                }
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }
            return testResults;
        }
    }
}