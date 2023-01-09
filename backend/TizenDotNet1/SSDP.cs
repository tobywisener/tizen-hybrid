using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Tizen.Applications.Messages;
using static TizenDotNet1.SSDP;

namespace TizenDotNet1
{
    public static class SSDP
    {

        private static Socket UdpSocket = null;
        public static string Servers = "";
        private static string NewServer = "";
        private static Thread THSend = null;
        private static bool Running = false;

        private static App app;
        private static MessagePort messagePort;

        public static void SetMessagePort(App _app, MessagePort _messagePort)
        {
            app = _app;
            messagePort = _messagePort;
        }
        public static void Start()
        {//Stop should be called in about 12 seconds which will kill the thread
            if (Running) return;
            Running = true;
            NewServer = "";
            Thread THSend = new Thread(SendRequest);
            THSend.Start();
            Thread TH = new Thread(Stop);
            TH.Start();
        }

        public static void Stop()
        {//OK time is up so lets return our DLNA server list
            Thread.Sleep(9000);
            Running = false;
            //  try
            //  {
            Thread.Sleep(1000);
            if (UdpSocket != null)
                UdpSocket.Close();
            if (THSend != null)
                THSend.Abort();
            //   }
            //  catch {; }
            if (NewServer.Length > 0) Servers = NewServer.Trim();//Bank in our new servers
        }

        private static void SendRequest()
        {
            //try {
            SendRequestNow();
            //}
            // {; }
        }

        public static event EventHandler<ServerFoundEventArgs> ServerFound;

        static void OnServerFound(ServerFoundEventArgs e)
        {
            EventHandler<ServerFoundEventArgs> handler = ServerFound;
            if (handler != null)
            {
                handler(null, e);
            }
        }

        private static void SendRequestNow()
        {//Uses UDP Multicast on 239.255.255.250 with port 1900 to send out invitations that are slow to be answered
            IPEndPoint LocalEndPoint = new IPEndPoint(IPAddress.Any, 6000);
            IPEndPoint MulticastEndPoint = new IPEndPoint(IPAddress.Parse("239.255.255.250"), 1900);//SSDP port
            Socket UdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            UdpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            UdpSocket.Bind(LocalEndPoint);
            UdpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(MulticastEndPoint.Address, IPAddress.Any));
            UdpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 10); // was 2
            UdpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, true);
            // ST (ServiceType) can be either ssdp::all or urn:schemas-upnp-org:service:ContentDirectory:1
            string SearchString = "M-SEARCH * HTTP/1.1\r\nHOST:239.255.255.250:1900\r\nMAN:\"ssdp:discover\"\r\nST:urn:schemas-upnp-org:service:ContentDirectory:1\r\nMX:3\r\n\r\n";
            UdpSocket.SendTo(Encoding.UTF8.GetBytes(SearchString), SocketFlags.None, MulticastEndPoint);
            byte[] ReceiveBuffer = new byte[4000];
            int ReceivedBytes = 0;
            int Count = 0;
            while (Running && Count < 100)
            {//Keep loopping until we timeout or stop is called but do wait for at least ten seconds 
                Count++;
                if (UdpSocket.Available > 0)
                {
                    ReceivedBytes = UdpSocket.Receive(ReceiveBuffer, SocketFlags.None);
                    if (ReceivedBytes > 0)
                    {
                        string Data = Encoding.UTF8.GetString(ReceiveBuffer, 0, ReceivedBytes);
                        if (Data.ToUpper().IndexOf("LOCATION: ") > -1)
                        {//ChopOffAfter is an extended string method added in Helper.cs
                            Data = Data.ChopOffBefore("LOCATION: ").ChopOffAfter(".xml");
                            if (NewServer.ToLower().IndexOf(Data.ToLower()) == -1)
                            {
                                NewServer += " " + Data;
                                ServerFoundEventArgs args = new ServerFoundEventArgs();
                                args.Data = Data + ".xml";
                                OnServerFound(args);
                            }

                        }
                    }
                }
                else
                    Thread.Sleep(100);
            }
            if (NewServer.Length > 0) Servers = NewServer.Trim();//Bank in our new servers nice and quick with minute risk of thread error due to not locking
            UdpSocket.Close();
            THSend = null;
            UdpSocket = null;
        }

        public static void browseContentDirectory(string baseUrl, string objectId)
        {
            /*
             * RestClient restClient = new RestClient(baseUrl);

            //client.Timeout = -1;
            var request = new RestRequest("/service/ContentDirectory_control", Method.Post);
            request.AddHeader("SOAPAction", "urn:schemas-upnp-org:service:ContentDirectory:1#Browse");
            var rawXml = "<?xml version=\"1.0\"?>\r\n<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">\r\n  <s:Body>\r\n    <u:Browse xmlns:u=\"urn:schemas-upnp-org:service:ContentDirectory:1\">\r\n      <ObjectID>" + objectId + "</ObjectID>\r\n      <BrowseFlag>BrowseDirectChildren</BrowseFlag>\r\n      <Filter>*</Filter>\r\n      <StartingIndex>0</StartingIndex>\r\n      <RequestedCount>0</RequestedCount>\r\n      <SortCriteria></SortCriteria>\r\n    </u:Browse>\r\n  </s:Body>\r\n</s:Envelope>";

            request.AddParameter("text/plain", rawXml, ParameterType.RequestBody);
            RestResponse response = restClient.Execute(request);
            string decodedResponse = System.Net.WebUtility.HtmlDecode(response.Content);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(decodedResponse);

            var nodes = xmlDoc.GetElementsByTagName("container");

            foreach (XmlNode node in nodes)
            {
                XmlAttribute id = node.Attributes["id"];
                Console.WriteLine(id.Value);

                Console.WriteLine(node["dc:title"].InnerText);
            }
            */
        }


        /*
         * START OF STRING UTILITY FUNCTIONS
         */

        public static string ChopOffBefore(this string s, string Before)
        {//Usefull function for chopping up strings
            int End = s.ToUpper().IndexOf(Before.ToUpper());
            if (End > -1)
            {
                return s.Substring(End + Before.Length);
            }
            return s;
        }

        public static string ChopOffAfter(this string s, string After)
        {//Usefull function for chopping up strings
            int End = s.ToUpper().IndexOf(After.ToUpper());
            if (End > -1)
            {
                return s.Substring(0, End);
            }
            return s;
        }

        public static string ReplaceIgnoreCase(this string Source, string Pattern, string Replacement)
        {// using \\$ in the pattern will screw this regex up
         //return Regex.Replace(Source, Pattern, Replacement, RegexOptions.IgnoreCase);

            if (Regex.IsMatch(Source, Pattern, RegexOptions.IgnoreCase))
                Source = Regex.Replace(Source, Pattern, Replacement, RegexOptions.IgnoreCase);
            return Source;
        }

        public class ServerFoundEventArgs
        {
            public string Data { get; set; }
        }
    }
}
