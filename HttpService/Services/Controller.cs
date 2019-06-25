using System;
using System.IO;
using System.Text;

namespace HttpService
{
    class CrudController : ICrudController
    {
        private readonly string _resourceLocation;
        private readonly string _indexResponseStatus = @"HTTP/1.1 200 OK\nContent-Type: text/plain\nContent-Length: {0}\n";
        private readonly string _errorResponseStatus = @"HTTP/1.1 500 Internal Error\nContent-Type: text/plain\nContent-Length: 0\n";
        private readonly string _indexFileName = "Index.html";

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
                var responseBodyBytes = File.ReadAllBytes(Path.Join(_resourceLocation, _indexFileName));
                var responseStatusString = String.Format(_indexResponseStatus, responseBodyBytes.Length);
                byte[] responseStatusBytes = Encoding.ASCII.GetBytes(responseStatusString);
                byte[] responseFullBytes = Helper.CombineByteArrays(responseStatusBytes, responseBodyBytes);
                return responseFullBytes;
            }
            catch
            {
                byte[] responseFullBytes = Encoding.ASCII.GetBytes(_errorResponseStatus);
                return responseFullBytes;
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
