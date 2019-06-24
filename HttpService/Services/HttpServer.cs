using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Reflection;

namespace HttpService
{
    public class HttpServer : IHttpServer
    {
        private Socket serverSocket;
        private readonly CrudController controller;

        private static ManualResetEvent manualReset = new ManualResetEvent(false);
        private readonly int _backlog;
        private readonly int _port;
        private readonly string _host = "localhost";
        private readonly string _notFoundResponse = @"HTTP/1.1 404 Not Found\nContent-Type: text/plain\nContent-Length: 0\n";

        public HttpServer(int port = 8080, int backlog = 100)
        {

            _port = port;
            _backlog = backlog;
            controller = new CrudController();
        }

        public void Execute()
        {
            try
            {
                Initiate();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Failed to initialize socket: " + ex.Message);
                return;
            }

            Console.WriteLine("You can reach index at http://"+ _host + ":" + _port);
            while (true)
            {
                try
                {
                    manualReset.Reset();
                    serverSocket.BeginAccept(new AsyncCallback(HandleAccept), serverSocket);
                    manualReset.WaitOne();
                }
                catch(Exception e)
                {
                    Console.WriteLine("Problems encountered while communicating with client: " + e.Message);
                }
            }
        }

        private void Initiate()
        {
            serverSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(Dns.GetHostEntry(_host).AddressList[0], _port);
            serverSocket.Bind(endPoint);
            serverSocket.Listen(_backlog);
        }

        #region Handlers
        private void HandleAccept(IAsyncResult result)
        {
            manualReset.Set();
            var listener = (Socket)result.AsyncState;
            Socket clientSocket = listener.EndAccept(result);
            int bufferSize = clientSocket.ReceiveBufferSize;
            byte[] buffer = new byte[bufferSize];
            clientSocket.BeginReceive(buffer,0,bufferSize,0, new AsyncCallback(HandleReceive), new StateModel(clientSocket,buffer));
        }

        private void HandleReceive(IAsyncResult result)
        {
            StateModel state = (StateModel) result.AsyncState;
            state.Connection.EndReceive(result);
            string requestString = Encoding.ASCII.GetString(state.Buffer);
            string method;
            try
            {
                method = Helper.GetRequestType(requestString);
            }
            catch(ArgumentOutOfRangeException)
            {
                state.Connection.Close();
                return;
            }

            var response = RouteToController(method);

            state.Connection.BeginSend(response, 0, response.Length, 0, new AsyncCallback(HandleResponse), state.Connection);
        }

        private void HandleResponse(IAsyncResult result)
        {
            var client = (Socket)result.AsyncState;
            client.EndSend(result);
            client.Close();
        }
        #endregion

        private byte[] RouteToController(string method)
        {
            byte[] response;
            try
            {
                Type thisType = typeof(CrudController);
                MethodInfo requestType = thisType.GetMethod(method);
                response = (byte[])requestType.Invoke(controller, null);
            }
            catch(Exception)
            {
                response = Encoding.ASCII.GetBytes(_notFoundResponse);
            }

            return response;
        }

    }
}
