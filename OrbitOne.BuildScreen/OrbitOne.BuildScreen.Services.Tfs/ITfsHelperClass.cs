using Microsoft.TeamFoundation.Client;

namespace OrbitOne.BuildScreen.Services.Tfs
{
    public interface ITfsHelperClass
    {
        TfsConfigurationServer GetTfsServer();
        string GetReportUrl(string tpc, string tp, string buildUri);
    }
}