using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDFR
{
    public partial class Main_frm : Form
    {
        public Main_frm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            DialogResult result = folderDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.textBox1.Text = folderDlg.SelectedPath;
                Environment.SpecialFolder root = folderDlg.RootFolder;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string folderp = textBox1.Text;

                if (Directory.Exists(folderp))
                {
                    try
                    {
                        ProcessDirectory(folderp);
                        MessageBox.Show("Rename Completed");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Can not parse PDF. Duplicate or unsuported type.");
                        throw;
                    }
                    
                }
                else
                {
                    MessageBox.Show("Choose a folder first! Not a valid directory");
                    Debug.WriteLine("{0} is not a valid directory.", folderp);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public void ProcessDirectory(string directory)
        {
            string[] fileEntries = Directory.GetFiles(directory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);

            if (recurse_chk.Checked) 
            {
                string[] subdirectories = Directory.GetDirectories(directory);
                foreach (string dir in subdirectories)
                    ProcessDirectory(dir);
            }
        }

        public void ProcessFile(string path)
        {
            string filename = System.IO.Path.GetFileNameWithoutExtension(path);
            if (System.IO.Path.GetExtension(path) == ".pdf")
            {
                string descr = String.Empty;
                descr = ExtractTextFromPdf(path);
                string newname =  textBox2.Text + " " + descr + " " + textBox3.Text + ".pdf";
                string newpath = System.IO.Path.GetDirectoryName(path) + "\\" + newname;
                File.Copy(path, newpath);
                Debug.WriteLine(path);
                Debug.WriteLine(newpath);
                Debug.WriteLine("Processed file '{0}'.", newpath);
            }
        }
        public string ExtractTextFromPdf(string path)
        {
            int jump = 0;
            if (ifra_chk.Checked) { jump = 3; }
            if (msds_chk.Checked) { jump = 9; }
            using (PdfReader reader = new PdfReader(path))
            {
                StringBuilder text = new StringBuilder();
                ITextExtractionStrategy Strategy = new iTextSharp.text.pdf.parser.LocationTextExtractionStrategy();

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    string page = "";

                    page = PdfTextExtractor.GetTextFromPage(reader, i, Strategy);
                    string[] lines = page.Split('\n');

                    foreach (string line in lines.Skip(jump))
                    {
                        var start = line.IndexOf(": ")+2;
                        var match2 = line.Substring(start, line.IndexOf("/") - start);
                        Debug.WriteLine(match2);
                        return match2;
                    }
                }
            }
            return String.Empty;
        }
    }
}
