using SplineLibrary;

namespace SplineApp
{
    public class ViewData
    {
        public Input input_data { get; set; }
        public SplinesData splines_data { get; set; }
        public ChartData graphics { get; set; }

        public ViewData()
        {
            input_data = new(25, 250, 0, 35, SPf.random, 1, 17, 34);
            input_data.error1 = false;
            input_data.error2 = false;
            splines_data = new(new(input_data), new(input_data));
            graphics = new(splines_data.m_data.grid);
        }

        public void update()
        {
            splines_data.m_data.update(input_data);
            splines_data.spline_params.update(input_data);
        }

        public double interpolate()
        {
            return splines_data.interpolate();
        }

        public double integrate()
        {
            return splines_data.integrate();
        }
    }
}