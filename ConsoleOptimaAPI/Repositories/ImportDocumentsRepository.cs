using Dapper;
using OptimaAPI.DB;
using OptimaAPI.Interfaces;
using OptimaAPI.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.Versioning;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System;

namespace OptimaAPI.Repositories
{
    public partial class ImportDocumentsRepository : IImportDocumentsRepository
    {
        private readonly DapperContext _context;
        public ImportDocumentsRepository(DapperContext dapperContext)
        {
            _context = dapperContext;
        }

        [SupportedOSPlatform("windows")]
        public async Task<InputMessage> PostFA()
        {
            var obj = new
            {
                Id = 1,
                DateTime = DateTime.Now.ToString("s"),
                Text = "text testowy " + nameof(PipeLineAlias.HandlePostFARequest)

            };

            // Tworzymy obiekt OutputMessage do wysłania
            OutputMessage dataToSend = new()
            {
                Guid = Guid.NewGuid(),
                OperationType = OperationType.ReadOnly,
                MethodName = PipeLineAlias.HandlePostFARequest, // Zamieniamy wartość enum na string
                ObjectType = nameof(ImportDocumentsRepository),
                Json = JsonConvert.SerializeObject(obj)
            };

            return await InvokePipeLineStream(dataToSend, ".", nameof(PipeLineAlias.HandlePostFARequest));
        }
        [SupportedOSPlatform("windows")]
        public async Task<InputMessage> PostFZ()
        {
            var obj = new
            {
                Id = 1,
                DateTime = DateTime.Now.ToString("s"),
                Text = "text testowy " + nameof(PipeLineAlias.HandlePostFZRequest)
            };

            // Tworzymy obiekt OutputMessage do wysłania
            OutputMessage dataToSend = new ()
            {
                Guid = Guid.NewGuid(),
                OperationType = OperationType.ReadOnly,
                MethodName = PipeLineAlias.HandlePostFZRequest, // Zamieniamy wartość enum na string
                ObjectType = nameof(ImportDocumentsRepository),
                Json = JsonConvert.SerializeObject(obj)
            };

            return await InvokePipeLineStream(dataToSend, ".", nameof(PipeLineAlias.HandlePostFZRequest));
        }
        [SupportedOSPlatform("windows")]
        public async Task<InputMessage> PostPZ()
        {
            var obj = new
            {
                Id = 1,
                DateTime = DateTime.Now.ToString("s"),
                Text = "text testowy " + nameof(PipeLineAlias.HandlePostPZRequest)
            };

            // Tworzymy obiekt OutputMessage do wysłania
            OutputMessage dataToSend = new()
            {
                Guid = Guid.NewGuid(),
                OperationType = OperationType.ReadOnly,
                MethodName = PipeLineAlias.HandlePostPZRequest, // Zamieniamy wartość enum na string
                ObjectType = nameof(ImportDocumentsRepository),
                Json = JsonConvert.SerializeObject(obj)
            };

            return await InvokePipeLineStream(dataToSend, ".", nameof(PipeLineAlias.HandlePostPZRequest));
        }
        [SupportedOSPlatform("windows")]
        public async Task<InputMessage> PostWZ()
        {
            var obj = new
            {
                Id = 1,
                DateTime = DateTime.Now.ToString("s"),
                Text = "text testowy " + nameof(PipeLineAlias.HandlePostWZRequest)
            };

            // Tworzymy obiekt OutputMessage do wysłania
            OutputMessage dataToSend = new()
            {
                Guid = Guid.NewGuid(),
                OperationType = OperationType.ReadOnly,
                MethodName = PipeLineAlias.HandlePostWZRequest, // Zamieniamy wartość enum na string
                ObjectType = nameof(ImportDocumentsRepository),
                Json = JsonConvert.SerializeObject(obj)
            };

            return await InvokePipeLineStream(dataToSend, ".", nameof(PipeLineAlias.HandlePostWZRequest));
        }
        [SupportedOSPlatform("windows")]
        public async Task<InputMessage> PutWMSDiscrepancies()
        {
            var obj = new
            {
                Id = 1,
                DateTime = DateTime.Now.ToString("s"),
                Text = "text testowy " + nameof(PipeLineAlias.HandlePutWmsDiscrepancies)
            };

            // Tworzymy obiekt OutputMessage do wysłania
            OutputMessage dataToSend = new()
            {
                Guid = Guid.NewGuid(),
                OperationType = OperationType.ReadOnly,
                MethodName = PipeLineAlias.HandlePutWmsDiscrepancies, // Zamieniamy wartość enum na string
                ObjectType = nameof(ImportDocumentsRepository),
                Json = JsonConvert.SerializeObject(obj)
            };

            return await InvokePipeLineStream(dataToSend, ".", nameof(PipeLineAlias.HandlePutWmsDiscrepancies));
        }
        [SupportedOSPlatform("windows")]
        public async Task<InputMessage> PutWMSSignature()
        {
            var obj = new
            {
                Id = 1,
                DateTime = DateTime.Now.ToString("s"),
                Text = "text testowy " + nameof(PipeLineAlias.HandlePutWmsStatusSignature)
            };

            // Tworzymy obiekt OutputMessage do wysłania
            OutputMessage dataToSend = new()
            {
                Guid = Guid.NewGuid(),
                OperationType = OperationType.ReadOnly,
                MethodName = PipeLineAlias.HandlePutWmsStatusSignature, // Zamieniamy wartość enum na string
                ObjectType = nameof(ImportDocumentsRepository),
                Json = JsonConvert.SerializeObject(obj)
            };

            return await InvokePipeLineStream(dataToSend, ".", nameof(PipeLineAlias.HandlePutWmsStatusSignature));
        }
        [SupportedOSPlatform("windows")]
        public async Task<InputMessage> PutWMSStatus()
        {
            var obj = new
            {
                Id = 1,
                DateTime = DateTime.Now.ToString("s"),
                Text = "text testowy " + nameof(PipeLineAlias.HandlePutWmsStatusRequest)
            };

            // Tworzymy obiekt OutputMessage do wysłania
            OutputMessage dataToSend = new()
            {
                Guid = Guid.NewGuid(),
                OperationType = OperationType.ReadOnly,
                MethodName = PipeLineAlias.HandlePutWmsStatusRequest, // Zamieniamy wartość enum na string
                ObjectType = nameof(ImportDocumentsRepository),
                Json = JsonConvert.SerializeObject(obj)
            };

            return await InvokePipeLineStream(dataToSend, ".", nameof(PipeLineAlias.HandlePutWmsStatusRequest));
        }

