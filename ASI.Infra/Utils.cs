using ASI.Core.Entities;
using Microsoft.ML;
using Microsoft.ML.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Infra
{
    public static class Utils
    {
        private static CultureInfo enUS = new CultureInfo("en-US");

        public static T[] ObtemValores<T>(Dictionary<string, string> item, string[] keys, eModeloCampoTipo tipoCampo)
        {
            var results = keys.Select(k => item[k]).ToArray();

            switch (tipoCampo)
            {
                case eModeloCampoTipo.Texto:
                    return results.Select(r => r).ToArray() as T[];
                case eModeloCampoTipo.NumeroFlutuante:
                    return results.Select(r => Convert.ToSingle(r, enUS)).ToArray() as T[];
                case eModeloCampoTipo.NumeroInteiro:
                    return results.Select(r => Convert.ToInt32(r, enUS)).ToArray() as T[];
                default:
                    throw new InvalidOperationException();
            }
        }

        public static (Dictionary<eModeloCampoTipo, string[]>, Dictionary<eModeloCampoTipo, string[]>) ObtemEntradasESaidasDoModelo(Modelo modelo)
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

        public static void ValidaDados(List<Dictionary<string, string>> registros, Modelo modelo, bool somenteEntradas = false)
        {
            var (dictEntrada, dictSaida) = ObtemEntradasESaidasDoModelo(modelo);

            foreach (var registro in registros)
            {
                foreach (var key in dictEntrada.Keys)
                {
                    switch (key)
                    {
                        case eModeloCampoTipo.NumeroFlutuante:
                            ObtemValores<float>(registro,
                                dictEntrada[eModeloCampoTipo.NumeroFlutuante],
                                eModeloCampoTipo.NumeroFlutuante);
                            break;
                        case eModeloCampoTipo.NumeroInteiro:
                            ObtemValores<int>(registro,
                                dictEntrada[eModeloCampoTipo.NumeroInteiro],
                                eModeloCampoTipo.NumeroInteiro);
                            break;
                        case eModeloCampoTipo.Texto:
                            ObtemValores<string>(registro,
                                dictEntrada[eModeloCampoTipo.Texto],
                                eModeloCampoTipo.Texto);
                            break;
                        default:
                            break;
                    }
                }

                if (somenteEntradas)
                {
                    return;
                }

                foreach (var key in dictSaida.Keys)
                {
                    switch (key)
                    {
                        case eModeloCampoTipo.NumeroFlutuante:
                            ObtemValores<float>(registro,
                                dictSaida[eModeloCampoTipo.NumeroFlutuante],
                                eModeloCampoTipo.NumeroFlutuante);
                            break;
                        case eModeloCampoTipo.NumeroInteiro:
                            ObtemValores<int>(registro,
                                dictSaida[eModeloCampoTipo.NumeroInteiro],
                                eModeloCampoTipo.NumeroInteiro);
                            break;
                        case eModeloCampoTipo.Texto:
                            ObtemValores<string>(registro,
                                dictSaida[eModeloCampoTipo.Texto],
                                eModeloCampoTipo.Texto);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public static IEnumerable<Core.Entities.TrainingDataRow> ConverteParaDadosDeTreino(Modelo modelo, IEnumerable<Dictionary<string, string>> rows, bool apenasEntradas = false)
        {
            var (dictEntrada, dictSaida) = Utils.ObtemEntradasESaidasDoModelo(modelo);

            foreach (var row in rows)
            {
                var item = row;

                var trainingDataRow = new Core.Entities.TrainingDataRow();

                foreach (var k in dictEntrada.Keys)
                {
                    switch (k)
                    {
                        case eModeloCampoTipo.NumeroFlutuante:
                            trainingDataRow.InputNumerosFlutuantes = Utils.ObtemValores<float>(item,
                                dictEntrada[eModeloCampoTipo.NumeroFlutuante],
                                eModeloCampoTipo.NumeroFlutuante);
                            break;
                        case eModeloCampoTipo.NumeroInteiro:
                            trainingDataRow.InputNumerosInteiros = Utils.ObtemValores<int>(item,
                                dictEntrada[eModeloCampoTipo.NumeroInteiro],
                                eModeloCampoTipo.NumeroInteiro);
                            break;
                        case eModeloCampoTipo.Texto:
                            trainingDataRow.InputTextos = Utils.ObtemValores<string>(item,
                                dictEntrada[eModeloCampoTipo.Texto],
                                eModeloCampoTipo.Texto);
                            break;
                        default:
                            break;
                    }
                }

                if (!apenasEntradas)
                {
                    foreach (var k in dictSaida.Keys)
                    {
                        switch (k)
                        {
                            case eModeloCampoTipo.NumeroFlutuante:
                                trainingDataRow.ResultNumerosFlutuantes = Utils.ObtemValores<float>(item,
                                    dictSaida[eModeloCampoTipo.NumeroFlutuante],
                                    eModeloCampoTipo.NumeroFlutuante);
                                break;
                            case eModeloCampoTipo.NumeroInteiro:
                                trainingDataRow.ResultNumerosInteiros = Utils.ObtemValores<int>(item,
                                    dictSaida[eModeloCampoTipo.NumeroInteiro],
                                    eModeloCampoTipo.NumeroInteiro);
                                break;
                            case eModeloCampoTipo.Texto:
                                trainingDataRow.ResultTextos = Utils.ObtemValores<string>(item,
                                    dictSaida[eModeloCampoTipo.Texto],
                                    eModeloCampoTipo.Texto);
                                break;
                            default:
                                break;
                        }
                    }
                }

                yield return trainingDataRow;
            }
        }

        public static void ConvertVectorToKnownSize(string name, int size, ref SchemaDefinition schema)
        {
            var type = ((VectorDataViewType)schema[name].ColumnType).ItemType;
            schema[name].ColumnType = new VectorDataViewType(type, size);
        }

        public static SchemaDefinition ConstroiDefinicaoSchema(Modelo modelo, bool ehMulticlasse)
        {
            var sd = SchemaDefinition.Create(typeof(Core.Entities.TrainingDataRow));

            var entradas = modelo.CamposEntrada.GroupBy(ce => ce.Tipo)
                .Select(g => (g.Key, g.Count()))
                .ToList();

            var saidas = modelo.CamposSaida.GroupBy(ce => ce.Tipo)
                .Select(g => (g.Key, g.Count()))
                .ToList();

            var camposUsados = new List<string>();

            var dvb = new DataViewSchema.Builder();

            foreach (var entrada in entradas)
            {
                switch (entrada.Key)
                {
                    case eModeloCampoTipo.Texto:
                        ConvertVectorToKnownSize(nameof(Core.Entities.TrainingDataRow.InputTextos), entrada.Item2, ref sd);
                        camposUsados.Add(nameof(Core.Entities.TrainingDataRow.InputTextos));
                        break;
                    case eModeloCampoTipo.NumeroFlutuante:
                        ConvertVectorToKnownSize(nameof(Core.Entities.TrainingDataRow.InputNumerosFlutuantes), entrada.Item2, ref sd);
                        camposUsados.Add(nameof(Core.Entities.TrainingDataRow.InputNumerosFlutuantes));
                        break;
                    case eModeloCampoTipo.NumeroInteiro:
                        ConvertVectorToKnownSize(nameof(Core.Entities.TrainingDataRow.InputNumerosInteiros), entrada.Item2, ref sd);
                        camposUsados.Add(nameof(Core.Entities.TrainingDataRow.InputNumerosInteiros));
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            foreach (var saida in saidas)
            {
                switch (saida.Key)
                {
                    case eModeloCampoTipo.Texto:
                        ConvertVectorToKnownSize(nameof(Core.Entities.TrainingDataRow.ResultTextos), saida.Item2, ref sd);
                        camposUsados.Add(!ehMulticlasse ? nameof(Core.Entities.TrainingDataRow.ResultTextos) : nameof(Core.Entities.TrainingDataRow.ResultTexto));
                        break;
                    case eModeloCampoTipo.NumeroFlutuante:
                        ConvertVectorToKnownSize(nameof(Core.Entities.TrainingDataRow.ResultNumerosFlutuantes), saida.Item2, ref sd);
                        camposUsados.Add(!ehMulticlasse ? nameof(Core.Entities.TrainingDataRow.ResultNumerosFlutuantes) : nameof(Core.Entities.TrainingDataRow.ResultNumeroFlutuante));
                        break;
                    case eModeloCampoTipo.NumeroInteiro:
                        ConvertVectorToKnownSize(nameof(Core.Entities.TrainingDataRow.ResultNumerosInteiros), saida.Item2, ref sd);
                        camposUsados.Add(!ehMulticlasse ? nameof(Core.Entities.TrainingDataRow.ResultNumerosInteiros) : nameof(Core.Entities.TrainingDataRow.ResultNumeroInteiro));
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            // remove os campos que não serão usados no treinamento
            sd.RemoveAll(c => !camposUsados.Contains(c.ColumnName));

            return sd;
        }
    }
}
