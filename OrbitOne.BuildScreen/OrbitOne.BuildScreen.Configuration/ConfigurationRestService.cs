using System.Configuration;

namespace OrbitOne.BuildScreen.Configuration
{
    public class ConfigurationRestService : ConfigurationSection, IConfigurationRestService
    {
        [ConfigurationProperty("RetrieveProjectsAsyncUrl", IsRequired = true)]
        public string RetrieveProjectsAsyncUrl { get { return this["RetrieveProjectsAsyncUrl"].ToString(); } }
        [ConfigurationProperty("RetrieveBuildsOnFinishtime", IsRequired = true)]
        public string RetrieveBuildsOnFinishtime { get { return this["RetrieveBuildsOnFinishtime"].ToString(); } }
        [ConfigurationProperty("RetrieveBuildsInProgress", IsRequired = true)]
        public string RetrieveBuildsInProgress { get { return this["RetrieveBuildsInProgress"].ToString(); } }
        [ConfigurationProperty("RetrieveLastBuildAsyncUrl", IsRequired = true)]
        public string RetrieveLastBuildAsyncUrl { get { return this["RetrieveLastBuildAsyncUrl"].ToString(); } }
        [ConfigurationProperty("RetrieveLastSuccessfulBuildUrl", IsRequired = true)]
        public string RetrieveLastSuccessfulBuildUrl { get { return this["RetrieveLastSuccessfulBuildUrl"].ToString(); } }
        [ConfigurationProperty("RetriveLastPartiallyOrFailedUrl", IsRequired = true)]
        public string RetriveLastPartiallyOrFailedUrl { get { return this["RetriveLastPartiallyOrFailedUrl"].ToString(); } }
        [ConfigurationProperty("RetrieveTestsAsyncUrl", IsRequired = true)]
        public string RetrieveTestsAsyncUrl { get { return this["RetrieveTestsAsyncUrl"].ToString(); } }
        [ConfigurationProperty("RetrieveBuildDefinitionsUrl", IsRequired = true)]
        public string RetrieveBuildDefinitionsUrl { get { return this["RetrieveBuildDefinitionsUrl"].ToString(); } }
        [ConfigurationProperty("RetrieveRunsAsyncUrl", IsRequired = true)]
        public string RetrieveRunsAsyncUrl { get { return this["RetrieveRunsAsyncUrl"].ToString(); } }
        [ConfigurationProperty("BuildDefinitionUri", IsRequired = true)]
        public string BuildDefinitionUri { get { return this["BuildDefinitionUri"].ToString(); } }
        [ConfigurationProperty("HourFormatRest", IsRequired = true)]
        public string HourFormatRest { get { return this["HourFormatRest"].ToString(); } }
        public static ConfigurationRestService Load()
        {
            return (ConfigurationRestService)ConfigurationManager.GetSection("ConfigurationRestService");
        }
    }

}
