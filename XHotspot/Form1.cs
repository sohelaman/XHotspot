using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XHotspot
{
    public partial class Form1 : Form
    {
        const int START = 1;
        const int STOP = 2;
        const int ALLOW = 3;
        int buttonAction = 0;

        public Form1()
        {
            InitializeComponent();
            string s = ExecuteCommand("netsh wlan show hostednetwork");
            string allow = "";
            if (s.Contains("Mode"))
            {
                StringReader sr = new StringReader(s);
                while (sr.Peek() >= 0)
                {
                    string tmp = sr.ReadLine();
                    if (tmp.Contains("Mode"))
                    {
                        allow = tmp.Replace(" ", String.Empty);
                        break;
                    }
                }
            }

            if (!allow.Contains(":Allowed"))
            {
                buttonAction = ALLOW;
                button1.Text = "Allow";
                textBox3.Text = "Not allowed";
            }
            else
            {
                textBox3.Text = "Howdy, buddy?";
                string status = "";
                if (s.Contains("Status  "))
                {
                    StringReader sr2 = new StringReader(s);
                    while (sr2.Peek() >= 0)
                    {
                        string tmp2 = sr2.ReadLine();
                        if (tmp2.Contains("Status  "))
                        {
                            status = tmp2.Replace(" ", String.Empty);
                            break;
                        }
                    }
                }
                //textBox3.Text = status;
                if (status.Contains(":Started"))
                {
                    buttonAction = STOP;
                    button1.Text = "Stop";
                    textBox3.Text = "Hotspot running";
                }
                else
                {
                    buttonAction = START;
                    button1.Text = "Start";
                    textBox3.Text = "Hotspot not running";
                }
            }

            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (buttonAction == ALLOW)
            {
                button1.Text = "Start";
                buttonAction = START;
                MessageBox.Show(ExecuteCommand("netsh wlan set hostednetwork mode=allow"), "Result");

            }
            else if (buttonAction == START)
            {
                string ssid, pass;
                ssid = textBox1.Text;
                pass = textBox2.Text;
                string validation = validateCredintials(ssid, pass);
                //textBox3.Text = validation;
                string s = ExecuteCommand("netsh wlan show hostednetwork");
                string allow = "";
                if (s.Contains("Mode"))
                {
                    StringReader sr = new StringReader(s);
                    while (sr.Peek() >= 0)
                    {
                        string tmp = sr.ReadLine();
                        if (tmp.Contains("Mode"))
                        {
                            allow = tmp.Replace(" ", String.Empty);
                            break;
                        }
                    }
                }

                if (allow.Contains(":Allowed"))
                {
                    if (validation.Equals("OK"))
                    {

                        string command = "netsh wlan set hostednetwork mode=allow ssid="+ssid+" key="+pass;
                        string abc = ExecuteCommand(command);
                        if(abc.Contains("successfully")) {
                            string res = ExecuteCommand("netsh wlan start hostednetwork");
                            if (res.Contains("started"))
                            {
                                buttonAction = STOP;
                                button1.Text = "Stop";
                                textBox3.Text = "Hotspot started";
                                MessageBox.Show(res, "Result");
                            }
                            else
                            {
                                textBox3.Text = "Error starting hotspot";
                                MessageBox.Show(res, "Result");
                            }
                        }
                        else
                        {
                            textBox3.Text = "Couldn't setup hotspot";
                            MessageBox.Show(abc, "Result");
                        }
                        
                    }
                    else
                    {
                        textBox3.Text = validation;
                    }
                }
                else
                {
                    textBox3.Text = "Hotspot not allowed";
                    button1.Text = "Allow";
                    buttonAction = ALLOW;
                }
            }
            else if (buttonAction == STOP)
            {
                buttonAction = START;
                button1.Text = "Start";
                textBox3.Text = "Hotspot not running";
                MessageBox.Show(ExecuteCommand("netsh wlan stop hostednetwork"), "Result");
            }
            else
            {
                MessageBox.Show("Something really bad happened!", "OMG");
            }
            
            
        }

        private void label1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Created by Sohel Aman \nsohelamankhan@gmail.com", "About");
        }

        private string ExecuteCommand(string command)
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

        private string validateCredintials(string id, string pas)
        {
            bool ssidOK = false;
            if (id.Length < 4)
            {
                return "SSID too short";
            }
            else
            {
                char first = id[0];
                int val=(int)first;
                if ((val >= 65 && val <= 90) || (val >= 97 && val <= 122))
                {
                    ssidOK = true;
                }
                else
                {
                    return "Invalid SSID";
                }
            }
            if (ssidOK)
            {
                if (pas.Length < 8)
                {
                    return "Password too short";
                }
            }
            return "OK";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(ExecuteCommand("netsh wlan show hostednetwork"), "Information");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string s1 = ExecuteCommand("netsh wlan show drivers");
            string s2 = "";
            if (s1.Contains("Interface name"))
            {
                StringReader sr = new StringReader(s1);
                while (sr.Peek() >= 0)
                {
                    string tmp = sr.ReadLine();
                    string tmp2 = Regex.Replace(tmp, @"\s+", " ");
                    s2 += tmp2 + "\n\r";
                    if (tmp.Contains("Hosted network supported"))
                    {
                        break;
                    }
                }

            }
            //MessageBox.Show(ExecuteCommand("netsh wlan show drivers"), "WLAN Details");
            MessageBox.Show(s2, "Details");
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

    }
}
