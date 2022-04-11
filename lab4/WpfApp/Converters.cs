using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using ClassLibrary;

namespace WpfApp
{
    public class BenchmarkChangedConverter: IValueConverter
    {
        public object Convert(object value, Type t, object o, CultureInfo c)
        {
            try
            {
                if ((bool)value) { return "Коллекция была изменена!!!"; }
                else { return ""; }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in BenchmarckChangedConverter: {ex.Message}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return "error";
            }
        }
        public object ConvertBack(object v, Type t, object o, CultureInfo c) { return false; }
    }

    public class DoubleConverter: IValueConverter
    {
        public object Convert(object value, Type t, object o, CultureInfo c)
        {
            try
            {
                double val = (double)value;
                return $"{val:0.00000000}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in DoubleConverter: {ex.Message}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return "0";
            }
        }
        public object ConvertBack(object v, Type t, object o, CultureInfo c) { return 0; }
    }

    public class MinCoefEPConverter: IValueConverter
    {
        public object Convert(object value, Type t, object o, CultureInfo c)
        {
            try
            {
                double val = (double)value;
                return $"min coef ep: {val:0.00000000}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in MinCoefEPConverter: {ex.Message}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return "min coef ep: error";
            }
        }
        public object ConvertBack(object v, Type t, object o, CultureInfo c) { return 0; }
    }

    public class MinCoefHAConverter: IValueConverter
    {
        public object Convert(object value, Type t, object o, CultureInfo c)
        {
            try
            {
                double val = (double)value;
                return $"min coef ha: {val:0.00000000}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in MinCoefHAConverter: {ex.Message}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return "min coef ha: error";
            }
        }
        public object ConvertBack(object v, Type t, object o, CultureInfo c) { return 0; }
    }

    public class VMTimeConverter: IValueConverter
    {
        public object Convert(object value, Type t, object o, CultureInfo c)
        {
            try
            {
                if (value != null)
                {
                    VMTime val = (VMTime)value;
                    return $"coef ha: {val.coef_ha:0.00000000} \n coef ep: {val.coef_ep:0.00000000}";
                }
                return "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in VMTimeConverter: {ex.Message}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return "coef ha: error \n coef ep: error";
            }
        }
        public object ConvertBack(object v, Type t, object o, CultureInfo c) { return new VMTime(); }
    }

    class VMAccuracyConverter : IValueConverter
    {
        public object Convert(object value, Type t, object o, CultureInfo c)
        {
            try
            {
                if (value != null)
                {
                    VMAccuracy val = (VMAccuracy)value;
                    return $"arg max diff: {val.arg_max_diff:0.00000000} \n value max diff: {val.value_max_diff:0.00000000}";
                }
                return "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in VMAccuracyConverter: {ex.Message}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return "arg max diff: error \n value max diff: error";
            }
        }
        public object ConvertBack(object v, Type t, object o, CultureInfo c) { return new VMAccuracy(); }
    }

    public class VMGridConverter: IMultiValueConverter
    {
        public object Convert(object[] values, Type t, object o, CultureInfo c)
        {
            try
            {
                return $"{values[0]} ; {values[1]:0.000} ; {values[2]:0.000}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in VMGridConverter: {ex.Message}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return "0 ; 0 ; 0";
            }
        }
        public object[] ConvertBack(object value, Type[] t, object o, CultureInfo c)
        {
            try
            {
                string val = value.ToString();
                string[] values = val.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length != 3)
                {
                    throw new InvalidOperationException("Должно быть введено 3 значения");
                }
                int val1 = int.Parse(values[0]);
                double val2 = double.Parse(values[1]);
                double val3 = double.Parse(values[2]);
                object[] res = new object[3];
                res[0] = val1;
                res[1] = val2;
                res[2] = val3;
                return res;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in VMGridConverter: {ex.Message}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                int val1 = 5;
                double val2 = 2.2;
                double val3 = 4.4;
                object[] res = new object[3];
                res[0] = val1;
                res[1] = val2;
                res[2] = val3;
                return res;
            }
        }
    }
}
