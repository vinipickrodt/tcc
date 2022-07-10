using Clientes;
using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ASI.Trainer
{
    public class Treinador
    {
        private Modelo modelo;

        public Treinador(Modelo modelo)
        {
            this.modelo = modelo;
        }

        public byte[] Treinar(IEnumerable<string> rows)
        {
            if (modelo.Tipo != EModeloTipo.ClassificadorMulticlasse)
                throw new NotSupportedException($"{modelo.Tipo} não é suportado.");

            var dvSchema = ConstroiDefinicaoSchema(modelo.Tipo == EModeloTipo.ClassificadorMulticlasse);
            var dadosTreino = ConverteParaDadosDeTreino(rows).ToList();

            MLContext mlContext = new MLContext();
            IDataView trainDataView = mlContext.Data.LoadFromEnumerable(dadosTreino, dvSchema);
            ITransformer modeloTrainado;

            switch (modelo.Tipo)
            {
                case EModeloTipo.ClassificadorBinario:
                    throw new NotSupportedException();
                case EModeloTipo.ClassificadorMulticlasse:
                    var experiment2 = mlContext.Auto().CreateMulticlassClassificationExperiment(60);
                    var r = experiment2.Execute(trainDataView, "ResultTexto");
                    modeloTrainado = r.BestRun.Model;
                    break;
                case EModeloTipo.Regressao:
                    throw new NotSupportedException();
                case EModeloTipo.Recomendacao:
                    throw new NotSupportedException();
                case EModeloTipo.Ranking:
                    throw new NotSupportedException();
                default:
                    throw new NotSupportedException();
            }

            var ms = new MemoryStream();
            mlContext.Model.Save(modeloTrainado, null, ms);
            return ms.ToArray();
        }

        private IEnumerable<Core.Entities.TrainingDataRow> ConverteParaDadosDeTreino(IEnumerable<string> rows)
        {
            var (dictEntrada, dictSaida) = Utils.ObtemEntradasESaidasDoModelo(modelo);

            foreach (var row in rows)
            {
                var item = JsonConvert.DeserializeObject<Dictionary<string, string>>(row);

                var trainingDataRow = new Core.Entities.TrainingDataRow();

                foreach (var k in dictEntrada.Keys)
                {
                    switch (k)
                    {
                        case EModeloCampoTipo.NumeroFlutuante:
                            trainingDataRow.InputNumerosFlutuantes = Utils.ObtemValores<float>(item,
                                dictEntrada[EModeloCampoTipo.NumeroFlutuante],
                                EModeloCampoTipo.NumeroFlutuante);
                            break;
                        case EModeloCampoTipo.NumeroInteiro:
                            trainingDataRow.InputNumerosInteiros = Utils.ObtemValores<int>(item,
                                dictEntrada[EModeloCampoTipo.NumeroInteiro],
                                EModeloCampoTipo.NumeroInteiro);
                            break;
                        case EModeloCampoTipo.Texto:
                            trainingDataRow.InputTextos = Utils.ObtemValores<string>(item,
                                dictEntrada[EModeloCampoTipo.Texto],
                                EModeloCampoTipo.Texto);
                            break;
                        default:
                            break;
                    }
                }

                foreach (var k in dictSaida.Keys)
                {
                    switch (k)
                    {
                        case EModeloCampoTipo.NumeroFlutuante:
                            trainingDataRow.ResultNumerosFlutuantes = Utils.ObtemValores<float>(item,
                                dictSaida[EModeloCampoTipo.NumeroFlutuante],
                                EModeloCampoTipo.NumeroFlutuante);
                            break;
                        case EModeloCampoTipo.NumeroInteiro:
                            trainingDataRow.ResultNumerosInteiros = Utils.ObtemValores<int>(item,
                                dictSaida[EModeloCampoTipo.NumeroInteiro],
                                EModeloCampoTipo.NumeroInteiro);
                            break;
                        case EModeloCampoTipo.Texto:
                            trainingDataRow.ResultTextos = Utils.ObtemValores<string>(item,
                                dictSaida[EModeloCampoTipo.Texto],
                                EModeloCampoTipo.Texto);
                            break;
                        default:
                            break;
                    }
                }

                yield return trainingDataRow;
            }
        }

        private void ConvertVectorToKnownSize(string name, int size, ref SchemaDefinition schema)
        {
            var type = ((VectorDataViewType)schema[name].ColumnType).ItemType;
            schema[name].ColumnType = new VectorDataViewType(type, size);
        }

        private SchemaDefinition ConstroiDefinicaoSchema(bool ehMulticlasse)
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
                    case EModeloCampoTipo.Texto:
                        ConvertVectorToKnownSize(nameof(Core.Entities.TrainingDataRow.InputTextos), entrada.Item2, ref sd);
                        camposUsados.Add(nameof(Core.Entities.TrainingDataRow.InputTextos));
                        break;
                    case EModeloCampoTipo.NumeroFlutuante:
                        ConvertVectorToKnownSize(nameof(Core.Entities.TrainingDataRow.InputNumerosFlutuantes), entrada.Item2, ref sd);
                        camposUsados.Add(nameof(Core.Entities.TrainingDataRow.InputNumerosFlutuantes));
                        break;
                    case EModeloCampoTipo.NumeroInteiro:
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
                    case EModeloCampoTipo.Texto:
                        ConvertVectorToKnownSize(nameof(Core.Entities.TrainingDataRow.ResultTextos), saida.Item2, ref sd);
                        camposUsados.Add(!ehMulticlasse ? nameof(Core.Entities.TrainingDataRow.ResultTextos) : nameof(Core.Entities.TrainingDataRow.ResultTexto));
                        break;
                    case EModeloCampoTipo.NumeroFlutuante:
                        ConvertVectorToKnownSize(nameof(Core.Entities.TrainingDataRow.ResultNumerosFlutuantes), saida.Item2, ref sd);
                        camposUsados.Add(!ehMulticlasse ? nameof(Core.Entities.TrainingDataRow.ResultNumerosFlutuantes) : nameof(Core.Entities.TrainingDataRow.ResultNumeroFlutuante));
                        break;
                    case EModeloCampoTipo.NumeroInteiro:
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