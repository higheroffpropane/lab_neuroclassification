using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace lab2
{
    public class NeuralNetwork
    {
        private double learningRate;
        private Layer[] layers;

        public NeuralNetwork(double learningRate, params int[] sizes)
        {
            this.learningRate = learningRate;
            layers = new Layer[sizes.Length];

            for (int i = 0; i < sizes.Length; i++)
            {
                int nextSize = i < sizes.Length - 1 ? sizes[i + 1] : 0;
                layers[i] = new Layer(sizes[i], nextSize);

                for (int j = 0; j < sizes[i]; j++)
                {
                    layers[i].Biases[j] = 0;
                    for (int k = 0; k < nextSize; k++)
                    {
                        layers[i].Weights[j][k] = new Random().NextDouble() * 2.0 - 1.0;
                    }
                }
            }
        }

        private static double Sigmoid(double x) => 1.0 / (1.0 + Math.Exp(-x));

        private static double DSigmoid(double y) => y * (1.0 - y);

        private static double[] Softmax(double[] outputs)
        {
            double max = outputs.Max();
            double[] exps = outputs.Select(o => Math.Exp(o - max)).ToArray();
            double sum = exps.Sum();
            return exps.Select(e => e / sum).ToArray();
        }

        public double[] FeedForward(double[] inputs)
        {
            Array.Copy(inputs, layers[0].Neurons, inputs.Length);

            for (int i = 1; i < layers.Length; i++)
            {
                Layer l = layers[i - 1];
                Layer l1 = layers[i];

                for (int j = 0; j < l1.Size; j++)
                {
                    l1.Neurons[j] = 0;

                    for (int k = 0; k < l.Size; k++)
                    {
                        l1.Neurons[j] += l.Neurons[k] * l.Weights[k][j];
                    }

                    l1.Neurons[j] += l1.Biases[j];
                    l1.Neurons[j] = Sigmoid(l1.Neurons[j]);
                }
            }

            return Softmax(layers[^1].Neurons);
        }

        public void Backpropagation(double[] targets)
        {
            double[] errors = new double[layers[^1].Size];

            for (int i = 0; i < layers[^1].Size; i++)
            {
                errors[i] = targets[i] - layers[^1].Neurons[i];
            }

            for (int k = layers.Length - 2; k >= 0; k--)
            {
                Layer l = layers[k];
                Layer l1 = layers[k + 1];

                double[] errorsNext = new double[l.Size];
                double[] gradients = new double[l1.Size];

                for (int i = 0; i < l1.Size; i++)
                {
                    gradients[i] = errors[i] * DSigmoid(l1.Neurons[i]) * learningRate;
                }

                double[][] deltas = new double[l1.Size][];
                for (int i = 0; i < l1.Size; i++)
                {
                    deltas[i] = new double[l.Size];
                    for (int j = 0; j < l.Size; j++)
                    {
                        deltas[i][j] = gradients[i] * l.Neurons[j];
                    }
                }

                for (int i = 0; i < l.Size; i++)
                {
                    errorsNext[i] = 0;
                    for (int j = 0; j < l1.Size; j++)
                    {
                        errorsNext[i] += l.Weights[i][j] * errors[j];
                    }
                }

                errors = errorsNext;

                for (int i = 0; i < l.Size; i++)
                {
                    for (int j = 0; j < l1.Size; j++)
                    {
                        l.Weights[i][j] -= learningRate * deltas[j][i];
                    }
                }

                for (int i = 0; i < l1.Size; i++)
                {
                    l1.Biases[i] += gradients[i];
                }
            }
        }


        public void Train(int epochs, int[,] data, double validationSplit = 0.2)
        {
            int samples = data.GetLength(0);
            int validationSize = (int)(samples * validationSplit);
            int trainSize = samples - validationSize;

            int[,] trainData = new int[trainSize, data.GetLength(1)];
            int[,] validationData = new int[validationSize, data.GetLength(1)];

            for (int i = 0; i < trainSize; i++)
                for (int j = 0; j < data.GetLength(1); j++)
                    trainData[i, j] = data[i, j];

            for (int i = 0; i < validationSize; i++)
                for (int j = 0; j < data.GetLength(1); j++)
                    validationData[i, j] = data[trainSize + i, j];

            using (StreamWriter writer = new StreamWriter("metrics.txt"))
            {
                for (int epoch = 1; epoch <= epochs; epoch++)
                {
                    double totalLoss = 0;

                    for (int imgIndex = 0; imgIndex < trainSize; imgIndex++)
                    {
                        double[] inputs = new double[1024];
                        for (int k = 0; k < 1024; k++)
                        {
                            inputs[k] = trainData[imgIndex, k + 2];
                        }

                        double[] targets = new double[10];
                        int actualClass = trainData[imgIndex, 1];
                        targets[actualClass - 1] = 1;

                        double[] outputs = FeedForward(inputs);

                        double exampleLoss = 0;
                        for (int k = 0; k < 10; k++)
                        {
                            double output = Math.Max(outputs[k], 1e-15);
                            exampleLoss -= targets[k] * Math.Log(output);
                        }

                        totalLoss += exampleLoss;
                        Backpropagation(targets);
                    }

                    double averageLoss = totalLoss / trainSize;

                    double valAccuracy = 0, valPrecision, valRecall;
                    int[] trueClasses = new int[validationSize];
                    int[] predictedClasses = new int[validationSize];

                    for (int imgIndex = 0; imgIndex < validationSize; imgIndex++)
                    {
                        double[] inputs = new double[1024];
                        for (int k = 0; k < 1024; k++)
                        {
                            inputs[k] = validationData[imgIndex, k + 2];
                        }

                        int actualClass = validationData[imgIndex, 1];
                        trueClasses[imgIndex] = actualClass - 1;

                        double[] outputs = FeedForward(inputs);
                        int predictedClass = Array.IndexOf(outputs, outputs.Max());
                        predictedClasses[imgIndex] = predictedClass;

                        if (predictedClass == trueClasses[imgIndex])
                        {
                            valAccuracy++;
                        }
                    }

                    valAccuracy /= validationSize;

                    valPrecision = CalculatePrecision(trueClasses, predictedClasses, 10);
                    valRecall = CalculateRecall(trueClasses, predictedClasses, 10);

                    writer.WriteLine($"Epoch {epoch}: Loss={Math.Round(averageLoss, 4)}, Accuracy={Math.Round(valAccuracy, 4)}, Precision={Math.Round(valPrecision, 4)}, Recall={Math.Round(valRecall, 4)}");
                    Console.WriteLine($"Epoch {epoch}: Loss={averageLoss}, Accuracy={valAccuracy}, Precision={valPrecision}, Recall={valRecall}");
                }
            }

            SaveWeights("weights.txt");
        }

        private double CalculatePrecision(int[] trueClasses, int[] predictedClasses, int numClasses)
        {
            double precisionSum = 0;

            for (int c = 0; c < numClasses; c++)
            {
                int tp = 0, fp = 0;

                for (int i = 0; i < trueClasses.Length; i++)
                {
                    if (predictedClasses[i] == c)
                    {
                        if (trueClasses[i] == c) tp++;  // True Positive
                        else fp++;  // False Positive
                    }
                }

                // Precision для текущего класса
                if ((tp + fp) > 0)
                    precisionSum += (double)tp / (tp + fp);
            }

            // Усредняем Precision по всем классам
            return precisionSum / numClasses;
        }

        private double CalculateRecall(int[] trueClasses, int[] predictedClasses, int numClasses)
        {
            double recallSum = 0;

            for (int c = 0; c < numClasses; c++)
            {
                int tp = 0, fn = 0;

                for (int i = 0; i < trueClasses.Length; i++)
                {
                    if (trueClasses[i] == c)
                    {
                        if (predictedClasses[i] == c) tp++;  // True Positive
                        else fn++;  // False Negative
                    }
                }

                // Recall для текущего класса
                if ((tp + fn) > 0)
                    recallSum += (double)tp / (tp + fn);
            }

            // Усредняем Recall по всем классам
            return recallSum / numClasses;
        }




        public int Predict(double[] inputs)
        {
            // Прогоняем входные данные через сеть
            double[] outputs = FeedForward(inputs);

            // Находим индекс с максимальным значением (это и будет предполагаемый класс)
            int predictedClass = 0;
            double maxOutput = outputs[0];
            for (int i = 1; i < outputs.Length; i++)
            {
                if (outputs[i] > maxOutput)
                {
                    maxOutput = outputs[i];
                    predictedClass = i;
                }
            }

            return predictedClass + 1; // Возвращаем класс (индексация с 1 до 10)
        }


        public void SaveWeights(string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                for (int k = 0; k < layers.Length - 1; k++)
                {
                    Layer layer = layers[k];
                    for (int i = 0; i < layer.Weights.Length; i++)
                    {
                        sw.WriteLine(string.Join(";", layer.Weights[i]));
                    }
                    sw.WriteLine(); // Пустая строка между слоями
                }
            }
        }

        public void LoadWeights(string filePath)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                for (int k = 0; k < layers.Length - 1; k++)
                {
                    Layer layer = layers[k];

                    // Для каждого слоя читаем веса
                    for (int i = 0; i < layer.Weights.Length; i++)
                    {
                        string line = sr.ReadLine();
                        if (line == null)
                        {
                            throw new Exception($"Not enough weight data for layer {k}. Expected weights for neuron {i}.");
                        }

                        string[] weights = line.Split(';');

                        // Проверяем, что длина массива weights соответствует ожидаемой длине
                        if (weights.Length != layer.Weights[i].Length)
                        {
                            throw new Exception($"Mismatch in weight count for layer {k}, neuron {i}: expected {layer.Weights[i].Length}, got {weights.Length}.");
                        }

                        for (int j = 0; j < weights.Length; j++)
                        {
                            layer.Weights[i][j] = double.Parse(weights[j]);
                        }
                    }

                    // Пропустить пустую строку между слоями, если она есть
                    if (sr.Peek() != -1)
                    {
                        sr.ReadLine(); // Пропускаем пустую строку
                    }
                }
            }
        }



    }

    public class Layer
    {
        public int Size { get; }
        public double[] Neurons { get; set; }
        public double[] Biases { get; set; }
        public double[][] Weights { get; set; }

        public Layer(int size, int nextSize)
        {
            Size = size;
            Neurons = new double[size];
            Biases = new double[size];
            Weights = new double[size][];
            for (int i = 0; i < size; i++)
            {
                Weights[i] = new double[nextSize];
            }
        }
    }

}
