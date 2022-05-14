using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace SplineApp
{
    class DoubleConverter: IValueConverter
    {
        public object Convert(object value, Type t, object o, CultureInfo c)
        {
            try
            {
                double val = (double)value;
                return $"{val:0.0}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in DoubleConverter: {ex.Message}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return "error";
            }
        }
        public object ConvertBack(object value, Type t, object o, CultureInfo c)
        {
            try
            {
                string val = value as string;
                return double.Parse(val);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in DoubleConverter: {ex.Message}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return "error";
            }
        }
    }

    class IntConverter : IValueConverter
    {
        public object Convert(object value, Type t, object o, CultureInfo c)
        {
            try
            {
                int val = (int)value;
                return $"{val}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in IntConverter: {ex.Message}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return "error";
            }
        }
        public object ConvertBack(object value, Type t, object o, CultureInfo c)
        {
            try
            {
                string val = value as string;
                return int.Parse(val);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in IntConverter: {ex.Message}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return "error";
            }
        }
    }
}