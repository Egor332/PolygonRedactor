using PolygonRedactor.Classes;
using PolygonRedactor.Enums;
using System.ComponentModel;

namespace PolygonRedactor
{
    public partial class Form1 : Form
    {
        protected bool shouldDrawLine = false;
        protected List<(Point, Point)> polygonEdges = new List<(Point, Point)>();
        protected ControlButtonStates controlButtonState = ControlButtonStates.Draw;

        public Form1()
        {
            InitializeComponent();
            ControlButton.Text = (controlButtonState).ToString();
        }

        private void ControlButton_Click(object sender, EventArgs e)
        {
            switch (controlButtonState)
            {
                case ControlButtonStates.Draw:
                    break;
                case ControlButtonStates.Stop:
                    break;
                case ControlButtonStates.Clean:
                    break;
            }
            shouldDrawLine = true;
            this.Invalidate();

            controlButtonState = (ControlButtonStates)(((int)controlButtonState + 1) % 3);
            ControlButton.Text = (controlButtonState).ToString();

            
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (shouldDrawLine)
            {
                Bresenham bresenham = new Bresenham(e.Graphics);
                bresenham.Draw(new Point(200, 600), new Point(100, 100));
            }

        }

        
            
        
    }
}
