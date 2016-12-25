using System;
using System.Configuration;

namespace WsOwinDemo.Configuration
{
    public class ApplicationConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("WebServices")]
        public WebServicesCollection WebServiceItems => (WebServicesCollection)base["WebServices"];

        [ConfigurationProperty("BaseAddress", IsKey = true, IsRequired = true)]
        public string BaseAddress => (string)base["BaseAddress"];

        [ConfigurationProperty("LogFilePath", IsKey = true, IsRequired = false)]
        public string LogFilePath => (string)base["LogFilePath"];
    }

    [ConfigurationCollection(typeof(WebServiceElement), AddItemName = "WebService")]
    public class WebServicesCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new WebServiceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (WebServiceElement)element;
        }

        public WebServiceElement this[int idx] => (WebServiceElement)BaseGet(idx);
    }

    public class WebServiceElement : ConfigurationElement, IWebService
    {
        public Uri Address => new Uri(AddressString);

        [ConfigurationProperty("address", DefaultValue = "", IsKey = true, IsRequired = true)]
        private string AddressString => (string)base["address"];

        public TimeSpan Interval => TimeSpan.FromSeconds(IntervalSeconds);

        [ConfigurationProperty("interval", DefaultValue = "60", IsKey = false, IsRequired = false)]
        private int IntervalSeconds => (int)base["interval"];
    }
}
