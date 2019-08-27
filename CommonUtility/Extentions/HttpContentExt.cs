using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CommonUtility.Extentions
{
    public static class HttpContentExt
    {
        public static async Task<string> ReadAsFileAsync(this HttpContent content, string filename, bool overwrite)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new Exception("Missing file name");
            }

            string pathname = Path.GetFullPath(filename);

            if (!overwrite && File.Exists(filename))
            {
                throw new InvalidOperationException($"File {pathname} already exists.");
            }
            var folder = Path.GetDirectoryName(filename);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            using (var fs = new FileStream(pathname, FileMode.OpenOrCreate))
            {
                try
                {
                    await content.CopyToAsync(fs);
                    return pathname;
                }
                catch (Exception e)
                {
                    fs.Close();
                    throw;
                }
            }
        }
    }
}
