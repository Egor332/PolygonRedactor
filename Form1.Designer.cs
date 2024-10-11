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
            SuspendLayout();
            // 
            // ControlButton
            // 
            ControlButton.Location = new Point(12, 12);
            ControlButton.Name = "ControlButton";
            ControlButton.Size = new Size(67, 36);
            ControlButton.TabIndex = 0;
            ControlButton.Text = "Draw";
            ControlButton.UseVisualStyleBackColor = true;
            ControlButton.Click += ControlButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(965, 525);
            Controls.Add(ControlButton);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private Button ControlButton;
    }
}
