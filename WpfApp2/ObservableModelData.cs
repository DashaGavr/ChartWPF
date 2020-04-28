using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    [Serializable]
    public class ObservableModelData: System.Collections.ObjectModel.ObservableCollection<ModelData>
    {
        public bool IsChanged { get; set; } //автореализуемое 

        public ObservableModelData()
        { 
            this.CollectionChanged += CollectionChangedEventHandler;
            this.AddDefaults();
            IsChanged = false;
        }

        public void Add_ModelData(ModelData m)
        {
            this.Add(m);
        }

        public void Remove_At(int index)
        {
            try
            {
                this.RemoveAt(index);
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void AddDefaults()
        {
            for (int i = 1; i <= 3; ++i)
            {
                ModelData tmp1 = new ModelData(4 * i, Math.Pow(2, i));
                this.Add(tmp1);
            }
        }

        public double p_max
        {
            get
            {
                return ModelData.pMax;
            }
        }

        public double p_min
        {
            get
            {
                return ModelData.pMin;
            }
        }

        public int n_max
        {
            get
            {
                return ModelData.nMax;
            }
        }

        public double n_min
        {
            get
            {
                return ModelData.nMin;
            }
        }

        public double[] All_F(double x)
        {
            double[] F = new double[this.Count];
            int i = 0;
            foreach(ModelData M in this)
            {
                F[i++] = M.F(x);
            }
            return F;
        }

        public override string ToString()
        {
            string s = "Collection of ModelData   all:" + this.Count.ToString() + "\n";
            foreach(ModelData M in this)
            {
                s += M.ToString();
            }
            return s;
        }

        public void CollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsChanged = true;
        }

        public static bool Save(string filename, ObservableModelData obj)
        {
            bool f = true;
            FileStream fs = null;
            try
            {
                obj.IsChanged = false;
                fs = File.Open(filename, FileMode.OpenOrCreate);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, obj);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                f = false;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return f;
        }

        public static bool Load(string filename, ref ObservableModelData obj)
        {
            bool f = true;
            FileStream fs = null;
            try
            {
                fs = File.OpenRead(filename);
                BinaryFormatter bf = new BinaryFormatter();
                obj = bf.Deserialize(fs) as ObservableModelData;
                obj.CollectionChanged += obj.CollectionChangedEventHandler;

            }
            catch (Exception ex)    //  мб другая обработка
            {
                MessageBox.Show(ex.Message);
                f = false;
            }
            finally
            {
                if (fs != null) fs.Close();
            }
            return f;

        }
    }
}
