using System;
using System.Windows;
using System.Windows.Input;

namespace SplineApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public ViewData data { get; set; }
        public bool is_measured { get; set; }
        public bool is_splined { get; set; }

        public MainWindow()
        {
            try
            {
                data = new();
                DataContext = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in MainWindow: {ex.Message}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            InitializeComponent();
            combobox_function.ItemsSource = Enum.GetValues(typeof(SplineLibrary.SPf));
        }

        private void MeasuredData_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !data.input_data.error1;
        }

        private void MeasuredData_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {               
                tb_deriv_first.Text = "";
                tb_deriv_second.Text = "";
                tb_integral1.Text = "";
                tb_integral2.Text = "";
                tb_spline1.Text = "";
                tb_spline2.Text = "";

                data.update();

                data.splines_data.m_data.create_grid();
                data.splines_data.m_data.set_values();

                is_measured = true;
                is_splined = false;

                data.graphics.clear_plot();
                data.graphics.AddSeries(data.splines_data.m_data.grid, data.splines_data.m_data.values, "Measured", 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in MeasuredData_Executed: {ex.Message}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Splines_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (!data.input_data.error2) && is_measured && (!is_splined);
        }

        private void Splines_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                is_splined = true;
                double status = data.interpolate();

                if (status == 0)
                {
                    tb_deriv_first.Text = $"Первые производные: {data.splines_data.derivs[0]:0.00}; {data.splines_data.derivs[1]:0.00}";
                    tb_deriv_second.Text = $"Вторые производные: {data.splines_data.derivs[2]:0.00}; {data.splines_data.derivs[3]:0.00}";
                    tb_spline1.Text = $"Сплайн в начале: {data.splines_data.spline_values[0]:0.00}";
                    tb_spline2.Text = $"Сплайн в конце: {data.splines_data.spline_values[data.splines_data.m_data.len - 1]:0.00}";

                    double[] GridUniform = new double[data.splines_data.spline_params.uniform_len];
                    double step = (data.splines_data.m_data.end - data.splines_data.m_data.begin) / (data.splines_data.spline_params.uniform_len - 1);
                    for (int i = 0; i < data.splines_data.spline_params.uniform_len; i++)
                    {
                        GridUniform[i] = data.splines_data.m_data.begin + (i * step);
                    }
                    data.graphics.AddSeries(GridUniform, data.splines_data.spline_values, "Spline", 1);
                    status = data.integrate();
                    if (status == 0)
                    {
                        tb_integral1.Text = $"Интеграл на 1ом отрезке: {data.splines_data.integrals[0]:0.00}";
                        tb_integral2.Text = $"Интеграл на 2ом отрезке: {data.splines_data.integrals[1]:0.00}";
                    }
                    else
                    {
                        MessageBox.Show($"Error in Integrate with status {status}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show($"Error in Interpolate with status {status}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public static class CustomCommands
    {
        public static readonly RoutedUICommand MeasuredData = new
            (
                "MeasuredData",
                "MeasuredData",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.D1, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand Splines = new
            (
                "Splines",
                "Splines",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.D2, ModifierKeys.Control)
                }
            );
    }
}