using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server_part
{
    public class Server
    {
        private TcpListener _listener;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;

        public Server(string ipAddress, int port)
        {
            IPAddress localAddr = IPAddress.Parse(ipAddress);
            _listener = new TcpListener(localAddr, port);
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }

        public void Start()
        {
            _listener.Start();
            Console.WriteLine("Server started... listening on port " + ((IPEndPoint)_listener.LocalEndpoint).Port);

            try
            {
                while (true)
                {
                    if (_cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    TcpClient client = _listener.AcceptTcpClient();
                    Thread clientThread = new Thread(() => HandleClient(client));
                    clientThread.Start();
                }
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.Interrupted)
            {
                // Handle the expected exception when stopping the listener
                Console.WriteLine("Listener stopped.");
            }
            finally
            {
                _listener.Stop();
            }
        }

        private void HandleClient(TcpClient client)
        {
            Guid clientId = Guid.NewGuid();
            Console.WriteLine($"Client {clientId} connected.");

            using (StreamWriter writer = new StreamWriter(client.GetStream()))
            using (StreamReader reader = new StreamReader(client.GetStream()))
            {
                writer.Flush();

                try
                {
                    while (client.Connected)
                    {
                        string message = reader.ReadLine();
                        if (message != null)
                        {
                            Console.WriteLine($"Received message from {clientId}: {message}");
                            writer.WriteLine(message + " From server");
                            writer.Flush();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occurred for client {clientId}: {ex.Message}");
                }
                finally
                {
                    Console.WriteLine($"Client disconnected: {clientId}");
                }
            }
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _listener.Stop();
        }
    }

}
