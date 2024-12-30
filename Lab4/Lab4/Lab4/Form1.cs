using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Lab4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadStartupPrograms();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void LoadStartupPrograms()
        {
            string globalStartupHtml = BuildStartupListHtml(GetAutoStartupProgramsGlobal(), "Global");
            string localStartupHtml = BuildStartupListHtml(GetAutoStartupProgramsLocal(), "Local");
            string schedulingTaskHtml = BuildTaskSchedulerHtml();

            webBrowser1.DocumentText = $@"
                <html>
                    <head>
                        <style>
                            table {{
                                border-collapse: collapse;
                                width: 50%;
                            }}
                            th, td {{
                                border: 1px solid #dddddd;
                                text-align: left;
                                padding: 8px;
                            }}
                            th {{
                                background-color: #f2f2f2;
                            }}
                        </style>
                    </head>
                    <body>
                        {globalStartupHtml}
                        {localStartupHtml}
                        {schedulingTaskHtml}
                    </body>
                </html>";
        }

        private string BuildStartupListHtml(List<string> programs, string location)
        {
            string html = $"<table><tr><th colspan='2'>{location}</th></tr>";
            foreach (string program in programs)
            {
                html += $"<tr><td>{program}</td></tr>";
            }
            html += "</table>";
            return html;
        }

        private List<string> GetAutoStartupProgramsGlobal()
        {
            List<string> programs = new List<string>();
            using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run"))
            {
                if (regKey != null)
                {
                    foreach (string program in regKey.GetValueNames())
                    {
                        programs.Add(regKey.GetValue(program).ToString());
                    }
                }
            }
            return programs;
        }

        private List<string> GetAutoStartupProgramsLocal()
        {
            List<string> programs = new List<string>();
            using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run"))
            {
                if (regKey != null)
                {
                    foreach (string program in regKey.GetValueNames())
                    {
                        programs.Add(regKey.GetValue(program).ToString());
                    }
                }
            }
            return programs;
        }

        private void btnAddToStart_Click(object sender, EventArgs e)
        {
            using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Exe Files (.exe)|*.exe|All Files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    regKey.SetValue($"New ", $"\"{openFileDialog.FileName}\"");
                }
            }
            LoadStartupPrograms();
        }
        private string BuildTaskSchedulerHtml()
        {
            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.Append("<h3>Scheduling Task</h3><ul>");

            try
            {
                using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Schedule\TaskCache\Tree"))
                {
                    if (regKey != null)
                    {
                        GetRegistryKeyValues(regKey, htmlBuilder);
                    }
                }
            }
            catch (Exception ex)
            {
                htmlBuilder.Append($"<li>Error: {ex.Message}</li>");
            }

            htmlBuilder.Append("</ul>");
            return htmlBuilder.ToString();
        }

        private void GetRegistryKeyValues(RegistryKey parentKey, StringBuilder htmlBuilder)
        {
            foreach (string subKeyName in parentKey.GetSubKeyNames())
            {
                using (RegistryKey subKey = parentKey.OpenSubKey(subKeyName))
                {
                    if (subKey != null)
                    {
                        htmlBuilder.Append($"<li>{subKey.Name}</li>");
                        GetRegistryKeyValues(subKey, htmlBuilder);
                    }
                }
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");
            if (key != null)
            {
                var valueNames = key.GetValueNames();
                if (valueNames.Length > 0)
                {
                    var valueName = valueNames[0];
                    var valueData = key.GetValue(valueName);

                    //запис
                    string regFilePath = "copyRegister.reg";
                    File.WriteAllText(regFilePath, $"Windows Registry Editor Version 5.00\r\n[{key.Name}]\r\n\"{valueName}\"=\"{valueData}\"");
                    MessageBox.Show($"Registry file successfully created: {regFilePath}");
                }
                else
                {
                    MessageBox.Show("Registry section is empty. File not created.");
                }
            }
            else
            {
                MessageBox.Show("Error accessing registry section. File not created.");
            }
        }
    }
}
