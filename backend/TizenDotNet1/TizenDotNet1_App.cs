using Tizen;
using Tizen.Applications;
using Tizen.Applications.Messages;
using Tizen.Network.Nsd;

namespace TizenDotNet1
{
    public class App : ServiceApplication
    {
        private MessagePort localPort;

        private const string PortName = "example_message_port";
        private const string MyRemoteAppId = "YyOUsYYoBY.Example";

        // https://docs.tizen.org/application/dotnet/api/TizenFX/latest/api/Tizen.Network.Nsd.SsdpBrowser.html
        SsdpBrowser browser;

        public App()
        {
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            this.localPort = new MessagePort("nativeMessagePort", false);
            this.localPort.Listen();

            browser = new SsdpBrowser("ssdp:all");

            // Uncomment the following line to break the service app
            browser.ServiceFound += (object sender, SsdpServiceFoundEventArgs ef) => {
            };
        }

        /**
         * Method used to send a message back to the web application.
         */
        protected void sendToWebApp(string title, string message)
        {
            var msg = new Bundle();
            msg.AddItem(title, message);
            this.localPort.Send(msg, MyRemoteAppId, PortName);
        }

        protected override void OnAppControlReceived(AppControlReceivedEventArgs e)
        {
            string action;
            ReceivedAppControl receivedAppControl = e.ReceivedAppControl;

            // Get Data coming from caller application
            action = receivedAppControl.ExtraData.Get<string>("key");

            /*
             *
             * Code below commented out to troubleshoot crash in service application on TVs

            switch (action)
            {
                case "browse":
                    string contentDirectoryControlUrl = receivedAppControl.ExtraData.Get<string>("url"),
                        objectId = receivedAppControl.ExtraData.Get<string>("objectId");

                    // Contact the DLNA server to retreive content directory listing
                    string response = SSDP.browseContentDirectory(contentDirectoryControlUrl, objectId);

                    // Send the response back
                    this.sendToWebApp("DLNA_BROWSE", response);
                    break;

                case "scan":
                    this.sendToWebApp("DLNA_START", "Starting SSDP network discovery... ");

                    // Kick off DLNA scan via SSDP
                    SSDP.Start();

                    // Sleep main thread for 14 seconds (replies come asynchronously via dlnaServerFoundHandler
                    Thread.Sleep(14000);

                    // Close the DLNA scan
                    SSDP.Stop();
                    break;
            }
            */

            if (receivedAppControl.IsReplyRequest)
            {
                // Send a formal reply to the received app control after processing
                AppControl replyRequest = new AppControl();
                replyRequest.ExtraData.Add("ReplyKey", action);

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
             * Global error handler - pretty useless unless you have Samsung Partnership
             * so you can actually diagnose issues via console/debugging.
             *
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
