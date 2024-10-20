using MathNet.Numerics.LinearAlgebra;
using System;

namespace lab2
{
    public partial class Form1 : Form
    {
        private NeuralNetwork network;
        private NeuralNetworkTrainer trainer;
        private List<Vector<double>> trainingData;
        private List<Vector<double>> trainingLabels;
        private double learningRate;
        private int epochs;
        public Form1()
        {
            InitializeComponent();

            // ������������� ��������� ���� (��������, 1024 �����, ��� ������� ���� �� 64 �������, 10 �������)
            network = new NeuralNetwork(1024, 64, 64, 10);
            learningRate = 0.01; // �������� (����� �������� �� TextBox ����� � �������)
            epochs = 100; // �������� ����� TextBox �� �����

            // ������������� �������
            trainer = new NeuralNetworkTrainer(network, learningRate, epochs);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            pictureBox1.Invalidate();
        }

        private void buttonPunish_Click(object sender, EventArgs e)
        {
            LoadTrainingData();
            trainer.Train(trainingData, trainingLabels, null, null); // ��������� �������� � ������ �������
            MessageBox.Show("���������� ���������!");
        }

        private void buttonLearn_Click(object sender, EventArgs e)
        {
            LoadTrainingData();
            trainer.Train(trainingData, trainingLabels, null, null);
            MessageBox.Show("�������� ���������!");
        }

        private void LoadTrainingData()
        {
            // ������ �������� ������������� ������ �� ����������� (����� �����������)
            trainingData = new List<Vector<double>>();
            trainingLabels = new List<Vector<double>>();

            // �������� �������� ����������� 32x32 � �� �����
        }


        // ���������� ������������� �����������
        private Vector<double> GetImageData()
        {
            // �������� ����������� �� PictureBox � ������������ �� 32x32 ��������
            Bitmap bitmap = new Bitmap(pictureBox1.Image, new Size(32, 32));

            // ����������� ����������� � ������ �������� ��������
            Vector<double> inputData = Vector<double>.Build.Dense(1024);
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    // ������������ ������� � �����-����� (��������, �������)
                    double grayValue = pixelColor.GetBrightness();
                    inputData[y * 32 + x] = grayValue; // ���������� �������� ������� � ������
                }
            }
            return inputData;
        }


        // ����������� ������ � �����
        private void PlotMetrics(List<double> accuracyData, List<double> precisionData, List<double> recallData, List<double> lossData)
        {
            Bitmap bitmap = new Bitmap(pictureBox2.Width, pictureBox2.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);

                // ������ ����������� accuracy ��� �����
                DrawLine(g, accuracyData, Color.Green, bitmap.Width, bitmap.Height);
                DrawLine(g, precisionData, Color.Blue, bitmap.Width, bitmap.Height);
                DrawLine(g, recallData, Color.Red, bitmap.Width, bitmap.Height);
                DrawLine(g, lossData, Color.Black, bitmap.Width, bitmap.Height);
            }

            pictureBox2.Image = bitmap;
        }

        private void DrawLine(Graphics g, List<double> data, Color color, int width, int height)
        {
            Pen pen = new Pen(color, 2);
            for (int i = 0; i < data.Count - 1; i++)
            {
                int x1 = (i * width) / data.Count;
                int y1 = height - (int)(data[i] * height);
                int x2 = ((i + 1) * width) / data.Count;
                int y2 = height - (int)(data[i + 1] * height);
                g.DrawLine(pen, x1, y1, x2, y2);
            }
        }

        // ������ �����
        private int brushSize = 5;

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            brushSize = trackBar1.Value;
        }

        // ��� ��������� ������� ���������
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                using (Graphics g = pictureBox1.CreateGraphics())
                {
                    g.FillEllipse(Brushes.Black, e.X, e.Y, brushSize, brushSize);
                }
            }
        }

        // ���������� � �������� �����
        private void buttonSave_Click(object sender, EventArgs e)
        {
            network.SaveWeights("weights.txt");
            MessageBox.Show("���� ���������!");
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            network.LoadWeights("weights.txt");
            MessageBox.Show("���� ���������!");
        }

    }
}