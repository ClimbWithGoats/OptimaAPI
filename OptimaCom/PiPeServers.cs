using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OptimaCom
{
    public class PiPeServers
    {
        public PiPeServers(PipeLineAlias parsedPipe, Guid parsedGuid)
        {
            Name = parsedPipe;
            guid = parsedGuid;
            //  RunOptimaMethod();
        }
        public PipeLineAlias Name { get; set; }
        public Guid guid { get; set; }
        public bool IsInitialized { get; set; } = false;
        ulong count = 0;
        public async void RunOptimaMethod()
        {
            if (guid == Guid.Empty)
            {
                Debug.Write(" pustych:" + count);
                return;
            }


            using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(Name.ToString() + "_" + guid, PipeDirection.InOut))
            {
                IsInitialized = true;
                Console.WriteLine($"Utworzono potok parsedPipe: {Name} z guid {guid}");

                try
                {
                    Console.WriteLine("Named Pipe Server {0} is waiting for connection...", Name.ToString());
                    await pipeServer.WaitForConnectionAsync();
                    Console.WriteLine("Named Pipe Server connected. {0}", Name.ToString() + "_" + guid);
                    
                    // Odczytaj dane z klienta (np. z aplikacji REST API)
                    byte[] buffer = new byte[10 * 1024 * 1024];
                    int bytesRead = await pipeServer.ReadAsync(buffer, 0, buffer.Length);
                    string ReadResult = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    //    Console.WriteLine(ReadResult);
                    InputMessage inputMessage = JsonConvert.DeserializeObject<InputMessage>(ReadResult);

                    Type type = typeof(OptimaController);

                    string JsonResult = "";

                    MethodInfo methodInfo = type.GetMethod(Name.ToString());
                    if (methodInfo != null && inputMessage != null)
                    {
                        object[] parameters = { Name, inputMessage.Message, inputMessage.Guid }; // Parametry metody
                        Task<OutputMessage> task = (Task<OutputMessage>)methodInfo.Invoke(null, parameters);
                        OutputMessage result = await task.ConfigureAwait(false); // Oczekaj na zakończenie zadania
                        JsonResult = JsonConvert.SerializeObject(result);
                        Debug.WriteLine(guid + " : Odpowiedź> " + inputMessage.Guid + " " + JsonResult);
                    }
                    
                    //    Console.WriteLine(JsonResult);
                    byte[] responseBytes = Encoding.UTF8.GetBytes(JsonResult);
                    await pipeServer.WriteAsync(responseBytes, 0, responseBytes.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{guid} : Error: {ex.Message}");
                }
                finally
                {
                    pipeServer.WaitForPipeDrain();
                    pipeServer.Close();
                    IsInitialized = false;
                }
            }
        }
    }
}
