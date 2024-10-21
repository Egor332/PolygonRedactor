using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonRedactor.Classes
{
    public class SetLengthForm : Form
    {
        private TextBox textBox;
        private Button okButton;
        private Button cancelButton;
        public string InputText { get; private set; }

        public SetLengthForm(string currentLength)
        {
            // Label
            Label promptLabel = new Label() { Left = 20, Top = 10, Text = "Set edge length:" };
            promptLabel.AutoSize = true;

            // TextBox
            textBox = new TextBox() { Left = 20, Top = 30, Width = 150, Text=currentLength };

            // OK Button
            okButton = new Button() { Text = "OK", Left = 100, Width = 70, Top = 60, Height=25, DialogResult = DialogResult.OK };
            okButton.Click += new EventHandler(OkButton_Click);

            // Cancel Button
            cancelButton = new Button() { Text = "Cancel", Left = 20, Width = 70, Top = 60, Height=25, DialogResult = DialogResult.Cancel };
            cancelButton.Click += new EventHandler(CancelButton_Click);

            // Form setup

            this.Controls.Add(promptLabel);
            this.Controls.Add(textBox);
            this.Controls.Add(okButton);
            this.Controls.Add(cancelButton);
            this.AcceptButton = okButton;  // Enter key submits the form
            this.CancelButton = cancelButton;  // Escape key cancels the form
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ClientSize = new System.Drawing.Size(200, 100);
            this.Text = "Set length";
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            InputText = textBox.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

 
        public string Show()
        {
            
            return ShowDialog() == DialogResult.OK ? InputText : null;
            
        }
    }

}
