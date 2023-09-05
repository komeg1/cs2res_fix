using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace cs2resfix
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int WIDTH_LINE = 9;
        private const int HEIGHT_LINE = 10;
        private const int FULLSCREEN_LINE = 13;
        private const int BORDERLESS_LINE = 15;
        private const int REFRESH_RATE_LINE = 11;
        private const int REFRESH_RATE_DENOM_LINE = 12;
        private const int ASPECT_RATIO_LINE = 53;
        
        
        private static List<DirectoryInfo> cs2Dirs = new List<DirectoryInfo>();
        private DirectoryInfo steamDirectory;


        public List<DirectoryInfo> Cs2Dirs { get { return cs2Dirs; } }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenInfoWindow(object sender, RoutedEventArgs e)
        {
            InfoWindow infoWindow = new InfoWindow();
            infoWindow.Show();
        }

        private void OpenFileBrowser(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowDialog();

            steamDirectory = new DirectoryInfo(folderBrowserDialog.SelectedPath + "/userdata");

            if (!steamDirectory.Exists)
            {
                System.Windows.MessageBox.Show("Pick valid Steam directory.");
                return;
            }

            FindCs2Data(steamDirectory);
        }

        private void FindCs2Data(DirectoryInfo steamDirectory)
        {
            cs2Dirs.Clear();
            foreach (DirectoryInfo directory in steamDirectory.GetDirectories())
            {
                if (Cs2DirExists(directory.FullName + "/730"))
                {
                    cs2Dirs.Add(directory);
                }
            }

            if (cs2Dirs.Count > 1)
            {
                ProfilePicker profilePicker = new ProfilePicker(this);
                profilePicker.ListSteamIds(cs2Dirs);
                profilePicker.ShowDialog();
                return;
            }
            SteamID.Text = cs2Dirs[0].Name;
            AccessCs2Cfg(SteamID.Text);
            

        }

        private bool Cs2DirExists(string dirPath)
        {
            if (new DirectoryInfo(dirPath).Exists)
                return true;
            return false;
        }

        public void AccessCs2Cfg(string steamId)
        {
            steamDirectory = new DirectoryInfo(steamDirectory.FullName + $"/{steamId}/730/local/cfg/");
            FileInfo cs2cfg = new FileInfo(steamDirectory.FullName + "cs2_video.txt");
            Console.Write(cs2cfg.FullName);
            if (!cs2cfg.Exists)
            {
                System.Windows.MessageBox.Show("cs2_video.txt not found");
                this.Close();
                return;
            }


            CfgFullscreen.IsEnabled = true;
            CfgWidth.IsEnabled = true;
            CfgHeight.IsEnabled = true;
            CfgRefreshRate.IsEnabled = true;
            ApplyButton.IsEnabled = true;
            OpenCs2DirButton.IsEnabled = true;
            AspectRatioComboBox.IsEnabled = true;
            


        }

        private void ModifyCs2Cfg(FileInfo cs2cfg)
        {
            string[] fileLines = File.ReadAllLines(cs2cfg.FullName);

            fileLines[WIDTH_LINE] = $"\t\"setting.defaultres\"\t\t\"{CfgWidth.Text}\"";
            fileLines[HEIGHT_LINE] = $"\t\"setting.defaultresheight\"\t\t\"{CfgHeight.Text}\"";
            fileLines[REFRESH_RATE_DENOM_LINE] = "\t\"setting.refreshrate_denominator\"\t\t\"1\"";
            fileLines[REFRESH_RATE_LINE] = $"\t\"setting.refreshrate_numerator\"\t\t\"{CfgRefreshRate.Text}\"";
            if (CfgFullscreen.IsChecked == true)
            {
                fileLines[FULLSCREEN_LINE] = "\t\"setting.fullscreen\"\t\t\"1\"";
                fileLines[BORDERLESS_LINE] = "\t\"setting.nowindowborder\"\t\t\"0\"";
            }
            else
            {
                fileLines[FULLSCREEN_LINE] = "\t\"setting.fullscreen\"\t\t\"0\"";
                fileLines[BORDERLESS_LINE] = "\t\"setting.nowindowborder\"\t\t\"1\"";
            }

            ComboBoxItem aspectRatio = (ComboBoxItem)AspectRatioComboBox.SelectedItem;
            string chosenAspectRatio = aspectRatio.Content.ToString();

            switch (chosenAspectRatio)
            {

                case "4:3":
                    fileLines[ASPECT_RATIO_LINE] = "\t\"setting.aspectratiomode\"\t\t\"0\"";
                    break;

                case "16:9":
                    fileLines[ASPECT_RATIO_LINE] = "\t\"setting.aspectratiomode\"\t\t\"1\"";
                    break;

                case "16:10":
                    fileLines[ASPECT_RATIO_LINE] = "\t\"setting.aspectratiomode\"\t\t\"2\"";
                    break;
            }
        
            try
            {
                File.WriteAllLines(cs2cfg.FullName, fileLines);
                System.Windows.MessageBox.Show("Options changed successfully. You can close the application.");
            }
            catch
            {
                System.Windows.MessageBox.Show("There was a problem with the cs2_video.txt file");
                this.Close();
                return;
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            ModifyCs2Cfg(new FileInfo(steamDirectory.FullName + "cs2_video.txt")); 
            
        }

        private void OpenCS2VideoDirectory_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer",steamDirectory.FullName);
        }

        private void HyperlinkGithub_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
