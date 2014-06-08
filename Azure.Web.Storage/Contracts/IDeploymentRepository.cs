using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Web.Storage.Models;
using Microsoft.WindowsAzure.Diagnostics;

namespace Azure.Web.Storage.Contracts
{
    public interface IDeploymentRepository
    {
        Task<Deployment> GetDeployment(int id);

        Task<IEnumerable<Deployment>> GetDeployments();

        Task<DeploymentDetail> GetDeploymentDetail(int id, string deploymentId);

        void UpdateAllInstanceDiagnostics(DiagnosticMonitorConfiguration config, string deploymentId, string roleName);

        void UpdateInstanceDiagnostics(DiagnosticMonitorConfiguration config, string deploymentId, string roleName,
            string instanceName);
    }
}
