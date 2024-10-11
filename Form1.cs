using PolygonRedactor.Classes;
using PolygonRedactor.Enums;
using System.ComponentModel;

namespace PolygonRedactor
{
    public partial class Form1 : Form
    {
        private bool _isDrawing = false;
        private bool _lineFinished = false;
        private Point _startPoint;
        private Point _currentPoint;

        protected List<(Point, Point)> polygonEdges = new List<(Point, Point)>();
        protected ControlButtonStates controlButtonState = ControlButtonStates.Draw;

        private Bresenham? _bresenham;

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
                    _bresenham = new Bresenham();
                    StartDrawing();
                    this.Invalidate();
                    break;
                case ControlButtonStates.Stop:
                    StopDrawing();
                    break;
                case ControlButtonStates.Clean:
                    break;
            }
            

            controlButtonState = (ControlButtonStates)(((int)controlButtonState + 1) % 3);
            ControlButton.Text = (controlButtonState).ToString();

            
        }

        private void StartDrawing()
        {
            this.MouseDown += Form_MouseDown;
            this.MouseMove += Form_MouseMove;
        }

        private void StopDrawing()
        {
            this.MouseDown -= Form_MouseDown;
            this.MouseMove -= Form_MouseMove;
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!_isDrawing)
                {
                    _startPoint = e.Location;
                    _isDrawing = true;
                    _lineFinished = false;
                } 
                else
                {
                    _lineFinished = true;
                    _isDrawing = false;
                }
            }
            this.Invalidate();
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing && !_lineFinished)
            {
                _currentPoint = e.Location;
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_isDrawing)
            {
                _bresenham.Draw(_startPoint, _currentPoint, e.Graphics);
            }

        }

        
            
        
    }
}
