using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Windows.Forms;

namespace WpfApp2
{
    public partial class ModelDataView: IDataErrorInfo
    {
        public List<string> ViewTypes;

        public ObservableModelData OMD;

        public ModelDataView(ObservableModelData M)
        {
            OMD = M;
            ViewTypes = new List<string>(2);
            ViewTypes.Add("Line");
            ViewTypes.Add("Spline");
            Order = 1;

        }

        public string this[string property] 
        {
            get
            {
                string msg = "";
                switch(property)
                {
                    case "Order":
                        if (Order < 1 || Order > 5)
                            msg = "Order cannot be less 1 or greater 5";
                        break;
                    default:
                        break;
                }
                return msg;
            }
            
        }

        public string ChoosenType { get; set; }

        public int Order { get; set; }

        public string Error 
        {
            get
            {
                return "error text";
                /*if (Order > 5)
                    return "Too many";
                if (1 > Order)
                    return "Too few";
                return "OK";*/
            }
        } 

        public void Draw(Chart chart, ModelData M) 
        {
            chart.ChartAreas.Clear();
            chart.Series.Clear();
            chart.Legends.Clear();

            chart.ChartAreas.Add(new ChartArea("f_with_less_p"));
            chart.Legends.Add("p");
            chart.Legends[0].Title = "parameter";
            //chart.Legends[0].Position.Auto = true;
            chart.ChartAreas[0].AxisX.LabelStyle.Format = "{0:0." + new String('0', this.Order) + "}";
            chart.ChartAreas[0].AxisY.LabelStyle.Format = "{0:0." + new String('0', this.Order) + "}";

            var q = from item in OMD
                    where item.p <= M.p
                    select item;
            int i = 0;
            foreach (var item in q)
            {
                chart.Series.Add("Series" + i);
                chart.Series[i].LegendText = "p = " + item.p;
                chart.Series[i].Points.DataBindXY(item.grid, item.values);
                chart.Series[i].BorderWidth = 2;
                chart.Series[i].MarkerStyle = MarkerStyle.None;
                if (this.ChoosenType == "Line")
                {
                    chart.Series[i].ChartType = SeriesChartType.Line;
                }
                if (this.ChoosenType == "Spline")
                {
                    chart.Series[i].ChartType = SeriesChartType.Spline;
                }
                chart.Series[i].ChartArea = "f_with_less_p";
                chart.Series[i].Legend = "p";
                i++;
            }
            
            
            //second 
            chart.ChartAreas.Add(new ChartArea("max_av_min"));
            chart.Legends.Add("max_av_min");
            //chart.Legends[1].Position.Auto = true;
            chart.Legends[0].Title = "max_av_min";
            chart.ChartAreas[1].AxisX.LabelStyle.Format = "{0:0." + new String('0', this.Order) + "}";
            chart.ChartAreas[1].AxisY.LabelStyle.Format = "{0:0." + new String('0', this.Order) + "}";

            List<double> av = new List<double>();
            List<double> min = new List<double>();
            List<double> max = new List<double>();

            int modelsCount = OMD.Count;

            for (int j = 0; j <= M.AmountOfGridNodes; ++j)
            {
                List<double> temp = new List<double>(OMD.All_F(M.grid[j]));
                max.Add(temp.Max());
                av.Add(temp.Average());
                min.Add(temp.Min());
            }
            //MAX
            chart.Series.Add("Series" + (i));
            chart.Series[i].LegendText = "max";
            chart.Series[i].Points.DataBindXY(M.grid, max);
            if (this.ChoosenType == "Line")
            {
                chart.Series[i].ChartType = SeriesChartType.Line;
            }
            if (this.ChoosenType == "Spline")
            {
                chart.Series[i].ChartType = SeriesChartType.Spline;
            }
            chart.Series[i].MarkerStyle = MarkerStyle.None;
            chart.Series[i].MarkerSize = 2;
            chart.Series[i].ChartArea = "max_av_min";
            chart.Series[i].Legend = "max_av_min";

            for (int j = 0; j < chart.Series[i].Points.Count; ++j)
            {
                chart.Series[i].Points[j].ToolTip =
                "x = " + chart.Series[i].Points[j].XValue.ToString("F" + Order) + "\n" + "y = " + chart.Series[i].Points[j].YValues[0].ToString("F" + Order);
            }

            chart.Series.Add("Series" + i + 1);
            chart.Series[i + 1].LegendText = "av";
            chart.Series[i + 1].Points.DataBindXY(M.grid, av);
            if (this.ChoosenType == "Line")
            {
                chart.Series[i + 1].ChartType = SeriesChartType.Line;
            }
            if (this.ChoosenType == "Spline")
            {
                chart.Series[i + 1].ChartType = SeriesChartType.Spline;
            }
            chart.Series[i + 1].MarkerStyle = MarkerStyle.Cross;
            chart.Series[i + 1].MarkerSize = 5;
            chart.Series[i + 1].ChartArea = "max_av_min";
            chart.Series[i + 1].Legend = "max_av_min";

            for (int j = 0; j < chart.Series[i + 1].Points.Count; ++j)
            {
                chart.Series[i + 1].Points[j].ToolTip =
                "x = " + chart.Series[i + 1].Points[j].XValue.ToString("F" + Order) + "\n" + "y = " + chart.Series[i + 1].Points[j].YValues[0].ToString("F" + Order);
            }

            chart.Series.Add("Series" + i + 2);
            chart.Series[i + 2].LegendText = "min";
            chart.Series[i + 2].Points.DataBindXY(M.grid, min);
            if (this.ChoosenType == "Line")
            {
                chart.Series[i + 2].ChartType = SeriesChartType.Line;
            }
            if (this.ChoosenType == "Spline")
            {
                chart.Series[i + 2].ChartType = SeriesChartType.Spline;
            }
            chart.Series[i + 2].MarkerStyle = MarkerStyle.Cross;
            chart.Series[i + 2].MarkerSize = 5;
            chart.Series[i + 2].ChartArea = "max_av_min";
            chart.Series[i + 2].Legend = "max_av_min";

            for (int j = 0; j < chart.Series[i + 2].Points.Count; ++j)
            {
                chart.Series[i + 2].Points[j].ToolTip =
                "x = " + chart.Series[i + 2].Points[j].XValue.ToString("F" + Order) + "\n" + "y = " + chart.Series[i + 2].Points[j].YValues[0].ToString("F" + Order);
            }
        }
    }
}
