using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OrbitOne.BuildScreen.Configuration;
using OrbitOne.BuildScreen.Services;

namespace OrbitOne.BuildScreen.Controllers
{
    public class ConfigurationController : ApiController
    {

        // GET api/configuration
        public IEnumerable<ServiceConfig> Get()
        {
            List<ServiceConfig> configs = null;
            try
            {
                configs = ServiceConfiguration.GetListOfConfigurationsNoPassword();
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }
            return configs;
        }

        // GET api/configuration/5
        public ServiceConfig Get(int id)
        {
            return ServiceConfiguration.GetConfiguration(id);
        }

        // POST api/configuration
        public HttpResponseMessage Post([FromBody]ServiceConfig value)
        {
            if (!ModelState.IsValid) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            try
            {
                ServiceConfiguration.AddConfiguration(value);
            }
            catch (ArgumentException exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, exception.Message);
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        // PUT api/configuration/5
        public HttpResponseMessage Put(int id, [FromBody]ServiceConfig value)
        {
            if (!ModelState.IsValid) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            try
            {
                ServiceConfiguration.UpdateConfiguration(id, value);
            }
            catch (ArgumentException exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, exception.Message);
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }
            
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        // DELETE api/configuration/5
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                ServiceConfiguration.RemoveConfiguration(id);
            }
            catch (ArgumentException exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, exception.Message);
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                throw;
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }

}
