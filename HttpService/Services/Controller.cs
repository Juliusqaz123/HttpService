using System;
using System.IO;
using System.Text;

namespace HttpService
{
    class CrudController : ICrudController
    {
        private readonly string _resourceLocation;
        private readonly string _indexResponseHeader = @"HTTP/1.1 200 OK\nContent-Type: text/plain\nContent-Length: {0}\n";
        private readonly string _errorResponse = @"HTTP/1.1 500 Internal Error\nContent-Type: text/plain\nContent-Length: 0\n";
        private readonly string _index = "Index.html";

        public CrudController()
        {
            _resourceLocation = Path.Join(Helper.GetProjectDirectory(), "Resources");
        }

        /// <summary>
        /// handler for Get request
        /// </summary>
        /// <returns></returns>
        public byte[] Get()
        {
            try
            {
                var responseBody = File.ReadAllBytes(Path.Join(_resourceLocation, _index));
                var responseHeaderString = String.Format(_indexResponseHeader, responseBody.Length);
                byte[] responseHeaderBytes = Encoding.ASCII.GetBytes(responseHeaderString);
                byte[] response = Helper.CombineByteArrays(responseHeaderBytes, responseBody);
                return response;
            }
            catch
            {
                byte[] response = Encoding.ASCII.GetBytes(_errorResponse);
                return response;
            }
        }

        /// <summary>
        /// handler for Put request
        /// </summary>
        /// <returns></returns>
        public byte[] Put()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// handler for Post request
        /// </summary>
        /// <returns></returns>
        public byte[] Post()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// handler for Delete request
        /// </summary>
        /// <returns></returns>
        public byte[] Delete()
        {
            throw new NotSupportedException();
        }


    }
}
