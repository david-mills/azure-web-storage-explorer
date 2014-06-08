using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Web.Storage.Models
{
    public class PerfCounter
    {
        public string CounterName { get; set; }
        public string RoleInstance { get; set; }
        public string DeploymentId { get; set; }
        public string RoleName { get; set; }
    }
}
