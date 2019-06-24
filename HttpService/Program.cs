namespace HttpService
{
    static class Program
    {
        public static void Main(string[] args)
        {
            HttpServer server = new HttpServer();
            server.Execute();
        }
    }
}
