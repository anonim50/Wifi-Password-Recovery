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
            //* Read the output (or the error)
            string outputPassword = cmd1.StandardOutput.ReadToEnd();
            //modifica start!
            int start = outputPassword.IndexOf("Cipher") + "Cipher".Length;
            int end = outputPassword.LastIndexOf("\n");
            outputPassword = outputPassword.Substring(start, end - start);
            outputPassword = outputPassword.Replace(":", "").Replace(" ","").Trim();

            passwordTextBox2.Text = outputPassword;

            System.IO.File.AppendAllText(@"Save.txt", Environment.NewLine + "SSID: " + ssid + "   \nPassword:" + outputPassword + Environment.NewLine +
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
            ////* Read the output
            string output = cmd.StandardOutput.ReadToEnd();

            int start1 = output.IndexOf("User profiles") + "User profiles".Length + "-----------------".Length;
            int end1 = output.LastIndexOf("\n");
            string ssidOutput = output.Substring(start1, end1 - start1);
            ssidOutput = ssidOutput.Replace("All User Profile", "").Replace(":", "");

            List<string> ssidNamesList = new List<string>();
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
