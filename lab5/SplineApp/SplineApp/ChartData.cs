using System;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;

namespace SplineApp
{
    public class ChartData
    {
        public SeriesCollection series_coll { get; set; }
        public Func<double, string> formatter { get; set; }

        public ChartData(double[] lables)
        {
            series_coll = new SeriesCollection();
            formatter = value => value.ToString("F4");
        }

        public void AddSeries(double[] points, double[] values, string title, int mode)
        {
            ChartValues<ObservablePoint> Values = new ChartValues<ObservablePoint>();
            for (int i = 0; i < values.Length; ++i)
            {
                Values.Add(new(points[i], values[i]));
            }

            if (mode == 0) // points
            {
                series_coll.Add(new ScatterSeries
                {
                    Title = title,
                    Values = Values,
                    Fill = Brushes.Black,
                    MinPointShapeDiameter = 5,
                    MaxPointShapeDiameter = 5
                });
            }
            else if (mode == 1) // splines
            {
                series_coll.Add(new LineSeries
                {
                    Title = title,
                    Values = Values,
                    Fill = Brushes.Transparent,
                    Stroke = Brushes.Orchid,
                    PointGeometry = null,
                    LineSmoothness = 0
                });
            }
        }

        public void clear_plot()
        {
            series_coll.Clear();
        }
    }
}