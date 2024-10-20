namespace lab2
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            textBox1 = new TextBox();
            button1 = new Button();
            buttonClear = new Button();
            buttonPunish = new Button();
            pictureBox1 = new PictureBox();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            buttonLearn = new Button();
            trackBar1 = new TrackBar();
            label1 = new Label();
            pictureBox2 = new PictureBox();
            label2 = new Label();
            checkedListBox1 = new CheckedListBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Location = new Point(56, 304);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(196, 23);
            textBox1.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(56, 342);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 1;
            button1.Text = "расп.";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // buttonClear
            // 
            buttonClear.Location = new Point(177, 342);
            buttonClear.Name = "buttonClear";
            buttonClear.Size = new Size(75, 23);
            buttonClear.TabIndex = 2;
            buttonClear.Text = "очистить";
            buttonClear.UseVisualStyleBackColor = true;
            buttonClear.Click += buttonClear_Click;
            // 
            // buttonPunish
            // 
            buttonPunish.Location = new Point(56, 371);
            buttonPunish.Name = "buttonPunish";
            buttonPunish.Size = new Size(196, 23);
            buttonPunish.TabIndex = 3;
            buttonPunish.Text = "наказать";
            buttonPunish.UseVisualStyleBackColor = true;
            buttonPunish.Click += buttonPunish_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.Location = new Point(56, 85);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(196, 189);
            pictureBox1.TabIndex = 4;
            pictureBox1.TabStop = false;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(285, 12);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(65, 23);
            textBox2.TabIndex = 5;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(375, 12);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(65, 23);
            textBox3.TabIndex = 6;
            // 
            // buttonLearn
            // 
            buttonLearn.Location = new Point(327, 55);
            buttonLearn.Name = "buttonLearn";
            buttonLearn.Size = new Size(75, 23);
            buttonLearn.TabIndex = 7;
            buttonLearn.Text = "обучить";
            buttonLearn.UseVisualStyleBackColor = true;
            buttonLearn.Click += buttonLearn_Click;
            // 
            // trackBar1
            // 
            trackBar1.Location = new Point(314, 113);
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(104, 45);
            trackBar1.TabIndex = 8;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(349, 143);
            label1.Name = "label1";
            label1.Size = new Size(38, 15);
            label1.TabIndex = 9;
            label1.Text = "Кисть";
            // 
            // pictureBox2
            // 
            pictureBox2.BackgroundImage = (Image)resources.GetObject("pictureBox2.BackgroundImage");
            pictureBox2.BorderStyle = BorderStyle.FixedSingle;
            pictureBox2.Location = new Point(500, 85);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(196, 189);
            pictureBox2.TabIndex = 10;
            pictureBox2.TabStop = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(575, 277);
            label2.Name = "label2";
            label2.Size = new Size(48, 15);
            label2.TabIndex = 11;
            label2.Text = "График";
            // 
            // checkedListBox1
            // 
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Items.AddRange(new object[] { "Accuracy", "Precision", "Recall", "значение Loss функции" });
            checkedListBox1.Location = new Point(500, 300);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(196, 94);
            checkedListBox1.TabIndex = 12;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(checkedListBox1);
            Controls.Add(label2);
            Controls.Add(pictureBox2);
            Controls.Add(label1);
            Controls.Add(trackBar1);
            Controls.Add(buttonLearn);
            Controls.Add(textBox3);
            Controls.Add(textBox2);
            Controls.Add(pictureBox1);
            Controls.Add(buttonPunish);
            Controls.Add(buttonClear);
            Controls.Add(button1);
            Controls.Add(textBox1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox1;
        private Button button1;
        private Button buttonClear;
        private Button buttonPunish;
        private PictureBox pictureBox1;
        private TextBox textBox2;
        private TextBox textBox3;
        private Button buttonLearn;
        private TrackBar trackBar1;
        private Label label1;
        private PictureBox pictureBox2;
        private Label label2;
        private CheckedListBox checkedListBox1;
    }
}