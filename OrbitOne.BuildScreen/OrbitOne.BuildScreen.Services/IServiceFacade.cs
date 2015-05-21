using System.Collections.Generic;
using OrbitOne.BuildScreen.Models;

namespace OrbitOne.BuildScreen.Services
{
    public interface IServiceFacade
    {
        List<BuildInfoDto> GetBuilds(string dateString = null);
    }
}
