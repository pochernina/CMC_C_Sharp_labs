using System;
using System.Windows;
using System.Windows.Controls;
using ClassLibrary;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow: Window
    {
        public ViewData vdata { get; set; }
        public VMGrid grid_app { get; set; } = new(5, 2.2, 4.4, VMf.vmdTan);

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            vdata = new();
        }

        private void New_command(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DisplaySaveFileMessage())
                {
                    vdata.benchmark.time_collection.Clear();
                    vdata.benchmark.accuracy_collection.Clear();
                    vdata.VMBenchmarkChanged = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in New command: {ex.Message}.", "New error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Open_command(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DisplaySaveFileMessage())
                {
                    Microsoft.Win32.OpenFileDialog dial = new Microsoft.Win32.OpenFileDialog();
                    dial.FileName = "";
                    dial.DefaultExt = ".txt";
                    dial.Filter = "Text documents (.txt)|*.txt";

                    bool? result = dial.ShowDialog();

                    if (result == true)
                    {
                        bool status = vdata.Load(dial.FileName);
                        vdata.VMBenchmarkChanged = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in Open command: {ex.Message}.", "Open error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveAs_command(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dial = new Microsoft.Win32.SaveFileDialog();
                dial.FileName = "text_doc";
                dial.DefaultExt = ".txt";
                dial.Filter = "Text documents (.txt)|*.txt";
                bool? result = dial.ShowDialog();

                if (result == true)
                {
                    bool status = vdata.Save(dial.FileName);
                    vdata.VMBenchmarkChanged = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in SaveAs command: {ex.Message}.", "Save error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu_item = (MenuItem)e.Source;
            try
            {
                if (menu_item.Header.ToString() == "Add VMTime")
                {
                    vdata.benchmark.AddVMTime(grid_app);
                }
                else if (menu_item.Header.ToString() == "Add VMAccuracy")
                {
                    vdata.benchmark.AddVMAccuracy(grid_app);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in MenuItem_Click: {ex.Message}.", "Menu error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool DisplaySaveFileMessage()
        {
            try
            {
                if (vdata.VMBenchmarkChanged)
                {
                    MessageBoxResult user_choice = MessageBox.Show($"Сохранить изменения в файл?", "WpfApp", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (user_choice == MessageBoxResult.Cancel) { return false; }
                    else if (user_choice == MessageBoxResult.Yes)
                    {
                        Microsoft.Win32.SaveFileDialog dial = new Microsoft.Win32.SaveFileDialog();
                        dial.FileName = "text_doc";
                        dial.DefaultExt = ".txt";
                        dial.Filter = "Text documents (.txt)|*.txt";

                        bool? result = dial.ShowDialog();

                        if (result == true)
                        {
                            string filename = dial.FileName;
                            bool status = vdata.Save(filename);
                            return status;
                        }
                        else { return false; }
                    }
                    else if (user_choice == MessageBoxResult.No) { return true; }
                    else { return false; }
                }
                else { return true; } // no changes
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in DisplaySaveFileMessage: {ex.Message}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (DisplaySaveFileMessage()) { base.OnClosing(e); }
        }
    }
}


