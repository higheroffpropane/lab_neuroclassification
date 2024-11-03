using MathNet.Numerics.LinearAlgebra;
using Microsoft.VisualBasic.ApplicationServices;
using System;

namespace lab2
{
    // что нужно сделать:
    // 1) на вход подаем одну строчку из таблицы csv
    // 2) дальше идут преобразования через 2 слоя нейронов
    // 3) на выходе после softmax получаем наибольшую вероятность в каком-то нейроне -> картинка распознана
    // для тренировочной выборки мы сравниваем выход с уже известным классом: если не верно -> меняем веса автоматически
    // для валидационной сами смотрим вручную на форме, правильно ли распознала
    // на каждой эпохе считаем значения 4 метрик, заносим в массив -> в конце по массиву строим график для каждой метрики

    using System;
    using System.Windows.Forms;

    public partial class Form1 : Form
    {
        private NeuralNetwork network;
        private Bitmap drawingBitmap;
        private bool isDrawing = false;
        private int brushSize = 5; // Начальный размер кисти

        public Form1()
        {
            InitializeComponent();
            network = new NeuralNetwork(learningRate: 0.01, sizes: new int[] { 1024, 64, 64, 10 });
            network.LoadWeights("weights.txt");
            drawingBitmap = new Bitmap(196, 196); // Инициализация Bitmap
            pictureBox1.Image = drawingBitmap; // Установка на pictureBox
        }

        private int[,] getCSVdata()
        {
            string filePath = "C:/Users/higheroffpropane/Desktop/annotations_bycolumns.csv"; // Укажите путь к вашему CSV-файлу
            int numRows = 1000; // Количество строк (изображений)
            int numColumns = 1026; // id + класс + 1024 пикселя

            // Создаем двумерный массив для хранения данных
            int[,] dataArray = new int[numRows, numColumns];

            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                int row = 0;

                // Пропускаем первую строку (заголовки)
                if ((line = sr.ReadLine()) != null)
                {
                }

                while ((line = sr.ReadLine()) != null && row < numRows)
                {
                    // Разделяем строку по ";"
                    string[] values = line.Split(';');

                    for (int col = 0; col < numColumns; col++)
                    {
                        dataArray[row, col] = int.Parse(values[col]);
                    }
                    row++;
                }
            }
            Console.WriteLine("Данные успешно загружены в массив.");
            return dataArray;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void buttonLearn_Click(object sender, EventArgs e)
        {
            int epochs = int.Parse(textBoxEpochs.Text);
            int delta = int.Parse(textBoxDelta.Text);
            network = new NeuralNetwork(learningRate: delta, sizes: new int[] { 1024, 64, 64, 10 });
            int[,] data = getCSVdata();
            int[,] trainData = new int[800, data.GetLength(1)];
            int[,] validationData = new int[200, data.GetLength(1)];

            for (int i = 0; i < 800; i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    trainData[i, j] = data[i, j];
                }
            }

            // Копируем данные во второй массив
            for (int i = 800; i < 1000; i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    validationData[i - 800, j] = data[i, j];
                }
            }

