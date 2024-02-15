using System;
using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Text;

namespace OptimaCom
{
    public static class OwnReflections
    {
        public static string GetPropertyValue(object obj, string propertyName)
        {
            if (obj != null)
            {
                var property = obj.GetType().GetProperty(propertyName);
                if (property != null)
                {
                    var value = property.GetValue(obj);
                    return value != null ? value.ToString() : string.Empty;
                }
            }

            return string.Empty;
        }

        // Helper method to get default value with null check
        public static string GetDefaultValue(object value)
        {
            return value != null ? value.ToString() : string.Empty;
        }
    }
    public static class PipeServersController
    {
        public static BlockingCollection<PiPeServers> PiPeServers = new BlockingCollection<PiPeServers>();

        public static void RunPipeLine()
        {
            foreach (var item in PiPeServers.GetConsumingEnumerable())
            {
                item.RunOptimaMethod();
            }
        }

        public static BlockingCollection<OptimaOperations> FIFO = new BlockingCollection<OptimaOperations>();

        public static void OptimaFIFO()
        {

            foreach (var optObject in FIFO.GetConsumingEnumerable())
            {
                try
                {
                    optObject.Run();
                }
                catch (Exception ex)
                {
                    var t = ex;

                }
            }
        }

        internal async static void InitMainPipeLine()
        {
            while (true)
            {
                using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("MainPipeClientStream", PipeDirection.InOut))
                {
                    string result = "ok";
                    try
                    {
                        await pipeServer.WaitForConnectionAsync();

                        byte[] buffer = new byte[10 * 1024 * 1024];
                        int bytesRead = await pipeServer.ReadAsync(buffer, 0, buffer.Length);
                        string ReadResult = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        string[] x = ReadResult.Split('_');
                        PipeLineAlias parsedPipe;
                        Enum.TryParse(x[0], out parsedPipe);

                        Guid parsedGuid;
                        Guid.TryParse(x[1], out parsedGuid);

                        PiPeServers.Add(new PiPeServers(parsedPipe, parsedGuid));
                    }
                    catch (Exception ex)
                    {
                        result = ex.Message;
                    }
                    byte[] responseBytes = Encoding.UTF8.GetBytes(result);
                    await pipeServer.WriteAsync(responseBytes, 0, responseBytes.Length);

                    pipeServer.WaitForPipeDrain();
                    pipeServer.Close();
                }
            }
        }
        internal async static void InitMain()
        {
            while (true)
            {
                using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("Optima", PipeDirection.InOut))
                {
                    try
                    {
                        await pipeServer.WaitForConnectionAsync();
                    }
                    catch (Exception ex)
                    {
                       
                    }
                    pipeServer.WaitForPipeDrain();
                    pipeServer.Close();
                }
            }
        }
    }
}
