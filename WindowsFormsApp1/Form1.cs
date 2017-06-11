using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text.exceptions;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        public byte[] PdfBytes;
        public char[] PdfChars;
        Dictionary<int, int> TJ16 = new Dictionary<int, int>();
        Dictionary<int, double> TJ16Percent = new Dictionary<int, double>();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            PdfReader reader = new PdfReader(openFileDialog1.FileName);
            toolStripStatusLabel1.Text = $"Версия PDF: 1.{reader.PdfVersion.ToString()}";
            richTextBox1.Text = reader.JavaScript;
            TJ16Percent.Clear();
            TJ16.Clear();


            PdfBytes = new byte[reader.SafeFile.Length];
            reader.SafeFile.ReadFully(PdfBytes);
            PdfChars = PdfBytes.ToCharArray();
            if (!CheckEof(PdfChars))
            {
                toolStripLabel3.Text = $"Данные после EOF: ДА";
            }
            if(!CheckDataBetweenObjects())
            {
                toolStripLabel4.Text = $"Данные между объектами: ДА";
            }
            PdfObject obj;
            listView1.Items.Clear();
            for (int i = 1; i <= reader.XrefSize; i++)
            {
                obj = reader.GetPdfObject(i);
                if (obj != null)
                {

                    if (obj.IsStream())
                    {
                        PRStream stream = (PRStream)obj;
                        byte[] b;
                        try
                        {
                            b = PdfReader.GetStreamBytes(stream);
                        }
                        catch (UnsupportedPdfException)
                        {
                            b = PdfReader.GetStreamBytesRaw(stream);
                        }
                        string objStr = new string(b.ToCharArray());
                        richTextBox3.AppendText(objStr);
                        foreach(var integer in objStr.ExtractIntegers().Where(h=>Math.Abs(h)<=16))
                        {
                            if (!TJ16.ContainsKey(integer)) TJ16.Add(integer, 0);
                            TJ16[integer]++;
                        }
                    }

                    if (obj.IsDictionary())
                    {
                        PdfDictionary dict = (PdfDictionary)obj;

                        if (dict.Get(PdfName.JAVASCRIPT) != null)
                        {
                            toolStripLabel1.Text = "Javascript: ДА";
                        }

                        if (dict.Get(PdfName.EMBEDDEDFILE) != null
                            || dict.Get(PdfName.EMBEDDED) != null
                            || dict.Get(PdfName.EMBEDDEDFILES) != null)
                        {
                            toolStripLabel2.Text = "Embedded: ДА";
                        }
                    }


                }
            }
            reader.Close();

            chart1.Series[0].XValueType = ChartValueType.Int32;
            chart1.Series[0].YValueType = ChartValueType.Double;
            double tj16sum = TJ16.Values.Sum();
            foreach(var kvp in TJ16)
            {
                TJ16Percent.Add(kvp.Key, (double)kvp.Value / tj16sum);
            }




                chart1.Series[0].Points.DataBindXY(TJ16Percent.Keys, TJ16Percent.Values);
            // richTextBox2.Text = File.ReadAllText(openFileDialog1.FileName);

        }

        private bool CheckDataBetweenObjects()
        {
            int startpos = 0;
            int idx = PdfChars.IndexOfSubArray("endobj".ToCharArray(), startpos);
            while (idx>-1)
            {
                var endIdx = PdfChars.IndexOfSubArray("obj".ToCharArray(), idx + 6);
                if (endIdx>idx+6)
                {
                    var between = PdfChars.Skip(idx + 6).Take(endIdx - idx - 6);
                    if(between.Any(c=>!char.IsDigit(c)&&c!=' '&&c!='\r'&&c!='\n'))
                    {
                        return false;
                    }

                }
                startpos = idx + 6;
                idx= PdfChars.IndexOfSubArray("endobj".ToCharArray(), startpos);
            }
            return true;
        }

        private bool CheckEof(char[] pdfChars)
        {
            char[] eof = { '%', '%', 'E', 'O', 'F' };
            if (pdfChars.Length < eof.Length) return false;
            for (int i = 1; i <= eof.Length; i++)
            {
                if (pdfChars[pdfChars.LongLength - i] != eof[eof.Length - i])
                {
                    return false;
                }
            }
            return true;
        }



        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var c in PdfBytes.ToCharArray().Select(i => i.ToString()))
            {
                sb.Append(c);
            }
            richTextBox2.Text = sb.ToString();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = Encoding.Unicode.GetString(Encoding.Convert(Encoding.UTF8, Encoding.Unicode, PdfBytes));
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = PdfBytes.ToHexString();
        }
    }
}
