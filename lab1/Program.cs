// Вариант 2
using System;
using System.Numerics;
using System.Collections.Generic;

namespace cs
{
    class Program
    {
        struct DataItem
        {
            public Vector2 vector {get; set;}
            public Complex complex {get; set;}
            public DataItem(Vector2 v, Complex c)
            {
                vector = v;
                complex = c;
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

        abstract class V2Data
        {
            public string str {get;}
            public DateTime date {get;}
            public V2Data(string s, DateTime d)
            {
                str = s;
                date = d;
            }
            public abstract int Count {get;}
            public abstract float MinDistance {get;}
            public abstract string ToLongString(string format);
            public override string ToString()
            {
                return $"Str: {str}, Date: {date}";
            }
        }

        class V2DataList: V2Data
        {
            public List<DataItem> l {get;}
            public V2DataList(string s, DateTime d): base(s,d)
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
        }

        class V2DataArray: V2Data
        {
            public int nx {get;}
            public int ny {get;}
            public Vector2 nxy {get;}
            public Complex[,] values {get;}
            public V2DataArray(string s, DateTime d): base(s,d)
            {
                values = new Complex[0,0];
            }
            public V2DataArray(string s, DateTime d, int nx, int ny, Vector2 nxy, Fv2Complex F): base(s,d)
            {
                this.nx = nx;
                this.ny = ny;
                this.nxy = nxy;
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
                    float res = Math.Min(nxy.X, nxy.Y);
                    if (nx == 1) res = nxy.Y;
                    if (ny == 1) res = nxy.X;
                    if (Count == 0 || Count == 1) res = 0;
                    return res;
                }
            }
            public override string ToString()
            {
                return $"V2DataArray: {base.ToString()}, nx: {nx}, ny: {ny}, nxy: {nxy.X} {nxy.Y}";
            }
            public override string ToLongString(string format)
            {
                string str = "";
                for (int i = 0; i < nx; ++i)
                {
                    for (int j = 0; j < ny; ++j)
                    {
                        str += $"\nX: {(i * nxy.X).ToString(format)}, Y: {(j * nxy.Y).ToString(format)}, Value: {values[i, j].ToString(format)}, Abs: {Complex.Abs(values[i, j]).ToString(format)}"; 
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
                        float x = i * arr.nxy.X;
                        float y = j * arr.nxy.Y;
                        Vector2 v = new Vector2(x, y);
                        Complex c = arr.values[i, j];
                        DataItem newItem = new DataItem(v, c);
                        list.Add(newItem);
                    }
                }
                return list;
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
        }

        public static Complex F1(Vector2 v2)
        {
            return new Complex(v2.X, v2.Y);
        }

        static void Main()
        {
            V2DataArray arr = new V2DataArray("Object", new DateTime(2021, 10, 9), 1, 2, new Vector2(1.5f, 2.5f), F1);
            Console.WriteLine(arr.ToLongString("N1"));
            V2DataList list = arr;
            Console.WriteLine(list.ToLongString("N1"));
            Console.WriteLine($"Count: {arr.Count}, MinDistance: {arr.MinDistance}");
            Console.WriteLine($"Count: {list.Count}, MinDistance: {list.MinDistance}");

            V2MainCollection collect = new V2MainCollection();
            V2DataArray arr2 = new V2DataArray("Object2", new DateTime(2021, 10, 9), 2, 0, new Vector2(0.5f, 2f), F1);
            V2DataList list2 = new V2DataList("List", new DateTime(2021, 10, 15));
            collect.Add(arr);
            collect.Add(list2);
            collect.Add(list);
            collect.Add(arr2);
            Console.WriteLine(collect.ToLongString("N1"));
            for (int i = 0; i < collect.Count; ++i)
            {
                Console.WriteLine($"Count: {collect[i].Count}, MinDistance: {collect[i].MinDistance}");
            }
        }
    }
}
