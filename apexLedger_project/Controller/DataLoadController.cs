using apexLedger.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;

namespace apexLedger.Controller
{
    [ApiController]
    [Route("api/{Controller}")]
    public class DataLoadController : ControllerBase
    {
        public LedgerContext _lc;
        public DataLoadController(LedgerContext ledgerContext) 
        { 
            _lc = ledgerContext;
        }

        [HttpGet("UserCount")]
        public ActionResult RefreshBuyerSet()
        {
            return Ok(_lc.Users.Count());
        }
    }
}
