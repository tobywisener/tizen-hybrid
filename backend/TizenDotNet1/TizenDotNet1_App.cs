using Tizen;
using Tizen.Applications;
using Tizen.Applications.Messages;

namespace TizenDotNet1
{
    class App : ServiceApplication
    {

        private MessagePort localPort;

        private const string PortName = "example_message_port";
        private const string MyRemoteAppId = "YyOUsYYoBY.Example";

        protected override void OnCreate()
        {
            base.OnCreate();

            Log.Debug("TAG", "Create");
            this.localPort = new MessagePort("nativeMessagePort", false);
            this.localPort.Listen();
        }

        protected void sendToWebApp(string message)
        {
            var msg = new Bundle();
            msg.AddItem("message", message);
            this.localPort.Send(msg, MyRemoteAppId, PortName);
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

            this.sendToWebApp("boom");
        }

/*        public void SsdpBrowser_ServiceFound(object sender, SsdpServiceFoundEventArgs e)
        {
            // Silence is golden (for now)
        }
*/
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
