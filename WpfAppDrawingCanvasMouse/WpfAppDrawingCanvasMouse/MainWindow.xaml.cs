using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using XamlWriter = System.Windows.Markup.XamlWriter;
using XamlReader = System.Windows.Markup.XamlReader;
using System.IO;
using System.Xml;

// https://skribb.io/

namespace WpfAppDrawingCanvasMouse
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        Point currentPoint = new Point();
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                currentPoint = e.GetPosition(this); // Memorizza la coordinata iniziale per utilizzarla dopo per creare la linea
                //Debug
                Tbx_Punto.Text = "X="+currentPoint.X.ToString("0.0")+"Y="+currentPoint.Y.ToString("0.0");
                Tbx_Linea.Clear();
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Line line = new Line(); //Crea una nuova linea
                line.Stroke = SystemColors.WindowFrameBrush;

                //Punto inizio linea
                line.X1 = currentPoint.X;
                line.Y1 = currentPoint.Y;
                //Ultimo punto dela linea
                line.X2 = e.GetPosition(this).X;
                line.Y2 = e.GetPosition(this).Y;

                Tbx_Linea.Text = line.X1.ToString("0.0") + "-" + line.Y1.ToString("0.0") + "     " + line.X2.ToString("0.0") + "-" + line.Y2.ToString("0.0");
                Tbx_Linea.ScrollToEnd();
                //Si mette come nuovo inizio della linea la fine della vecchia così da poterla creare continua
                currentPoint = e.GetPosition(this);
                //Disegnamo i movimenti
                Canvas_Draw.Children.Add(line);

                string stLine = XamlWriter.Save(line);
                StringReader stringReader = new StringReader(stLine);
                XmlReader xmlReader = XmlReader.Create(stringReader);
                Line linea = (Line)XamlReader.Load(xmlReader);
                Canvas_Result.Children.Add(linea);
            }
        }
    }
}
