using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Core.Entities
{
    public class Modelo
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime? DataUltimoTreinamento { get; set; }
        public eModeloTipo Tipo { get; set; }
        public double Acuracia { get; set; }
        public bool Habilitado { get; set; }
        public eModeloStatusTreinamento StatusTreinamento { get; set; }
        public ModeloCampo[] CamposEntrada { get; set; }
        public ModeloCampo[] CamposSaida { get; set; }
        public string TreinamentoFrequencia { get; set; }

        [JsonIgnore]
        public bool IsValid => !string.IsNullOrWhiteSpace(Nome) &&
            CamposEntrada.Length > 0 && CamposEntrada.All(ce => ce.IsValid) &&
            CamposSaida.Length > 0 && CamposSaida.All(ce => ce.IsValid) &&
            Enum.IsDefined(typeof(eModeloTipo), Tipo);

        public int ParametrosModeloId { get; set; }
    }

    public class ModeloCampo
    {
        public string Nome { get; set; }
        public eModeloCampoTipo Tipo { get; set; }

        [JsonIgnore]
        public bool IsValid => !string.IsNullOrWhiteSpace(Nome) &&
            Enum.IsDefined(typeof(eModeloCampoTipo), Tipo);
    }

    public enum eModeloCampoTipo
    {
        Texto = 1,
        NumeroFlutuante = 2,
        NumeroInteiro = 3,
    }

    public enum eModeloTipo
    {
        ClassificadorBinario = 1,
        ClassificadorMulticlasse = 2,
        Regressao = 3,
        Recomendacao = 4,
        Ranking = 5
    }

    public enum eModeloStatusTreinamento
    {
        Parado = 1,
        Treinando = 2,
    }
}
