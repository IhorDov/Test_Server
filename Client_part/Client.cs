using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_part 
{
    public class Client : IClient
    {
        private readonly ITcpClient _client;
        private readonly StreamWriter _writer;
        private readonly StreamReader _reader;
        private readonly Thread _receiveThread;
        private bool _isRunning;

        public Client(ITcpClient client, string server, int port)
        {
            _client = client;
            _client.Connect(server, port);
            Console.WriteLine("Connected to server...");

            var networkStream = _client.GetStream();
            _writer = networkStream.GetWriter();
            _reader = networkStream.GetReader();

            // Start a thread to receive messages
            _isRunning = true;
            _receiveThread = new Thread(ReceiveMessages);
            _receiveThread.Start();
        }

        public void SendMessage(string message)
        {
            _writer.WriteLine(message);
            _writer.Flush();
        }

        private void ReceiveMessages()
        {
            while (_isRunning)
            {
                try
                {
                    string message = _reader.ReadLine();
                    if (message != null)
                    {
                        Console.WriteLine(message);
                    }
                }
                catch (IOException)
                {
                    // Handle disconnection
                    _isRunning = false;
                }
            }
        }

        public void Close()
        {
            _isRunning = false;
            _receiveThread.Join();

            _writer.Close();
            _reader.Close();
            //_client.Close();
        }

        public void Dispose()
        {
            Close();
        }
    }
}
