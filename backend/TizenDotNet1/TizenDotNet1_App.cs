﻿using Tizen.Applications;

namespace TizenDotNet1
{
    class App : ServiceApplication
    {
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnAppControlReceived(AppControlReceivedEventArgs e)
        {
            string message;
            ReceivedAppControl receivedAppControl = e.ReceivedAppControl;
            // Get Data coming from caller application
            message = receivedAppControl.ExtraData.Get<string>("key");

            if (receivedAppControl.IsReplyRequest)
            {
                AppControl replyRequest = new AppControl();
                replyRequest.ExtraData.Add("ReplyKey", message);

                // Send reply to the caller application
                receivedAppControl.ReplyToLaunchRequest(replyRequest, AppControlReplyResult.Succeeded);
            }

            base.OnAppControlReceived(e);
        }

        protected override void OnDeviceOrientationChanged(DeviceOrientationEventArgs e)
        {
            base.OnDeviceOrientationChanged(e);
        }

        protected override void OnLocaleChanged(LocaleChangedEventArgs e)
        {
            base.OnLocaleChanged(e);
        }

        protected override void OnLowBattery(LowBatteryEventArgs e)
        {
            base.OnLowBattery(e);
        }

        protected override void OnLowMemory(LowMemoryEventArgs e)
        {
            base.OnLowMemory(e);
        }

        protected override void OnRegionFormatChanged(RegionFormatChangedEventArgs e)
        {
            base.OnRegionFormatChanged(e);
        }

        protected override void OnTerminate()
        {
            base.OnTerminate();
        }

        static void Main(string[] args)
        {
            App app = new App();
            app.Run(args);
        }
    }
}
