/*using System;
using System.Collections.Generic;
using System.Linq;
using OrbitOne.BuildScreen.Models;


namespace OrbitOne.BuildScreen.Converters
{
    public class VsoConverter : IConverter<TeamProject>
    {
        public List<BuildInfoDto> Convert(TeamProject tp)
        {
            var buildInfoDtos = new List<BuildInfoDto>();
            foreach (var bd in tp.BuildDefinitions.Where(bd => bd.Buildlast != null))
            {
                var lastBuildTime = new TimeSpan();
                var latestBuild = bd.Buildlast;
                var totalNumberOfTests = 0;
                var passedNumberOfTests = 0;

                if (latestBuild != null && latestBuild.Status.ToLower().Equals("partiallysucceeded"))
                {
                    totalNumberOfTests = latestBuild.TestResults.Count();
                    passedNumberOfTests = latestBuild.TestResults.Count(t => t.Outcome.ToLower().Equals("passed"));
                }

                if (bd.BuildSecondLastSuccesful != null && bd.BuildSecondLastSuccesful.Status.Equals("succeeded"))
                {
                    lastBuildTime = (bd.BuildSecondLastSuccesful.FinishTime -
                                             bd.BuildSecondLastSuccesful.StartTime);
                }

               
                    var buildInfoDto = new BuildInfoDto
                    {
                        TeamProject = tp.Name,
                        Status = latestBuild.Status,
                        Builddefinition = bd.Name,
                        StartBuildDateTime = latestBuild.StartTime,
                        FinishBuildDateTime = latestBuild.FinishTime,
                        RequestedByName = latestBuild.Requests.First().RequestedFor.DisplayName,
                        RequestedByPictureUrl = latestBuild.Requests.First().RequestedFor.ImageUrl,
                        LastBuildTime = lastBuildTime,
                        TotalNumberOfTests = totalNumberOfTests,
                        PassedNumberOfTests = passedNumberOfTests
                    };
                    buildInfoDtos.Add(buildInfoDto);
              
            }
            return buildInfoDtos; //leeg object megeven
        }
    }
}*/