﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace WPFApp
{
    public class AppServiceConsumer
    {
        private static AppServiceConsumer _instance;

        public static AppServiceConsumer Instance => _instance ?? (_instance = new AppServiceConsumer());


        public AppServiceConnection SampleAppServiceConnection;


        public async Task<string> GetResponse(ValueSet input)
        {
            var result = "";

            using (SampleAppServiceConnection = new AppServiceConnection())
            {
                //declaring the service and the package family name
                SampleAppServiceConnection.AppServiceName = "com.ilink-systems.uwpappservice";

                //this one can be found in the Package.appxmanifest file
                SampleAppServiceConnection.PackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName;

                //trying to connect to he AppService
                AppServiceConnectionStatus status = await SampleAppServiceConnection.OpenAsync();

                //no success with the AppServiceConnection
                if (status != AppServiceConnectionStatus.Success)
                {
                    return GetStatusDetail(status);
                }
                //if successful
                else
                {
                    //sending the input parameters
                    AppServiceResponse response = await SampleAppServiceConnection.SendMessageAsync(input);


                    //handling the response
                    switch (response.Status)
                    {
                        case AppServiceResponseStatus.Success:
                            result = (string)response.Message["Result"];
                            break;
                        case AppServiceResponseStatus.Failure:
                            result = "app service called failed, most likely due to wrong parameters sent to it";
                            break;
                        case AppServiceResponseStatus.ResourceLimitsExceeded:
                            result = "app service exceeded the resources allocated to it and had to be terminated";
                            break;
                        case AppServiceResponseStatus.Unknown:
                            result = "unknown error while sending the request";
                            break;
                    }
                }
            }

            return result;
        }


        private string GetStatusDetail(AppServiceConnectionStatus status)
        {
            var result = "";
            switch (status)
            {
                case AppServiceConnectionStatus.Success:
                    result = "connected";
                    break;
                case AppServiceConnectionStatus.AppNotInstalled:
                    result = "AppServiceSample seems to be not installed";
                    break;
                case AppServiceConnectionStatus.AppUnavailable:
                    result =
                        "App is currently not available (could be running an update or the drive it was installed to is not available)";
                    break;
                case AppServiceConnectionStatus.AppServiceUnavailable:
                    result = "App is installed, but the Service does not respond";
                    break;
                case AppServiceConnectionStatus.Unknown:
                    result = "Unknown error with the AppService";
                    break;
            }

            return result;
        }
    }
}
