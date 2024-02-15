using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using OptimaCom.Controller;

namespace OptimaCom.Controller
{
    public class OptimaWrapper
    {
        Optima _Optima { get; set; }
        bool LoginStatus;
        bool IsOptimaAction;
        bool LockWhenOptimaAction;
        bool IsStaticConnection;

        private static OptimaControllerRepository repository;
        public OptimaWrapper(bool isStaticConnection)
        {
            InitConfiguration();
            IsStaticConnection = isStaticConnection;

        }
        public void LoginToOptima()
        {
            try
            {
                Optima.LogowanieAutomatyczne(configuration.OptimaUsers?.First());
            }
            catch (COMException e)
            {
                if (e.InnerException != null)
                    System.Console.WriteLine("Błąd: " + e.InnerException.Message);
                else
                    System.Console.WriteLine("Błąd: " + e.Message);
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                    System.Console.WriteLine("Błąd: " + e.InnerException.Message);
                else
                    System.Console.WriteLine("Błąd: " + e.Message);
            }
            finally
            {
            }
        }

        internal void AddOrUpdateContractor(List<Contractor> contractorsList)
        {
            try
            {
                foreach (Contractor contractor in contractorsList)
                    Optima.EdytujAtrybutyKontrahenta(contractor, configuration, repository);
            }
            catch (COMException e)
            {
                if (e.InnerException != null)
                    System.Console.WriteLine("Błąd: " + e.InnerException.Message);
                else
                    System.Console.WriteLine("Błąd: " + e.Message);
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                    System.Console.WriteLine("Błąd: " + e.InnerException.Message);
                else
                    System.Console.WriteLine("Błąd: " + e.Message);
            }
            finally
            {
            }
        }

        internal void AddOrUpdateResources(List<Commodity> commodities)
        {
            try
            {
                foreach (Commodity comm in commodities)
                    Optima.EdytujAtrybutyTowaru(comm, configuration, repository);
            }
            catch (COMException e)
            {
                if (e.InnerException != null)
                    System.Console.WriteLine("Błąd: " + e.InnerException.Message);
                else
                    System.Console.WriteLine("Błąd: " + e.Message);
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                    System.Console.WriteLine("Błąd: " + e.InnerException.Message);
                else
                    System.Console.WriteLine("Błąd: " + e.Message);
            }
            finally
            {
            }
        }

        internal void AddOrUpdateDocuments(List<Document> documents)
        {
            try
            {
                foreach (Document document in documents)
                    Optima.EdytujAtrybutyDokumentu(document, configuration, repository);
            }
            catch (COMException e)
            {
                if (e.InnerException != null)
                    System.Console.WriteLine("Błąd: " + e.InnerException.Message);
                else
                    System.Console.WriteLine("Błąd: " + e.Message);
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                    System.Console.WriteLine("Błąd: " + e.InnerException.Message);
                else
                    System.Console.WriteLine("Błąd: " + e.Message);
            }
            finally
            {
            }
        }
        private static ConfigRoot configuration;
        public void InitConfiguration()
        {
            //var ttt = JsonConvert.SerializeObject(new ConfigRoot());
            string currentDirectory = Directory.GetCurrentDirectory();
            string appJsonPath = Path.Combine(currentDirectory, "appsettings.json");
            string jsonContent = File.ReadAllText(appJsonPath);
            configuration = JsonConvert.DeserializeObject<ConfigRoot>(jsonContent);
            repository = new OptimaControllerRepository(configuration.ConnectionStrings.DBContext);
        }

        public void Wyloguj()
        {
            Optima.Wylogowanie();
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            Console.CancelKeyPress += CancelKeyPressTrapper;
            
            try
            {
                Task.Run(() => PipeServersController.InitMain());
                OptimaController.OptimaWrapper.LoginToOptima();
                Task.Run(() => PipeServersController.InitMainPipeLine());
                Task.Run(() => PipeServersController.RunPipeLine());
                Task.Run(() => PipeServersController.OptimaFIFO());

                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                OptimaController.OptimaWrapper.Wyloguj();
                Environment.Exit(1);
            }
        }

        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;
            Console.WriteLine($"Unhandled Exception: {exception?.Message}");
            OptimaController.OptimaWrapper.Wyloguj();
            Environment.Exit(1);
        }
        
        private static void CancelKeyPressTrapper(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Console application is being terminated.");
            OptimaController.OptimaWrapper.Wyloguj();
            Environment.Exit(0);
        }
    }
}
