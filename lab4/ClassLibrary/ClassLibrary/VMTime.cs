namespace ClassLibrary
{
    public struct VMTime
    {
        public VMGrid grid { get; set; }
        public double time_ha { get; set; }
        public double time_ep { get; set; }
        public double coef_ha { get; set; }
        public double coef_ep { get; set; }

        public override string ToString()
        {
            return string.Format($"{grid.length}, {grid.segment_begin:0.00000000}, {grid.segment_end:0.00000000}, {grid.step:0.00000000}, {grid.VMf_property}\n{time_ha:0.00000000}; {time_ep:0.00000000}; {coef_ha:0.00000000}; {coef_ep:0.00000000}.");
        }
    }
}