using Clientes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ASI.Trainer
{
    public class Utils
    {
        public static (Dictionary<EModeloCampoTipo, string[]>, Dictionary<EModeloCampoTipo, string[]>) ObtemEntradasESaidasDoModelo(Modelo modelo)
        {
            var dictEntradas = modelo.CamposEntrada
                .GroupBy(ce => ce.Tipo)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Select(c => c.Nome).OrderBy(c => c).ToArray());

            var dictSaidas = modelo.CamposSaida
                .GroupBy(ce => ce.Tipo)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Select(c => c.Nome).OrderBy(c => c).ToArray());

            return (dictEntradas, dictSaidas);
        }

        private static CultureInfo enUS = new CultureInfo("en-US");

        public static T[] ObtemValores<T>(Dictionary<string, string> item, string[] keys, EModeloCampoTipo tipoCampo)
        {
            var results = keys.Select(k => item[k]).ToArray();

            switch (tipoCampo)
            {
                case EModeloCampoTipo.Texto:
                    return results.Select(r => r).ToArray() as T[];
                case EModeloCampoTipo.NumeroFlutuante:
                    return results.Select(r => Convert.ToSingle(r, enUS)).ToArray() as T[];
                case EModeloCampoTipo.NumeroInteiro:
                    return results.Select(r => Convert.ToInt32(r, enUS)).ToArray() as T[];
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
