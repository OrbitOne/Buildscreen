using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbitOne.BuildScreen.Models
{
    public class TeamProject
    {
        public List<BuildDefinition> BuildDefinitions { get; set; }
        #region Properties
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        #endregion

    }
}