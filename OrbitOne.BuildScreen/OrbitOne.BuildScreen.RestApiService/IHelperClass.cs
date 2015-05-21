using System;
using System.Threading.Tasks;

namespace OrbitOne.BuildScreen.RestApiService
{
    public interface IHelperClass
    {
        Task<T[]> RetrieveTask<T>(string formattedUrl);
        string ConvertReportUrl(string teamProjectName, string buildUri, Boolean summary);
    }
}