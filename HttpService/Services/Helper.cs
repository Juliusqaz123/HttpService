using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace HttpService
{
    static class Helper
    {
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

        public static string GetProjectDirectory()
        {
            return Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        }

        public static string GetRequestType(string request)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            var method = request.Substring(0, request.IndexOf(" ")).ToLower();
            return textInfo.ToTitleCase(method);
        }
    }
}
