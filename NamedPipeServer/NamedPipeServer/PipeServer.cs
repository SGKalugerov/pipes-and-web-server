namespace NamedPipeServer
{
    using System.Collections.Concurrent;
    using System.IO.Pipes;
    using System.Text;
    internal class PipeServer
    {
        private ConcurrentDictionary<int, NamedPipeServerStream> clients = new ConcurrentDictionary<int, NamedPipeServerStream>();
        private int clientNumber = 0;
        internal void StartNamedPipeServer()
        {
            Console.WriteLine("Named Pipe Server is waiting for client connections...");

            Thread inputThread = new Thread(HandleConsoleInput);
            inputThread.Start();

            while (true)
            {
                var pipeServer = new NamedPipeServerStream("testpipe", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

                Console.WriteLine("Waiting for a client to connect...");
                pipeServer.WaitForConnection();

                int currentClientNumber = clientNumber++;
                clients[currentClientNumber] = pipeServer;

                var clientThread = new Thread(() => HandleClient(pipeServer, currentClientNumber));
                clientThread.Start();
            }
        }

        private void HandleClient(NamedPipeServerStream pipeServer, int clientNumber)
        {
            Console.WriteLine($"Client {clientNumber} connected.");

            try
            {
                using (StreamReader reader = new StreamReader(pipeServer, Encoding.UTF8))
                {
                    string message;
                    while ((message = reader.ReadLine()) != null)
                    {
                        Console.WriteLine($"Client {clientNumber}: {message}");
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine($"Client {clientNumber} disconnected. Error: {e.Message}");
            }
            finally
            {
                clients.TryRemove(clientNumber, out _);
                pipeServer.Close();
                pipeServer.Dispose();
            }
        }

        private void HandleConsoleInput()
        {
            while (true)
            {
                string input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    ProcessInput(input);
                }
            }
        }

        private void ProcessInput(string input)
        {
            //fix this shit
            if (input.StartsWith("Client"))
            {
                var split = input.Split(':');
                if (clients.ContainsKey(int.Parse(split[1]) - 1))
                {
                    SendMessageToClient(split[2], int.Parse(split[1]) - 1);
                }
                else
                {
                    Console.WriteLine("Invalid input or client not found.");
                }
            }
        }

        private void SendMessageToClient(string message, int clientNumber)
        {
            if (clients.TryGetValue(clientNumber, out var pipeServer))
            {
                using (StreamWriter writer = new StreamWriter(pipeServer, Encoding.UTF8) { AutoFlush = true })
                {
                    writer.WriteLine(message);
                }
            }
        }
    }
}
