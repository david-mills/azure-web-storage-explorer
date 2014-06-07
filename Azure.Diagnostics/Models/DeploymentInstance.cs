

using Microsoft.WindowsAzure.Diagnostics;

namespace Azure.Diagnostics.Models
{
    public class DeploymentInstance
    {
        public string Id { get; set; }
        public DiagnosticMonitorConfiguration Configuration { get; set; }
    }
}
