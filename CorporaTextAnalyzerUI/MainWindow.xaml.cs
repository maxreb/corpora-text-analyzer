using CorporaTextAnalyzer;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace CorporaTextAnalyzerUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static Analyzer _CTA = null;
        public MainWindow()
        {
            InitializeComponent();
            InputFile.TextChanged += InputFile_TextChanged;
            Database.Text = System.IO.Path.GetFullPath("database/deu_mixed-typical_2011_100K-words.txt");
        }



        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            string database = Database.Text;
            string input = InputFile.Text;
            string output = OutputFile.Text;
            if (File.Exists(output))
            {
                if (MessageBox.Show("Output file already exist. Overwrite?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    return;

            }
            if (!File.Exists(database)) { MessageBox.Show("Database file does not exist."); return; }
            if (!File.Exists(input)) { MessageBox.Show("Input file does not exist."); return; }
            StartBut.IsEnabled = false;
            Database.IsEnabled = false;
            DatabaseBut.IsEnabled = false;

            ProgressBar1.IsIndeterminate = true;

            await Task.Factory.StartNew(() =>
            {
                if (_CTA == null)
                {
                    _CTA = new Analyzer();
                    _CTA.Initalize(database);
                    _CTA.SetProgressFunction(new Progress<int>(percentage => ProgressBar1.Dispatcher.Invoke(() => ProgressBar1.Value = percentage)));

                }
                ProgressBar1.Dispatcher.Invoke(() => ProgressBar1.IsIndeterminate = false);
                _CTA.AnalyzeFile(input, output);


            });

            MessageBox.Show("Completed!");
            ProgressBar1.Value = 100;
            StartBut.IsEnabled = true;
        }

        private void InputFile_TextChanged(object sender, TextChangedEventArgs e)
        {
            OutputFile.Text = System.IO.Path.ChangeExtension(InputFile.Text, null) + "-output.csv";
        }

        private void DatabaseBut_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "TXT Files|*.txt"
            };
            if (ofd.ShowDialog() == true)
            {
                Database.Text = ofd.FileName;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "CSV Files|*.csv"
            };
            if (ofd.ShowDialog() == true)
            {
                InputFile.Text = ofd.FileName;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "CSV Files|*.csv"
            };
            if (ofd.ShowDialog() == true)
            {
                OutputFile.Text = ofd.FileName;
            }
        }
    }
}
