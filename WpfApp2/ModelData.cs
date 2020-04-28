using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;

namespace WpfApp2
{
    interface IDeepCopy
    {
        Object DeepCopy();
    }

    [Serializable]
    public partial class ModelData: IDataErrorInfo, IDeepCopy
    {
        //private int _AmountOfGridNodes;
        //private double _p;

        public static double pMin = -100.0;
        public static double pMax = 100.0;
        public static int nMin = 3;
        public static int nMax = 1000; 

        public int AmountOfGridNodes { get; set; }
        public double p { get; set; }
       
        public double[] grid { get; set; }
        public double[] values { get; set; }

   
        public ModelData(int n = 3, double p = 0)
        {
            this.p = p;
            AmountOfGridNodes = n;
            values = new double[n + 1];
            grid = new double[n + 1];
            try
            {
                for (int i = 0; i <= n; ++i)
                {
                    grid[i] = ((double)1 / n) * i;
                    values[i] = p * grid[i] * grid[i] + p; //F(grid[i]);
                }

            }
            catch(Exception ex) // DataErrorInfo
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        public double F(double x) // если между узлами сетки - аппроксимация
        {
            //if (x < grid[0] || x > grid[AmountOfGridNodes])
            //    throw new Exception();
            double a, b;
            if (x == grid[AmountOfGridNodes])
                return values[AmountOfGridNodes];
            for (int i = 0; i < AmountOfGridNodes; i++)
            {
                if (x == grid[i])
                    return values[i];
                else if (x > grid[i] && x < grid[i + 1])
                {
                    a = (values[i + 1] - values[i]) / (grid[i + 1] - grid[i]);
                    b = values[i] - a * grid[i];
                    return a * x + b;
                }
            }
            MessageBox.Show("OOPs");
            return -1; 
        }

        public string Error
        {
            get
            {
                return "error text"; //throw new NotImplementedException();
            }   
        }

        public string this[string property] { 
            get
            {
                string msg = "";
                if (p < pMin || p > pMax)
                    msg += "p is not in right diapason";
                    //break;
                if (AmountOfGridNodes < nMin || AmountOfGridNodes > nMax)
                    msg += "too many grid nodes";
                    //break;
                return msg;
            }
        }

        public override string ToString()
        {
            string s = "p = " + p + " n = " + AmountOfGridNodes + "\n";
            return s;
        }

        public virtual object DeepCopy()
        {
            ModelData new_mod = new ModelData(AmountOfGridNodes, p);
            return new_mod;
        }
    }

}
