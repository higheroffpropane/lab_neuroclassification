using MathNet.Numerics.LinearAlgebra;
using Microsoft.VisualBasic.ApplicationServices;
using System;

namespace lab2
{

    using System;
    using System.Windows.Forms;

    public partial class Form1 : Form
    {
        private NeuralNetwork network;
        private Bitmap drawingBitmap;
        private bool isDrawing = false;
        char[] classToLetter = { '�', '�', '�', '�', '�', '�', '�', '�', '�', '�' };

        public Form1()
        {
            InitializeComponent();
            network = new NeuralNetwork(learningRate: 0.01, sizes: new int[] { 1024, 256, 64, 10 });
            network.LoadWeights("weights.txt");
            drawingBitmap = new Bitmap(32, 32); // ������������� Bitmap
            pictureBox1.Image = drawingBitmap; // ��������� �� pictureBox
        }

        private int[,] getCSVdata()
        {
            string filePath = "C:/Users/higheroffpropane/Desktop/new_annotations.csv"; // ������� ���� � ������ CSV-�����
            int numRows = 1000; // ���������� ����� (�����������)
            int numColumns = 1026; // id + ����� + 1024 �������

            // ������� ��������� ������ ��� �������� ������
            int[,] dataArray = new int[numRows, numColumns];

            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                int row = 0;

                // ���������� ������ ������ (���������)
                if ((line = sr.ReadLine()) != null)
                {
                }

                while ((line = sr.ReadLine()) != null && row < numRows)
                {
                    // ��������� ������ �� ";"
                    string[] values = line.Split(';');

                    for (int col = 0; col < numColumns; col++)
                    {
                        dataArray[row, col] = int.Parse(values[col]);
                    }
                    row++;
                }
            }
            Console.WriteLine("������ ������� ��������� � ������.");
            return dataArray;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void buttonLearn_Click(object sender, EventArgs e)
        {
            int epochs = int.Parse(textBoxEpochs.Text);
            double delta = double.Parse(textBoxDelta.Text);
            network = new NeuralNetwork(learningRate: delta, sizes: new int[] { 1024, 256, 64, 10 });
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

            // �������� ������ �� ������ ������
            for (int i = 800; i < 1000; i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    validationData[i - 800, j] = data[i, j];
                }
            }

            network.Train(epochs, getCSVdata());
            network.SaveWeights("weights.txt");
            MessageBox.Show("�������� ���������.");
        }


        private void buttonIdentify_Click(object sender, EventArgs e)
        {
            // ���������, ���� �� ����������� � pictureBox
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("����������, ��������� ��� ��������� �����������.");
                return;
            }

            // ������� ����� Bitmap �������� 32x32
            Bitmap scaledImage = new Bitmap(32, 32);

            using (Graphics g = Graphics.FromImage(scaledImage))
            {
                // ������������ ����������� �� pictureBox �� 32x32
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(pictureBox1.Image, 0, 0, 32, 32);
            }

            // ����������� ����������� � ������ 0 � 1
            double[] inputArray = new double[32 * 32];
            for (int y = 0; y < scaledImage.Height; y++)
            {
                for (int x = 0; x < scaledImage.Width; x++)
                {
                    // �������� ���� �������
                    Color pixelColor = scaledImage.GetPixel(x, y);

                    // ���� ������� ������, �� ������������� �������� 1, ����� 0
                    inputArray[y * 32 + x] = (pixelColor.R != 255 || pixelColor.G != 255 || pixelColor.B != 255) ? 1.0 : 0.0;
                }
            }

            // �������� ������ � ��������� ���� ��� �������������
            int predictedClass = network.Predict(inputArray);
            textBox1.Text = classToLetter[predictedClass - 1].ToString();
        }


        private void DrawOnBitmap(MouseEventArgs e)
        {
            // �������� �������������� �����������
            float scaleX = (float)pictureBox1.Width / drawingBitmap.Width;
            float scaleY = (float)pictureBox1.Height / drawingBitmap.Height;

            // ��������� ����� ���������� ��� ��������� �� Bitmap
            int drawX = (int)(e.X / scaleX);
            int drawY = (int)(e.Y / scaleY);

            // ������ �� �������� Bitmap
            using (Graphics g = Graphics.FromImage(drawingBitmap))
            {
                g.FillRectangle(Brushes.Black, drawX, drawY, 1, 1); // ������ �������
            }

            // �������������� PictureBox, ����� �������� �����������
            pictureBox1.Invalidate();
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            // ������ ���������� �����������
            e.Graphics.DrawImage(drawingBitmap, 0, 0, pictureBox1.Width, pictureBox1.Height);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrawing = true;
                DrawOnBitmap(e);
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                DrawOnBitmap(e);
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isDrawing = false;
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            drawingBitmap = new Bitmap(32, 32); // ������������� Bitmap
            using (Graphics g = Graphics.FromImage(drawingBitmap))
            {
                g.Clear(Color.White); // ������� ����� ������
            }
            pictureBox1.Image = drawingBitmap; // ��������� �� pictureBox
        }

        private void buttonPunish_Click(object sender, EventArgs e)
        {
            // ���������, ��������� �� ���������� ��������
            char predictedClassToLetter = char.Parse(textBox1.Text);
            int predictedClass = Array.IndexOf(classToLetter, predictedClassToLetter) + 1;

            // ��������� ������ ��� ����� ����������� ������
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "������� ���������� ����� ��� ��������:",
                "���������� �����",
                "0", // �������� �� ���������
                -1, -1); // ��������� ����

            if (int.TryParse(input, out int correctClass))
            {
                double[] inputArray = GetInputArrayFromPictureBox(pictureBox1);
                PunishNetwork(predictedClass, correctClass, inputArray);
            }
            else
            {
                MessageBox.Show("������������ ����. ����������, ������� �����.");
            }
        }

        private double[] GetInputArrayFromPictureBox(PictureBox pictureBox)
        {
            // ����� �� ������ ������������ ���� ������ ��� �������������� ����������� � ������
            // ������ �������������� ����������� �� PictureBox � ������ 0 � 1
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
            // ������� ������ ��� input � output � ������ �������, �� ���������� ������� ������ � int
            int[,] data = new int[1, 1026]; // 1024 ��� ������ � 2 ��� ������
            for (int k = 0; k < 1024; k++)
            {
                data[0, k + 2] = (inputArray[k] == 1.0) ? 1 : 0; // ������� ������
            }
            data[0, 1] = correctClass; // ���������� �����

            // ��������� �������� �� ����� �������, ����� "��������" ����
            network.Train(1, data);
        }


        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void buttonChoose_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // ��������� ������ ��� ������ ������ �����������
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*";
                openFileDialog.Title = "�������� �����������";

                // ���������, ������ �� ������������ ����
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // ��������� ��������� ����������� � pictureBox1
                        pictureBox1.Image = Image.FromFile(openFileDialog.FileName);
                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage; // �������� ����� �����������
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("������ �������� �����������: " + ex.Message);
                    }
                }
            }
        }

    }



}