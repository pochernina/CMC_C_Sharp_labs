namespace ClassLibrary
{
    public class VMGrid
    {
        public int length { get; set; }
        public double segment_begin { get; set; }
        public double segment_end { get; set; }
        public double step
        { 
            get
            {
                if (length < 2) return 0;
                return (segment_end - segment_begin) / (length - 1);
            }
        }
        public VMf VMf_property { get; set; }

        public VMGrid(int len, double begin, double end, VMf VMf)
        {
            length = len;
            segment_begin = begin;
            segment_end = end;
            VMf_property = VMf;
        }

        public VMGrid(VMGrid other)
        {
            length = other.length;
            segment_begin = other.segment_begin;
            segment_end = other.segment_end;
            VMf_property = other.VMf_property;
        }
    }
}
