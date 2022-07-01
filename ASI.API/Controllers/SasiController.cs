using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SasiController : ControllerBase
    {
        private readonly ILogger<SasiController> _logger;

        public SasiController(ILogger<SasiController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ValidarDadosResult ValidarDados(int modeloId, string dados)
        {
            return new ValidarDadosResult();
        }
    }
}
