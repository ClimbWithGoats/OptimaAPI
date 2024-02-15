using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using static OptimaCom.ResourceOperations;

namespace OptimaCom
{

    public class DocumentOperations : OptimaOperations
    {
     

        public DocumentOperations(string json, Guid guid) : base(json, guid)
        {
         
        }
        public override void Run()
        {
            Console.WriteLine($"{nameof(DocumentOperations)} {this.Guid}");
            try
            {
                var documents = JsonConvert.DeserializeObject<List<Document>>(Json);
                OptimaController.OptimaWrapper.AddOrUpdateDocuments(documents);
            }
            catch (Exception ex)
            {
                var t = ex;
                throw;
            }
        }

    }
    public class ResourceOperations : OptimaOperations
    {
        public ResourceOperations(string json, Guid guid) : base(json, guid)
        {
        }

        public override void Run()
        {
            Console.WriteLine($"{nameof(ResourceOperations)} {this.Guid}");
            try
            {
                var commodities = JsonConvert.DeserializeObject<List<Commodity>>(Json);
                OptimaController.OptimaWrapper.AddOrUpdateResources(commodities);
            }
            catch (Exception ex)
            {
                var t = ex;
                throw;
            }
        }

    }
    public class ContractorsOperations : OptimaOperations
    {

        public ContractorsOperations(string json, Guid guid) : base(json, guid)
        {
        }

        public override void Run()
        {
            Console.WriteLine($"{nameof(ContractorsOperations)} {this.Guid}");
            try
            {
                var contractorsList = JsonConvert.DeserializeObject<List<Contractor>>(Json);
                OptimaController.OptimaWrapper.AddOrUpdateContractor(contractorsList);
            }
            catch (Exception ex)
            {
                var t = ex;
                throw;
            }
     
        }

    }
    public abstract class OptimaOperations
    {
        protected OptimaOperations(string json, Guid guid)
        {
            Json = json;
            Guid = guid;
        }

        public string Json { get; set; }
        public Guid Guid { get; set; }
        public abstract void Run();
    }
}