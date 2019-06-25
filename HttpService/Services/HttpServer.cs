using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace HttpService
{
    public class HttpServer : IHttpServer
    {
        private Socket _serverSocket;
        private readonly ICrudController _controller;

        private static ManualResetEvent manualReset = new ManualResetEvent(false);
        private readonly int _backlog;
        private readonly int _port;
        private readonly string _host = "localhost";
        private readonly string _notFoundResponse = @"HTTP/1.1 404 Not Found\nContent-Type: text/plain\nContent-Length: 0\n";

        public HttpServer(int port = 8080, int backlog = 100)
        {

            _port = port;
            _backlog = backlog;
            _controller = new CrudController();
        }
        /// <summary>
        /// Serves as a starting point in running the server
        /// </summary>
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
                    _serverSocket.BeginAccept(new AsyncCallback(HandleAccept), _serverSocket);
                    manualReset.WaitOne();
                }
                catch(Exception e)
                {
                    Console.WriteLine("Problems encountered while communicating with client: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Initializes the socket
        /// </summary>
        private void Initiate()
        {
            _serverSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(Dns.GetHostEntry(_host).AddressList[0], _port);
            _serverSocket.Bind(endPoint);
            _serverSocket.Listen(_backlog);
        }

        #region Handlers
        /// <summary>
        /// Handles the acceptance of the connection from the client
        /// </summary>
        /// <param name="result"></param>
        private void HandleAccept(IAsyncResult result)
        {
            manualReset.Set();
            var listener = (Socket)result.AsyncState;
            Socket clientSocket = listener.EndAccept(result);
            int bufferSize = clientSocket.ReceiveBufferSize;
            byte[] buffer = new byte[bufferSize];
            clientSocket.BeginReceive(buffer,0,bufferSize,0, new AsyncCallback(HandleReceive), new StateModel(clientSocket,buffer));
        }
        /// <summary>
        /// Handles the receiving of data from the client
        /// </summary>
        /// <param name="result"></param>
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

        /// <summary>
        /// Handles the sending of the response for the clienta
        /// </summary>
        /// <param name="result"></param>
        private void HandleResponse(IAsyncResult result)
        {
            var client = (Socket)result.AsyncState;
            client.EndSend(result);
            client.Close();
        }
        #endregion

        /// <summary>
        /// Routes the control to controller class method which represents request type
        /// </summary>
        /// <param name="method">method name of the http request</param>
        /// <returns>byte array which represents response for the client</returns>
        private byte[] RouteToController(string method)
        {
            byte[] response;
            try
            {
                Type thisType = typeof(CrudController);
                MethodInfo requestType = thisType.GetMethod(method);
                response = (byte[])requestType.Invoke(_controller, null);
            }
            catch(Exception)
            {
                response = Encoding.ASCII.GetBytes(_notFoundResponse);
            }

            return response;
        }

    }
}
