using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Diagnostics.Models;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.Diagnostics.Management;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Diagnostics.Management;

namespace Azure.Diagnostics.Contracts
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
