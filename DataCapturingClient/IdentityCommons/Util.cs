using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IdentityCommons
{
    public class Util
    {

        public static byte[] AddWaterMark(MemoryStream ms, string watermarkText)
        {
            MemoryStream outstream = new MemoryStream();
            System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
            Graphics gr = Graphics.FromImage(img);
            Font font = new Font("Tahoma", (float)15, FontStyle.Bold);
            Color color = Color.FromArgb(50, 241, 235, 105);
            double tangent = (double)img.Height / (double)img.Width;
            tangent = 0;
            double angle = Math.Atan(tangent) * (180 / Math.PI);
            double halfHypotenuse = Math.Sqrt((img.Height * img.Height) + (img.Width * img.Width)) / 2;

            halfHypotenuse = 0;
            //double sin, cos, opp1, adj1, opp2, adj2;
            //for (int i = 100; i > 0; i--)
            //{
            //    font = new Font("Tahoma", i, FontStyle.Bold);
            //    SizeF sizef = gr.MeasureString(watermarkText, font, int.MaxValue);
            //    sin = Math.Sin(angle * (Math.PI / 180));
            //    cos = Math.Cos(angle * (Math.PI / 180));
            //    opp1 = sin * sizef.Width;
            //    adj1 = cos * sizef.Height;
            //    opp2 = sin * sizef.Height;
            //    adj2 = cos * sizef.Width;
            //    if (opp1 + adj1 < img.Height && opp2 + adj2 < img.Width)
            //        break;                //     
            //}
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;

            //stringFormat.LineAlignment = StringAlignment.;
            gr.SmoothingMode = SmoothingMode.AntiAlias;
            SizeF sizef = gr.MeasureString(watermarkText, font, int.MaxValue, stringFormat);
            //gr.RotateTransform((float)angle);
            //  gr.DrawString(watermarkText, font, new SolidBrush(color), new Point((int)halfHypotenuse, img.Height - 40), stringFormat);
            float startpoint = (img.Width - sizef.Width) / 2;
            gr.DrawString(watermarkText, font, new SolidBrush(color), new RectangleF(startpoint, img.Height - sizef.Height, sizef.Width, sizef.Height), stringFormat);

            // gr.DrawString(watermarkText, font, new SolidBrush(color), new Point((int)halfHypotenuse, img.Height), stringFormat);

            img.Save(outstream, ImageFormat.Jpeg);
            return outstream.GetBuffer();
        }

        // Hash an input string and return the hash as
        // a 32 character hexadecimal string.
        public static string getMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        public static bool verifyMd5Hash(string input, string hash)
        {
            // Hash the input.
            string hashOfInput = getMd5Hash(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.Ordinal;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
