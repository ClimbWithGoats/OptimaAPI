using OptimaCom.Controller;
using System;
using System.Threading.Tasks;

namespace OptimaCom
{
    public static class OptimaController
    {

        public static OptimaWrapper OptimaWrapper = new OptimaWrapper(true);
        public static Task<OutputMessage> HandleTestingRunProccesRequest(PipeLineAlias Name, string Json, Guid Guid)
        {
            OptimaWrapper.LoginToOptima();

            OutputMessage outputMessage = new OutputMessage()
            {
                GuidResult = Guid,
                Result = "WykonanieJson w HandleTestingRunProccesRequest",
                Errors = "Inne błędy"

            };
            Console.WriteLine($"METODA {nameof(HandleTestingRunProccesRequest)}");
            return Task.FromResult(outputMessage);
        }
        public static Task<OutputMessage> HandlePostPZRequest(PipeLineAlias Name, string Json, Guid Guid)
        {

            OutputMessage outputMessage = new OutputMessage()
            {
                GuidResult = Guid,
                Result = "WykonanieJson w HandlePostPZRequest",
                Errors = "Inne błędy"
            };
            Console.WriteLine($"METODA {nameof(HandlePostPZRequest)}");
            return Task.FromResult(outputMessage);
        }
        public static Task<OutputMessage> HandlePostWZRequest(PipeLineAlias Name, string Json, Guid Guid)
        {
            OutputMessage outputMessage = new OutputMessage()
            {
                GuidResult = Guid,
                Result = "WykonanieJson w HandlePostWZRequest",
                Errors = "Inne błędy"
            };
            Console.WriteLine($"METODA {nameof(HandlePostWZRequest)}");
            return Task.FromResult(outputMessage);
        }
        public static Task<OutputMessage> HandlePostFZRequest(PipeLineAlias Name, string Json, Guid Guid)
        {

            PipeServersController.FIFO.Add(new DocumentOperations(Json, Guid));

            OutputMessage outputMessage = new OutputMessage()
            {
                GuidResult = Guid,
                Result = "WykonanieJson w HandlePostFZRequest",
                Errors = "Inne błędy"
            };
            Console.WriteLine($"METODA {nameof(HandlePostFZRequest)}");
            return Task.FromResult(outputMessage);
        }
        public static Task<OutputMessage> HandlePostFARequest(PipeLineAlias Name, string Json, Guid Guid)
        {
            PipeServersController.FIFO.Add(new DocumentOperations(Json, Guid));

            OutputMessage outputMessage = new OutputMessage()
            {
                GuidResult = Guid,
                Result = "WykonanieJson w HandlePostFARequest",
                Errors = "Inne błędy"
            };
            Console.WriteLine($"METODA {nameof(HandlePostFARequest)}");
            return Task.FromResult(outputMessage);
        }
        public static Task<OutputMessage> HandlePutWmsDiscrepancies(PipeLineAlias Name, string Json, Guid Guid)
        {

            OutputMessage outputMessage = new OutputMessage()
            {
                GuidResult = Guid,
                Result = "WykonanieJson w HandlePutWmsDiscrepancies",
                Errors = "Inne błędy"
            };
            Console.WriteLine($"METODA {nameof(HandlePutWmsDiscrepancies)}");
            return Task.FromResult(outputMessage);
        }
        public static Task<OutputMessage> HandlePutWmsStatusRequest(PipeLineAlias Name, string Json, Guid Guid)
        {

            OutputMessage outputMessage = new OutputMessage()
            {
                GuidResult = Guid,
                Result = "WykonanieJson w HandlePutWmsStatusRequest",
                Errors = "Inne błędy"
            };
            Console.WriteLine($"METODA {nameof(HandlePutWmsStatusRequest)}");

            return Task.FromResult(outputMessage);
        }
        public static Task<OutputMessage> HandlePutWmsStatusSignature(PipeLineAlias Name, string Json, Guid Guid)
        {
            OutputMessage outputMessage = new OutputMessage()
            {
                GuidResult = Guid,
                Result = "WykonanieJson w HandlePutWmsStatusSignature",
                Errors = "Inne błędy"
            };
            Console.WriteLine($"METODA {nameof(HandlePutWmsStatusSignature)}");

            return Task.FromResult(outputMessage);
        }
        public static Task<OutputMessage> HandleAddOrUpdateContractors(PipeLineAlias Name, string Json, Guid Guid)
        {
            PipeServersController.FIFO.Add(new ContractorsOperations(Json, Guid));

            OutputMessage outputMessage = new OutputMessage()
            {
                GuidResult = Guid,
                Result = "WykonanieJson w HandleAddOrUpdateContractors",
                Errors = "Inne błędy"
            };
            Console.WriteLine($"METODA {nameof(HandleAddOrUpdateContractors)}");

            return Task.FromResult(outputMessage);
        }
        public static Task<OutputMessage> HandleAddOrUpdateDocuments(PipeLineAlias Name, string Json, Guid Guid)
        {
            PipeServersController.FIFO.Add(new DocumentOperations(Json, Guid));


            OutputMessage outputMessage = new OutputMessage()
            {
                GuidResult = Guid,
                Result = "WykonanieJson w HandleAddOrUpdateDocuments",
                Errors = "Inne błędy"
            };
            Console.WriteLine($"METODA {nameof(HandleAddOrUpdateDocuments)}");

            return Task.FromResult(outputMessage);
        }
        public static Task<OutputMessage> HandleAddOrUpdateResource(PipeLineAlias Name, string Json, Guid Guid)
        {
            PipeServersController.FIFO.Add(new ResourceOperations(Json, Guid));

            OutputMessage outputMessage = new OutputMessage()
            {
                GuidResult = Guid,
                Result = "WykonanieJson w HandleAddOrUpdateResource",
                Errors = "Inne błędy"
            };
            Console.WriteLine($"METODA {nameof(HandleAddOrUpdateResource)}");
            return Task.FromResult(outputMessage);
        }
    }
}
