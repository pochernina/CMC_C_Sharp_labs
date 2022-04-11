using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ClassLibrary
{
    public class VMBenchmark: INotifyPropertyChanged
    {
        public ObservableCollection<VMTime> time_collection { get; set; }
        public ObservableCollection<VMAccuracy> accuracy_collection { get; set; }

        public double min_coef_ha
        {
            get
            {
                if (time_collection.Count <= 0) return -1;
                double min_coef = time_collection[0].coef_ha;
                foreach (VMTime item in time_collection)
                    if (item.coef_ha < min_coef) { min_coef = item.coef_ha; }
                return min_coef;
            }
        }

        public double min_coef_ep
        {
            get
            {
                if (time_collection.Count <= 0) return -1;
                double min_coef = time_collection[0].coef_ep;
                foreach (VMTime item in time_collection)
                    if (item.coef_ep < min_coef) { min_coef = item.coef_ep; }
                return min_coef;
            }
        }


        [DllImport("..\\..\\..\\..\\ClassLibrary\\x64\\Debug\\Dll_mkl.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern double MKL_function(int len, double[] args, int VMf, double[] res_ha, double[] res_ep, double[] res_without_mkl, double[] results);

        public void AddVMTime(VMGrid grid)
        {
            VMTime elem = new();
            elem.grid = new(grid);
            double[] args = new double[grid.length];
            for (int i = 0; i < grid.length; ++i)
            {
                args[i] = grid.segment_begin + grid.step * i;
            }
            double[] res_ha = new double[grid.length];
            double[] res_ep = new double[grid.length];
            double[] res_without_mkl = new double[grid.length];
            double[] results = new double[3];
            double status = MKL_function(grid.length, args, (int)grid.VMf_property, res_ha, res_ep, res_without_mkl, results);
            if (status != 0) throw new InvalidCastException($"MKL function failed with status: {status}");
            elem.time_ha = results[0];
            elem.time_ep = results[1];
            elem.coef_ha = results[0] / results[2];
            elem.coef_ep = results[1] / results[2];
            time_collection.Add(elem);
        }

        public void AddVMAccuracy(VMGrid grid)
        {
            VMAccuracy elem = new();
            elem.grid = new(grid);
            double[] args = new double[grid.length];
            for (int i = 0; i < grid.length; ++i)
            {
                args[i] = grid.segment_begin + grid.step * i;
            }
            double[] res_ha = new double[grid.length];
            double[] res_ep = new double[grid.length];
            double[] res_without_mkl = new double[grid.length];
            double[] results = new double[3];
            double status = MKL_function(grid.length, args, (int)grid.VMf_property, res_ha, res_ep, res_without_mkl, results);
            if (status != 0) throw new InvalidCastException($"MKL function failed with status: {status}");
            double max_diff = 0;
            int index = -1;
            for (int i = 0; i < grid.length; ++i)
            {
                if (Math.Abs(res_ha[i] - res_ep[i]) > max_diff)
                {
                    index = i;
                    max_diff = Math.Abs(res_ha[i] - res_ep[i]);
                }
            }
            if (index == -1)
            {
                elem.value_max_diff = 0;
                elem.arg_max_diff = 0;
                elem.value_ha = 0;
                elem.value_ep = 0;
            }
            else
            {
                elem.value_max_diff = max_diff;
                elem.arg_max_diff = args[index];
                elem.value_ha = Math.Abs(res_ha[index]);
                elem.value_ep = Math.Abs(res_ep[index]);
            }
            accuracy_collection.Add(elem);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(min_coef_ha)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(min_coef_ep)));
        }

        public VMBenchmark()
        {
            time_collection = new();
            accuracy_collection = new();
            time_collection.CollectionChanged += Collection_CollectionChanged;
        }
    }
}
