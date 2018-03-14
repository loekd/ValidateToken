using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace ValidateToken.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new[]
            {
                "private-resource1",
                "private-resource2"
            };
        }

        // GET api/values/anonymous
        [HttpGet("anonymous")]
        public IEnumerable<string> GetAnonymous()
        {
            return new[]
            {
                "anonymous-resource1",
                "anonymous-resource2"
            };
        }
    }
}
