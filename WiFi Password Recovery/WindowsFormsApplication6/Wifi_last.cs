using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process cmd1 = new Process();
            cmd1.StartInfo.FileName = "cmd.exe";
            string ssid = ssidTextBox1.Text;

            cmd1.StartInfo.Arguments = @"/c netsh wlan show profiles """ + ssid + @""" key=clear";
            cmd1.StartInfo.UseShellExecute = false;
            cmd1.StartInfo.RedirectStandardOutput = true;
            cmd1.StartInfo.RedirectStandardError = true;
            cmd1.Start();
            cmd1.WaitForExit();

            char[] delimiter = { ':','\n'};
            
            string outputText = cmd1.StandardOutput.ReadToEnd();
            string textPassword = "";
            bool nextValue=false;
            string[] words = outputText.Split(delimiter);
            foreach (var password in words)
            {
                if (nextValue == true)
                {
                    textPassword = password;
                    break;
                }
                if (password == "    Key Content            ")
                    nextValue = true;
            }

            passwordTextBox2.Text = textPassword;

            System.IO.File.AppendAllText(@"Save.txt", Environment.NewLine + "SSID: " + ssid + "   \nPassword:" + textPassword + Environment.NewLine +
                                                     @"-----------------------------------------------------" + Environment.NewLine);
        }

        private void button2_Click(object sender, EventArgs e)
        { 
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c netsh wlan show profiles";
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.Start();

            string output = cmd.StandardOutput.ReadToEnd();
            int startIndex = output.IndexOf("User profiles") + "User profiles".Length + "-----------------".Length;
            int endIndex = output.LastIndexOf("\n");
            string ssidOutput = output.Substring(startIndex, endIndex - startIndex);
            ssidOutput = ssidOutput.Replace("All User Profile", "").Replace(":", "").Replace("          ","").Trim();
          //  bool nextValue = false;
          //  char[] delimiter = "-------------".ToCharArray();
          //  string[] words = output.Split(delimiter);
            List<string> ssidNamesList = new List<string>();
            //foreach (var ssid in words)
            //{
            //    if (nextValue == true)
            //    {
            //        Console.WriteLine(ssid);
            //        nextValue = false;
            //    }
            //    if (ssid == "    All User Profile     ")
            //    {
            //        nextValue = true;
            //    }
                
            //}

            string temporaryName = null;

            for (int i = 0; i < ssidOutput.Length; i++)
            {
                if (ssidOutput[i] != '\n')
                    temporaryName += ssidOutput[i];
                else
                {
                    ssidNamesList.Add(temporaryName);
                    temporaryName = null;
                }
            }
            ssidNamesList.Add(temporaryName);//ultimul SSID

            DataTable table = new DataTable();
            table.Columns.Add("SSID (WiFi Name)");
            foreach (var ssidName in ssidNamesList)
            {
                table.Rows.Add(ssidName);
            }
            dataGridView1.DataSource = table;

            this.dataGridView1.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.Sort(this.dataGridView1.Columns["SSID (WiFi Name)"], ListSortDirection.Descending);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
                ssidTextBox1.Text = row.Cells["SSID (WiFi Name)"].Value.ToString();
            }
            catch{}
        }

    }
}
