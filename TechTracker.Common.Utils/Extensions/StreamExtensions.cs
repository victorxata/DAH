using System.IO;
using System.Text;

namespace TechTracker.Common.Utils.Extensions
{
    public static class StreamExtensions
    {
        /// <summary>
        /// TODO: I don't belong here!
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Encoding GetEncoding(string name)
        {
            switch (name)
            {
                case Utf8NoBomEncoding.Name:
                    return new Utf8NoBomEncoding();

                default:
                    return Encoding.GetEncoding(name);
            }
        }

        /// <summary>
        ///Figure out the Byte-Order-Mark if there is one, the ones that can be detected include
        ///utf-16LE    [2] ff fe
        ///utf-16BE    [2] fe ff
        ///utf-32LE    [4] ff fe 0 0
        ///utf-8       [3] ef bb bf
        ///
        /// Default: UTF-8 with no BOM if we were not able to detect the encoding
        /// </summary>
        public static Encoding GetByteOrderEncoding(this Stream fs)
        {
            byte[] bom = {0, 0, 0, 0};

            var position = fs.Position;
            try
            {
                fs.Position = 0;
                fs.Read(bom, 0, 4);

                /*
                 * See Also: http://msdn.microsoft.com/en-us/library/windows/desktop/dd374101(v=vs.85).aspx
                 *
                 * The order of the IF statements below IS important.
                 */
                if (bom[0] == 0xFF && bom[1] == 0xFE && bom[2] == 0 && bom[3] == 0) return Encoding.UTF32;
                if (bom[0] == 0xFF && bom[1] == 0xFE) return Encoding.Unicode;
                if (bom[0] == 0xFE && bom[1] == 0xFF) return Encoding.BigEndianUnicode;
                if (bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF) return Encoding.UTF8;

                return new Utf8NoBomEncoding();
            }
            finally
            {
                fs.Position = position;
            }
        }
    }
}