            network.Train(epochs, getCSVdata());
            network.SaveWeights("weights.txt");
            MessageBox.Show("Обучение завершено.");
        }


        private void buttonIdentify_Click(object sender, EventArgs e)
        {
            // Проверяем, есть ли изображение в pictureBox
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Пожалуйста, загрузите или нарисуйте изображение.");
                return;
            }

            // Создаем новый Bitmap размером 32x32
            Bitmap scaledImage = new Bitmap(32, 32);

            using (Graphics g = Graphics.FromImage(scaledImage))
            {
                // Масштабируем изображение из pictureBox до 32x32
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(pictureBox1.Image, 0, 0, 32, 32);
            }

            // Преобразуем изображение в массив 0 и 1
            double[] inputArray = new double[32 * 32];
            for (int y = 0; y < scaledImage.Height; y++)
            {
                for (int x = 0; x < scaledImage.Width; x++)
                {
                    // Получаем цвет пикселя
                    Color pixelColor = scaledImage.GetPixel(x, y);

                    // Если пиксель черный, то устанавливаем значение 1, иначе 0
                    inputArray[y * 32 + x] = (pixelColor.R != 255 || pixelColor.G != 255 || pixelColor.B != 255) ? 1.0 : 0.0;
                }
            }

            // Передаем массив в нейронную сеть для классификации
            int predictedClass = network.Predict(inputArray);
            textBox1.Text = predictedClass.ToString();
        }


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            isDrawing = true;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                using (Graphics g = Graphics.FromImage(drawingBitmap))
                {
                    g.FillEllipse(Brushes.Black, e.X, e.Y, 5, 5); // Рисуем точку
                }
                pictureBox1.Invalidate(); // Перерисовываем pictureBox
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isDrawing = false;
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            drawingBitmap = new Bitmap(196, 196); // Инициализация Bitmap
            using (Graphics g = Graphics.FromImage(drawingBitmap))
            {
                g.Clear(Color.White); // Очистка белым цветом
            }
            pictureBox1.Image = drawingBitmap; // Установка на pictureBox
        }

        private void buttonPunish_Click(object sender, EventArgs e)
        {
            // Проверяем, правильно ли распознана картинка
            int predictedClass = int.Parse(textBox1.Text);

            // Открываем диалог для ввода правильного класса
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "Введите правильный класс для картинки:",
                "Правильный класс",
                "0", // Значение по умолчанию
                -1, -1); // Положение окна

            if (int.TryParse(input, out int correctClass))
            {
                double[] inputArray = GetInputArrayFromPictureBox(pictureBox1);
                PunishNetwork(predictedClass, correctClass, inputArray);
            }
            else
            {
                MessageBox.Show("Некорректный ввод. Пожалуйста, введите число.");
            }
        }

        private double[] GetInputArrayFromPictureBox(PictureBox pictureBox)
        {
            // Здесь вы можете использовать свою логику для преобразования изображения в массив
            // Пример преобразования изображение из PictureBox в массив 0 и 1
            Bitmap image = new Bitmap(pictureBox.Image);
            double[] inputArray = new double[1024];

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    inputArray[y * 32 + x] = (pixelColor.R != 255 || pixelColor.G != 255 || pixelColor.B != 255) ? 1.0 : 0.0;
                }
            }

            return inputArray;
        }

        private void PunishNetwork(int predictedClass, int correctClass, double[] inputArray)
        {
            // Определяем коэффициент для изменения выходов
            double punishmentFactor = 0.1; // Настройте этот коэффициент по мере необходимости

            // Создаем массив выходов для целевой метки
            double[] output = new double[10]; // Предположим, что у вас 10 классов
            output[correctClass - 1] = 1.0; // Устанавливаем правильный выход

            // Понижаем значение для неправильно предсказанного класса
            output[predictedClass] -= punishmentFactor; // Уменьшаем неправильно предсказанный класс
            for (int i = 0; i < output.Length; i++)
            {
                // Убеждаемся, что выходы не отрицательные
                if (output[i] < 0) output[i] = 0;
            }

            // Теперь формируем данные в формате, который принимает Train
            // Создаем массив для input и output в нужном формате
            int[,] data = new int[1, 1026]; // 1024 для входов и 2 для класса
            for (int k = 0; k < 1024; k++)
            {
                data[0, k + 2] = (int)inputArray[k]; // Входные данные (приводим к int)
            }
            data[0, 1] = correctClass; // Правильный класс

            // Запускаем обучение нейросети с обновленными выходами
            network.Train(1, data); // Обучаем на одном примере
        }




        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void buttonChoose_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Указываем фильтр для выбора только изображений
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*";
                openFileDialog.Title = "Выберите изображение";

                // Проверяем, выбрал ли пользователь файл
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Загружаем выбранное изображение в pictureBox1
                        pictureBox1.Image = Image.FromFile(openFileDialog.FileName);
                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage; // Изменяем режим отображения
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка загрузки изображения: " + ex.Message);
                    }
                }
            }
        }
    }



}