namespace OrbitOne.BuildScreen.Configuration
{
    public interface IConfigurationRestService
    {
        string RetrieveProjectsAsyncUrl { get; }
        string RetrieveBuildsOnFinishtime { get; }
        string RetrieveBuildsInProgress { get; }
        string RetrieveLastBuildAsyncUrl { get; }
        string RetrieveLastSuccessfulBuildUrl { get; }
        string RetriveLastPartiallyOrFailedUrl { get; }
        string RetrieveTestsAsyncUrl { get; }
        string RetrieveBuildDefinitionsUrl { get; }
        string RetrieveRunsAsyncUrl { get; }
        string BuildDefinitionUri { get; }
        string HourFormatRest { get; }
    }
}
