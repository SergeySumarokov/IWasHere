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
        private int mapX = 0, mapY = 0; // положение ЛВ-угла карты относительно ЛВ-угла элемента управления; должно быть отрицательным
        private float scaleView = 1F; // масштаб отображения карты (2 = уменьшение до 50%)
        private float scaleMin = 0.5F; // Минимальный масштаб
        private float scaleMax = 4.0F; // Максимальный масштаб
        private float scaleStep = 1.2F; // Шаг изменения масштаба
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

        /// <summary>
        /// Добавляет линию в список подлежащих отрисовке
        /// </summary>
        /// <param name="line"></param>
        public void AddLine(LineToDraw line)
        {
            linesToDraw.Add(line);
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
            DrawLines(pe.Graphics);
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

        #region Положение и масштаб карты

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

        // Пересчитываем коэффициент масштаба
        private void SetScale(int delta)
        {
            // Запоминаем текущее положение мыши над картой
            Point cursorOnMap = new Point(); // положение мыши на карте в оригинальном размере
            Point cursorOnControl = this.PointToClient(Cursor.Position); // положение мыши на этом элементе
            cursorOnMap.X = (int)((mapX - cursorOnControl.X) * scaleView);
            cursorOnMap.Y = (int)((mapY - cursorOnControl.Y) * scaleView);
            // Меняем масштаб карты
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
            // Меняем положение карты так, чтобы точка под указателем мыши осталась на месте
            mapX = (int)(cursorOnMap.X / scaleView + cursorOnControl.X);
            mapY = (int)(cursorOnMap.Y / scaleView + cursorOnControl.Y);
        }

        // Устанавливает положение и масшаб карты в допустимых пределах
        private void SetOrigin()
        {
            // Не допускаем уменьшения карты меньше размера окна
            Size scaledSize = GetScaledSize();
            if (scaledSize.Width < this.ClientSize.Width)
                scaleView = (float)mapImage.Width / this.ClientSize.Width;
            if (scaledSize.Height < this.ClientSize.Height)
                scaleView = (float)mapImage.Height / this.ClientSize.Height;
            // Не допускаем появления в окне границы карты
            int xmin = this.ClientSize.Width - scaledSize.Width;
            if (xmin > 0) xmin = 0;
            if (mapX < xmin) mapX = xmin;
            else if (mapX > 0) mapX = 0;
            int ymin = this.ClientSize.Height - scaledSize.Height;
            if (ymin > 0) ymin = 0;
            if (mapY < ymin) mapY = ymin;
            else if (mapY > 0) mapY = 0;
            // Инициируем пересчёт координат линий
            RecalcLines();
        }

        // Возвращает размер карты с учётом масштаба
        private Size GetScaledSize()
        {
            return new Size((int)(mapImage.Width / scaleView), (int)(mapImage.Height / scaleView));
        }

        // Отрисовывает карту
        private void DrawMap()
        {
            if (mapImage == null) return;
            // Проверяем положение и масштаб карты
            SetOrigin();
            // Get the destination area.
            Size scaledSize = GetScaledSize();
            PointF[] dest_points =
            {
                new PointF(mapX, mapY),
                new PointF(mapX + scaledSize.Width, mapY),
                new PointF(mapX, mapY + scaledSize.Height),
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

        #region Перемещение карты

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

        #region Рисование линий

        // Пересчитывает положение точек всех линий на карте в оригинальном размере
        private void RecalcLines()
        {
            double scaleLat =  mapImage.Height / (mapNordWest.Latitude - mapSouthEast.Latitude).Radians;
            double scaleLon = mapImage.Width / (mapNordWest.Longitude - mapSouthEast.Longitude).Radians;
            // Обходим каждую линию
            foreach (LineToDraw line in linesToDraw)
            {
                // Пересчитываем все точки
                Point[] points = new Point[line.Coordinates.Length];
                for (int i=0; i<line.Coordinates.Length; i++)
                {
                    points[i].X = mapX + (int)((mapNordWest.Longitude - line.Coordinates[i].Longitude).Radians * scaleLon / scaleView);
                    points[i].Y = mapY + (int)((mapNordWest.Latitude - line.Coordinates[i].Latitude).Radians * scaleLat / scaleView);
                }
                line.Points = points;
            }
        }

        private void DrawLines(Graphics g)
        {
            foreach (LineToDraw line in linesToDraw)
            {
                g.DrawLines(line.Pen, line.Points);
            }

        }

        #endregion

    }

    /// <summary>
    /// Структура линии для отрисовки на карте
    /// </summary>
    public class LineToDraw
    {
        public System.Drawing.Pen Pen;
        public Geography.Coordinates[] Coordinates;
        public System.Drawing.Point[] Points;
    }

}
