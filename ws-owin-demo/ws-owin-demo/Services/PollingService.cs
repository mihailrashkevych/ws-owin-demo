using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using Microsoft.Owin.Hosting;
using WsOwinDemo.Configuration;
using WsOwinDemo.Monitoring;

namespace WsOwinDemo.Services
{
    public partial class PollingService : ServiceBase
    {
        private readonly IDisposable _service;
        private readonly List<ServiceMonitor> _monitors;

        public PollingService(ConfigSettings config)
        {
            var logger = new Logging.LogWriter(config.LogFilePath);
            _service = WebApp.Start<Startup>(config.BaseAddress);
            _monitors = new List<ServiceMonitor>();
            _monitors.AddRange(config.WebServices.Select(ws => new ServiceMonitor(ws.Address, ws.Interval, logger)));
            
            InitializeComponent();
        }

        public void StartPolling()
        {
            _monitors.ForEach(monitor => monitor.Start());
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            StartPolling();
        }

        protected override void OnStop()
        {
            _monitors.ForEach(monitor => monitor.Stop());
        }
    }
}
