using OptimaAPI.DB;
using OptimaAPI.Models;
using OptimaAPI.Repositories;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;
using static OptimaAPI.Models.ContractorRequest;
using static OptimaAPI.Repositories.ImportDocumentsRepository;

namespace OptimaAPI.Controllers
{
    public static class StaticSenderController
    {
        private static readonly IConfiguration configuration;
        public static DapperContext dapperContext;
        private static Timer timer;

        static StaticSenderController()
        {
            // Tutaj uzyskaj IConfiguration, np. z AppSettings
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
            .Build();
            dapperContext = new DapperContext(configuration);
        }
        private static void AddNewContrahentCallback(object state)
        {
            // Wywołaj metodę do dodawania nowego kontrahenta
            AddNewContractors();
        }

        //static public void InitializeProccess()
        //{
        //    try
        //    {

        //        AddNewOperationalThreaded();
        //        Task.Run(() => ProcessQueue());

        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Błąd podczas wykonywania ExecuteResponseAsync: {ex.Message}");
        //    }

        //}

        public static readonly BlockingCollection<Request> blockingCollectionToSendXLApi = new();

        //private static async Task ProcessQueue()
        //{
        //    foreach (var responseTask in blockingCollectionToSendXLApi.GetConsumingEnumerable())
        //    {
        //        try
        //        {
        //            Request response = responseTask;
        //            // Przetwórz response
        //            if (response != null)
        //            {
        //                Debug.WriteLine($"Metoda {nameof(ProcessQueue)} :>" + " Wysłanie response:" + response.Guid);
        //                var t = await ExecuteResponseAsync(response);
        //                Debug.WriteLine($"Metoda {nameof(ProcessQueue)} :>" + " odpowiedź z response:" + response.Guid + " " + t.Message);
        //            }
        //            if (blockingCollectionToSendXLApi.Count == 0)
        //            {
        //                AddNewOperationalThreaded();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //        }
        //    }
        //}

        private static async Task<InputMessage> ExecuteResponseAsync(Request resp)
        {
            try
            {
                var result = await resp.SendDataToXL();
                if (result != null)
                {
                    try
                    {
                        Debug.WriteLine("Przetwarzanie odpowiedzi z " + resp.Guid + " do Optimy");
                        resp.ResultMessage = result;

                        await ProcessQueueToSendOptima(resp);
                        Debug.WriteLine("Zakończono " + resp.Guid + " do Optimy");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(resp.Guid + ":> " + ex.Message);
                    }
                }

                return result ?? new InputMessage() { Message = $"Zwrócone puste wartosci" };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd podczas wykonywania ExecuteResponseAsync: {ex.Message}");
                return new InputMessage() { Message = $"Błąd podczas wykonywania ExecuteResponseAsync: {ex.Message}" };
            }
        }

        private static void AddNewOperationalThreaded()
        {
            AddNewContractors();
            AddNewContractorsSQL();
            ////// AddCommodities();
            AddResources();
            //AddMerchandiseCards();
            AddDocuments();
        }

        public static async void AddCategories()
        {
            if (StaticSenderController.dapperContext != null)
            {
                CommodityGoupsRepository repository = new CommodityGoupsRepository(StaticSenderController.dapperContext);
                var result = await repository.GetCommodityGroups();
                var chunks = ChunkList<CommodityGroup>(result.ToList(), 200);
                foreach (var chunk in chunks)
                {
                    var createReq = new Models.CommRequest(result);
                    blockingCollectionToSendXLApi.Add(createReq);
                }
            }
        }

        public static async void AddCommodities()
        {
            if (StaticSenderController.dapperContext != null)
            {
                CommodityGoupsRepository repository = new CommodityGoupsRepository(StaticSenderController.dapperContext);
                var result = await repository.GetCommodityGroups();
                var chunks = ChunkList<CommodityGroup>(result.ToList(), 200);
                foreach (var chunk in chunks)
                {
                    var createReq = new Models.CommRequest(result);
                    blockingCollectionToSendXLApi.Add(createReq);
                }
            }
        }

        public static async void AddNewContractors()
        {
            if (StaticSenderController.dapperContext != null)
            {
                ContractorsRepository repository = new ContractorsRepository(StaticSenderController.dapperContext);
                var result = await repository.GetContractors();
                var chunks = ChunkList<Contractor>(result.ToList(), 200);
                foreach (var chunk in chunks)
                {
                    var newContractor = new ContractorRequest(chunk);
                    blockingCollectionToSendXLApi.Add(newContractor);
                }
            }
        }

