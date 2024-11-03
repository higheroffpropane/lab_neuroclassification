using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2
{
    using System;

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
                    layers[i].Biases[j] = new Random().NextDouble() * 2.0 - 1.0;
                    for (int k = 0; k < nextSize; k++)
                    {
                        layers[i].Weights[j][k] = new Random().NextDouble() * 2.0 - 1.0;
                    }
                }
            }
        }

        // Sigmoid активационная функция
        private static double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }

        // Производная сигмоиды
        private static double DSigmoid(double y)
        {
            return y * (1.0 - y);
        }
        
        // Softmax
        private static double[] Softmax(double[] outputs)
        {
            double max = outputs.Max();
            double sum = 0;

            for (int i = 0; i < outputs.Length; i++)
            {
                outputs[i] = Math.Exp(outputs[i] - max); // Для числовой стабильности
                sum += outputs[i];
            }

            for (int i = 0; i < outputs.Length; i++)
            {
                outputs[i] /= sum;
            }

            return outputs;
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

                    // Используем сигмоид на всех слоях, кроме последнего
                    l1.Neurons[j] = Sigmoid(l1.Neurons[j]);
                }
            }

            // Применяем softmax на выходном слое для получения вероятностей классов
            return Softmax(layers[^1].Neurons);
        }

        public void Backpropagation(double[] targets)
        {
            // Массив для хранения ошибок выходного слоя
            double[] errors = new double[layers[^1].Size];

            // Вычисление ошибок для выходного слоя
            for (int i = 0; i < layers[^1].Size; i++)
            {
                errors[i] = targets[i] - layers[^1].Neurons[i];
            }

            // Обратное распространение ошибки по слоям
            for (int k = layers.Length - 2; k >= 0; k--)
            {
                Layer l = layers[k];
                Layer l1 = layers[k + 1];

                double[] errorsNext = new double[l.Size];
                double[] gradients = new double[l1.Size];

                // Вычисление градиентов для текущего слоя
                for (int i = 0; i < l1.Size; i++)
                {
                    gradients[i] = errors[i] * DSigmoid(l1.Neurons[i]);
                    gradients[i] *= learningRate; // Умножаем на скорость обучения
                }

                // Инициализация массива делт
                double[][] deltas = new double[l1.Size][];
                for (int i = 0; i < l1.Size; i++)
                {
                    deltas[i] = new double[l.Size];
                    for (int j = 0; j < l.Size; j++)
                    {
                        deltas[i][j] = gradients[i] * l.Neurons[j];
                    }
                }

                // Вычисление ошибок для предыдущего слоя
                for (int i = 0; i < l.Size; i++)
                {
                    errorsNext[i] = 0;
                    for (int j = 0; j < l1.Size; j++)
                    {
                        errorsNext[i] += l.Weights[i][j] * errors[j]; // Умножаем веса на ошибки
                    }
                }

                // Копируем ошибки для следующей итерации
                errors = new double[l.Size];
                Array.Copy(errorsNext, errors, l.Size);

                // Обновление весов
                double[][] weightsNew = new double[l.Size][];
                for (int i = 0; i < l.Size; i++)
                {
                    weightsNew[i] = new double[l1.Size];
                    for (int j = 0; j < l1.Size; j++)
                    {
                        // Обновляем веса
                        weightsNew[i][j] = l.Weights[i][j] + deltas[j][i];
                    }
                }
                l.Weights = weightsNew;

                // Обновление смещений
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

            // Разделяем данные на обучающую и валидационную выборки
            int[,] trainData = new int[trainSize, data.GetLength(1)];
            int[,] validationData = new int[validationSize, data.GetLength(1)];
            Array.Copy(data, 0, trainData, 0, trainSize * data.GetLength(1));
            Array.Copy(data, trainSize * data.GetLength(1), validationData, 0, validationSize * data.GetLength(1));

            // Открываем файл для записи метрик
            using (StreamWriter writer = new StreamWriter("metrics.txt"))
            {
                for (int epoch = 1; epoch <= epochs; epoch++)
                {
                    int correctCount = 0;
                    double totalError = 0;

                    // Обучение на обучающей выборке
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

                        int predictedClass = 0;
                        double maxOutput = outputs[0];
                        for (int k = 1; k < outputs.Length; k++)
                        {
                            if (outputs[k] > maxOutput)
                            {
                                maxOutput = outputs[k];
                                predictedClass = k;
                            }
                        }

                        if (predictedClass == actualClass - 1)
                        {
                            correctCount++;
                        }

                        for (int k = 0; k < 10; k++)
                        {
                            double error = targets[k] - outputs[k];
                            totalError += error * error;
                        }

                        Backpropagation(targets);
                    }

                    // Проверка на валидационной выборке
                    double valAccuracy = 0, valPrecision = 0, valRecall = 0, valLoss = 0;
                    for (int imgIndex = 0; imgIndex < validationSize; imgIndex++)
                    {
                        double[] inputs = new double[1024];
                        for (int k = 0; k < 1024; k++)
                        {
                            inputs[k] = validationData[imgIndex, k + 2];
                        }

                        double[] targets = new double[10];
                        int actualClass = validationData[imgIndex, 1];
                        targets[actualClass - 1] = 1;

                        double[] outputs = FeedForward(inputs);

                        // Метрика Accuracy
                        int predictedClass = Array.IndexOf(outputs, outputs.Max());
                        if (predictedClass == actualClass - 1)
                        {
                            valAccuracy++;
                        }

                        // Метрика Loss
                        for (int k = 0; k < 10; k++)
                        {
                            double error = targets[k] - outputs[k];
                            valLoss += error * error;
                        }

                        // Метрики Precision и Recall (рассчитываются только для бинарных меток)
                        valPrecision += CalculatePrecision(targets, outputs);
                        valRecall += CalculateRecall(targets, outputs);
                    }

                    // Нормализация метрик
                    valAccuracy /= validationSize;
                    valLoss /= validationSize;
                    valPrecision /= validationSize;
                    valRecall /= validationSize;

                    // Запись метрик в файл
                    writer.WriteLine($"{valAccuracy} {valPrecision} {valRecall} {valLoss}");

                    Console.WriteLine($"Epoch {epoch}: Accuracy={valAccuracy}, Precision={valPrecision}, Recall={valRecall}, Loss={valLoss}");
                }
            }
        }

        // Функции для вычисления Precision и Recall
        private double CalculatePrecision(double[] targets, double[] outputs)
        {
            int truePositives = 0, predictedPositives = 0;

            for (int i = 0; i < targets.Length; i++)
            {
                if (outputs[i] >= 0.5) predictedPositives++;
                if (targets[i] == 1 && outputs[i] >= 0.5) truePositives++;
            }

            return predictedPositives == 0 ? 0 : (double)truePositives / predictedPositives;
        }

        private double CalculateRecall(double[] targets, double[] outputs)
        {
            int truePositives = 0, actualPositives = 0;

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] == 1) actualPositives++;
                if (targets[i] == 1 && outputs[i] >= 0.5) truePositives++;
            }

            return actualPositives == 0 ? 0 : (double)truePositives / actualPositives;
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
