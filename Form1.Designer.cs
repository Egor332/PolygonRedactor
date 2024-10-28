namespace PolygonRedactor
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
            ControlButton = new Button();
            bresenhamRadioButton = new RadioButton();
            drawLineRadioButton = new RadioButton();
            InstructionBox = new TextBox();
            ShowAlgButton = new Button();
            SuspendLayout();
            // 
            // ControlButton
            // 
            ControlButton.Location = new Point(14, 16);
            ControlButton.Margin = new Padding(3, 4, 3, 4);
            ControlButton.Name = "ControlButton";
            ControlButton.Size = new Size(77, 48);
            ControlButton.TabIndex = 0;
            ControlButton.Text = "Draw";
            ControlButton.UseVisualStyleBackColor = true;
            ControlButton.Click += ControlButton_Click;
            // 
            // bresenhamRadioButton
            // 
            bresenhamRadioButton.AutoSize = true;
            bresenhamRadioButton.Location = new Point(12, 71);
            bresenhamRadioButton.Name = "bresenhamRadioButton";
            bresenhamRadioButton.Size = new Size(103, 24);
            bresenhamRadioButton.TabIndex = 1;
            bresenhamRadioButton.TabStop = true;
            bresenhamRadioButton.Text = "Bresenham";
            bresenhamRadioButton.UseVisualStyleBackColor = true;
            bresenhamRadioButton.CheckedChanged += bresenhamRadioButton_CheckedChanged;
            // 
            // drawLineRadioButton
            // 
            drawLineRadioButton.AutoSize = true;
            drawLineRadioButton.Location = new Point(12, 101);
            drawLineRadioButton.Name = "drawLineRadioButton";
            drawLineRadioButton.Size = new Size(92, 24);
            drawLineRadioButton.TabIndex = 2;
            drawLineRadioButton.TabStop = true;
            drawLineRadioButton.Text = "DrawLine";
            drawLineRadioButton.UseVisualStyleBackColor = true;
            drawLineRadioButton.CheckedChanged += drawLineRadioButton_CheckedChanged;
            // 
            // InstructionBox
            // 
            InstructionBox.Location = new Point(1, 131);
            InstructionBox.Name = "InstructionBox";
            InstructionBox.Size = new Size(247, 27);
            InstructionBox.TabIndex = 3;
            // 
            // ShowAlgButton
            // 
            ShowAlgButton.Location = new Point(10, 659);
            ShowAlgButton.Name = "ShowAlgButton";
            ShowAlgButton.Size = new Size(274, 29);
            ShowAlgButton.TabIndex = 4;
            ShowAlgButton.Text = "Show algorithm description";
            ShowAlgButton.UseVisualStyleBackColor = true;
            ShowAlgButton.Click += ShowAlgButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1103, 700);
            Controls.Add(ShowAlgButton);
            Controls.Add(InstructionBox);
            Controls.Add(drawLineRadioButton);
            Controls.Add(bresenhamRadioButton);
            Controls.Add(ControlButton);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button ControlButton;
        private RadioButton bresenhamRadioButton;
        private RadioButton drawLineRadioButton;
        private TextBox InstructionBox;
        private Button ShowAlgButton;
    }
}
