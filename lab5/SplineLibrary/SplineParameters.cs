namespace SplineLibrary
{
    public class SplineParameters
    {
        public int uniform_len { get; set; }
        public double x1 { get; set; }
        public double x2 { get; set; }
        public double x3 { get; set; }

        public SplineParameters(Input input)
        {
            uniform_len = input.uniform_len;
            x1 = input.x1;
            x2 = input.x2;
            x3 = input.x3;
        }

        public void update(Input input)
        {
            uniform_len = input.uniform_len;
            x1 = input.x1;
            x2 = input.x2;
            x3 = input.x3;
        }
    }
}