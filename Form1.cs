using PolygonRedactor.Classes;
using PolygonRedactor.Classes.Polygon;
using PolygonRedactor.Enums;
using System.ComponentModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using PolygonRedactor.Classes.ForSerialization;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace PolygonRedactor
{

    public partial class Form1 : Form
    {
        private const string _algDesc = "Opis algorytmu:\r\nWielokąt powstaje z listy wierzchołków oraz listy krawędzi. " +
            "Każda krawędź posiada dwa wierzchołki i status swojego ograniczenia. Każdy wierzchołek posiada pozycje, " +
            "związane z nim krawędzi, oraz ciągłość potencjalnej krzywej Beziera w tym punkcie.\r\nPozycje wierzchołków" +
            " opisane liczbami całkowitymi.\r\nPrzy przenoszeniu wierzchołka on wysyła sygnały o przenoszeniu do sąsiednich" +
            " wierzchołków, aby zachowane zostały ograniczenia krawędzi.\r\nPrzypuścimy, że wierzchołek V został " +
            "przeniesiony na (dx, dy), wtedy wierzchołek U związany z V krawędzią UV musi przynieść się na:\r\nUV " +
            "pozioma: ten sam dy co V\r\nUV pionowa: ten sam dx co V\r\nUV ma stałą długość: w punkt przecięcia odcinka " +
            "VU oraz okręgu z centrem w V i promieniem równym długości krawędzi UV.\r\nKrzywa Beziera jest tworzona algorytmem" +
            " przyrostowym, zachowanie ciągłości zrobione w następny sposób:\r\nDwie sąsiadujące krzywe Beziera:\r\nG1: punkty" +
            " kontrolne obracają się jakby były na jednej prostej.\r\nC1: punkty kontrolne obracają się jakby były na jednej" +
            " prostej i zachowują wspólną długość.\r\nKrzywa Beziera sąsiaduje ze zwykłą krawędzią:\r\nG1: wierzchołek wielokąta" +
            " i punkt kontrolny obracają się jakby były na jednej prostej, niewybrany punkt zachowuję swoją odległość od punkt" +
            "u średniego.\r\nC1: wierzchołek wielokąta i punkt kontrolny obracają się jakby były na jednej prostej, zachowana" +
            " zostaję długość odcinków.\r\nUwagi: przy przemieszczeniu punktu kontrolnego krzywej Beziera sąsiadującego z krzywej" +
            " poziomej/pionowej, zablokowane zostaje przenoszenie punktu kontrolnego wzdłuż osi X/Y odpowiednio.";
        private const string _startInstruction = "Aby narysować nowy wielokąt, trzeba kliknąć przycisk \"Draw\".";
        private const string _drawingInstruction = "Aby dodać nowy punkt, trzeba kliknąć lewy przycisk myszy.\r\n" +
            "Aby skończyć rysowanie wielokąta, nie trzeba zamykać wielokąta, trzeba kliknąć \"Stop\"." +
            "Między pierwszym a ostatnim punktem automatycznie pojawi się krawędź.\r\n" +
            "Uwagi:\r\n" +
            "Aby skończyć rysowanie, muszą istnieć co najmniej 3 wierzchołki.";
        private const string _modifyInstruction = "Aby usunąć wielokąt, trzeba kliknąć \"Clean\".\r\n" +
            "Aby przenieść cały wielokąt, trzeba kliknąć lewym klawiszem myszy" +
            " wewnątrz wielokąta, wielokąt będzie podążał za kursorem. Drugie kliknięcie lewego klawiszu skończy przenoszenie.\r\n" +
            "Aby przenieść wierzchołek/krawędź, trzeba kliknąć obiekt lewym przyciskiem myszy. Obiekt będzie podążał za kursorem. " +
            "Drugie kliknięcie lewego klawiszu skończy przenoszenie.\r\n" +
            "Kliknięcie prawym klawiszem myszy na krawędź powoduje pojawienie menu kontekstowego, w którym można wybrać ograniczenie.\r\n" +
            "Legenda:\r\n" +
            "Zaznaczony obiekt jest pokolorowany na czerwono.\r\n" +
            "Krawędź pozioma zaznaczona znaczkiem \"-\".\r\n" +
            "Krawędź pionowa zaznaczona znaczkiem \"|\".\r\n" +
            "Krawędź stałej długości zaznaczona zielonym kółkiem w środku.\r\n" +
            "Uwagi:\r\n" +
            "Długość krawędzi wskazana w pikselach.\r\n" +
            "Krawędzi z ograniczniami nie można przenosić";


        private bool _isDrawing = false;
        private Point _startPoint;
        private Point _currentPoint;
        private ContextMenuStrip _contextMenu = new ContextMenuStrip();
        private ContextMenuStrip _contextMenuVertex = new ContextMenuStrip();

        // private bool _vertexSelected = false;
        private Vertex? _vertexSelected = null;
        private Edge? _edgeSelected = null;
        private BezierControlPoint? _bezierSelected = null;
        private Vertex? _asociatedVertex = null;

        private Polygon _polygon = new Polygon();
        private bool _isPolygonSelected = false;

        protected ControlButtonStates controlButtonState = ControlButtonStates.Draw;
        private bool isAlg = false;

        private Bresenham? _bresenham = new Bresenham();

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            _contextMenu.Items.Add("Add new point", null, Option1_AddPoint);
            _contextMenu.Items.Add("Set vertical", null, Option2_SetVertical);
            _contextMenu.Items.Add("Set horizontal", null, Option3_SetHorizontal);
            _contextMenu.Items.Add("Set fixed", null, Option4_SetFixed);
            _contextMenu.Items.Add("Bezier", null, Option5_Bezier);
            _contextMenu.Items.Add("Remove constraint", null, Option6_Unset);
            _contextMenuVertex.Items.Add("Delete vertex", null, Option_DeleteVertex);
            _contextMenuVertex.Items.Add("Set G0", null, Option_SetG0);
            _contextMenuVertex.Items.Add("Set G1", null, Option_SetG1);
            _contextMenuVertex.Items.Add("Set C1", null, Option_SetC1);
            ControlButton.Text = (controlButtonState).ToString();
            InstructionBox.Multiline = true;
            InstructionBox.Height = this.Height - 215;
            InstructionBox.ScrollBars = ScrollBars.Vertical;
            InstructionBox.Text = _startInstruction;
            bresenhamRadioButton.Checked = true;
            ControlButton_Click(this, EventArgs.Empty);
            _polygon.AddNewVertex(new Point(150 + 200, 150 + 200));
            _polygon.AddNewVertex(new Point(230 + 200, 150 + 200));
            _polygon.AddNewEdge();
            _polygon.AddNewVertex(new Point(230 + 200, 300 + 200));
            _polygon.AddNewEdge();
            _polygon.AddNewVertex(new Point(120 + 200, 300 + 200));
            _polygon.AddNewEdge();
            ControlButton_Click(this, EventArgs.Empty);
            _edgeSelected = _polygon.edges[0];
            Option5_Bezier(this, EventArgs.Empty);
            _edgeSelected = _polygon.edges[1];
            Option2_SetVertical(this, EventArgs.Empty);
            _edgeSelected = _polygon.edges[2];
            Option3_SetHorizontal(this, EventArgs.Empty);
            InstructionBox.Multiline = true;
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
                    if (_polygon.vertices.Count < 3)
                    {
                        MessageBox.Show("Invalidate operation: polygon must have at least 3 vertices", "Warning",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        controlButtonState = (ControlButtonStates)(((int)controlButtonState + 1) % 3);
                        ControlButton.Text = (controlButtonState).ToString();
                        StopModifying();
                        CleanPolygon();
                    }
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
            if (isAlg == false) InstructionBox.Text = _drawingInstruction;
            this.MouseDown += Draw_MouseDown;
            this.MouseMove += Draw_MouseMove;
        }

        private void StopDrawing()
        {
            //foreach (Edge e in _polygon.edges)
            //{
            //    this.Controls.Add(e.stateLabel);
            //}
            _polygon.AddFinalEdge();
            this.MouseDown -= Draw_MouseDown;
            this.MouseMove -= Draw_MouseMove;
            _isDrawing = false;
            this.Invalidate();
        }

        private void CleanPolygon()
        {
            //foreach (Edge e in _polygon.edges) 
            //{
            //    this.Controls.Remove(e.stateLabel);
            //}
            _polygon = new Polygon();
            _vertexSelected = null;
            _edgeSelected = null;
            _isPolygonSelected = false;
            _bezierSelected = null;
            _asociatedVertex = null;
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
            if (isAlg == false) InstructionBox.Text = _modifyInstruction;
            this.MouseDown += Modify_MouseDown;
            this.MouseMove += Modify_MouseMove;
        }

        private void StopModifying()
        {
            if (isAlg == false) InstructionBox.Text = _startInstruction;
            this.MouseDown -= Modify_MouseDown;
            this.MouseMove -= Modify_MouseMove;
        }

        private void Modify_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) && (_vertexSelected == null) &&
                (_edgeSelected == null) && (!_isPolygonSelected) && (_bezierSelected == null))
            {
                foreach (Vertex v in _polygon.vertices)
                {
                    if (v.CheckIsInArea(e.Location))
                    {
                        v.isSelected = true;
                        _vertexSelected = v;
                        _polygon.ResetAllLengthes();
                        break;
                    }
                }

                if (_vertexSelected == null)
                {
                    foreach (Edge edge in _polygon.edges)
                    {
                        if (edge.IsOnEdge(e.Location))
                        {
                            if (edge.state != EdgeStates.None) continue;
                            edge.isSelected = true;
                            _edgeSelected = edge;
                            _edgeSelected.pressPoint = e.Location;
                            _polygon.ResetAllLengthes();
                            break;
                        }
                    }

                }

                if ((_vertexSelected == null) && (_edgeSelected == null))
                {
                    foreach (Edge edge in _polygon.edges)
                    {
                        if (edge.state != EdgeStates.Bezier) continue;

                        if (edge.bezierControlPoints[0].CheckIsInArea(e.Location))
                        {
                            _bezierSelected = edge.bezierControlPoints[0];
                            _bezierSelected.isSelected = true;
                            _asociatedVertex = edge.start;
                            _polygon.ResetAllLengthes();
                            break;
                        }
                        if (edge.bezierControlPoints[1].CheckIsInArea(e.Location))
                        {
                            _bezierSelected = edge.bezierControlPoints[1];
                            _bezierSelected.isSelected = true;
                            _asociatedVertex = edge.end;
                            _polygon.ResetAllLengthes();
                            break;
                        }
                    }
                }

                if ((_vertexSelected == null) && (_edgeSelected == null) && (_bezierSelected == null))
                {
                    if (_polygon.CheckIsInside(e.Location))
                    {
                        _polygon.isSelected = true;
                        _polygon.pressPoint = e.Location;
                        _isPolygonSelected = true;
                    }
                }


            }
            else if ((e.Button == MouseButtons.Left))
            {
                if ((_vertexSelected == null) && (!_isPolygonSelected) && (_bezierSelected == null))
                {
                    _edgeSelected.isSelected = false;
                    _edgeSelected.pressPoint = null;
                    _edgeSelected = null;
                }
                else if ((_edgeSelected == null) && (!_isPolygonSelected) && (_bezierSelected == null))
                {
                    _vertexSelected.isSelected = false;
                    _vertexSelected = null;
                }
                else if (_bezierSelected != null)
                {
                    _bezierSelected.isSelected = false;
                    _asociatedVertex = null;
                    _bezierSelected = null;
                }
                else
                {
                    _polygon.isSelected = false;
                    _polygon.pressPoint = null;
                    _isPolygonSelected = false;
                }
            }
            else if ((e.Button == MouseButtons.Right) && (_vertexSelected == null) &&
                (_edgeSelected == null))
            {
                foreach (Vertex v in _polygon.vertices)
                {
                    if (v.CheckIsInArea(e.Location))
                    {
                        v.isSelected = true;
                        _vertexSelected = v;
                        this.Invalidate();
                        _contextMenuVertex.Show(this, e.Location);
                        break;
                    }
                }

                foreach (Edge edge in _polygon.edges)
                {
                    if (edge.IsOnEdge(e.Location))
                    {
                        edge.isSelected = true;
                        _edgeSelected = edge;
                        this.Invalidate();
                        _contextMenu.Show(this, e.Location);
                        break;
                    }
                }
            }
            else if ((e.Button == MouseButtons.Right) && (_edgeSelected != null))
            {
                _edgeSelected.isSelected = false;
                _edgeSelected = null;
            }
            this.Invalidate();
        }

        private void Modify_MouseMove(object sender, MouseEventArgs e)
        {
            if (_vertexSelected != null)
            {
                _vertexSelected.MovePoint(e.Location, _vertexSelected.position);
                _polygon.ResetBezier();
                this.Invalidate();
            }
            if (_edgeSelected != null)
            {
                _edgeSelected.MoveEdge(e.Location, this.Width, this.Height);
                _edgeSelected.pressPoint = e.Location;
                _polygon.ResetBezier();
                this.Invalidate();
            }
            if (_isPolygonSelected)
            {
                _polygon.MovePolygon(e.Location, this.Width, this.Height);
                _polygon.pressPoint = e.Location;
                this.Invalidate();
            }
            if (_bezierSelected != null)
            {
                _bezierSelected.MovePoint(e.Location, _asociatedVertex);
                _polygon.ResetBezier();
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            _bresenham.g = e.Graphics;
            _polygon.DrawPolygon(_bresenham, e.Graphics, bresenhamRadioButton.Checked);
            if (_isDrawing)
            {
                if (bresenhamRadioButton.Checked)
                {
                    _bresenham.Draw(_startPoint, _currentPoint);
                }
                else
                {
                    e.Graphics.DrawLine(new Pen(_bresenham.brush), _startPoint, _currentPoint);
                }
            }
        }

        private void Option_DeleteVertex(object sender, EventArgs e)
        {
            _polygon.ResetAllLengthes();
            _polygon.RemoveVertex(_vertexSelected);
            _vertexSelected.isSelected = false;
            _vertexSelected = null;
            _polygon.ResetBezier();
            this.Invalidate();
        }

        private void Option_SetG0(object sender, EventArgs e)
        {
            _vertexSelected.SetBezierState(BezierStates.G0);
            _vertexSelected.isSelected = false;
            _vertexSelected = null;
            this.Invalidate();
        }

        private void Option_SetG1(object sender, EventArgs e)
        {
            _polygon.ResetAllLengthes();
            _vertexSelected.SetBezierState(BezierStates.G1);
            _vertexSelected.isSelected = false;
            _polygon.ResetBezier();
            _vertexSelected = null;

            this.Invalidate();
        }

        private void Option_SetC1(object sender, EventArgs e)
        {
            _polygon.ResetAllLengthes();
            _vertexSelected.SetBezierState(BezierStates.C1);
            _vertexSelected.isSelected = false;
            _vertexSelected = null;
            _polygon.ResetBezier();
            this.Invalidate();
        }

        private void Option1_AddPoint(object sender, EventArgs e)
        {
            _polygon.ResetAllLengthes();
            _polygon.AddNewVertex(_edgeSelected);
            _edgeSelected.isSelected = false;
            _edgeSelected = null;
            _polygon.ResetBezier();
            this.Invalidate();
        }

        public void Option2_SetVertical(object sender, EventArgs e)
        {
            _polygon.ResetAllLengthes();
            if (IsAvaliableConstraint(EdgeStates.Vertical))
            {
                _edgeSelected.ChangeState(EdgeStates.Vertical);
            }
            else
            {
                MessageBox.Show("Invalidate operation", "Warning",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Information);

            }
            _edgeSelected.isSelected = false;
            _edgeSelected = null;
            _polygon.ResetBezier();
            this.Invalidate();
        }

        public void Option3_SetHorizontal(object sender, EventArgs e)
        {
            _polygon.ResetAllLengthes();
            if (IsAvaliableConstraint(EdgeStates.Horizontal))
            {
                _edgeSelected.ChangeState(EdgeStates.Horizontal);
            }
            else
            {
                MessageBox.Show("Invalidate operation", "Warning",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Information);

            }
            _edgeSelected.isSelected = false;
            _edgeSelected = null;
            _polygon.ResetBezier();
            this.Invalidate();
        }

        public bool IsAvaliableConstraint(EdgeStates toSet)
        {
            int i = 0;
            int prev = _polygon.edges.Count - 1;

            foreach (Edge e in _polygon.edges)
            {
                if (e == _edgeSelected) break;
                prev = i;
                i++;
            }
            int next = i + 1;
            if (i == _polygon.edges.Count - 1)
            {
                next = 0;
            }
            if ((_polygon.edges[prev].state == toSet) || (_polygon.edges[next].state == toSet))
            {
                return false;
            }

            return true;
        }

        public void Option4_SetFixed(object sender, EventArgs e)
        {
            _polygon.ResetAllLengthes();
            _edgeSelected.ChangeState(EdgeStates.Fixed);
            _edgeSelected.isSelected = false;
            _edgeSelected = null;
            _polygon.ResetBezier();
            this.Invalidate();
        }

        public void Option5_Bezier(object sender, EventArgs e)
        {
            _polygon.ResetAllLengthes();
            _edgeSelected.ChangeState(EdgeStates.Bezier);
            _edgeSelected.isSelected = false;
            _edgeSelected = null;
            // _polygon.ResetBezier();
            this.Invalidate();
        }

        public void Option6_Unset(object sender, EventArgs e)
        {
            _polygon.ResetAllLengthes();
            _edgeSelected.ChangeState(EdgeStates.None);
            _edgeSelected.isSelected = false;
            _edgeSelected = null;
            _polygon.ResetBezier();
            this.Invalidate();
        }

        private void bresenhamRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void drawLineRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void ShowAlgButton_Click(object sender, EventArgs e)
        {
            if (isAlg)
            {
                isAlg = false;
                switch (controlButtonState)
                {
                    case ControlButtonStates.Draw:
                        InstructionBox.Text = _startInstruction;
                        break;
                    case ControlButtonStates.Stop:
                        InstructionBox.Text = _drawingInstruction;
                        break;
                    case ControlButtonStates.Clean:
                        InstructionBox.Text = _modifyInstruction;
                        break;
                }
            }
            else
            {
                isAlg = true;
                InstructionBox.Text = _algDesc;
                ShowAlgButton.Text = "Show user manual";
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            PolygonForSerialization polygonForSerialization = new PolygonForSerialization();
            polygonForSerialization.vertices = new List<VertexForSerialization>();
            polygonForSerialization.edges = new List<EdgeForSerialization>();
            foreach (Vertex v in _polygon.vertices)
            {
                VertexForSerialization vS = new VertexForSerialization();
                vS.position = v.position;
                vS.bezierState = v.bezierState;
                polygonForSerialization.vertices.Add(vS);
            }
            foreach (Edge ed in _polygon.edges)
            {
                EdgeForSerialization eS = new EdgeForSerialization();
                eS.state = ed.state;
                eS.length = ed.length;
                eS.bezierControlPoints = new BezierForSerialization?[2];
                if (ed.bezierControlPoints[0] != null)
                {
                    BezierForSerialization bF0 = new BezierForSerialization();
                    BezierForSerialization bF1 = new BezierForSerialization();
                    bF0.position = ed.bezierControlPoints[0].position;
                    bF0.length = ed.bezierControlPoints[0].length;
                    bF1.position = ed.bezierControlPoints[1].position;
                    bF1.length = ed.bezierControlPoints[1].length;
                    eS.bezierControlPoints[0] = bF0;
                    eS.bezierControlPoints[1] = bF1;
                }
                polygonForSerialization.edges.Add(eS);
            }
            JsonSerializerOptions options = new JsonSerializerOptions { IncludeFields = true, WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(polygonForSerialization, options);
            File.WriteAllText("PolygonInJson.json", jsonString);
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            string jsonString = File.ReadAllText("PolygonInJson.json");
            PolygonForSerialization pS = JsonSerializer.Deserialize<PolygonForSerialization>(jsonString, options);
            if (controlButtonState == ControlButtonStates.Draw)
            {
                ControlButton_Click(this, EventArgs.Empty);
            }
            else if (controlButtonState == ControlButtonStates.Stop) 
            {
                MessageBox.Show("Invalidate operation: you can't load, during drawing process", "Warning",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                return;
            }
            else if (controlButtonState == ControlButtonStates.Clean)
            {
                ControlButton_Click(this, EventArgs.Empty);
                ControlButton_Click(this, EventArgs.Empty);
            }
            ;

            _polygon.AddNewVertex(pS.vertices[0].position);
            _polygon.vertices[0].bezierState = pS.vertices[0].bezierState;
            for (int i = 1; i < pS.vertices.Count; i++)
            {
                _polygon.AddNewVertex(pS.vertices[i].position);
                _polygon.vertices[i].bezierState = pS.vertices[i].bezierState;
                _polygon.AddNewEdge();
                _polygon.edges[i - 1].state = pS.edges[i - 1].state;
                _polygon.edges[i - 1].length = pS.edges[i - 1].length;
                if (pS.edges[i - 1].bezierControlPoints[0] != null)
                {
                    BezierControlPoint b0 = new BezierControlPoint(pS.edges[i - 1].bezierControlPoints[0].position);
                    BezierControlPoint b1 = new BezierControlPoint(pS.edges[i - 1].bezierControlPoints[1].position);
                    b0.length = pS.edges[i - 1].bezierControlPoints[0].length;
                    b1.length = pS.edges[i - 1].bezierControlPoints[1].length;
                    _polygon.edges[i - 1].bezierControlPoints[0] = b0;
                    _polygon.edges[i - 1].bezierControlPoints[1] = b1;
                }
            }
            _polygon.AddFinalEdge();
            _polygon.edges[_polygon.edges.Count - 1].state = pS.edges[_polygon.edges.Count - 1].state;
            _polygon.edges[_polygon.edges.Count - 1].length = pS.edges[_polygon.edges.Count - 1].length;
            if (pS.edges[_polygon.edges.Count - 1].bezierControlPoints[0] != null)
            {
                BezierControlPoint b0 = new BezierControlPoint(pS.edges[pS.edges.Count - 1].bezierControlPoints[0].position);
                BezierControlPoint b1 = new BezierControlPoint(pS.edges[pS.edges.Count - 1].bezierControlPoints[1].position);
                b0.length = pS.edges[pS.edges.Count - 1].bezierControlPoints[0].length;
                b1.length = pS.edges[pS.edges.Count - 1].bezierControlPoints[1].length;
                _polygon.edges[_polygon.edges.Count - 1].bezierControlPoints[0] = b0;
                _polygon.edges[_polygon.edges.Count - 1].bezierControlPoints[1] = b1;
            }
            ControlButton_Click(this, EventArgs.Empty);

        }
    }
}
