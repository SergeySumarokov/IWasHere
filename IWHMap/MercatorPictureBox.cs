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

        #region Определения

        // Файл карты и привязка
        private Bitmap mapImage;
        private Coordinates mapNordWest;
        private Coordinates mapSouthEast;
        private Bitmap visibleImage;
        private Graphics visibleGraphics;
        // Положение и масштаб
        private int mapX = 0, mapY = 0; // левый верхний угол видимой части карты
        private float scaleView = 1F; // масштаб отображения (2 = уменьшение до 50%)
        private float scaleMin = 0.5F;
        private float scaleMax = 4.0F;
        private float scaleStep = 1.1F;
        private float scaleLat; // коэффициент для пересчёта радиан широты в пиксели
        private float scaleLon; // коэффициент для пересчёта радиан долготы в пиксели
        // Перемещение
        private bool dragging = false;
        private int dragLastX, dralLastY;
        private System.Diagnostics.Stopwatch dragTimer; // Для ограничения частоты обновления
        // Отрисовка
        private List<LineToDraw> linesToDraw = new List<LineToDraw>();

        #endregion

        #region Конструкторы

        public MercatorPictureBox()
        {

            InitializeComponent();

            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.this_MouseWheel);
        }

        #endregion

        #region Методы

        /// <summary>
        /// Принимает карту с привязкой к координатам
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="nordWest"></param>
        /// <param name="southEast"></param>
        public void BindMap(Bitmap bitmap, Coordinates nordWest, Coordinates southEast)
        {
            mapImage = bitmap;
            mapNordWest = nordWest;
            mapSouthEast = southEast;
            PrepareGraphics();
            DrawMap();
        }

        #endregion

        #region События

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            PrepareGraphics();
            DrawMap();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // Отрисовываем карту
            base.OnPaint(pe);
            // Отрисовываем линии
            Graphics g = pe.Graphics;
            foreach (LineToDraw line in linesToDraw)
            {
                g.DrawLines(line.Pen, line.Points);
            }
        }

        private void this_MouseEnter(object sender, EventArgs e)
        {
            this.Focus(); // Это нужно, чтобы MouseWheel отрабатывал в любом случае.
        }

        public void this_MouseWheel(object sender, MouseEventArgs e)
        {
            SetScale(Math.Sign(e.Delta));
            DrawMap();
        }

        #endregion

        #region Рисование

        // Необходимо вызывать при изменении размера элемента
        private void PrepareGraphics()
        {
            // Skip it if we've been minimized.
            if ((this.ClientSize.Width == 0) || (this.ClientSize.Height == 0)) return;
            // Free old resources.
            if (visibleGraphics != null)
            {
                this.Image = null;
                visibleGraphics.Dispose();
                visibleImage.Dispose();
            }
            // Make the new Bitmap and Graphics.
            visibleImage = new Bitmap(this.ClientSize.Width,this.ClientSize.Height);
            visibleGraphics = Graphics.FromImage(visibleImage);
            visibleGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            // Display the Bitmap.
            this.Image = visibleImage;
        }

        // Пересчитываем коэффициенты масштаба
        private void SetScale(int delta)
        {
            if (delta > 0 & scaleView < scaleMax)
            {
                scaleView *= scaleStep;
                if (scaleView > scaleMax) scaleView = scaleMax;
            }
            if (delta < 0 & scaleView > scaleMin)
            {
                scaleView /= scaleStep;
                if (scaleView < scaleMin) scaleView = scaleMin;
            }
            System.Drawing.Point relativePoint = this.PointToClient(Cursor.Position);

        }

        // Устанавливает положение карты
        private void SetOrigin()
        {
            // Не допускаем уменьшения карты меньше размера окна
            int scaled_width = (int)(mapImage.Width / scaleView);
            if (scaled_width < this.ClientSize.Width)
                scaleView = (float)mapImage.Width / this.ClientSize.Width;
            int scaled_height = (int)(mapImage.Height / scaleView);
            if (scaled_height < this.ClientSize.Height)
                scaleView = (float)mapImage.Height / this.ClientSize.Height;
            // Не допускаем выхода карты за границу окна
            int xmin = this.ClientSize.Width - scaled_width;
            if (xmin > 0) xmin = 0;
            if (mapX < xmin) mapX = xmin;
            else if (mapX > 0) mapX = 0;

            int ymin = this.ClientSize.Height - scaled_height;
            if (ymin > 0) ymin = 0;
            if (mapY < ymin) mapY = ymin;
            else if (mapY > 0) mapY = 0;
        }

        // Draw the image at the correct scale and location.
        private void DrawMap()
        {

            if (mapImage == null) return;

            // Validate PicX and PicY.
            SetOrigin();

            // Get the destination area.
            float scaled_width = mapImage.Width / scaleView;
            float scaled_height = mapImage.Height / scaleView;
            PointF[] dest_points =
            {
                new PointF(mapX, mapY),
                new PointF(mapX + scaled_width, mapY),
                new PointF(mapX, mapY + scaled_height),
            };

            // Draw the whole image.
            RectangleF source_rect = new RectangleF(
                0, 0, mapImage.Width, mapImage.Height);

            // Draw.
            visibleGraphics.Clear(this.BackColor);
            visibleGraphics.DrawImage(mapImage, dest_points, source_rect, GraphicsUnit.Pixel);

            // Update the display.
            this.Refresh();
        }

        #endregion

        #region Перемещение

        private void this_MouseDown(object sender, MouseEventArgs e)
        {
            dragLastX = e.X;
            dralLastY = e.Y;
            dragTimer = new System.Diagnostics.Stopwatch();
            dragTimer.Start();
            dragging = true;
        }

        private void this_MouseMove(object sender, MouseEventArgs e)
        {
            if (!dragging) return;

            mapX += e.X - dragLastX;
            mapY += e.Y - dralLastY;
            dragLastX = e.X;
            dralLastY = e.Y;

            if (dragTimer.ElapsedMilliseconds <= 30) return;

            DrawMap();
            dragTimer.Restart();
        }

        private void this_MouseUp(object sender, MouseEventArgs e)
        {
            dragTimer = null;
            dragging = false;
        }

        #endregion

    }

    /// <summary>
    /// Структура линии для отрисовки на карте
    /// </summary>
    public struct LineToDraw
    {
        public Pen Pen;
        public Geography.Coordinates[] Coordinates;
        public System.Drawing.Point[] Points;
    }
}
