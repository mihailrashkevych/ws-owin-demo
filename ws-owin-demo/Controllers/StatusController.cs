using System.Web.Http;

namespace WsOwinDemo.Controllers
{
    public class StatusController : ApiController
    {
        [HttpGet]
        [Route("CheckStatus")]
        public bool CheckStatus()
        {
            return true;
        }
    }
}
