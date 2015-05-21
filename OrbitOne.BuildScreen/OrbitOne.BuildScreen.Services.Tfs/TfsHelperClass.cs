using System;
using System.Net;
using Microsoft.TeamFoundation.Client;
using OrbitOne.BuildScreen.Configuration;

namespace OrbitOne.BuildScreen.Services.Tfs
{
    public class TfsHelperClass : ITfsHelperClass
    {
        private readonly IServiceConfig _configurationTfsService;

        /* URL-part for a summary on Team Foundation Server */
        private const string SummaryString = "/_build#_a=summary&buildUri=";
        public TfsHelperClass(IServiceConfig configurationTfsService)
        {
            _configurationTfsService = configurationTfsService;
        }
        public TfsConfigurationServer GetTfsServer()
        {
            TfsConfigurationServer server = null;
            try
            {
                var tfsUri = new Uri(_configurationTfsService.Uri);
                var credentials = new TfsClientCredentials(new WindowsCredential(new NetworkCredential(_configurationTfsService.Username, _configurationTfsService.Password)));
                server = new TfsConfigurationServer(tfsUri, credentials);
                server.EnsureAuthenticated();

            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }
            return server;
        }
        public string GetReportUrl(string tpc, string tp, string buildUri)
        {
            var basic = "";
            if (tpc.Contains("https"))
            {
                basic = tpc;
            }
            else
            {
                basic = "https://" + tpc.Replace("\\", "/tfs/");
            }
            
            return basic + "/" + tp + SummaryString + buildUri;
        }
    }
}
