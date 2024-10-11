using PolygonRedactor.Classes;
using PolygonRedactor.Classes.Polygon;
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

        private Polygon _polygon = new Polygon();

        protected ControlButtonStates controlButtonState = ControlButtonStates.Draw;

        private Bresenham? _bresenham = new Bresenham();

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
                    StartDrawing();
                    this.Invalidate();
                    break;
                case ControlButtonStates.Stop:
                    StopDrawing();
                    break;
                case ControlButtonStates.Clean:
                    CleanPolygon();
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
            _polygon.AddFinalEdge();            
            this.MouseDown -= Form_MouseDown;
            this.MouseMove -= Form_MouseMove;
            _isDrawing = false;
            this.Invalidate();
        }

        private void CleanPolygon()
        {
            _polygon = new Polygon();
            this.Invalidate();
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!_isDrawing)
                {
                    _startPoint = e.Location;
                    _polygon.AddNewVertex(_startPoint);
                    _isDrawing = true;
                    
                } 
                else
                {
                    _startPoint = e.Location;
                    _polygon.AddNewVertex(_startPoint);
                    _polygon.AddNewEdge();
                    
                    
                }
            }
            this.Invalidate();
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing)
            {
                _currentPoint = e.Location;
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {


            base.OnPaint(e);
            _bresenham.g = e.Graphics;
            _polygon.Draw(_bresenham);
            if (_isDrawing)
            {
                _bresenham.Draw(_startPoint, _currentPoint);
            }

        }    
            
        
    }
}
