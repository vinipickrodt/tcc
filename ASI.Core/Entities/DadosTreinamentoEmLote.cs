using System.Collections.Generic;

namespace ASI.Core.Entities
{
    public class DadosTreinamentoEmLote
    {
        public int ModeloId { get; set; }
        public List<Dictionary<string, string>> Dados { get; set; }
    }
}