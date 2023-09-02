using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.IO;

namespace cs2resfix
{
    /// <summary>
    /// Logika interakcji dla klasy ProfilePicker.xaml
    /// </summary>
    public partial class ProfilePicker : Window
    {
        MainWindow windowRef;
        public ProfilePicker(MainWindow window)
        {
            windowRef = window;
            InitializeComponent();
        }

        public void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri){ UseShellExecute = true });
            e.Handled = true;
        }

        public void ListSteamIds(List<DirectoryInfo> dirs)
        {
            
            foreach (DirectoryInfo dir in dirs)
            {
                System.Windows.Controls.Button newId = new Button();

                newId.Content = dir.Name;
                newId.Name = "Button" + dir.Name;
                newId.Click += ChosenID;
                sp.Children.Add(newId);
            }

        }
        public void ChosenID(object sender, RoutedEventArgs e)
        {

            string chosenId = (e.Source as Button).Content.ToString();
            windowRef.SteamID.Text = chosenId;
            windowRef.AccessCs2Cfg(chosenId);
            this.Close();
            
        }
    }

    
}
