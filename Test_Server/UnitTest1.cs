using Server_part;
using System.Net.Sockets;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using Client_part;

namespace Test_Server
{
    [TestClass]
    public class ServerTests
    {
        private Server _server;

        [TestInitialize]
        public void Setup()
        {
            _server = new Server("127.0.0.1", 12000);
        }

        [TestMethod]
        public void TestServerStartAndStop()
        {
            // Start the server in a separate thread
            Thread serverThread = new Thread(() => _server.Start());
            serverThread.Start();

            // Give some time for the server to start
            Thread.Sleep(1000);

            // Stop the server
            _server.Stop();
            serverThread.Join(); // Ensure the thread has finished

            Assert.IsFalse(serverThread.IsAlive);
        }
    }
}