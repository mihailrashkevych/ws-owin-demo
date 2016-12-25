using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace WsOwinDemo.Configuration
{
    public class ConfigSettings
    {
        public IEnumerable<IWebService> WebServices { get; private set; }
        public string BaseAddress { get; private set; }
        public string LogFilePath { get; private set; }

        public static ConfigSettings GetConfiguration()
        {
            var section = (ApplicationConfigSection)ConfigurationManager.GetSection("ApplicationConfigSection");

            return new ConfigSettings
            {  
                WebServices = section.WebServiceItems.Cast<IWebService>(),
                BaseAddress = section.BaseAddress,
                LogFilePath = section.LogFilePath
            };
        }
    }

    public interface IWebService
    {
        Uri Address { get; }
        TimeSpan Interval { get; }
    }
}
