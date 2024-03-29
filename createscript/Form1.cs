﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using createscript;
using System.Threading.Tasks;
using System.Threading;

namespace myspace
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            tb_time.Text = "8";
            lb_status.Text = "Ready to start";
        }

        private void bt_start_Click(object sender, EventArgs e)
        {   
            LogFileCreator myLogFile = new LogFileCreator();
            ITextFileReadable listOfModels = new TextFileReader("List of model pathes.txt");

            try
            {
                listOfModels.ReadFile();
            }
            catch(Exception ex)
            {
                myLogFile.WriteText($"ERROR: {ex.Message};\n");
            }
            
            List<string> modelList = listOfModels.GetListFromFile();

            if(modelList.Count != 0)
            {
                string timeToWaitForOpening = tb_time.Text;

                foreach(var model in modelList)
                {
                    string[] teklaVersion = model.Split(',');
                    
                    try
                    {
                        ChangeEXE(model, timeToWaitForOpening, teklaVersion[0]);
                    }
                    catch(Exception ex)
                    {
                        myLogFile.WriteText($"ERROR: {ex.Message};\n");
                    }      
                }

                myLogFile.WriteText($"All models are downloaded;\n");
                MessageBox.Show("Finished"); 
            }
            else
            {
                myLogFile.WriteText($"The list of models is empty. Downloading can not be started;\n");
            } 
        }

       /*  private void button2_Click(object sender, EventArgs e)
        {   

        } */

        public void ChangeEXE(string model, string timeToWaitForOpening, string teklaVersion)
        {
            Process proc = new Process();
            string teklaExePath = SettingsHandler.GetTeklaEXEPath(teklaVersion);
            string teklaBypassPath = SettingsHandler.GetBypassPath(teklaVersion);

            if(teklaVersion == "2020"|| teklaVersion == "2019")
            {
                proc.StartInfo.FileName = GetEXEpath() + $"Tekla{teklaVersion}modelHandler.exe";
                proc.StartInfo.Arguments = $"\"{model}\" " + $"\"{timeToWaitForOpening}\" " + $"\"{teklaExePath}\" " + $"\"{teklaBypassPath}\" ";
                proc.Start();

                proc.WaitForExit();
            }
            else
            {
                throw new Exception($"The model {model} in the list has a version which is not supported.");
            }
        }
        public static string GetEXEpath()
        {
            var PathEXEfileRaw = System.Reflection.Assembly.GetEntryAssembly().Location;
            string EXEpath = System.IO.Path.GetDirectoryName(PathEXEfileRaw)+"\\";
            return  EXEpath;
        }
    }
}
