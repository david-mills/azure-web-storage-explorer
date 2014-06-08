using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Azure.Web.Storage.Configuration;
using Azure.Web.Storage.Contracts;
using Azure.Web.Storage.Models;
using Azure.Web.Storage;
using System.Collections.Generic;
using System.Web.Http;

namespace Azure.Web.Storage.Website.Controllers
{
    public class DeploymentsController : ApiController
    {
        private readonly IDeploymentRepository repository;

        public DeploymentsController()
        {
            var config = Settings.StorageConfiguration;
            this.repository = new DeploymentRepository(config);
        }
        
        public async Task<IEnumerable<Deployment>> Get()
        {
            return await this.repository.GetDeployments();
        }

        public async Task<DeploymentDetail> Get(int id, string deployId)
        {
            return await repository.GetDeploymentDetail(id, deployId);
        }

        [HttpPut]
        public async Task<HttpResponseMessage> PutAll(int id, string deployId, [FromBody]DeploymentInstance detail)
        {
            var deployment = await this.repository.GetDeployment(id);
            repository.UpdateAllInstanceDiagnostics(detail.Configuration, deployId, deployment.RoleName);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPut]
        public async Task<HttpResponseMessage> Put(int id, string deployId, [FromBody]DeploymentInstance detail)
        {
            var deployment = await this.repository.GetDeployment(id);
            repository.UpdateInstanceDiagnostics(detail.Configuration, deployId, deployment.RoleName, detail.Id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}