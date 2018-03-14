using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

namespace UWPApp.AppServiceLibrary
{
    public sealed class UWPAppServiceHandler : IBackgroundTask
    {

        private BackgroundTaskDeferral _backgroundTaskDeferral;
        private AppServiceConnection _appServiceConnection;


        public void Run(IBackgroundTaskInstance taskInstance)
        {
            //get the task instance deferral
            this._backgroundTaskDeferral = taskInstance.GetDeferral();

            //hooking up to the Canceled event to close app connection
            taskInstance.Canceled += OnTaskCanceled;

            //getting the AppServiceTriggerDetails and hooking up to the RequestReceived event
            var details = (AppServiceTriggerDetails)taskInstance.TriggerDetails;
            _appServiceConnection = details.AppServiceConnection;
            _appServiceConnection.RequestReceived += OnRequestReceived;
        }

        private void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            //async operation needs a deferral
            var msgDeferral = args.GetDeferral();

            try
            {
                var input = args.Request.Message;
                var result = new ValueSet();

                ValueSet returnData = new ValueSet();

                double amount = Convert.ToInt32(input["Amount"] as string);
                int period = Convert.ToInt32(input["Period"] as string);
                double rate = Convert.ToDouble(input["Rate"] as string);

                ValueSet valueSet = new ValueSet();
                valueSet.Add("Result", GetFDInterest(amount, period, rate).ToString());
                args.Request.SendResponseAsync(valueSet).Completed += delegate { };
            }
            //using finally because we need to tell the OS we have finished, no matter of the result
            finally
            {
                msgDeferral?.Complete();
            }
        }


        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            //we need to tell the OS we have finished also if our background task gets cancelled for whatever reason
            this._backgroundTaskDeferral?.Complete();


            //todo: implement a logging mechanism for the BackgroundTaskCancellationReason 
        }

        public static int GetFDInterest(double amount, int period, double rate)
        {
            /*
             Formula
             A = P x (1 + r/n)nt 
             I = A - P 

            Where, 
                A = Maturity Value 
                P = Principal Amount 
                r = Rate of Interest 
                t = Number of Period 
                n = Compounded Interest Frequency 
                I = Interest Earned Amount
             */

            var A = amount * (Math.Pow((1 + (rate/100) / 1), period));
            var I = A - amount;

            return Convert.ToInt32(I);
        }
    }
}