        public static async void AddNewContractorsSQL()
        {
            if (StaticSenderController.dapperContext != null)
            {
                ContractorsRepository repository = new ContractorsRepository(StaticSenderController.dapperContext);
                var result = await repository.GetSQLContractors();

                // var createReq = new Models.ContractorSQLRequest(result);
                var chunks = ChunkList<SQLContractor>(result.ToList(), 200);
                foreach (var chunk in chunks)
                {
                    var newContractor = new ContractorSQLRequest(chunk);
                    blockingCollectionToSendXLApi.Add(newContractor);
                }
            }
        }

        private static List<List<T>> ChunkList<T>(List<T> source, int chunkSize)
        {
            return source
                .Select((value, index) => new { Index = index, Value = value })
                .GroupBy(x => x.Index / chunkSize)
                .Select(group => group.Select(x => x.Value).ToList())
                .ToList();
        }
        public static async void AddDocuments()
        {
            if (StaticSenderController.dapperContext != null)
            {
                DocumentsRepository repository = new DocumentsRepository(StaticSenderController.dapperContext);

                var fa = Models.DocRequest.Init(await repository.GetFADocuments(), "FA");
                if (fa.Json.Count > 0)
                    blockingCollectionToSendXLApi.Add(fa);

                var fz = Models.DocRequest.Init(await repository.GetFZDocuments(), "FZ");
                if (fz.Json.Count > 0)
                    blockingCollectionToSendXLApi.Add(fz);

                //var fpa = Models.DocRequest.Init(await repository.GetFPADocuments(), "FPA");
                //if (fpa.Json.Count > 0)
                //    blockingCollectionToSendXLApi.Add(fpa);

                //var frod = Models.DocRequest.Init(await repository.GetFRODocuments(), "FROD");
                //if (frod.Json.Count > 0)
                //    blockingCollectionToSendXLApi.Add(frod);

                //var fpf = Models.DocRequest.Init(await repository.GetFPFDocuments(), "FPF");
                //if (fpf.Json.Count > 0)
                //    blockingCollectionToSendXLApi.Add(fpf);

                //var pw = Models.DocRequest.Init(await repository.GetPWDocuments(), "PW");
                //if (pw.Json.Count > 0)
                //    blockingCollectionToSendXLApi.Add(pw);

                //var rw = Models.DocRequest.Init(await repository.GetRWDocuments(), "RW");
                //if (rw.Json.Count > 0)
                //    blockingCollectionToSendXLApi.Add(rw);

                //var pa = Models.DocRequest.Init(await repository.GetPADocuments(), "PA");
                //if (pa.Json.Count > 0)
                //    blockingCollectionToSendXLApi.Add(pa);

                //var pawz = Models.DocRequest.Init(await repository.GetPAWZDocuments(), "PAWZ");
                //if (pawz.Json.Count > 0)
                //    blockingCollectionToSendXLApi.Add(pawz);

                //var paro = Models.DocRequest.Init(await repository.GetPAROocuments(), "PARO");
                //if (paro.Json.Count > 0)
                //    blockingCollectionToSendXLApi.Add(paro);

                //var wz = Models.DocRequest.Init(await repository.GetWZDocuments(), "WZ");
                //if (wz.Json.Count > 0)
                //    blockingCollectionToSendXLApi.Add(wz);

                //var pz = Models.DocRequest.Init(await repository.GetPZDocuments(), "PZ");
                //if (pz.Json.Count > 0)
                //    blockingCollectionToSendXLApi.Add(pz);

                //var pzzd = Models.DocRequest.Init(await repository.GetPZZDDocuments(), "PZZD");
                //if (pzzd.Json.Count > 0)
                //    blockingCollectionToSendXLApi.Add(pzzd);

                //var ro = Models.DocRequest.Init(await repository.GetRODocuments(), "RO");
                //if (ro.Json.Count > 0)
                //    blockingCollectionToSendXLApi.Add(ro);

                //var ropf = Models.DocRequest.Init(await repository.GetROPFDocuments(), "ROPF");
                //if (ropf.Json.Count > 0)
                //    blockingCollectionToSendXLApi.Add(ropf);

                //var zd = Models.DocRequest.Init(await repository.GetZDDocuments(), "ZD");
                //if (zd.Json.Count > 0)
                //    blockingCollectionToSendXLApi.Add(zd);

                //var zrod = Models.DocRequest.Init(await repository.GetZDRODocuments(), "ZROD");
                //if (zrod.Json.Count > 0)
                //    blockingCollectionToSendXLApi.Add(zrod);

                //var mm = Models.DocRequest.Init(await repository.GetMMDocuments(), "MM");
                //if (mm.Json.Count > 0)
                //    blockingCollectionToSendXLApi.Add(mm);
            }
        }

