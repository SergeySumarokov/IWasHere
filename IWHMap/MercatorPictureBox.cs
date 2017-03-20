using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Geography;

namespace IWHMap
{

    [ToolboxItem(true)]
    public partial class MercatorPictureBox : System.Windows.Forms.PictureBox
    {

        // Файл карты и привязка
        private Bitmap originalImage;
        private Coordinates mapNordWest;
        private Coordinates mapSouthEast;
        private Bitmap VisibleImage = null;
        private Graphics VisibleGraphics = null;

        private float scaleView = 1; // масштаб отображения (2 = уменьшение до 50%)
        private float scaleLat; // коэффициент для пересчёта радиан широты в пиксели
        private float scaleLon; // коэффициент для пересчёта радиан долготы в пиксели

        // Upper left corner of the image in the PictureBox.
        private int PicX = 0, PicY = 0;

        public MercatorPictureBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Принимает карту с привязкой к координатам
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="nordWest"></param>
        /// <param name="southEast"></param>
        public void BindMap(Bitmap bitmap, Coordinates nordWest, Coordinates southEast)
        {
            originalImage = bitmap;
            mapNordWest = nordWest;
            mapSouthEast = southEast;
            PrepareGraphics();
            DrawMap();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            PrepareGraphics();
            DrawMap();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            // Дальше пойдет отрисовка линий из массива
        }

        private void PrepareGraphics()
        {
            // Skip it if we've been minimized.
            if ((this.ClientSize.Width == 0) ||
                (this.ClientSize.Height == 0)) return;

            // Free old resources.
            if (VisibleGraphics != null)
            {
                this.Image = null;
                VisibleGraphics.Dispose();
                VisibleImage.Dispose();
            }

            // Make the new Bitmap and Graphics.
            VisibleImage = new Bitmap(this.ClientSize.Width,this.ClientSize.Height);
            VisibleGraphics = Graphics.FromImage(VisibleImage);
            VisibleGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            // Display the Bitmap.
            this.Image = VisibleImage;
        }

        // Пересчитываем коэффициенты масштаба для широты/долготы
        private void SetScale()
        {
        }
        
        // Set the PictureBox's position.
        private void SetOrigin()
        {
            // Keep x and y within bounds.
            float scaled_width = scaleView * originalImage.Width;
            int xmin = (int)(this.ClientSize.Width - scaled_width);
            if (xmin > 0) xmin = 0;
            if (PicX < xmin) PicX = xmin;
            else if (PicX > 0) PicX = 0;

            float scaled_height = scaleView * originalImage.Height;
            int ymin = (int)(this.ClientSize.Height - scaled_height);
            if (ymin > 0) ymin = 0;
            if (PicY < ymin) PicY = ymin;
            else if (PicY > 0) PicY = 0;
        }

        // Draw the image at the correct scale and location.
        private void DrawMap()
        {

            if (originalImage == null) return;

            // Validate PicX and PicY.
            SetOrigin();

            // Get the destination area.
            float scaled_width = scaleView * originalImage.Width;
            float scaled_height = scaleView * originalImage.Height;
            PointF[] dest_points =
            {
                new PointF(PicX, PicY),
                new PointF(PicX + scaled_width, PicY),
                new PointF(PicX, PicY + scaled_height),
            };

            // Draw the whole image.
            RectangleF source_rect = new RectangleF(
                0, 0, originalImage.Width, originalImage.Height);

            // Draw.
            VisibleGraphics.Clear(this.BackColor);
            VisibleGraphics.DrawImage(originalImage, dest_points, source_rect, GraphicsUnit.Pixel);

            // Update the display.
            this.Refresh();
        }

        #region Dragging

        // Let the user drag the image around.
        private bool Dragging = false;
        private int LastX, LastY;
        private System.Diagnostics.Stopwatch dragTimer; // Для ограничения частоты обновления

        private void this_MouseDown(object sender, MouseEventArgs e)
        {
            LastX = e.X;
            LastY = e.Y;
            dragTimer = new System.Diagnostics.Stopwatch();
            dragTimer.Start();
            Dragging = true;
        }

        private void this_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Dragging) return;

            PicX += e.X - LastX;
            PicY += e.Y - LastY;
            LastX = e.X;
            LastY = e.Y;

            if (dragTimer.ElapsedMilliseconds <= 30) return;

            DrawMap();
            dragTimer.Restart();
        }

        private void this_MouseUp(object sender, MouseEventArgs e)
        {
            dragTimer = null;
            Dragging = false;
        }

        #endregion

    }

    /// <summary>
    /// Структура линии для отрисовки на карте
    /// </summary>
    public struct LineToDraw
    {
        public Pen Pen;
        public List<Geography.Coordinates> Coordinates;
    }
}
