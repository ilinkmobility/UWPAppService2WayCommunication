using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace WPFApp
{
    public class WPFAppServiceHandler
    {
        static AppServiceConnection connection = null;
        static IMainWindowCallback mainWindowCallback = null;

        public static void StartWPFAppService(IMainWindowCallback mainWindowCallback)
        {
            WPFAppServiceHandler.mainWindowCallback = mainWindowCallback;

            Thread appServiceThread = new Thread(new ThreadStart(ThreadProc));
            appServiceThread.Start();
        }

        static async void ThreadProc()
        {
            connection = new AppServiceConnection();
            connection.AppServiceName = "com.ilink-systems.wpfappservice";
            connection.PackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName;
            connection.RequestReceived += Connection_RequestReceived;

            AppServiceConnectionStatus status = await connection.OpenAsync();
        }

        /// <summary>
        /// Receives message from UWP app and sends a response back
        /// </summary>
        private static void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var input = args.Request.Message;

            if (input["Request"].ToString() == "Conversion")
            {
                ValueSet returnData = new ValueSet();

                double amount = Convert.ToInt32(input["Amount"] as string);
                int period = Convert.ToInt32(input["Period"] as string);
                double rate = Convert.ToDouble(input["Rate"] as string);

                ValueSet valueSet = new ValueSet();
                valueSet.Add("Response", GEtEMI(amount, period, rate).ToString());
                args.Request.SendResponseAsync(valueSet).Completed += delegate { };
            }
            else if (input["Request"].ToString() == "ColorChange")
            {
                mainWindowCallback.OnBackgroundColorChange(input["ColorCode"].ToString());
            }
        }

        private static int GEtEMI(double amount, int period, double rate)
        {
            //Principale amount.
            var A = amount;

            //Number of Periodic Payments(N) = Payments per year times number of years.
            var N = period * 12;

            //Periodic Interest Rate(I) = Annual rate divided by number of payments per year.
            var I = ((float)rate / 100) / 12;

            //X = {[(1 + i) ^n] - 1}.
            var X = Math.Pow(1 + I, N) - 1;
            //Y = [i(1 + i)^n].
            var Y = Math.Pow(1 + I, N) * I;

            //Discount Factor (D) = {[(1 + i) ^n] - 1} / [i(1 + i)^n].
            var D = X / Y;

            //Loan Payment = Amount (A)/ Discount Factor(D).
            var emiAmount = A / D;

            return (int)emiAmount;
        }
    }
}
