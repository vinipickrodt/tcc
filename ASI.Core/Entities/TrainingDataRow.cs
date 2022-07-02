using System;
using System.Collections.Generic;
using System.Text;

namespace ASI.Core.Entities
{
    public class TrainingDataRow
    {
        public float[] InputNumerosFlutuantes { get; set; }
        public int[] InputNumerosInteiros { get; set; }
        public bool[] InputBooleanos { get; set; }
        public string[] InputTextos { get; set; }
        public DateTime[] InputDatas { get; set; }

        public float[] ResultNumerosFlutuantes { get; set; }
        public int[] ResultNumerosInteiros { get; set; }
        public bool[] ResultBooleanos { get; set; }
        public string[] ResultTextos { get; set; }
        public DateTime[] ResultDatas { get; set; }

        public float ResultNumeroFlutuante => ResultNumerosFlutuantes.Length > 0 ? ResultNumerosFlutuantes[0] : 0;
        public int ResultNumeroInteiro => ResultNumerosInteiros.Length > 0 ? ResultNumerosInteiros[0] : 0;
        public bool ResultBooleano => ResultBooleanos.Length > 0 ? ResultBooleanos[0] : false;
        public string ResultTexto => ResultTextos.Length > 0 ? ResultTextos[0] : null;
    }
}
