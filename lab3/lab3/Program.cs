// Вариант 2
using System;
using System.Collections;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace cs
{
    class Program
    {
        struct DataItem
        {
            public Vector2 vector { get; set; }
            public Complex complex { get; set; }

            public DataItem(Vector2 v, Complex c)
            {
                vector = v;
                complex = c;
            }

            public double Abs
            {
                get => Complex.Abs(complex);
            }

            public string ToLongString(string format)
            {
                return $"X: {vector.X.ToString(format)}, Y: {vector.Y.ToString(format)}, Value: {complex.ToString(format)}, Abs: {Complex.Abs(complex).ToString(format)}";
            }

            public override string ToString()
            {
                return ToLongString("");
            }
        }

        public delegate Complex Fv2Complex(Vector2 v2);

        abstract class V2Data : IEnumerable<DataItem>
        {
            public string str { get; protected set; }
            public DateTime date { get; protected set; }

            public V2Data(string s, DateTime d)
            {
                str = s;
                date = d;
            }

            public abstract int Count { get; }
            public abstract float MinDistance { get; }
            public abstract double Abs(DataItem item);
            public abstract double MaxAbs { get; }
            public abstract string ToLongString(string format);

            public override string ToString()
            {
                return $"Str: {str}, Date: {date}";
            }

            public abstract IEnumerator<DataItem> GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        class V2DataList : V2Data
        {
            public List<DataItem> l { get; }

            public V2DataList(string s, DateTime d) : base(s, d)
            {
                l = new List<DataItem>();
            }

            public bool Add(DataItem newItem)
            {
                if (l.Exists(x => x.vector == newItem.vector))
                {
                    return false;
                }
                else
                {
                    l.Add(newItem);
                    return true;
                }
            }

            public int AddDefaults(int nItems, Fv2Complex F)
            {
                var rand = new Random();
                int n = 0;
                for (int i = 0; i < nItems; ++i)
                {
                    int x = rand.Next(-50, 51);
                    int y = rand.Next(-50, 51);
                    Vector2 v = new Vector2(x, y);
                    Complex c = F(v);
                    DataItem newItem = new DataItem(v, c);
                    if (Add(newItem)) ++n;
                }
                return n;
            }

            public override int Count
            {
                get => l.Count;
            }

            public override float MinDistance
            {
                get
                {
                    float min_d = float.MaxValue;
                    for (int i = 0; i < Count; ++i)
                        for (int j = i + 1; j < Count; ++j)
                        {
                            float cur_d = Vector2.Distance(l[i].vector, l[j].vector);
                            if (cur_d < min_d)
                            {
                                min_d = cur_d;
                            }
                        }
                    if (Count == 0 || Count == 1) min_d = 0;
                    return min_d;
                }
            }

            public override double Abs(DataItem item)
            {
                return Complex.Abs(item.complex);
            }

            public override double MaxAbs
            {
                get
                {
                    double max = 0;
                    foreach (DataItem elem in l)
                    {
                        if (Abs(elem) > max)
                            max = Abs(elem);
                    }
                    return max;
                }
            }

            public override string ToString()
            {
                return "V2DataList: " + base.ToString() + " Count: " + l.Count;
            }

            public override string ToLongString(string format)
            {
                string str = "";
                for (int i = 0; i < Count; ++i)
                {
                    str += "\n" + l[i].ToLongString(format);
                }
                return ToString() + str + "\n";
            }

            public override IEnumerator<DataItem> GetEnumerator()
            {
                return l.GetEnumerator();
            }

            public bool SaveBinary(string filename)
            {
                try
                {
                    using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.Create)))
                    {
                        writer.Write(str);
                        writer.Write($"{date}");
                        writer.Write(Count);
                        foreach (var item in l)
                        {
                            writer.Write(item.vector.X);
                            writer.Write(item.vector.Y);
                            writer.Write(item.complex.Real);
                            writer.Write(item.complex.Imaginary);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }
                finally
                {
                    Console.WriteLine("Successfully saved.");
                }
                return true;
            }

            public bool LoadBinary(string filename, ref V2DataList v2)
            {
                try
                {
                    string str;
                    string d;
                    DateTime date;
                    int count;
                    float vx, vy;
                    double cr, ci;
                    if (File.Exists(filename))
                    {
                        using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open)))
                        {
                            str = reader.ReadString();
                            d = reader.ReadString();
                            date = DateTime.Parse(d);
                            V2DataList lst = new V2DataList(str, date);
                            count = reader.ReadInt32();
                            for (int i = 0; i < count; ++i)
                            {
                                vx = reader.ReadSingle();
                                vy = reader.ReadSingle();
                                cr = reader.ReadDouble();
                                ci = reader.ReadDouble();
                                lst.Add(new DataItem(new Vector2(vx, vy), new Complex(cr, ci)));
                            }
                            v2 = lst;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }
                finally
                {
                    Console.WriteLine("Successfully loaded.");
                }
                return true;
            }
        }

        class V2DataArray : V2Data
        {
            public int nx { get; private set; }
            public int ny { get; private set; }
            public Vector2 step { get; private set; }
            public Complex[,] values { get; private set; }

            public V2DataArray(string s, DateTime d) : base(s, d)
            {
                values = new Complex[0, 0];
            }

            public V2DataArray(string s, DateTime d, int nx, int ny, Vector2 nxy, Fv2Complex F) : base(s, d)
            {
                this.nx = nx;
                this.ny = ny;
                this.step = nxy;
                values = new Complex[nx, ny];
                for (int i = 0; i < nx; ++i)
                {
                    for (int j = 0; j < ny; ++j)
                    {
                        float x = i * nxy.X;
                        float y = j * nxy.Y;
                        Vector2 v = new Vector2(x, y);
                        values[i, j] = F(v);
                    }
                }
            }

            public override int Count
            {
                get => nx * ny;
            }

            public override float MinDistance
            {
                get
                {
                    float res = Math.Min(step.X, step.Y);
                    if (nx == 1) res = step.Y;
                    if (ny == 1) res = step.X;
                    if (Count == 0 || Count == 1) res = 0;
                    return res;
                }
            }

            public override double Abs(DataItem item)
            {
                V2DataList list = this;
                return list.Abs(item);
            }

            public override double MaxAbs
            {
                get
                {
                    V2DataList list = this;
                    return list.MaxAbs;
                }
            }

            public override string ToString()
            {
                return $"V2DataArray: {base.ToString()}, nx: {nx}, ny: {ny}, nxy: {step.X} {step.Y}";
            }

            public override string ToLongString(string format)
            {
                string str = "";
                for (int i = 0; i < nx; ++i)
                {
                    for (int j = 0; j < ny; ++j)
                    {
                        str += $"\nX: {(i * step.X).ToString(format)}, Y: {(j * step.Y).ToString(format)}, Value: {values[i, j].ToString(format)}, Abs: {Complex.Abs(values[i, j]).ToString(format)}";
                    }
                }
                return ToString() + str + "\n";
            }

            public static implicit operator V2DataList(V2DataArray arr)
            {
                V2DataList list = new V2DataList(arr.str, arr.date);
                for (int i = 0; i < arr.nx; ++i)
                {
                    for (int j = 0; j < arr.ny; ++j)
                    {
                        float x = i * arr.step.X;
                        float y = j * arr.step.Y;
                        Vector2 v = new Vector2(x, y);
                        Complex c = arr.values[i, j];
                        DataItem newItem = new DataItem(v, c);
                        list.Add(newItem);
                    }
                }
                return list;
            }

            public override IEnumerator<DataItem> GetEnumerator()
            {
                for (int i = 0; i < nx; ++i)
                {
                    for (int j = 0; j < ny; ++j)
                    {
                        yield return new DataItem(new Vector2(step.X * i, step.Y * j), values[i, j]);
                    }
                }
            }

            public bool SaveAsText(string filename)
            {
                CultureInfo cultureInfo = new CultureInfo("ru-RU");
                cultureInfo.NumberFormat.NumberDecimalSeparator = ",";
                try
                {
                    StreamWriter sw = new StreamWriter(filename);
                    sw.WriteLine($"{str}\n{date}\n{nx}\n{ny}\n{step.X}\n{step.Y}");
                    sw.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }
                finally
                {
                    Console.WriteLine("Successfully saved.");
                }
                return true;
            }

            public bool LoadAsText(string filename, ref V2DataArray v2)
            {
                CultureInfo cultureInfo = new CultureInfo("ru-RU");
                cultureInfo.NumberFormat.NumberDecimalSeparator = ",";
                try
                {
                    StreamReader sr = new StreamReader(filename);
                    string str = sr.ReadLine();
                    DateTime date = DateTime.Parse(sr.ReadLine());
                    int nx = int.Parse(sr.ReadLine());
                    int ny = int.Parse(sr.ReadLine());
                    float x = float.Parse(sr.ReadLine());
                    float y = float.Parse(sr.ReadLine());
                    Vector2 nxy = new Vector2(x, y);
                    v2 = new V2DataArray(str, date, nx, ny, nxy, F1);
                    sr.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }
                finally
                {
                    Console.WriteLine("Successfully loaded.");
                }

                return true;
            }

            [DllImport("..\\..\\..\\..\\x64\\Debug\\Dll_mkl.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern double Spline(double[] values, int nx, int ny, double step, double[] res);

            public Complex[,] first_deriv_spline { get; private set; }

            public bool FirstDerivative()
            {
                double[] double_deriv = new double[nx * ny * 2];
                double[] double_values = new double[nx * ny * 2];
                for (int i = 0; i < nx; ++i)
                {
                    for (int j = 0; j < ny; ++j)
                    {
                        double_values[i * 2 * ny + j * 2] = values[i, j].Real;
                        double_values[i * 2 * ny + j * 2 + 1] = values[i, j].Imaginary;
                    }
                }
                double status = Spline(double_values, nx, ny * 2, step.X, double_deriv);
                if (status == 0)
                {
                    first_deriv_spline = new Complex[nx, ny];
                    for (int i = 0; i < nx; ++i)
                    {
                        for (int j = 0; j < ny; ++j)
                        {
                            first_deriv_spline[i, j] = new(double_deriv[i * 2 * ny + j * 2], double_deriv[i * 2 * ny + j * 2 + 1]);
                        }
                    }
                    return true;
                }
                else
                {
                    Console.WriteLine(status);
                    return false;
                }
            }

            public Complex? FirstDerivativeSplineAt(int jx, int jy)
            {
                if (jx < 0 || jy < 0 || jx >= nx || jy >= ny) return null;
                else return first_deriv_spline[jx, jy];
            }

            public Complex? FirstDerivativeLeftAt(int jx, int jy)
            {
                if (jx < 1 || jy < 0 || jx >= nx || jy >= ny || step.X == 0) return null;
                else return (values[jx, jy] - values[jx - 1, jy]) / step.X;
            }

            public Complex? FirstDerivativeRightAt(int jx, int jy)
            {
                if (jx < 0 || jy < 0 || jx >= nx - 1 || jy >= ny || step.X == 0) return null;
                else return (values[jx + 1, jy] - values[jx, jy]) / step.X;
            }
        }

        class V2MainCollection
        {
            private List<V2Data> l;

            public V2MainCollection()
            {
                l = new List<V2Data>();
            }

            public int Count
            {
                get => l.Count;
            }

            public bool AllNull
            {
                get
                {
                    foreach (var elem in l)
                    {
                        if (elem.Count != 0) return false;
                    }
                    return true;
                }
            }

            public V2Data this[int i]
            {
                get => l[i];
            }

            public bool Contains(string ID)
            {
                return l.Exists(x => x.str == ID);
            }

            public bool Add(V2Data v2Data)
            {
                if (Contains(v2Data.str))
                {
                    return false;
                }
                else
                {
                    l.Add(v2Data);
                    return true;
                }
            }

            public double Max
            {
                get
                {
                    double max = 0;
                    foreach (var item in l)
                    {
                        if (item.MaxAbs > max)
                            max = item.MaxAbs;
                    }
                    return max;
                }
            }

            /* Возвращает объект DataItem с максимальным значением
            модуля поля среди всех результатов измерений. */
            public DataItem? DataItemWithMaxAbs
            {
                get
                {
                    if (AllNull) return null;
                    var data = l.Select(DataListCast);
                    var a = from list in data
                            from item in list
                            where list.Abs(item) == Max
                            select item;
                    return a.First();
                }
            }

            /* Перечисляет все точки измерения поля, такие, что они есть
            в элементах типа V2DataList, но их нет в элементах V2DataArray */
            public IEnumerable<Vector2> DataListExceptDataArray
            {
                get
                {
                    var dataArray = l.Where(x => x is V2DataArray).Select(DataListCast);
                    var dataList = l.Where(x => x is V2DataList).Select(DataListCast);

                    var vectorsDataArray = from data in dataArray
                                           from vector in data
                                           select vector.vector;

                    var vectorsDataList = from data in dataList
                                          from vector in data
                                          select vector.vector;

                    return vectorsDataList.Except(vectorsDataArray).Distinct();
                }
            }

            /* Группирует все элементы из List<V2Data>, по числу результатов измерения поля. */
            public IEnumerable<IGrouping<int, V2Data>> Group
            {
                get
                {
                    var data = l.Select(DataListCast);
                    var group = from list in data
                                group list by list.Count into newGroup
                                select newGroup;
                    return group;
                }
            }

            public string ToLongString(string format)
            {
                string str = "";
                foreach (var data in l)
                {
                    str += "\n" + data.ToLongString(format);
                }
                return str;
            }

            public override string ToString()
            {
                string str = "";
                foreach (var data in l)
                {
                    str += data.ToString() + "\n";
                }
                return str;
            }

            private V2DataList DataListCast(V2Data elem)
            {
                return elem is V2DataArray ? (V2DataList)(elem as V2DataArray) : elem as V2DataList;
            }
        }

        public static Complex F1(Vector2 v2)
        {
            return new Complex(v2.X, v2.Y);
        }

        public static Complex Polynom3D(Vector2 v2)
        {
            double x = v2.X;
            double y = x * x * x + x * x;
            return new Complex(y, y + 2);
        }

        static void Interpolate(V2DataArray arr)
        {
            bool res = arr.FirstDerivative();

            Console.WriteLine("\n_______FirstDerivativeSpline_______:");
            if (res)
            {
                for (int i = 0; i < arr.nx; ++i)
                {
                    for (int j = 0; j < arr.ny; ++j)
                    {
                        Console.WriteLine(arr.FirstDerivativeSplineAt(i, j));
                    }
                }
            }
            else Console.WriteLine("null");

            Console.WriteLine("\n\n_______FirstDerivativeLeftAt_______:");
            for (int i = 0; i < arr.nx; ++i)
            {
                for (int j = 0; j < arr.ny; ++j)
                {
                    Complex? res1 = arr.FirstDerivativeLeftAt(i, j);
                    if (res1 != null) Console.WriteLine(res1);
                    else Console.WriteLine("null");
                }
            }

            Console.WriteLine("\n\n_______FirstDerivativeRightAt_______:");
            for (int i = 0; i < arr.nx; ++i)
            {
                for (int j = 0; j < arr.ny; ++j)
                {
                    Complex? res1 = arr.FirstDerivativeRightAt(i, j);
                    if (res1 != null) Console.WriteLine(res1);
                    else Console.WriteLine("null");
                }
            }
        }

        static void Main()
        {
            Console.WriteLine("_______Test1_______\n\n");
            V2DataArray arr1 = new("Array1", new DateTime(2022, 3, 14), 5, 1, new Vector2(0.5f, 1), Polynom3D);
            Console.WriteLine("_______Array_______:");
            Console.WriteLine(arr1.ToLongString("N2"));
            Interpolate(arr1);

            Console.WriteLine("\n\n_______Test2_______\n\n");
            V2DataArray arr2 = new("Array2", new DateTime(2022, 3, 14), 5, 2, new Vector2(0.25f, 1), Polynom3D);
            Console.WriteLine("_______Array_______:");
            Console.WriteLine(arr2.ToLongString("N2"));
            Interpolate(arr2);
        }
    }
}
