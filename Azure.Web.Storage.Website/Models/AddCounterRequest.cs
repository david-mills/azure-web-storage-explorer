using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Azure.Web.Storage.Website.Models
{
    public class AddCounterRequest
    {
        public IEnumerable<string> Counters { get; set; }
        public int Id { get; set; }
        public string DeploymentId { get; set; }
    }
}