        public static async void AddResources()
        {
            if (StaticSenderController.dapperContext != null)
            {
                ResourcesRepository repository = new ResourcesRepository(StaticSenderController.dapperContext);
                var result = await repository.GetResources();
                var createReq = new Models.RessourcesRequest(result);
                blockingCollectionToSendXLApi.Add(createReq);
            }
        }

        public static async void AddMerchandiseCards()
        {
            if (StaticSenderController.dapperContext != null)
            {
                MerchandiseCardsRepository repository = new MerchandiseCardsRepository(StaticSenderController.dapperContext);
                var result = await repository.GetMerchandiseCards();
                var createReq = new Models.MerchCardsRequest(result);
                blockingCollectionToSendXLApi.Add(createReq);
            }
        }

        public static BlockingCollection<Request> blockingCollectionToSendOptimaApi = new();
        private static async Task ProcessQueueToSendOptima(Request responseTask)
        {
            try
            {
                //   SprawdzPolaczeniePotoku("MainPipeClientStream");
                if (responseTask != null)
                {
                    if (!string.IsNullOrEmpty(responseTask.GetPipeName()))
                    {
                        using (var MainPipeClient = new NamedPipeClientStream(".", "MainPipeClientStream", PipeDirection.InOut, PipeOptions.None))
                        {
                            if (!string.IsNullOrEmpty(responseTask.GetPipeName()) && responseTask?.ResultMessage?.Guid != null)
                            {
                                string responseData = "";
                                string pipe_name = responseTask.GetPipeName() + "_" + responseTask?.ResultMessage.Guid;
                                try
                                {
                                    MainPipeClient.Connect();
                                    byte[] requestBytes = Encoding.UTF8.GetBytes(pipe_name);
                                    await MainPipeClient.WriteAsync(requestBytes, 0, requestBytes.Length);

                                    byte[] responseBytes = new byte[1024 * 1024 * 10]; // 10 MB
                                    int bytesRead = await MainPipeClient.ReadAsync(responseBytes, 0, responseBytes.Length);
                                    responseData = Encoding.UTF8.GetString(responseBytes, 0, bytesRead);
                                    MainPipeClient.Close();
                                }
                                catch (IOException ex)
                                {
                                    var xxx = ex;
                                }
                                catch (Exception ex)
                                {
                                    var xxx = ex;
                                }

                                if (!string.IsNullOrEmpty(responseData) && responseData == "ok" && !string.IsNullOrEmpty(responseTask?.ResultMessage.ResultJson))
                                {
                                    using (var potok1 = new NamedPipeClientStream(".", pipe_name, PipeDirection.InOut, PipeOptions.None))
                                    {
                                        potok1.Connect();
                                        string text = JsonConvert.SerializeObject(responseTask.ResultMessage, Formatting.Indented);
                                        byte[] requestBytes = Encoding.UTF8.GetBytes(text);
                                        await potok1.WriteAsync(requestBytes, 0, requestBytes.Length);

                                        byte[] responseBytes = new byte[1024 * 1024 * 10]; // 10 MB
                                        int bytesRead = await potok1.ReadAsync(responseBytes, 0, responseBytes.Length);
                                        responseData = Encoding.UTF8.GetString(responseBytes, 0, bytesRead);
                                        potok1.Close();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                var xxx = ex;
            }
            catch (Exception ex)
            {
                var xxx = ex;
            }

        }

        static bool SprawdzPolaczeniePotoku(string nazwaPotoku)
        {
            string _nazwaPotoku = "MainPipeClientStream";

            if (!SprawdzPolaczeniePotoku(_nazwaPotoku))
            {
                UtworzPolaczeniePotoku(_nazwaPotoku);
            }
            else
            {
                Console.WriteLine($"Połączenie w potoku {_nazwaPotoku} już istnieje.");
            }

            try
            {
                using (var potok = new NamedPipeClientStream(".", _nazwaPotoku, PipeDirection.InOut, PipeOptions.None))
                {
                    potok.Connect(100); // Czas oczekiwania na połączenie w milisekundach
                    return potok.IsConnected;
                }
            }
            catch (IOException)
            {
                return true; // Potok jest już używany
            }
            catch (Exception)
            {
                return false;
            }
        }

        static void UtworzPolaczeniePotoku(string nazwaPotoku)
        {
            try
            {
                using (var potok = new NamedPipeServerStream(nazwaPotoku))
                {
                    Console.WriteLine($"Utworzono potok o nazwie {nazwaPotoku}");
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Potok {nazwaPotoku} jest już używany: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas tworzenia potoku: {ex.Message}");
            }
        }
    }
}
