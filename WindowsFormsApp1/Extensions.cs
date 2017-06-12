using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public static class Extensions
    {
        public static string ToHexString(this byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static byte[] ToByteArray(this string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static char[] ToCharArray(this byte[] b)
        {
            char[] ret = new char[b.LongLength];
            for (long i = 0; i < b.LongLength; i++)
            {
                ret[i] = (char)b[i];
            }
            return ret;
        }

        public static int IndexOfSubArray(this char[] arr, char[] subarr, int startPos = 0)
        {
            for (int i = startPos; i < arr.Length - subarr.Length; i++)
            {
                bool flag = true;
                for (int j = 0; j < subarr.Length; j++)
                {
                    if (arr[i + j] != subarr[j])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    return i;
                }
            }
            return -1;
        }

        public static IEnumerable<int> ExtractIntegers(this string str)
        {
            var nsStr = str.Replace(" ", "");
            List<int> ret = new List<int>();
            for (int i = 0; i < str.Length; i++)
            {
                try
                {
                    if (str[i - 1] == ')' && (str[i] == '-' || char.IsDigit(str[i])))
                    {
                        int add = 0;
                        if (int.TryParse(str.Substring(i, str.IndexOf('(', i) - i), out add)) ret.Add(add);
                    }
                }
                catch
                {
                    continue;
                }
            }
            return ret;
        }


        /*public static void ExtractImagesFromPDF(string sourcePdf, string outputPath)
        {
            // NOTE:  This will only get the first image it finds per page.
            PdfReader pdf = new PdfReader(sourcePdf);
            RandomAccessFileOrArray raf = new iTextSharp.text.pdf.RandomAccessFileOrArray(sourcePdf);

            try
            {
                for (int pageNumber = 1; pageNumber <= pdf.NumberOfPages; pageNumber++)
                {
                    PdfDictionary pg = pdf.GetPageN(pageNumber);

                    // recursively search pages, forms and groups for images.
                    PdfObject obj = FindImageInPDFDictionary(pg);
                    if (obj != null)
                    {

                        int XrefIndex = Convert.ToInt32(((PRIndirectReference)obj).Number.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        PdfObject pdfObj = pdf.GetPdfObject(XrefIndex);
                        PdfStream pdfStrem = (PdfStream)pdfObj;
                        byte[] bytes = PdfReader.GetStreamBytesRaw((PRStream)pdfStrem);
                        if ((bytes != null))
                        {
                            using (System.IO.MemoryStream memStream = new System.IO.MemoryStream(bytes))
                            {
                                memStream.Position = 0;
                                System.Drawing.Image img = System.Drawing.Image.FromStream(memStream);
                                // must save the file while stream is open.
                                if (!Directory.Exists(outputPath))
                                    Directory.CreateDirectory(outputPath);

                                string path = Path.Combine(outputPath, String.Format(@"{0}.jpg", pageNumber));
                                System.Drawing.Imaging.EncoderParameters parms = new System.Drawing.Imaging.EncoderParameters(1);
                                parms.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Compression, 0);
                                System.Drawing.Imaging.ImageCodecInfo jpegEncoder = Utilities.GetImageEncoder("JPEG");
                                img.Save(path, jpegEncoder, parms);
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                pdf.Close();
                raf.Close();
            }


        }

        private static PdfObject FindImageInPDFDictionary(PdfDictionary pg)
        {
            PdfDictionary res =
                (PdfDictionary)PdfReader.GetPdfObject(pg.Get(PdfName.RESOURCES));


            PdfDictionary xobj =
              (PdfDictionary)PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT));
            if (xobj != null)
            {
                foreach (PdfName name in xobj.Keys)
                {

                    PdfObject obj = xobj.Get(name);
                    if (obj.IsIndirect())
                    {
                        PdfDictionary tg = (PdfDictionary)PdfReader.GetPdfObject(obj);

                        PdfName type =
                          (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE));

                        //image at the root of the pdf
                        if (PdfName.IMAGE.Equals(type))
                        {
                            return obj;
                        }// image inside a form
                        else if (PdfName.FORM.Equals(type))
                        {
                            return FindImageInPDFDictionary(tg);
                        } //image inside a group
                        else if (PdfName.GROUP.Equals(type))
                        {
                            return FindImageInPDFDictionary(tg);
                        }

                    }
                }
            }

            return null;

        }*/
    }

    public class PdfObjectWrapper
    {
        public PdfObject PdfObject { get; private set; }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (PdfObject.IsStream())
            {
                var dObject = (PRStream)PdfObject;
                sb.Append(dObject.ObjNum);
                sb.Append(' ');
                sb.Append(dObject.ObjGen);
                sb.Append(dObject.ToString());
            }
            else if (PdfObject.IsDictionary())
            {
                var dObject = (PdfDictionary)PdfObject;
                //sb.Append(dObject.ToString);
               // sb.Append(' ');
                //sb.Append(dObject.ObjGen);
                //sb.Append(' ');
                sb.Append(dObject.ToString());
            }
            else
            {
                sb.Append(this.PdfObject.ToString());
            }
            return sb.ToString().Trim();
        }

        public PdfObjectWrapper(PdfObject obj)
        {
            this.PdfObject = obj;
        }
    }

    
}



