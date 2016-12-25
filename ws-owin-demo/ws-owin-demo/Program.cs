using System;
using System.Net;
using System.ServiceProcess;
using System.Threading;
using WsOwinDemo.Configuration;
using WsOwinDemo.Services;

namespace WsOwinDemo
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            using (var service = new PollingService(ConfigSettings.GetConfiguration()))
            {
                if (Environment.UserInteractive)
                {
                    service.StartPolling();
                    Thread.Sleep(Timeout.Infinite);
                }
                else
                {
                    ServiceBase.Run(service);
                }
            }
        }
    }
}

