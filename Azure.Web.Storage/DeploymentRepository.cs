using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Web.Storage.Configuration;
using Azure.Web.Storage.Contracts;
using Azure.Web.Storage.Models;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.Diagnostics.Management;

namespace Azure.Web.Storage
{
    public class DeploymentRepository : IDeploymentRepository
    {
        private readonly string _connectionString;
        private string[] _roles;

        public DeploymentRepository(string connectionString)
        {
            _connectionString = connectionString;
            _roles = Settings.AvailableRoles;
        }

        public Task<Deployment> GetDeployment(int id)
        {
            return Task.FromResult(GetDeployments().Result.First(t => t.Id == id));
        }

        public Task<IEnumerable<Deployment>> GetDeployments()
        {
            var deployments = new List<Deployment>();
            int id = 0;
            foreach (string role in _roles)
            {
                    deployments.Add(new Deployment
                    {
                        RoleName = role,
                        Id = id
                    });
                    id++;
            }

            return Task.FromResult(deployments.AsEnumerable());
        }

        public async Task<DeploymentDetail> GetDeploymentDetail(int id, string deploymentId)
        {
            var deployment = this.GetDeployments().Result.First(t => t.Id.Equals(id));
            var detail = new DeploymentDetail
            {
                Id = id,
                RoleName = deployment.RoleName,
                DeploymentId = deploymentId
            };
            var deploymentDiagnosticManager = new DeploymentDiagnosticManager(_connectionString, deploymentId);
            var instanceIds = deploymentDiagnosticManager.GetRoleInstanceIdsForRole(deployment.RoleName);
            var instances = new List<DeploymentInstance>();
            foreach (string instanceId in instanceIds)
            {

                var manager = deploymentDiagnosticManager.GetRoleInstanceDiagnosticManager(deployment.RoleName, instanceId);
                var config = manager.GetCurrentConfiguration();
                instances.Add(new DeploymentInstance
                {
                    Id = instanceId,
                    Configuration = config
                });

            }
            detail.Instances = instances;
            return detail;
        }


        public void UpdateAllInstanceDiagnostics(DiagnosticMonitorConfiguration config, string deploymentId, string roleName)
        {
            var deploymentDiagnosticManager = new DeploymentDiagnosticManager(this._connectionString, deploymentId);
            var instances = deploymentDiagnosticManager.GetRoleInstanceDiagnosticManagersForRole(roleName);
            foreach(var instance in instances)
            {
                instance.SetCurrentConfiguration(config);
            }
        }
        
        public void UpdateInstanceDiagnostics(DiagnosticMonitorConfiguration config, string deploymentId, string roleName, string instanceName)
        {
            var deploymentDiagnosticManager = new DeploymentDiagnosticManager(this._connectionString, deploymentId);
            var instance = deploymentDiagnosticManager.GetRoleInstanceDiagnosticManagersForRole(roleName)
                .FirstOrDefault(t => t.RoleInstanceId.Equals(instanceName, StringComparison.InvariantCultureIgnoreCase));
            if (instance != null)
            {
                instance.SetCurrentConfiguration(config);
            }
        }
    }
}
