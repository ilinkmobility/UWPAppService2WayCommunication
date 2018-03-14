using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Foundation.Collections;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindowCallback
    {
        public MainWindow()
        {
            InitializeComponent();

            WPFAppServiceHandler.StartWPFAppService(this);
        }

        public void OnBackgroundColorChange(string colorHex)
        {
            try
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                    new Action(delegate ()
                    {
                        this.Background = (Brush)new BrushConverter().ConvertFrom(colorHex);
                    }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ValueSet valueSet = new ValueSet
                {
                    { "Amount", txtAmount.Text },
                    { "Period", txtPeriod.Text },
                    { "Rate", txtRate.Text }
                };

                var result = await AppServiceConsumer.Instance.GetResponse(valueSet);

                txtResponse.Text = "Interest Gained : " + result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