        [SupportedOSPlatform("windows")]
        public async Task<InputMessage> TestingRunProcces()
        {
            var obj = new
            {
                Id = 1,
                DateTime = DateTime.Now.ToString("s"),
                Text = "text testowy " + nameof(PipeLineAlias.HandleTestingRunProccesRequest)
            };

            // Tworzymy obiekt OutputMessage do wysłania
            OutputMessage dataToSend = new()
            {
                Guid = Guid.NewGuid(),
                OperationType = OperationType.ReadOnly,
                MethodName = PipeLineAlias.HandleTestingRunProccesRequest, // Zamieniamy wartość enum na string
                ObjectType = nameof(ImportDocumentsRepository),
                Json = JsonConvert.SerializeObject(obj)
            };

            return await InvokePipeLineStream(dataToSend, ".", nameof(PipeLineAlias.HandleTestingRunProccesRequest));
        }

        [SupportedOSPlatform("windows")]
        public async Task<InputMessage> InvokePipeLineStream(OutputMessage outputMessage, string server, string name)
        {
            using NamedPipeClientStream pipeClient = new(server, name, PipeDirection.InOut);
            await pipeClient.ConnectAsync();

            // Serializujemy obiekt OutputMessage do ciągu bajtów
            byte[] dataBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(outputMessage));
            await pipeClient.WriteAsync(dataBytes);
            pipeClient.WaitForPipeDrain(); // Oczekujemy na opróżnienie bufora

            // Odczytujemy odpowiedź od aplikacji .NET Framework 4.8
            byte[] buffer = new byte[4096];
            int bytesRead = await pipeClient.ReadAsync(buffer);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            InputMessage input = new();
            try
            {
                input = JsonConvert.DeserializeObject<InputMessage>(response) ?? new InputMessage() { Message = "Nieoczekiwana reguła która zwraca pustą wartość", Guid = outputMessage.Guid };
            }
            catch (JsonException ex)
            {
                if (ex.InnerException != null)
                    input = new InputMessage() { Message = ex.Message, Guid = outputMessage.Guid };
                else if (ex.Message != null)
                {
                    input = new InputMessage() { Message = ex.Message, Guid = outputMessage.Guid };
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    input = new InputMessage() { Message = ex.Message, Guid = outputMessage.Guid };
                else if (ex.Message != null)
                {
                    input = new InputMessage() { Message = ex.Message, Guid = outputMessage.Guid };
                }
            }


            Console.WriteLine($"Response from .NET Framework 4.8 App: {response}");

            pipeClient.Close();
            return input ?? new InputMessage() { Message = "Nieoczekiwana reguła która zwraca pustą wartość", Guid = outputMessage.Guid };
        }


        public enum PipeLineAlias
        {
            HandlePostPZRequest = 1,
            HandlePostFZRequest = 2,
            HandlePostWZRequest = 3,
            HandlePostFARequest = 4,
            HandlePutWmsDiscrepancies = 5,
            HandlePutWmsStatusRequest = 6,
            HandlePutWmsStatusSignature = 7,
            HandleTestingRunProccesRequest = 0
        }

        public enum OperationType
        {
            ReadOnly = 0,
            Insert = 1,
            Update = 2,
            Delete = 3
        }


        public class OutputMessage
        {
            public Guid Guid { get; set; } = Guid.NewGuid();
            public string? ObjectType { get; set; }
            public PipeLineAlias MethodName { get; set; }
            //public string[] param { get; set; }
            public OperationType OperationType { get; set; }
            public string? Json { get; set; }
        }

    
            public class InputMessage
    {
        public string? Date { get; set; }
        public int? ResultCode { get; set; }
        public string? Message { get; set; }
        public string? InnerMessage { get; set; }
        public string? ResultJson { get; set; }
        public string? Methods { get; set; }
        public Guid? Guid { get; set; }

        public static Task<InputMessage> BadGuid(string guid, string Message)
        {
            return Task.FromResult(new InputMessage()
            {
                Date = DateTime.Now.ToString("s"),
                Message = Message,
                Methods = nameof(BadGuid)
            });
        }

        public static Task<InputMessage> ResultOk(string message, string v)
        {
            return Task.FromResult(
             new InputMessage()
             {
                 Date = DateTime.Now.ToString("s"),
                 Message = message,
                 Methods = nameof(ResultOk)
             });
        }
    }


    }
}
