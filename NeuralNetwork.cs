using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace lab2
{
    public class NeuralNetwork
    {
        private int inputSize, hiddenSize1, hiddenSize2, outputSize;
        private Matrix<double> weightsInputToHidden1, weightsHidden1ToHidden2, weightsHidden2ToOutput;
        private Vector<double> biasHidden1, biasHidden2, biasOutput;

        public NeuralNetwork(int inputSize, int hiddenSize1, int hiddenSize2, int outputSize)
        {
            this.inputSize = inputSize;
            this.hiddenSize1 = hiddenSize1;
            this.hiddenSize2 = hiddenSize2;
            this.outputSize = outputSize;

            // Инициализация весов и смещений случайными значениями
            weightsInputToHidden1 = Matrix<double>.Build.Random(hiddenSize1, inputSize);
            weightsHidden1ToHidden2 = Matrix<double>.Build.Random(hiddenSize2, hiddenSize1);
            weightsHidden2ToOutput = Matrix<double>.Build.Random(outputSize, hiddenSize2);
            biasHidden1 = Vector<double>.Build.Random(hiddenSize1);
            biasHidden2 = Vector<double>.Build.Random(hiddenSize2);
            biasOutput = Vector<double>.Build.Random(outputSize);
        }

        // Функция активации (сигмоида)
        private double Sigmoid(double x) => 1.0 / (1.0 + Math.Exp(-x));

        // Производная сигмоиды
        private double SigmoidDerivative(double x) => Sigmoid(x) * (1 - Sigmoid(x));

        // Прямое распространение
        public Vector<double> FeedForward(Vector<double> input)
        {
            var hiddenLayer1 = (weightsInputToHidden1 * input + biasHidden1).Map(Sigmoid);
            var hiddenLayer2 = (weightsHidden1ToHidden2 * hiddenLayer1 + biasHidden2).Map(Sigmoid);
            var outputLayer = (weightsHidden2ToOutput * hiddenLayer2 + biasOutput).Map(Sigmoid);
            return outputLayer;
        }

        // Обратное распространение ошибки (обучение)
        public void Backpropagate(Vector<double> input, Vector<double> target, double learningRate)
        {
            // Прямое распространение
            var hiddenLayer1 = (weightsInputToHidden1 * input + biasHidden1).Map(Sigmoid);
            var hiddenLayer2 = (weightsHidden1ToHidden2 * hiddenLayer1 + biasHidden2).Map(Sigmoid);
            var outputLayer = (weightsHidden2ToOutput * hiddenLayer2 + biasOutput).Map(Sigmoid);

            // Вычисление ошибки на выходе
            var outputError = target - outputLayer;
            var outputDelta = outputError.PointwiseMultiply(outputLayer.Map(SigmoidDerivative));

            // Ошибка на втором скрытом слое
            var hidden2Error = weightsHidden2ToOutput.TransposeThisAndMultiply(outputDelta);
            var hidden2Delta = hidden2Error.PointwiseMultiply(hiddenLayer2.Map(SigmoidDerivative));

            // Ошибка на первом скрытом слое
            var hidden1Error = weightsHidden1ToHidden2.TransposeThisAndMultiply(hidden2Delta);
            var hidden1Delta = hidden1Error.PointwiseMultiply(hiddenLayer1.Map(SigmoidDerivative));

            // Обновление весов
            weightsHidden2ToOutput += learningRate * outputDelta.OuterProduct(hiddenLayer2);
            weightsHidden1ToHidden2 += learningRate * hidden2Delta.OuterProduct(hiddenLayer1);
            weightsInputToHidden1 += learningRate * hidden1Delta.OuterProduct(input);

            // Обновление смещений
            biasOutput += learningRate * outputDelta;
            biasHidden2 += learningRate * hidden2Delta;
            biasHidden1 += learningRate * hidden1Delta;
        }

        public void SaveWeights(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(weightsInputToHidden1.ToMatrixString());
                writer.WriteLine(weightsHidden1ToHidden2.ToMatrixString());
                writer.WriteLine(weightsHidden2ToOutput.ToMatrixString());
            }
        }

        private Matrix<double> LoadWeights(StreamReader reader, int rows, int columns)
        {
            Matrix<double> weights = Matrix<double>.Build.Dense(rows, columns);
            for (int i = 0; i < rows; i++)
            {
                string[] lineData = reader.ReadLine().Split(',');
                for (int j = 0; j < columns; j++)
                {
                    weights[i, j] = double.Parse(lineData[j]);
                }
            }
            return weights;
        }


        public void LoadWeights(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                weightsInputToHidden1 = LoadWeights(reader, inputSize, hiddenSize1);
                weightsHidden1ToHidden2 = LoadWeights(reader, hiddenSize1, hiddenSize2);
                weightsHidden2ToOutput = LoadWeights(reader, hiddenSize2, outputSize);
            }
        }

    }

    public class NeuralNetworkTrainer
    {
        private NeuralNetwork network;
        private double learningRate;
        private int epochs;

        public NeuralNetworkTrainer(NeuralNetwork network, double learningRate, int epochs)
        {
            this.network = network;
            this.learningRate = learningRate;
            this.epochs = epochs;
        }

        public void Train(List<Vector<double>> trainingData, List<Vector<double>> trainingLabels,
                          List<Vector<double>> validationData, List<Vector<double>> validationLabels)
        {
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                double totalLoss = 0;
                int correctPredictions = 0;

                // Обучение на тренировочной выборке
                for (int i = 0; i < trainingData.Count; i++)
                {
                    var output = network.FeedForward(trainingData[i]);
                    network.Backpropagate(trainingData[i], trainingLabels[i], learningRate);

                    // Вычисление ошибки (кросс-энтропия)
                    totalLoss += CrossEntropyLoss(output, trainingLabels[i]);
                    if (GetPrediction(output) == GetPrediction(trainingLabels[i]))
                    {
                        correctPredictions++;
                    }
                }

                double accuracy = (double)correctPredictions / trainingData.Count;
                Console.WriteLine($"Эпоха {epoch + 1}/{epochs} - Loss: {totalLoss}, Accuracy: {accuracy}");
            }
        }

        private double CrossEntropyLoss(Vector<double> output, Vector<double> target)
        {
            double loss = 0;
            for (int i = 0; i < output.Count; i++)
            {
                loss -= target[i] * Math.Log(output[i]);
            }
            return loss;
        }

        private int GetPrediction(Vector<double> output)
        {
            return output.MaximumIndex();
        }
    }

}
