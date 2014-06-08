

using System.Collections.Generic;

namespace Azure.Web.Storage.Models
{
    public class DeploymentDetail
    {
        public int Id { get; set; }
        public string DeploymentId { get; set; }
        public string RoleName { get; set; }
        public IEnumerable<DeploymentInstance> Instances { get; set; }
        
    }
}
