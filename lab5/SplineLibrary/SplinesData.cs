using System.Runtime.InteropServices;

namespace SplineLibrary
{
    public class SplinesData
    {
        public SplineParameters spline_params { get; set; }
        public MeasuredData m_data { get; set; }
        public double[] spline_values { get; set; }
        public double[] integrals { get; set; } = new double[2];
        public double[] derivs { get; set; } = new double[4];

        public SplinesData(MeasuredData md, SplineParameters sp)
        {
            m_data = md;
            spline_params = sp;
        }

        [DllImport("..\\..\\..\\..\\x64\\Debug\\SplineDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern double Interpolate(int len, int uniform_len, double[] points, double[] values, double[] results);
        [DllImport("..\\..\\..\\..\\x64\\Debug\\SplineDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern double Integrate(int len, double[] points, double[] values, double[] limits, double[] integrals);

        public double interpolate()
        {
            double[] interp_results = new double[spline_params.uniform_len * 3];
            double status = Interpolate(m_data.len, spline_params.uniform_len, m_data.grid, m_data.values, interp_results);
            if (status == 0)
            {
                double[] result = new double[spline_params.uniform_len];
                for (int i = 0; i < spline_params.uniform_len; ++i)
                {
                    result[i] = interp_results[3 * i];
                }
                spline_values = result;

                derivs[0] = interp_results[1];
                derivs[1] = interp_results[3 * spline_params.uniform_len - 2];
                derivs[2] = interp_results[2];
                derivs[3] = interp_results[3 * spline_params.uniform_len - 1];
                return 0;
            }
            else return status;
        }

        public double integrate()
        {
            double[] integrals = new double[m_data.len];
            double status = Integrate(m_data.len, m_data.grid, m_data.values, new double[] { spline_params.x1, spline_params.x2 }, integrals);
            if (status == 0)
            {
                this.integrals[0] = integrals.Sum();
                status = Integrate(m_data.len, m_data.grid, m_data.values, new double[] { spline_params.x2, spline_params.x3 }, integrals);
                if (status == 0)
                {
                    this.integrals[1] = integrals.Sum();
                    return 0;
                }
                else return status;
            }
            else return status;
        }
    }
}