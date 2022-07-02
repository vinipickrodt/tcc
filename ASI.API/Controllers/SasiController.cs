using ASI.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Net;
using ASI.Infra;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace ASI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class SasiController : ControllerBase
    {
        private readonly ILogger<SasiController> _logger;
        private Repository repository = new Repository();

        public SasiController(ILogger<SasiController> logger)
        {
            _logger = logger;
        }

        [HttpPost(nameof(ValidarDados))]
        [ProducesResponseType(typeof(ValidarDadosResult), (int)HttpStatusCode.OK)]
        public IActionResult ValidarDados(int modeloId, string dados)
        {
            return Ok(new ValidarDadosResult());
        }

        [HttpPost(nameof(ObterSugestoesDados))]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public IActionResult ObterSugestoesDados(int modeloId, Dictionary<string, string> dados)
        {
            var modeloTreinado = repository.BuscaModeloTreinado(modeloId);
            var output = modeloTreinado.Predict(dados);
            return Ok(output);
        }

        [HttpPost(nameof(CriarModelo))]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        public IActionResult CriarModelo(Modelo modelo)
        {
            if (modelo.IsValid)
            {
                var modeloId = repository.CriarModelo(modelo);

                return Ok(modeloId);
            }

            return BadRequest();
        }

        [HttpPost(nameof(InserirDadosTreinamentoEmLote))]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        public IActionResult InserirDadosTreinamentoEmLote(DadosTreinamentoEmLote modeloViewModel)
        {
            var modelo = repository.BuscaModelo(modeloViewModel.ModeloId);

            Utils.ValidaDados(modeloViewModel.Dados, modelo);

            repository.InserirDadosDeTreinamentoEmLote(modeloViewModel);

            return Ok();
        }

        [HttpPost(nameof(ObterProximoModeloParaTreinar))]
        [ProducesResponseType(typeof(Modelo), (int)HttpStatusCode.OK)]
        public IActionResult ObterProximoModeloParaTreinar()
        {
            var modelo = repository.ObtemProximoModeloParaTreinar();
            return Ok(modelo);
        }

        [HttpPost(nameof(ObterDadosTreinamento))]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.OK)]
        public IActionResult ObterDadosTreinamento(int parametrosModeloId)
        {
            var dados = repository.ObtemDadosTreinamento(parametrosModeloId);
            return Ok(dados);
        }

        [HttpPost(nameof(AtualizarModeloTreinamento))]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> AtualizarModeloTreinamento(TreinoModeloArquivo treinoModeloArquivo)
        {
            var modelo = repository.BuscaModelo(treinoModeloArquivo.ModeloId);

            if (modelo == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(treinoModeloArquivo.ModeloTreinadoBase64))
                return BadRequest("modelo treinado não foi encontrado na requisição.");

            var dados = Convert.FromBase64String(treinoModeloArquivo.ModeloTreinadoBase64);

            repository.GravaTreinamentoModelo(treinoModeloArquivo.ModeloId, dados);

            return Ok(true);
        }
    }

    public class TreinoModeloArquivo
    {
        public int ModeloId { get; set; }
        public string ModeloTreinadoBase64 { get; set; }
    }
}
