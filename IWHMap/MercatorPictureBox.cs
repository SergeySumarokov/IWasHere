using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace IWHMap
{

    [ToolboxItem(true)]
    public partial class MercatorPictureBox : System.Windows.Forms.PictureBox
    {

        //private Bitmap OriginalImage;
        private Bitmap OriginalImage;
        private float CurrentScale = 1;
        private Bitmap VisibleImage = null;
        private Graphics VisibleGraphics = null;

        // Upper left corner of the image in the PictureBox.
        private int PicX = 0, PicY = 0;

        /// <summary>
        /// Растровая карта
        /// </summary>
        public Bitmap MapImage
        {
            get { return OriginalImage; }
            set
            {
                OriginalImage = value;
                PrepareGraphics();
                DrawMap();
            }
        }

        /// <summary>
        /// Масштаб изображения - расстояние на один пиксель.
        /// </summary>
        public Primitives.Distance MapScale { get; set; }

        /// <summary>
        /// Координаты центральной точки карты
        /// </summary>
        public Geography.Coordinates CenterPoint { get; set; }


        public MercatorPictureBox()
        {
            InitializeComponent();

            // Get ready to draw.
            PrepareGraphics();

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
        
        // Set the PictureBox's position.
        private void SetOrigin()
        {
            // Keep x and y within bounds.
            float scaled_width = CurrentScale * OriginalImage.Width;
            int xmin = (int)(this.ClientSize.Width - scaled_width);
            if (xmin > 0) xmin = 0;
            if (PicX < xmin) PicX = xmin;
            else if (PicX > 0) PicX = 0;

            float scaled_height = CurrentScale * OriginalImage.Height;
            int ymin = (int)(this.ClientSize.Height - scaled_height);
            if (ymin > 0) ymin = 0;
            if (PicY < ymin) PicY = ymin;
            else if (PicY > 0) PicY = 0;
        }

        // Draw the image at the correct scale and location.
        private void DrawMap()
        {

            if (OriginalImage == null) return;

            // Validate PicX and PicY.
            SetOrigin();

            // Get the destination area.
            float scaled_width = CurrentScale * OriginalImage.Width;
            float scaled_height = CurrentScale * OriginalImage.Height;
            PointF[] dest_points =
            {
                new PointF(PicX, PicY),
                new PointF(PicX + scaled_width, PicY),
                new PointF(PicX, PicY + scaled_height),
            };

            // Draw the whole image.
            RectangleF source_rect = new RectangleF(
                0, 0, OriginalImage.Width, OriginalImage.Height);

            // Draw.
            VisibleGraphics.Clear(this.BackColor);
            VisibleGraphics.DrawImage(OriginalImage, dest_points, source_rect, GraphicsUnit.Pixel);

            // Update the display.
            this.Refresh();
        }

        #region Dragging

        // Let the user drag the image around.
        private bool Dragging = false;
        private int LastX, LastY;

        private void this_MouseDown(object sender, MouseEventArgs e)
        {
            LastX = e.X;
            LastY = e.Y;
            Dragging = true;
        }

        private void this_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Dragging) return;

            PicX += e.X - LastX;
            PicY += e.Y - LastY;
            LastX = e.X;
            LastY = e.Y;

            DrawMap();
        }

        private void this_MouseUp(object sender, MouseEventArgs e)
        {
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
