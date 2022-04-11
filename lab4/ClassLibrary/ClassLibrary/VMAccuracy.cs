namespace ClassLibrary
{
    public struct VMAccuracy
    {
        public VMGrid grid { get; set; }
        public double value_max_diff { get; set; }
        public double arg_max_diff { get; set; }
        public double value_ha { get; set; }
        public double value_ep { get; set; }

        public override string ToString()
        {
            return string.Format($"{grid.length}, {grid.segment_begin:0.00000000}, {grid.segment_end:0.00000000}, {grid.step:0.00000000}, {grid.VMf_property}\n{value_max_diff:0.00000000}; {arg_max_diff:0.00000000}; {value_ha:0.00000000}; {value_ep:0.00000000}.");
        }
    }
}
