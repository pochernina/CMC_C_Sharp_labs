using System.Collections.ObjectModel;

namespace SplineLibrary
{
    public class MeasuredData
    {
        public int len { get; set; }
        public double begin { get; set; }
        public double end { get; set; }
        public SPf function { get; set; }
        public double[] grid { get; set; }
        public double[] values { get; set; }
        public ObservableCollection<string> coll { get; set; }

        public MeasuredData(Input input)
        {
            len = input.len;
            begin = input.begin;
            end = input.end;
            function = input.function;
            coll = new();
        }

        public void update(Input input)
        {
            len = input.len;
            begin = input.begin;
            end = input.end;
            function = input.function;
        }

        public void create_grid()
        {
            grid = new double[len];
            grid[0] = begin;
            grid[len-1] = end;
            var random = new Random();
            for (int i = 1; i < len - 1; ++i)
            {
                double random_value = begin;
                while (random_value <= begin)
                {
                    random_value = end * random.NextDouble();
                }
                grid[i] = random_value;
            }
            Array.Sort(grid);
        }

        public void set_values()
        {
            values = new double[len];

            switch (function)
            {
                case SPf.linear:
                    for (int i = 0; i < len; ++i)
                    {
                        values[i] = grid[i];
                    }
                    break;
                case SPf.cubic:
                    for (int i = 0; i < len; ++i)
                    {
                        values[i] = Math.Pow(grid[i], 3);
                    }
                    break;
                case SPf.random:
                    var rand = new Random();
                    for (int i = 0; i < len; ++i)
                    {
                        values[i] = 20 * rand.NextDouble();
                    }
                    break;
                default:
                    break;
            }

            coll.Clear();
            for (int i = 0; i < len; ++i)
            {
                coll.Add($"Point: {grid[i]}\nValue: {values[i]}\n");
            }
        }
    }
}