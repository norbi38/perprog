using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace perprog
{
    class BruteForce:INotifyPropertyChanged
    {
        ZipFile zip;
        string[] mainChars;
        string[] allChars;
        string filename;
        int minPwLength;
        int maxPwLength;
        int progress;
        bool kesz2;
        int threadId;
        int pwlength;

        public BruteForce(string filename, string[] mainChars,string[] allChars,int minPwLength,int maxPwLength, int threadId)
        {
            this.filename = filename;
            this.zip = ZipFile.Read(filename);
            this.mainChars = mainChars;
            this.allChars = allChars;
            this.minPwLength = minPwLength;
            this.maxPwLength = maxPwLength;
            progress = 0;
            kesz2 = false;
            this.ThreadId = threadId;
            pwlength = 1;
        }

        public int Progress
        {
            get => progress;
            set
            {
                progress = value;
                OnPropertyChanged("progress"+ThreadId);
            }
        }

        public bool Kesz
        {
            get => kesz2;
            set
            {
                kesz2 = value;
                if(value==true)
                    OnPropertyChanged("kesz");
                else
                    OnPropertyChanged("fail");
            }
        }

        public int ThreadId { get => threadId; set => threadId = value; }
        public int Pwlength { get => pwlength; set => pwlength = value; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged!=null)
            {
                PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
            }
        }

        public void Start(ref bool kesz)
        {
            string pw = "";
            string outdir =filename.Remove(filename.Length-4);
            bool sajat = false;
            while (filename[filename.Length - 1] != '\\')
            {
                filename = filename.Remove(filename.Length - 1);

            }
            try
            {
                zip.ExtractAll(filename,ExtractExistingFileAction.OverwriteSilently);
                kesz = true;
            }
            catch
            {                
                Pwlength = minPwLength;               
                int currChar;
                while(!kesz&&Pwlength<=maxPwLength)
                {
                    int i = 0;
                    while(!kesz&&i<mainChars.Length)
                    {
                        pw = mainChars[i];
                        currChar = Pwlength-1;
                        for (int j = 1; j < Pwlength; j++)
                        {
                            pw += allChars[0];
                        }
                        int k = 0;
                        try
                        {
                            if (ZipFile.CheckZipPassword(zip.Name, pw))
                            {
                                zip.Password = pw;
                                zip.ExtractAll(filename, ExtractExistingFileAction.OverwriteSilently);
                                kesz = true;
                                sajat = true;
                            }
                        }
                        catch { }
                        Progress++;
                        //try
                        //{
                        //    zip.Password = pw;
                        //    Progress++;
                        //    zip.ExtractAll(filename, ExtractExistingFileAction.OverwriteSilently);
                        //    kesz = true;
                        //}
                        //catch { }
                        while (!kesz&&currChar>0)
                        {
                            if (k < allChars.Length)
                            {
                                pw = pw.Remove(pw.Length - 1, 1) + allChars[k];
                                try
                                {
                                    if (ZipFile.CheckZipPassword(zip.Name, pw))
                                    {
                                        zip.Password = pw;
                                        zip.ExtractAll(filename, ExtractExistingFileAction.OverwriteSilently);
                                        kesz = true;
                                        sajat = true;
                                    }
                                }
                                catch { }
                                Progress++;
                                //try
                                //{
                                //    zip.Password = pw;
                                //    Progress++;                                    
                                //    zip.ExtractAll(filename, ExtractExistingFileAction.OverwriteSilently);

                                //    kesz = true;
                                //}
                                //catch { }
                                k++;
                            }
                            else
                            {
                                k = 0;
                                int n = 1;
                                while(pw[n] != allChars[allChars.Length - 1][0])
                                {
                                    n++;
                                }
                                currChar = n - 1;
                                if (currChar > 0)
                                {
                                    int m = 0;
                                    string valami = pw[currChar].ToString();
                                    while (valami != allChars[m])
                                    {
                                        m++;
                                    }
                                    pw = pw.Remove(currChar, Pwlength - currChar) + allChars[m + 1];
                                    for (int l = currChar + 1; l < Pwlength; l++)
                                    {
                                        pw += allChars[0];
                                    }
                                }
                            }
                        }
                        i++;
                    }
                    Pwlength++;
                }
  
            }
            if (sajat)
            {
                Kesz = true;
                MessageBox.Show("Sikeres kicsomagolás Jelszó: " + pw);
            }
            else
                Kesz = false;
        }
    }
}
