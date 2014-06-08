using Microsoft.WindowsAzure.Diagnostics;

namespace Azure.Web.Storage.Models
{
    public class DeploymentInstance
    {
        public string Id { get; set; }
        public DiagnosticMonitorConfiguration Configuration { get; set; }
    }
}
