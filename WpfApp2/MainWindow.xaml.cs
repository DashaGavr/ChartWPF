using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    // "{Binding Source={x:Static local:MainWindow.p_min}, Path=Source}"
    public partial class MainWindow : Window
    {
        public static ObservableModelData OMD = new ObservableModelData();
        public static RoutedCommand AddModelCommand = new RoutedCommand("AddModelCommand", typeof(MainWindow));
        public static RoutedCommand DrawCommand = new RoutedCommand("DrawCommand", typeof(MainWindow));
        public ModelData MD = new ModelData();
        public ModelDataView MDView = new ModelDataView(OMD);
        public Chart chart = new Chart();
        public static double p_min = ModelData.pMin;
        public static double p_max = ModelData.pMax;

        public MainWindow()
        {
            InitializeComponent();
            //OMD.AddDefaults();
            this.DataContext = OMD;
            DataTemplate tmpl = this.TryFindResource("VIEW") as DataTemplate;
            if (tmpl != null)
            {
                Element_p.ItemTemplate = tmpl;
            }
            Binding_Settings();
            winFormsHost.Child = chart;
            

        }

        private void Binding_Settings()
        {
            Binding b1 = new Binding();
            b1.Source = MD;
            b1.Path = new PropertyPath("AmountOfGridNodes");
            b1.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            b1.ValidatesOnDataErrors = true;
            Enter_n.SetBinding(TextBox.TextProperty, b1);

            Binding b2 = new Binding();
            b2.Source = MD;
            b2.Path = new PropertyPath("p");
            b2.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            b2.ValidatesOnDataErrors = true;
            Enter_p.SetBinding(TextBox.TextProperty, b2);

            Binding b3 = new Binding();
            b3.Source = MDView;
            Type.ItemsSource = MDView.ViewTypes;
            b3.Path = new PropertyPath("ChoosenType");
            b3.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            Type.SetBinding(ComboBox.TextProperty, b3);

            Binding b4 = new Binding();
            b4.Source = MDView;
            b4.Path = new PropertyPath("Order");
            b4.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            Order.SetBinding(TextBox.TextProperty, b4);

            /*Binding b5 = new Binding();
            //b5.Source = this;
            b5.Path = new PropertyPath("p_max");
            //b5.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            b5.ValidatesOnDataErrors = true;
            p_Max.SetBinding(TextBox.TextProperty, b5);*/

        }

        private void New_Command(object sender, ExecutedRoutedEventArgs e)
        {
            if (OMD.IsChanged)
            {
                if (MessageBox.Show("Do you want to save?", "title", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    if (sfd.ShowDialog() == true)
                    {
                       ObservableModelData.Save(sfd.FileName, OMD);
                    }
                    OMD.IsChanged = false;
                }
            }
            OMD.Clear();
            OMD.AddDefaults();
            MDView = new ModelDataView(OMD);
            this.DataContext = OMD;
            DataTemplate tmpl = this.TryFindResource("VIEW") as DataTemplate;
            if (tmpl != null)
            {
                Element_p.ItemTemplate = tmpl;
            }
            Binding_Settings();
        }

        private void Open_Command(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            SaveFileDialog sfd = new SaveFileDialog();
            bool flag;
            if (ofd.ShowDialog() == true)
            {
                if (OMD.IsChanged == true)
                {
                    if (MessageBox.Show("Do you want to save?", "Open", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        if (sfd.ShowDialog() == true)
                        {
                            if((flag = ObservableModelData.Save(sfd.FileName, OMD)) == false)
                            {
                                MessageBox.Show("Error!");
                            }
                        }
                    }
                }
                ObservableModelData.Load(ofd.FileName, ref OMD);
                OMD.IsChanged = false;
                this.DataContext = OMD;
            }
        }

        private void Can_Save_Command(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = OMD.IsChanged;
        }

        private void Save_Command(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == true)
            {
                ObservableModelData.Save(sfd.FileName, OMD);
            }
            OMD.IsChanged = false;
        }

        private void Can_Delete_Command(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Element_p.SelectedIndex >= 0;
        }

        private void Delete_Command(object sender, ExecutedRoutedEventArgs e)
        {
            OMD.Remove_At(Element_p.SelectedIndex);
        }

        private void Can_Add_Command(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !(Validation.GetHasError(Enter_n) || Validation.GetHasError(Enter_p));
        }

        private void Add_Command(object sender, ExecutedRoutedEventArgs e)
        {
            //OMD.Add_ModelData(new ModelData(Convert.ToInt32(Enter_n.Text), Convert.ToDouble(Enter_p.Text)));
            OMD.Add_ModelData((ModelData)MD.DeepCopy());
        }

        private void Can_Draw_Command(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = ((Element_p.SelectedIndex >= 0) && !(Validation.GetHasError(Order)));
            e.CanExecute = true;
        }

        private void Draw_Command(object sender, ExecutedRoutedEventArgs e)
        {
            MDView.Draw(chart, OMD[Element_p.SelectedIndex]);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (OMD.IsChanged)
            {
                if (MessageBox.Show("Do you want to save?", "Exit", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    if (sfd.ShowDialog() == true)
                    {
                        ObservableModelData.Save(sfd.FileName, OMD);
                    }
                    OMD.IsChanged = false;
                }
            }
        }
    
    }
}
