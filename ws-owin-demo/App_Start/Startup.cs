using System.Web.Http;
using Owin;

namespace WsOwinDemo
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            builder.UseWebApi(config);
        }
    }
}
