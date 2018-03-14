using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            Task.Run(async () => {
                await Windows.ApplicationModel.FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            });
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ValueSet valueSet = new ValueSet
                {
                    { "Request", "Conversion" },
                    { "Amount", txtAmount.Text },
                    { "Period", txtPeriod.Text },
                    { "Rate", txtRate.Text }
                };

                if (App.Connection != null)
                {
                    AppServiceResponse response = await App.Connection.SendMessageAsync(valueSet);
                    txtResponse.Text = "Monthly EMI : " + (response.Message["Response"] as string);
                }
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                ValueSet valueSet = new ValueSet
                {
                    { "Request", "ColorChange" },
                    { "ColorCode", txtColor.Text }
                };

                if (App.Connection != null)
                {
                    await App.Connection.SendMessageAsync(valueSet);
                }
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.ToString()).ShowAsync();
            }
        }
    }
}
