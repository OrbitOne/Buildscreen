using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.TeamFoundation.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrbitOne.BuildScreen.Services;

namespace OrbitOne.BuildScreen.Configuration
{
    public static class ServiceConfiguration
    {
        private static readonly string FilePath = HttpContext.Current.Server.MapPath("~/App_Data/Configuration.json");
        private static readonly string FolderPath = HttpContext.Current.Server.MapPath("~/App_Data/");
        private const string ReturnPassword = "**********";

        public static JArray GetJsonConfigurations()
        {
            JArray json = new JArray();
            try
            {
                if (!File.Exists(FilePath))
                {
                    CreateNewConfiguration();
                }
                json = JArray.Parse(File.ReadAllText(FilePath));
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }
            return json;
        }
        public static void CreateNewConfiguration()
        {
            try
            {
                if (!Directory.Exists(FolderPath))
                {
                    Directory.CreateDirectory(FolderPath);
                }
                WriteToFile(new List<ServiceConfig>());
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }
            
        }
        public static void UpdateConfiguration(int id, ServiceConfig ic)
        {
            var list = GetListOfConfigurationsInternal();
            var configuration = list.FirstOrDefault(c => c.Id == id);

            if (configuration == null)
                throw new ArgumentException("Configuration with Id \"" + ic.Id + "\" can't be found.");
            if (string.IsNullOrEmpty(ic.Password) || ic.Password.Equals(ReturnPassword))
            { 
                var password = configuration.Password;
                ic.Password = password;
            }

            var errorMessage = CheckValidityOfConfig(ic);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new ArgumentException(errorMessage);
            }
            list[list.IndexOf(configuration)] = ic;
            WriteToFile(list);
            ConfigurationListChanged();
        }

        public static ServiceConfig GetConfiguration(int id)
        {

            ServiceConfig configs = null;
            try
            {
                configs = GetListOfConfigurationsInternal().FirstOrDefault(c => c.Id == id);
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            } 
            return configs;
        }

        public static void AddConfiguration(ServiceConfig ic)
        {
            try
            {
                ic.Password = AesDecryptor.DecryptStringAES(ic.Password);

                var list = GetListOfConfigurationsInternal();
                if (list.FirstOrDefault(c => String.Equals(c.Uri, ic.Uri, StringComparison.CurrentCultureIgnoreCase)) != null)
                    throw new ArgumentException("Configuration with Uri \"" + ic.Uri + "\" Already exists.");
                ic.Id = list.Any() ? (list.Max(c => c.Id) + 1) : (0);
                var errorMessage = CheckValidityOfConfig(ic);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    throw new ArgumentException(errorMessage);
                }
                list.Add(ic);
                WriteToFile(list);
                ConfigurationListChanged();
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }

        }

        public static void RemoveConfiguration(int id)
        {
            try
            {
                var list = GetListOfConfigurationsInternal();
                var configuration = list.FirstOrDefault(c => c.Id == id);
                if (configuration == null)
                    throw new ArgumentException("Configuration with Id \"" + id + "\" can't be found.");
                list.Remove(configuration);
                WriteToFile(list);
                ConfigurationListChanged();
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }
        }
        public static void WriteToFile(List<ServiceConfig> c)
        {
            try
            {
                var json = JsonConvert.SerializeObject(c, Formatting.Indented);

                File.WriteAllText(FilePath, json);
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }

        }

        public static List<ServiceConfig> GetListOfConfigurationsInternal()
        {
            List<ServiceConfig> configs = null;
            try
            {
                configs = GetJsonConfigurations().ToObject<List<ServiceConfig>>();
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }

            return configs;
        }
        public static List<ServiceConfig> GetListOfConfigurationsNoPassword()
        {
            List<ServiceConfig> configs = null;
            try
            {
                configs = GetListOfConfigurationsInternal().Select(c =>{c.Password = ReturnPassword; return c;}).ToList();
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }
            return configs;
        }

        public static List<ServiceConfig> GetSpecificConfigurations(string service)
        {
            List<ServiceConfig> configs = null;
            try
            {
                configs = GetListOfConfigurationsInternal().Where(c => c.ServiceType == service).ToList();
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }
            return configs;
        }

        public static event EventHandler ConfigurationListChangedHandler;
        public static void ConfigurationListChanged()
        {
            try
            {
                var handler = ConfigurationListChangedHandler;
                if (handler != null)
                {
                    handler(null, null);
                }
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }

        }
        private static string CheckValidityOfConfig(IServiceConfig config)
        {
            try
            {
                Uri tempValue;
                if (!Uri.TryCreate(config.Uri, UriKind.Absolute, out tempValue))
                {
                    return "Uri is has a wrong format.";
                }
                if (!config.Uri.ToLower().Contains(ConfigurationManager.AppSettings[config.ServiceType + "Regex"]))
                {
                    return "Url did not contain a valid domain or service";
                }
                if (!CanLogIn(config.Username, config.Password, config.Uri, config.ServiceType).Result)
                {
                    return "Could not authenticate, username and/or password are wrong.";
                }
            }
            catch (AggregateException e)
            {
                return "DNS could not find server adress";
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }
            return null;
        }
        private static async Task<bool> CanLogIn(string username, string password, string uri, string configType)
        {
            bool response = false;
            try
            {
                // VSO check
                if (configType == "VSO")
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(uri);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                            Convert.ToBase64String(
                                Encoding.ASCII.GetBytes(string.Format("{0}:{1}", username, password))));
                        Debug.WriteLine("Code: " + client.GetAsync("").Result.StatusCode);
                        response = client.GetAsync("").Result.StatusCode == HttpStatusCode.OK;
                    }
                }
                // TFS check
                else if (configType == "TFS")
                {
                    var credentials = new TfsClientCredentials(new WindowsCredential(new NetworkCredential(username, password)));
                    var server = new TfsConfigurationServer(new Uri(uri), credentials);
                    server.EnsureAuthenticated();
                    response = server.HasAuthenticated;
                }
            }
            catch (Exception e)
            { 
                LogService.WriteError(e);
                throw;
            }
            return response;
        }
    }

    public interface IServiceConfig
    {
        int Id { get; set; }
        string Username { get; set; }
        string Name { get; set; }
        string Password { get; set; }
        string Uri { get; set; }
        string ServiceType { get; set; }
    }

    public class ServiceConfig : IServiceConfig
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public string Password { get; set; }
        [Required(ErrorMessage = "URI is required")]
        public string Uri { get; set; }
        [Required(ErrorMessage = "Servicetype is required")]
        public string ServiceType { get; set; }
    }

}
