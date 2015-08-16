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

namespace XHotspot
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox3.Text = "Welcome!";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string s = ExecuteCommand("netsh wlan show hostednetwork");
            if (s.Contains("Mode"))
            {
                StringReader sr = new StringReader(s);
                while (sr.Peek() >= 0)
                {
                    string tmp = sr.ReadLine();
                    if (tmp.Contains("Mode"))
                    {
                        textBox3.Text = tmp.Replace(" ", String.Empty);
                        break;
                    }
                }

            }
            //textBox3.Text = 
        }

        private void label1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Created by Sohel Aman \nsohelamankhan@gmail.com!", "About");
        }

        string ExecuteCommand(string command)
        {
            int exitCode;
            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            process = Process.Start(processInfo);
            process.WaitForExit();

            // *** Read the streams ***
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            exitCode = process.ExitCode;

            string outText = (String.IsNullOrEmpty(output) ? "None" : output);
            string errorText = "error>>" + (String.IsNullOrEmpty(error) ? "Error occured" : error);
            //textBox3.Text = "ExitCode: " + exitCode.ToString();
            process.Close();
            return outText;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(ExecuteCommand("netsh wlan show hostednetwork"), "Information");
        }

        //static void Main()
        //{
        //    ExecuteCommand("echo testing");
        //}
    }
}
