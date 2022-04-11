using System;
using System.IO;
using System.Windows;
using System.ComponentModel;
using ClassLibrary;

namespace WpfApp
{
    public class ViewData: INotifyPropertyChanged
    {
        public VMBenchmark benchmark { get; set; }
        private bool _VMBenchmarkChanged;
        public bool VMBenchmarkChanged
        {
            get => _VMBenchmarkChanged;
            set
            {
                if (value != _VMBenchmarkChanged)
                {
                    _VMBenchmarkChanged = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VMBenchmarkChanged)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewData()
        {
            benchmark = new();
            VMBenchmarkChanged = false;
            benchmark.time_collection.CollectionChanged += Collection_CollectionChanged;
            benchmark.accuracy_collection.CollectionChanged += Collection_CollectionChanged;
        }

        private void Collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            VMBenchmarkChanged = true;
        }

        public void AddVMTime(VMGrid grid)
        {
            try { benchmark.AddVMTime(grid); }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in AddVMTime: {ex.Message}.", "Add error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AddVMAccuracy(VMGrid grid)
        {
            try { benchmark.AddVMAccuracy(grid); }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in AddVMAccuracy: {ex.Message}.", "Add error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool Save(string filename)
        {
            try
            {
                StreamWriter sw = new(filename, false);
                try
                {
                    sw.WriteLine(benchmark.time_collection.Count);
                    foreach (VMTime item in benchmark.time_collection)
                    {
                        sw.WriteLine(item.grid.length);
                        sw.WriteLine($"{item.grid.segment_begin:0.00000000}");
                        sw.WriteLine($"{item.grid.segment_end:0.00000000}");
                        sw.WriteLine((int)item.grid.VMf_property);
                        sw.WriteLine($"{item.time_ha:0.00000000}");
                        sw.WriteLine($"{item.time_ep:0.00000000}");
                        sw.WriteLine($"{item.coef_ha:0.00000000}");
                        sw.WriteLine($"{item.coef_ep:0.00000000}");
                    }
                    sw.WriteLine(benchmark.accuracy_collection.Count);
                    foreach (VMAccuracy item in benchmark.accuracy_collection)
                    {
                        sw.WriteLine(item.grid.length);
                        sw.WriteLine($"{item.grid.segment_begin:0.00000000}");
                        sw.WriteLine($"{item.grid.segment_end:0.00000000}");
                        sw.WriteLine((int)item.grid.VMf_property);
                        sw.WriteLine($"{item.value_max_diff:0.00000000}");
                        sw.WriteLine($"{item.arg_max_diff:0.00000000}");
                        sw.WriteLine($"{item.value_ha:0.00000000}");
                        sw.WriteLine($"{item.value_ep:0.00000000}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unable to save file: {ex.Message}.", "Save error", MessageBoxButton.OK, MessageBoxImage.Error);
                    sw.Close();
                    return false;
                }
                finally { sw.Close(); }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to open/create file: {ex.Message}.", "Save error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        public bool Load(string filename)
        {
            try
            {
                StreamReader sr = new StreamReader(filename);
                try
                {
                    benchmark.time_collection.Clear();
                    benchmark.accuracy_collection.Clear();
                    int time_count = Int32.Parse(sr.ReadLine());
                    for (int i = 0; i < time_count; ++i)
                    {
                        VMTime item = new();
                        int len = Int32.Parse(sr.ReadLine());
                        double begin = double.Parse(sr.ReadLine());
                        double end = double.Parse(sr.ReadLine());
                        VMf VMf = (VMf)int.Parse(sr.ReadLine());
                        item.grid = new VMGrid(len, begin, end, VMf);
                        item.time_ha = double.Parse(sr.ReadLine());
                        item.time_ep = double.Parse(sr.ReadLine());
                        item.coef_ha = double.Parse(sr.ReadLine());
                        item.coef_ep = double.Parse(sr.ReadLine());
                        benchmark.time_collection.Add(item);
                    }
                    int acc_count = Int32.Parse(sr.ReadLine());
                    for (int i = 0; i < acc_count; ++i)
                    {
                        VMAccuracy item = new();
                        int len = Int32.Parse(sr.ReadLine());
                        double begin = double.Parse(sr.ReadLine());
                        double end = double.Parse(sr.ReadLine());
                        VMf VMf = (VMf)int.Parse(sr.ReadLine());
                        item.grid = new VMGrid(len, begin, end, VMf);
                        item.value_max_diff = double.Parse(sr.ReadLine());
                        item.arg_max_diff = double.Parse(sr.ReadLine());
                        item.value_ha = double.Parse(sr.ReadLine());
                        item.value_ep = double.Parse(sr.ReadLine());
                        benchmark.accuracy_collection.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unable to load file: {ex.Message}.", "Load error", MessageBoxButton.OK, MessageBoxImage.Error);
                    sr.Close();
                    return false;
                }
                finally { sr.Close(); }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to open file: {ex.Message}.", "Load error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
    }
}
