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
using System.Configuration;
using System.Security.Policy;

namespace cs2resfix
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int WIDTH_LINE = 9;
        private int HEIGHT_LINE = 10;
        private int FULLSCREEN_LINE = 13;
        private int BORDERLESS_LINE = 15;
        private int REFRESH_RATE_LINE = 11;   
        private int REFRESH_RATE_DENOM_LINE = 12;
        private int ASPECT_RATIO_LINE = 53;
        
        
        private static List<DirectoryInfo> cs2Dirs = new List<DirectoryInfo>();
        private DirectoryInfo steamDirectory;
        private DirectoryInfo csgoDirectory;


        public List<DirectoryInfo> Cs2Dirs { get { return cs2Dirs; } }
        private string _universalCfgPath;
        public string universalCfgPath
        {
            get { return _universalCfgPath; }
            set { _universalCfgPath = value; }
        }


        public MainWindow()
        {
            InitializeComponent();
            ApplyConfigSettings();
        }

        private void ApplyConfigSettings()
        {
            //ENABLE INTERPOLATION SETTINGS
            string enableInterpCfgVal = ConfigurationManager.AppSettings["enableInterpSettings"];
            string enableUniversalCfgPath = ConfigurationManager.AppSettings["enableUniversalCfgPath"];
            if (enableInterpCfgVal == "false")
                CsgoLocationButton.IsEnabled = false;
            if (enableUniversalCfgPath == "true")
            {
                CsgoLocationButton.IsEnabled = false;
                SteamLocationButton.IsEnabled = false;
                GetUniversalCfgPath();
            }


            

        }
        private void GetUniversalCfgPath()
        {
            if (Environment.GetEnvironmentVariable("USRLOCALCSGO") == null)
            {
                System.Windows.MessageBox.Show("Universal CSGO environment variable not found. Please turn off the option in config.");
                //exit app
                this.Close();
                return;
            }
            Console.WriteLine(Environment.GetEnvironmentVariable("USRLOCALCSGO"));
            universalCfgPath = Environment.GetEnvironmentVariable("USRLOCALCSGO");
            //check if universal cfg path exists
            if (!new DirectoryInfo(universalCfgPath).Exists)
            {
                MessageBoxResult dr = System.Windows.MessageBox.Show($"Universal CSGO config path hasn't been found in spite of having environment variable set.\n\n Your USRLOCALCSGO environment variable refers to: \n{universalCfgPath} \n\n Click 'OK' to create the directories","cs2resfix",MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (dr == MessageBoxResult.OK)
                {
                    try
                    {
                        if (!Directory.Exists(universalCfgPath))
                            Directory.CreateDirectory(universalCfgPath);
                        if (!Directory.Exists(universalCfgPath + "/cfg"))
                            Directory.CreateDirectory(universalCfgPath + "/cfg");
                    }
                    catch
                    {
                        System.Windows.MessageBox.Show("There was a problem with creating the directories");
                        this.Close();
                        return;
                    }
                }
                else
                {
                    this.Close();
                    return;
                }
            }

            //enable appropiate fields
            steamDirectory = new DirectoryInfo(universalCfgPath + "/cfg/");
            csgoDirectory = new DirectoryInfo(universalCfgPath + "/cfg/");
            universalCfgEnabledText.Visibility = Visibility.Visible;
            AccessCs2Cfg("None");
            AccessAutoexecSettings(true);

        }
        private void OpenInfoWindow(object sender, RoutedEventArgs e)
        {
            InfoWindow infoWindow = new InfoWindow();
            infoWindow.Show();
        }

        private void OpenSteamFileBrowser(object sender, RoutedEventArgs e)
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

        public void AccessCs2Cfg(string steamId="None")
        {
            if (!steamId.Equals("None"))
            {
                steamDirectory = new DirectoryInfo(steamDirectory.FullName + $"/{steamId}/730/local/cfg/");
                FileInfo cs2cfg = new FileInfo(steamDirectory.FullName + "cs2_video.txt");
                Console.Write(cs2cfg.FullName);
                if (!cs2cfg.Exists)
                {
                    MessageBoxResult dr = System.Windows.MessageBox.Show("cs2_video.txt not found \n\n Run CS2 to create the file", "cs2resfix", MessageBoxButton.OK, MessageBoxImage.Warning);
                    this.Close();
                    return;

                }
            }
            else
            {
                steamDirectory = new DirectoryInfo(universalCfgPath + "/cfg/");
                FileInfo cs2cfg = new FileInfo(steamDirectory.FullName + "cs2_video.txt");
                Console.Write(cs2cfg.FullName);
                if (!cs2cfg.Exists)
                {
                    MessageBoxResult dr = System.Windows.MessageBox.Show("cs2_video.txt not found \n\n Click 'OK' to create the file", "cs2resfix", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (dr == MessageBoxResult.OK)
                    {
                        try
                        {
                            if (!Directory.Exists(steamDirectory.FullName))
                                Directory.CreateDirectory(steamDirectory.FullName);
                            using (File.Create(cs2cfg.FullName)) { }
                        }
                        catch
                        {
                            System.Windows.MessageBox.Show("There was a problem with creating the file");
                            this.Close();
                            return;
                        }
                    }
                    else
                    {
                        this.Close();
                        return;
                    }
                }
                CsgoLocationButton.IsEnabled = false;
            }

            FillUIWithCfgData();

            CfgFullscreen.IsEnabled = true;
            CfgWidth.IsEnabled = true;
            CfgHeight.IsEnabled = true;
            CfgRefreshRate.IsEnabled = true;
            ApplyButton.IsEnabled = true;
            OpenCs2DirButton.IsEnabled = true;
            AspectRatioComboBox.IsEnabled = true;
            


        }
        private void FillUIWithCfgData()
        {
            //find values in cs2_video.txt and fill the fields in the app's ui

            string[] fileLines = File.ReadAllLines(steamDirectory.FullName + "cs2_video.txt");
            for (int i = 0; i < fileLines.Length; i++)
            {
                if (fileLines[i].Contains("setting.defaultres\""))
                    CfgWidth.Text = fileLines[i].Split('"')[3];
                if (fileLines[i].Contains("setting.defaultresheight"))
                    CfgHeight.Text = fileLines[i].Split('"')[3];
                if (fileLines[i].Contains("setting.fullscreen"))
                {
                    if (fileLines[i].Split('"')[3].Equals("1"))
                        CfgFullscreen.IsChecked = true;
                    else
                        CfgFullscreen.IsChecked = false;
                }
                if (fileLines[i].Contains("setting.refreshrate_numerator"))
                    CfgRefreshRate.Text = fileLines[i].Split('"')[3];
                if (fileLines[i].Contains("setting.aspectratiomode"))
                {
                    switch (fileLines[i].Split('"')[3])
                    {
                        case "0":
                            AspectRatioComboBox.SelectedIndex = 0;
                            break;
                        case "1":
                            AspectRatioComboBox.SelectedIndex = 1;
                            break;
                        case "2":
                            AspectRatioComboBox.SelectedIndex = 2;
                            break;
                    }
                }
            }
        }
        private void FindCs2CfgLinesIndex(FileInfo cs2cfg)
        {
            // search through the file for the lines we need to modify
            string[] fileLines = File.ReadAllLines(cs2cfg.FullName);
            for (int i = 0; i < fileLines.Length; i++)
            {
                if (fileLines[i].Contains("setting.defaultres\""))
                    WIDTH_LINE = i;
                if (fileLines[i].Contains("setting.defaultresheight"))
                    HEIGHT_LINE = i;
                if (fileLines[i].Contains("setting.fullscreen"))
                    FULLSCREEN_LINE = i;
                if (fileLines[i].Contains("setting.nowindowborder"))
                    BORDERLESS_LINE = i;
                if (fileLines[i].Contains("setting.refreshrate_numerator"))
                    REFRESH_RATE_LINE = i;
                if (fileLines[i].Contains("setting.refreshrate_denominator"))
                    REFRESH_RATE_DENOM_LINE = i;
                if (fileLines[i].Contains("setting.aspectratiomode"))
                    ASPECT_RATIO_LINE = i;
            }
        }

        private void ModifyCs2Cfg(FileInfo cs2cfg)
        {
            FindCs2CfgLinesIndex(cs2cfg);
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
        private void OpenAutoexecDirectory_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer", csgoDirectory.FullName);
        }

        private void HyperlinkGithub_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void OpenCsgoFileBrowser(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowDialog();

            csgoDirectory = new DirectoryInfo(folderBrowserDialog.SelectedPath + "/csgo/cfg");

            if (!csgoDirectory.Exists)
            {
                System.Windows.MessageBox.Show("Pick valid CS:GO directory.");
                return;
            }

            AccessAutoexecSettings();
        }

        private void AccessAutoexecSettings(bool isUniversalCfgenabled = false)
        {
            if (isUniversalCfgenabled)
            {
                CsgoLocationButton.IsEnabled = false;
            }

            //open autoexec.cfg
            FillUiWithAutoexecData(isUniversalCfgenabled);

            interpRatio.IsEnabled = true;
            updateRate.IsEnabled = true;
            allowInterp.IsEnabled = true;
            interp.IsEnabled = true;
            ModifyAutoexecButton.IsEnabled = true;
            OpenAutoexecDirectoryButton.IsEnabled = true;
            
        }

        private void FillUiWithAutoexecData(bool isUniversalCfgenabled = false)
        {
            FileInfo autoexecInfo;
            if (isUniversalCfgenabled)
            {
                autoexecInfo = new FileInfo(csgoDirectory.FullName + "/autoexec.cfg");
            }
            else
            {
                autoexecInfo = new FileInfo(universalCfgPath + "/cfg/autoexec.cfg");
            }
            //set autoexec values to apps ui
            string[] fileLines = File.ReadAllLines(autoexecInfo.FullName);
            for (int i = 0; i < fileLines.Length; i++)
            {
                if (fileLines[i].Contains("cl_interp_ratio"))
                {
                    interpRatio.IsChecked = true;
                }
                if (fileLines[i].Contains("cl_updaterate"))
                {
                    updateRate.IsChecked = true;
                }
                if (fileLines[i].Contains("cl_interp "))
                {
                    allowInterp.IsChecked = true;
                    if (fileLines[i].Contains("0.015625"))
                    {
                        interp.SelectedIndex = 0;
                    }
                    else if (fileLines[i].Contains("0.046875"))
                    {
                        interp.SelectedIndex = 1;
                    }
                    else
                    {
                        interp.SelectedIndex = 2;
                    }
                }
            }
        }

        private void ModifyAutoexec_Click(object sender, RoutedEventArgs e)
        {
            ModifyAutoexecData();
        }
        private void ModifyAutoexecData()
        {
            FileInfo autoexecInfo;
            if (universalCfgPath == null)
            {
                autoexecInfo = new FileInfo(csgoDirectory.FullName + "/autoexec.cfg");
            }
            else
            {
                autoexecInfo = new FileInfo(universalCfgPath + "/cfg/autoexec.cfg");
            }
            if (!autoexecInfo.Exists)
                CreateAutoexec(autoexecInfo);

            DeleteCurrentInterpolationConfigLines(autoexecInfo);

            string configModifier = "//THIS PART HAS BEEN ADDED BY cs2resfix - github.com/komeg1/cs2resfix";
            if (interpRatio.IsChecked == true)
                configModifier+= "\ncl_interp_ratio 1";
            if (updateRate.IsChecked == true)
                configModifier += "\ncl_updaterate 128";

            if (allowInterp.IsChecked == true)
            { 
                ComboBoxItem interpSettings = (ComboBoxItem)interp.SelectedItem;
                if (interpSettings.Content.ToString().Equals("cl__interp 0.015625 (stable/wired internet)"))
                {
                    configModifier += "\ncl_interp 0.015625";
                }
                else if(interpSettings.Content.ToString().Equals("cl_interp 0.046875 (default after 06.09.2023)"))
                {
                    configModifier += "\ncl_interp 0.046875";
                }
                else
                {
                    configModifier += "\ncl_interp 0.03125";
                }
            }
            configModifier += "\n//END OF cs2resfix PART";
            try
            {
                File.AppendAllText(autoexecInfo.FullName, configModifier);
                System.Windows.MessageBox.Show("Autoexec changed succesfully.","cs2resfix",MessageBoxButton.OK);
            }
            catch
            {
                System.Windows.MessageBox.Show("There was a problem with modifying autoexec.cfg");
            }



        }
        private void CreateAutoexec(FileInfo autoexecInfo)
        {
            using (File.Create(autoexecInfo.FullName)) { }
        }
        private void AllowInterp(object sender, RoutedEventArgs e)
        {
            if(allowInterp.IsChecked == true)
                interp.IsEnabled= true;
            else
                interp.IsEnabled= false;
        }

        private void DeleteCurrentInterpolationConfigLines(FileInfo autoexecInfo)
        {
            string[] fileLines = File.ReadAllLines(autoexecInfo.FullName);
            for(int i=0;i<fileLines.Length;i++)
            {
                if (fileLines[i].Contains("//THIS PART HAS BEEN ADDED BY cs2resfix - github.com/komeg1/cs2resfix"))
                    fileLines[i] = "";
                if (fileLines[i].Contains("//END OF cs2resfix PART"))
                    fileLines[i] = "";
                if (fileLines[i].Contains("cl_interp "))
                    fileLines[i] = "";
                if (fileLines[i].Contains("cl_interp_ratio"))
                    fileLines[i] = "";
                if (fileLines[i].Contains("cl_updaterate"))
                    fileLines[i] = "";
            }
            
            try
            {
                File.WriteAllLines(autoexecInfo.FullName, fileLines);
                //Delete blank lines
                File.WriteAllLines(autoexecInfo.FullName, File.ReadAllLines(autoexecInfo.FullName).
                    Where(e => !string.IsNullOrWhiteSpace(e)));
            }
            catch
            {
                System.Windows.MessageBox.Show("There was a problem with the autoexec.cfg file");
                this.Close();
                return;
            }



        }
    }
}
