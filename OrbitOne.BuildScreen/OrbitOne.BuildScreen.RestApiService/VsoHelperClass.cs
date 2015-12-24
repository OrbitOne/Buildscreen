using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using OrbitOne.BuildScreen.Configuration;
using OrbitOne.BuildScreen.Models;
using OrbitOne.BuildScreen.Services;

namespace OrbitOne.BuildScreen.RestApiService
{

    public class VsoHelperClass : IHelperClass
    {
        private readonly IServiceConfig _config;
        private readonly IConfigurationRestService _configurationRestService;

        /* URL-part for a summary on Visual Studio Online, used when buildStatus is not inProgress */
        private const string SummaryString = "/_build#_a=summary&buildId=";

        /* URL-part for a log on Visual Studio Online, used when buildStatus is inProgress */
        private const string LogString = "/_build#_a=log&buildUri=";

        public VsoHelperClass(IServiceConfig config, IConfigurationRestService configurationRestService)
        {
            _config = config;
            _configurationRestService = configurationRestService;
        }

        public async Task<T[]> RetrieveTask<T>(string formattedUrl)
        {
            var objects = new T[] { };

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_config.Uri);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            Encoding.ASCII.GetBytes(string.Format("{0}:{1}", _config.Username,
                                _config.Password))));

                    client.Timeout = new TimeSpan(0, 3, 0);

                    var response = await client.GetAsync(HttpUtility.UrlPathEncode(formattedUrl)).ConfigureAwait(continueOnCapturedContext: false);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonstr =
                            await response.Content.ReadAsStringAsync();
                        objects =
                            JsonConvert.DeserializeObject<JsonWrapper<T>>(jsonstr).Value;
                    } 
                }
            }
            catch (AggregateException e)
            {
                e.Handle((x) =>
                {
                    LogService.WriteError(x.InnerException);
                    return true;
                });
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
            }
            return objects;
        }

        public string ConvertReportUrl(string teamProjectName, string buildUri, Boolean summary)
        {
            if (string.IsNullOrEmpty(buildUri))
            {
                return _config.Uri + "/DefaultCollection/" + teamProjectName + "/_build";
            }

            var urlpart = (summary) ? SummaryString : LogString;

            var firstPart = _config.Uri + "/DefaultCollection/" + teamProjectName + urlpart;
            
            var lastIndexOf = buildUri.LastIndexOf("/");
            var number = "";
           
            try
            {
                if (lastIndexOf != -1)
                {
                    var temp = buildUri.Substring(lastIndexOf, buildUri.Length - lastIndexOf);
                    number = temp.Replace("/", "");
                }
            }
            catch (Exception ex)
            {
                LogService.WriteError(ex);
            }
           

            return firstPart + number;
        }
    }
}

