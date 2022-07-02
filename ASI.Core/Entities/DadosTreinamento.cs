using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASI.Core.Entities
{
    public class DadosTreinamento
    {
        private List<Dictionary<string, string>> items;

        public DadosTreinamento() { }
        public DadosTreinamento(string dadosTreinamento)
        {
            items = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(dadosTreinamento);
        }
    }
}
