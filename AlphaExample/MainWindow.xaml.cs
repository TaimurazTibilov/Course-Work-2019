using AlphaMiner;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace AlphaExample
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LogPathFinder_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "SQ3 file (*.sq3)|*.sq3";
            if (openFile.ShowDialog() == true)
            {
                LogPath.Text = openFile.FileName;
            }
        }

        private void DirectoryPathFinder_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DirectoryPath.Text = folderBrowser.SelectedPath;
            }
        }

        private void GetGraph_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Alpha alpha = new Alpha();
                alpha.StartAlpha(true, GraphName.Text, LogPath.Text, DirectoryPath.Text, GraphFileType.PNG);
                string image = DirectoryPath.Text + "\\" + GraphName.Text + ".png";
                if (File.Exists(image))
                {
                    var source = new BitmapImage();
                    source.BeginInit();
                    source.UriSource = new Uri(image);
                    source.EndInit();
                    Result.Source = source;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }
    }
}
