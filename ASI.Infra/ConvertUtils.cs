using ASI.Core.Entities;
using ASI.Infra.ML;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace ASI.Infra
{
    public class ConvertUtils
    {
        internal static Modelo ConverterModeloDb(Data.Models.Modelo modelo)
        {
            var result = new Modelo();

            result.Id = modelo.Id;
            result.Nome = modelo.Nome;
            result.Habilitado = modelo.Status == 1;
            result.DataUltimoTreinamento = modelo.DataUltimoTreinamento;
            result.StatusTreinamento = (eModeloStatusTreinamento)modelo.StatusTreinamento;
            //result.Acuracia = modelo.Acuracia;
            result.Tipo = (eModeloTipo)modelo.TipoModelo;
            result.TreinamentoFrequencia = modelo.FrequenciaTreinamentos;

            var parametrosMaisRecentes = modelo.ParametrosModelo.OrderByDescending(pm => pm.Data).FirstOrDefault();

            if (parametrosMaisRecentes != null)
            {
                result.ParametrosModeloId = parametrosMaisRecentes.Id;
                result.CamposEntrada = JsonConvert.DeserializeObject<ModeloCampo[]>(parametrosMaisRecentes.CamposEntrada);
                result.CamposSaida = JsonConvert.DeserializeObject<ModeloCampo[]>(parametrosMaisRecentes.CamposSaida);
            }

            return result;
        }

        internal static ModeloTreinado ConvertModeloTreinadoDb(Data.Models.ModeloTreinado modeloTreinado)
        {
            return new ModeloTreinado()
            {
                Id = modeloTreinado.Id,
                Dados = modeloTreinado.Dados,
                ParametrosModeloId = modeloTreinado.ParametrosModeloId
            };
        }
    }
}