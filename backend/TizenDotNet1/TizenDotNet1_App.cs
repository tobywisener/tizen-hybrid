using System;
using System.Net;
using System.Threading;
using Tizen;
using Tizen.Applications;
using Tizen.Applications.Messages;
using Tizen.Network.Nsd;
using Tizen.Pims.Contacts.ContactsViews;
using static TizenDotNet1.SSDP;

namespace TizenDotNet1
{
    public class App : ServiceApplication
    {

        private MessagePort localPort;

        private const string PortName = "example_message_port";
        private const string MyRemoteAppId = "YyOUsYYoBY.Example";

        public App()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://google.com");
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            this.localPort = new MessagePort("nativeMessagePort", false);
            this.localPort.Listen();
        }

        protected void sendToWebApp(string title, string message)
        {
            var msg = new Bundle();
            msg.AddItem(title, message);
            this.localPort.Send(msg, MyRemoteAppId, PortName);
        }
        protected override void OnAppControlReceived(AppControlReceivedEventArgs e)
        {
            EventHandler<ServerFoundEventArgs> handler = (object sender, ServerFoundEventArgs ef) =>
            {
                this.sendToWebApp("DLNA_FOUND", ef.Data);
            };

            string message;
            ReceivedAppControl receivedAppControl = e.ReceivedAppControl;
            // Get Data coming from caller application
            message = receivedAppControl.ExtraData.Get<string>("key");

            this.sendToWebApp("DLNA_START", "Starting SSDP network discovery... ");

            SSDP.ServerFound += handler;

            SSDP.Start();
            Thread.Sleep(14000);
            SSDP.Stop();

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
            /*
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Tizen.Log.Fatal("MyApp", $"Caught {e.ExceptionObject}");
                Tizen.Log.Fatal("MyApp", $"Terminating!");
            };
            */

            App app = new App();
            app.Run(args);
        }
    }
}
