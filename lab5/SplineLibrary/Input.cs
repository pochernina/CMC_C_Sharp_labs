using System.ComponentModel;

namespace SplineLibrary
{
    public class Input: IDataErrorInfo
    {
        public bool error1 { get; set; }
        public bool error2 { get; set; }

        private int _len;
        public int len
        {
            get => _len;
            set
            {
                _len = value;
                error1 = false;
            }
        }

        private int _uniform_len;
        public int uniform_len
        {
            get => _uniform_len;
            set
            {
                _uniform_len = value;
                error2 = false;
            }
        }

        private double _begin;
        public double begin
        {
            get => _begin;
            set
            {
                _begin = value;
                error1 = false;
            }
        }

        private double _end;
        public double end
        {
            get => _end;
            set
            {
                _end = value;
                error1 = false;
            }
        }

        private double _x1;
        public double x1
        {
            get => _x1;
            set
            {
                _x1 = value;
                error2 = false;
            }
        }

        private double _x2;
        public double x2
        {
            get => _x2;
            set
            {
                _x2 = value;
                error2 = false;
            }
        }

        private double _x3;
        public double x3
        {
            get => _x3;
            set
            {
                _x3 = value;
                error2 = false;
            }
        }

        public SPf function { get; set; }

        public Input(int length, int uni_length, double seg_begin, double seg_end, SPf f, double X1, double X2, double X3)
        {
            len = length;
            uniform_len = uni_length;
            begin = seg_begin;
            end = seg_end;
            function = f;
            x1 = X1;
            x2 = X2;
            x3 = X3;
        }

        public string this[string column_name]
        {
            get
            {
                string error = null;
                switch (column_name)
                {
                    case "len":
                        if ((len < 3) || (len > 100000))
                        {
                            error = "length";
                            error1 = true;
                        }
                        break;
                    case "begin":
                    case "end":
                        if ((end < begin) || (begin > x1) || (end < x3))
                        {
                            error = "borders";
                            error1 = true;
                        }
                        break;
                    case "uniform_len":
                        if ((uniform_len < 3) || (uniform_len > 100000))
                        {
                            error = "uniform_length";
                            error2 = true;
                        }
                        break;
                    case "x1":
                    case "x2":
                    case "x3":
                        if ((begin > x1) || (x1 >= x2) || (x2 >= x3) || (x3 > end))
                        {
                            error = "limits";
                            error2 = true;
                        }
                        break;
                    default:
                        break;
                }
                return error;
            }
        }

        public string Error
        {
            get => throw new NotImplementedException();
        }
    }
}