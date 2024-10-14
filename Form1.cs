using PolygonRedactor.Classes;
using PolygonRedactor.Classes.Polygon;
using PolygonRedactor.Enums;
using System.ComponentModel;

namespace PolygonRedactor
{

    public partial class Form1 : Form
    {
        private bool _isDrawing = false;
        private Point _startPoint;
        private Point _currentPoint;

        // private bool _vertexSelected = false;
        private Vertex? _vertexSelected = null;
        private Edge? _edgeSelected = null;

        private Polygon _polygon = new Polygon();

        protected ControlButtonStates controlButtonState = ControlButtonStates.Draw;

        private Bresenham? _bresenham = new Bresenham();

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
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
                    StartModifying();
                    break;
                case ControlButtonStates.Clean:
                    StopModifying();
                    CleanPolygon();
                    break;
            }


            controlButtonState = (ControlButtonStates)(((int)controlButtonState + 1) % 3);
            ControlButton.Text = (controlButtonState).ToString();


        }

        private void StartDrawing()
        {
            this.MouseDown += Draw_MouseDown;
            this.MouseMove += Draw_MouseMove;
        }

        private void StopDrawing()
        {
            _polygon.AddFinalEdge();
            this.MouseDown -= Draw_MouseDown;
            this.MouseMove -= Draw_MouseMove;
            _isDrawing = false;
            this.Invalidate();
        }

        private void CleanPolygon()
        {
            _polygon = new Polygon();
            this.Invalidate();
        }

        private void Draw_MouseDown(object sender, MouseEventArgs e)
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

        private void Draw_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing)
            {
                _currentPoint = e.Location;
                this.Invalidate();
            }
        }

        private void StartModifying()
        {
            this.MouseDown += Modify_MouseDown;
            this.MouseMove += Modify_MouseMove;
        }

        private void StopModifying()
        {
            this.MouseDown -= Modify_MouseDown;
            this.MouseMove -= Modify_MouseMove;
        }

        private void Modify_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) && (_vertexSelected == null) &&
                (_edgeSelected == null))
            {
                foreach (Vertex v in _polygon.vertices)
                {
                    if (v.CheckIsInArea(e.Location))
                    {
                        v.isSelected = true;
                        _vertexSelected = v;                        
                        break;
                    }
                }

                if (_vertexSelected == null)
                {
                    foreach (Edge edge in _polygon.edges)
                    {
                        if (edge.IsOnEdge(e.Location))
                        {
                            edge.isSelected = true;
                            _edgeSelected = edge;
                            _edgeSelected.pressPoint = e.Location;
                            break;
                        }
                    }

                }


            }            
            else if ((e.Button == MouseButtons.Left) )
            {
                if (_vertexSelected == null)
                {
                    _edgeSelected.isSelected = false;
                    _edgeSelected.pressPoint = null;
                    _edgeSelected = null;
                }
                else 
                {
                    _vertexSelected.isSelected = false;
                    _vertexSelected = null;
                }
            }
            this.Invalidate();
        }

        private void Modify_MouseMove(object sender, MouseEventArgs e)
        {
            if (_vertexSelected != null)
            {
                _vertexSelected.position = e.Location;
                this.Invalidate();
            }
            if (_edgeSelected != null)
            {
                _edgeSelected.MoveEdge(e.Location);
                _edgeSelected.pressPoint = e.Location;
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            _bresenham.g = e.Graphics;
            _polygon.DrawPolygon(_bresenham, e.Graphics);
            if (_isDrawing)
            {
                _bresenham.Draw(_startPoint, _currentPoint);
            }

        }


    }
}
