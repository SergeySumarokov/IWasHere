using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IWHRouteConvertor
{
    public partial class MainForm : Form
    {

        private Route _route = new Route();

        public MainForm()
        {
            InitializeComponent();

            _route = RouteReader.FromYandexURL(Helper.GetDebugRouteString(RouteFormat.YandexURL));
            //route = DebugHelper.GetDebugRoute();

            InitializeControls();

            SetStatusText("Готово");
        }

        private void InitializeControls()
        {
            comboFormatList.DataSource = Enum.GetValues(typeof(RouteFormat));
            FillControls();
        }

        private void FillControls()
        {
            FillRouteText();
            FillClipboadText();
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            FillClipboadText();
        }

        private void FillClipboadText()
        {
            if (Clipboard.ContainsText())
            {
                textClipboard.Text = Clipboard.GetText();
            }
            else
            {
                textClipboard.Text = "<Тест недоступен>";
            }
        }

        private void FillRouteText()
        {
            textRoute.Clear();
            foreach (routePoint point in _route.Points)
            {
                textRoute.Text = _route.ToText();
            }
        }

         private void SetStatusText(String StatusText)
        {
            toolStripStatusLabel.Text = StatusText;
        }
        
        private void ButtonRead_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                Route route = RouteReader.FromText(Clipboard.GetText());
                if (route == null)
                {
                    SetStatusText("Текст не распознан");
                }
                else
                {
                    _route = route;
                    FillControls();
                    SetStatusText("Маршрут загружен");
                }
            }
            else
            {
                SetStatusText("В буфере нет текста");
            }
        }

        private void ButtonReverse_Click(object sender, EventArgs e)
        {
            // Считывает текст маршрута
            Route newRoute = RouteReader.FromNative(textRoute.Text);
            if (newRoute == null)
            {
                SetStatusText("Ошибка в маршруте");
                return;
            }
            _route = newRoute;
            //
            _route.Points.Reverse();
            FillControls();
            SetStatusText("Маршрут инвертирован");
        }

        private void ButtonWrite_Click(object sender, EventArgs e)
        {
            // Считывает текст маршрута
            Route newRoute = RouteReader.FromNative(textRoute.Text);
            if (newRoute == null)
            {
                SetStatusText("Ошибка в маршруте");
                return;
            }
            _route = newRoute;
            //
            String routeText = RouteWriter.ToText(_route, (RouteFormat) comboFormatList.SelectedItem);
            if (String.IsNullOrEmpty(routeText))
            {
                SetStatusText("Не получилось");
                return;
            }
            Clipboard.SetText(routeText);
            FillControls();
            SetStatusText("Маршрут выгружен");
        }

    }
}
