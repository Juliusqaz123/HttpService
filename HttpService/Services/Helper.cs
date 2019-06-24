using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace HttpService
{
    static class Helper
    {
        /// <summary>
        /// Appends numerous byte arrays
        /// </summary>
        /// <param name="arrays">Arrays to append</param>
        /// <returns></returns>
        public static byte[] CombineByteArrays(params byte[][] arrays)
        {
            byte[] result = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                Buffer.BlockCopy(array, 0, result, offset, array.Length);
                offset += array.Length;
            }
            return result;
        }

        /// <summary>
        /// Gets the Project Directory
        /// </summary>
        /// <returns>Full path of the main Project directory</returns>
        public static string GetProjectDirectory()
        {
            return Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        }
        
        /// <summary>
        /// Returns the method type from the request
        /// </summary>
        /// <param name="request">Full http request received from the client</param>
        /// <returns>Method type in CamelCase</returns>
        public static string GetRequestType(string request)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            var method = request.Substring(0, request.IndexOf(" ")).ToLower();
            return textInfo.ToTitleCase(method);
        }
    }
}
