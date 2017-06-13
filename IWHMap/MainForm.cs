using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IWHMap
{
    public partial class MainForm : Form
    {
        public MainForm()
        {

            InitializeComponent();

            // 
            GPS.Gpx gpxTrack = GPS.Gpx.FromXmlFile(@"\Projects\IWasHere\Resources\Way_Visited.gpx");


            foreach (GPS.Track track in gpxTrack.Tracks)
            {
                foreach (GPS.TrackSegment segment in track.Segments)
                {
                    var coordinates = new List<Geography.Coordinates>();
                    var line = new LineToDraw();
                    line.Pen = new System.Drawing.Pen(Color.Red, 3);
                    foreach (GPS.TrackPoint point in segment.Points)
                    {
                        coordinates.Add(point.Coordinates);
                    }
                    line.Coordinates = coordinates.ToArray();
                    if (line.Coordinates.Length==0)
                        { }
                    else
                        MercatorMap.AddLine(line);
                }
            }
            // Загрузка и привязка карты
            MercatorMap.BindMap(
                new Bitmap(@"\Projects\IWasHere\Resources\RU-LEN_map.jpg", false),
                new Geography.Coordinates(61.34507817, 27.69927978, 0),
                new Geography.Coordinates(58.38460903, 35.86486816, 0));
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }
    }
}
