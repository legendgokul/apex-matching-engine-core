using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace apexLedger.Controller
{
    [ApiController]
    [Route("api/{Controller}")]
    public class LedgerController : ControllerBase
    {

        [HttpGet("{name=User}")]
        public ActionResult<String> PingEndpoint(string name)
        {
            return Ok($"Api working name : {name}");
        }


    }
}
