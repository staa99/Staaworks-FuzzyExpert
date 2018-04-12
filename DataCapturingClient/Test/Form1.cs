
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BackgroundSync sync = new BackgroundSync();
            Thread foregroundThread =
                new Thread(new ThreadStart(sync.Sync));
            foregroundThread.Start();
        }


       

    }
    public class BackgroundSync
    {
        public byte[] AddWaterMark(MemoryStream ms, string watermarkText)
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

        public void Sync()
        {
            NewFolder1.IdentityDBEntitieskldkldks idb = new NewFolder1.IdentityDBEntitieskldkldks();

            int i = 0;

            foreach (NewFolder1.Person p in idb.Persons.Where(p=>p.CurrentPhoto!= null && p.Id> 32025).ToList())
            {
                if (p.CurrentPhoto != null)
                {
                    try
                    {

                   
                    i++;
                    byte[] bytes = p.CurrentPhoto.ToArray();
                    MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length, true, true);

                    bytes = AddWaterMark(ms, "FUNAAB");
                    p.WatermarkedPhoto = bytes;
                   // if (i % 100 == 0)
                   // {
                        idb.SaveChanges();
                        // }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
           // idb.SaveChanges();
        }
    }
        }
