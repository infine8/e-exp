using System.Text;
using Ionic.Zip;
using Ionic.Zlib;

namespace EthExplorer.Infrastructure.Common.Extensions
{
    public static class StringCompression
    {
        static StringCompression()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public static byte[] Zip(this string str, CompressionLevel compressionLevel = CompressionLevel.Default)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using var mso = new MemoryStream();
            using (var zip = new ZipOutputStream(mso))
            {
                zip.CompressionLevel = compressionLevel;
                zip.PutNextEntry("entry");

                zip.Write(bytes, 0, bytes.Length);
            }

            return mso.ToArray();
        }

        public static string Unzip(this byte[] bytes)
        {
            using var msi = new MemoryStream(bytes);
            using var zip = new ZipInputStream(msi);
            zip.GetNextEntry();

            return new StreamReader(zip, Encoding.UTF8).ReadToEnd();
        }
    }
}
