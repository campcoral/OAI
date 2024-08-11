﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OAI
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            string pickedFilePath = null;
            button1.Click += (sender, e) => {
                 OpenFileDialog ofd = new OpenFileDialog();
                 ofd.Filter = "APK files (*.apk)|*.apk";
            
                 if(ofd.ShowDialog() == DialogResult.OK) {
                     textBox1.Text = ofd.FileName;
                     pickedFilePath = ofd.FileName;
                 }
            };
            button2.Click += (sender, e) =>
            {
                InstallApp(button2, pickedFilePath);
            };

            var thread = new Thread(async () =>
            {
                while (true)
                {
                    if (CheckADBConnection())
                    {
                        label2.Text = "Connected";
                        label2.ForeColor = Color.Green;
                        button2.Enabled = true;
                    }
                    else
                    {
                        label2.Text = "Disconnected";
                        label2.ForeColor = Color.Red;
                        button2.Enabled = false;
                    }
                    await Task.Delay(500);   
                }
            });
            thread.Start();
        }

        private static void InstallApp(Button button, string path)
        {
            string adbCommand = $"adb install --bypass-low-target-sdk-block {path}";
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {adbCommand}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();
                button.Text = "Wait...";
                
                process.WaitForExit();
                button.Text = "Install";
            }
        }
        private static bool CheckADBConnection()
        {
            try
            {
                string adbCommand = "adb devices";
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {adbCommand}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    StringReader st = new StringReader(output);
                    st.ReadLine();
                    var x = st.ReadLine();
                    return x.Contains("device");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return false;
            }
        }
    }
}