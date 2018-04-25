using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace perprog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string[] az = new string[] { "a","á","b", "c", "d", "e", "é", "f", "g", "h", "i", "í", "j", "k", "l", "m",
                                            "n","o","ó","ö","ő","p","q","r","s","t","u","ú","ü","ű","v","w","x","y","z"};
        static string[] AZ = new string[] {"A","Á","B", "C", "D", "E", "É", "F", "G", "H", "I", "Í", "J", "K", "L", "M",
                                            "N","O","Ó","Ö","Ő","P","Q","R","S","T","U","Ú","Ü","Ű","V","W","X","Y","Z"};
        static string[] szamok = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        Stopwatch sw;
        DispatcherTimer dt;
        BruteForce[] bfs;
        int[] progress;
        ProgressBar[] pBar;
        bool kesz;
        int threads;

        public MainWindow()
        {
            InitializeComponent();
            lbl_time.Content = String.Format("{0:00}:{1:00}:{2:00}",
                0, 0, 0);
            sw = new Stopwatch();
            dt = new DispatcherTimer();
            dt.Tick += Dt_Tick;
            dt.Interval = new TimeSpan(0, 0, 0, 0, 10);           
            kesz = false;
        }

        private void Dt_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = sw.Elapsed;
            string currentTime = String.Format("{0:00}:{1:00}:{2:00}",
            ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            lbl_time.Content = currentTime;           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".zip";
            Nullable<bool> result = dlg.ShowDialog();
            if (result==true)
            {
                string filename = dlg.FileName;
                l_filename.Content = filename;
                l_filename.Visibility = Visibility.Visible;
                btn_Start.IsEnabled = true;
            }
            
        }
        private void AddProgressBar(int cpuNumber,int allLenght, int mainLenght,int main2Lenght)
        {
            double progmax = 0;
            int min = txtMin.Text != "" ? int.Parse(txtMin.Text) : 1;
            int max = txtMax.Text != "" ? int.Parse(txtMax.Text) : 100;
            progress = new int[cpuNumber];

            if (pBar == null)
            {

                pBar = new ProgressBar[cpuNumber];

                for (int i = 0; i < cpuNumber; i++)
                {
                    progmax = 0;
                    for (int j = min; j <= max; j++)
                    {
                        int mainComb = i != cpuNumber - 1 ? mainLenght : main2Lenght;
                        progmax += mainComb * Math.Pow(allLenght, j - 1);
                    }

                    pBar[i] = new ProgressBar();

                    pBar[i].Maximum = progmax;
                    pBar[i].Visibility = Visibility.Visible;
                    pBar[i].Height = 22;
                    pBar[i].Width = this.Width;
                    pBar[i].VerticalAlignment = VerticalAlignment.Top;
                    pBar[i].HorizontalAlignment = HorizontalAlignment.Left;
                    pBar[i].Margin = new Thickness(0, 5, 0, 0);

                    myGrid.RowDefinitions.Add(new RowDefinition());
                    myGrid.Children.Add(pBar[i]);
                    Grid.SetRow(pBar[i], myGrid.RowDefinitions.Count - 1);
                }
            }
            else
            {
                for (int i = 0; i < cpuNumber; i++)
                {
                    progmax = 0;
                    for (int j = min; j <= max; j++)
                    {
                        int mainComb = i != cpuNumber - 1 ? mainLenght : main2Lenght;
                        progmax += mainComb * Math.Pow(allLenght, j - 1);
                    }
                    pBar[i].Maximum = progmax;
                }
            }
            
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            kesz = false;
            string[] all = new string[0]; 
            if (chkBox1.IsChecked==true)
            {
                all = all.Union(az).ToArray();
            }
            if (chkBox2.IsChecked == true)
            {
                all = all.Union(AZ).ToArray();
            }
            if (chkBox3.IsChecked == true)
            {
                all = all.Union(szamok).ToArray();
            }

            char[] egye = txtOthers.Text.ToCharArray();          
            if(egye!=null)
            {
                string[] egyeb = new string[egye.Length];
                for (int i = 0; i < egye.Length; i++)
                {
                    egyeb[i] = egye[i].ToString();
                }
                all = all.Union(egyeb).ToArray();
            }
            int min = txtMin.Text != "" ? int.Parse(txtMin.Text) : 1;
            int max = txtMax.Text != "" ? int.Parse(txtMax.Text) : 100;

            if (rdb_Max.IsChecked == true)
            {
                
                int cpuNumber= Environment.ProcessorCount;
                threads = cpuNumber;
                int a = all.Length / cpuNumber;
                Task[] tasks = new Task[cpuNumber];
                bfs = new BruteForce[cpuNumber];
                if (all.Length % cpuNumber==0)
                {
                    string[] mai = new string[a];
                    for (int i = 0; i < cpuNumber; i++)
                    {                       
                        Array.Copy(all, i * a, mai, 0, a);
                        bfs[i] = new BruteForce(l_filename.Content.ToString(), mai, all, min, max,i);
                        bfs[i].PropertyChanged += Bf_PropertyChanged;
                    }
                    AddProgressBar(cpuNumber, all.Length,mai.Length,mai.Length);
                }
                else
                {
                    string[] mai = new string[a];
                    for (int i = 0; i < cpuNumber-1; i++)
                    {                       
                        Array.Copy(all, i * a, mai, 0, a);
                        bfs[i] = new BruteForce(l_filename.Content.ToString(), mai, all, min, max,i);
                        bfs[i].PropertyChanged += Bf_PropertyChanged;
                    }
                    string[] mai2 = new string[a + all.Length % cpuNumber];
                    Array.Copy(all, (cpuNumber-1) * a, mai2, 0, a+all.Length%cpuNumber);
                    bfs[cpuNumber-1] = new BruteForce(l_filename.Content.ToString(), mai2, all, min, max,cpuNumber-1);
                    bfs[cpuNumber - 1].PropertyChanged += Bf_PropertyChanged;
                    AddProgressBar(cpuNumber, all.Length, mai.Length,mai2.Length);
                }
                for (int i = 0; i < cpuNumber; i++)
                {
                    int tmp = i;
                    tasks[tmp] = new Task(() => 
                        bfs[tmp].Start(ref kesz), 
                        TaskCreationOptions.LongRunning);
                    tasks[tmp].Start();
                }
            }
            else
            {
                AddProgressBar(1,all.Length,all.Length,all.Length);
                BruteForce bf = new BruteForce(l_filename.Content.ToString(), all, all, min, max,0);
                bf.PropertyChanged += Bf_PropertyChanged;

                Task t = Task.Run(() =>
                {
                    bf.Start(ref kesz);
                });               
            }

            btn_Start.IsEnabled = false;
            btn_Stop.IsEnabled = true;
            dt.Start();
            sw.Start();
        }

        private void Bf_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            int id = (sender as BruteForce).ThreadId;
            string valami = e.PropertyName == "kesz" || e.PropertyName == "fail" ? e.PropertyName : "progress";
            switch (valami)
            {
                case "kesz":
                    dt.Stop();
                    sw.Stop();
                    sw.Reset();
                    break;
                case "progress" :
                    this.Dispatcher.Invoke(() =>
                    {                        
                        progress[id]++;
                        pBar[id].Value = progress[id];
                    });
                    break;
                case "fail":
                    threads--;
                    if(threads==0)
                    {
                        dt.Stop();
                        sw.Stop();
                        sw.Reset();
                        MessageBox.Show("Nem talált jelszót!");
                    }
                    break;
            }            
                   
        }

        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            kesz = true;
            btn_Start.IsEnabled = true;
            btn_Stop.IsEnabled = false;
            int[] pwLenghts = new int[bfs.Length];
            for (int i = 0; i < bfs.Length; i++)
            {
                pwLenghts[i] = bfs[i].Pwlength;
            }
            if (pwLenghts.Average() == (double)pwLenghts[0])
            {
                MessageBox.Show("Keresés megállítva!\nUtoljára keresett jelszóhossz: " + pwLenghts[0]);
            }
            else
            {
                MessageBox.Show("Keresés megállítva!\nUtoljára keresett jelszóhossz: " + pwLenghts.Min());
            }
        }
    }
}
