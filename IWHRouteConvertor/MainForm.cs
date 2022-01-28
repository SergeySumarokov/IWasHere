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

            _route = RouteReader.GetFromYandexURL(Helper.GetDebugRouteString(RouteFormat.YandexURL));
            //route = DebugHelper.GetDebugRoute();

            InitializeControls();

            SetStatusText("Готово");
        }


        private void InitializeControls()
        {
            FillControls();
        }

        private void FillControls()
        {
            if (Clipboard.ContainsText())
            {
                clipboardText.Text = Clipboard.GetText();
            }
            else
            {
                clipboardText.Text = "<Тест недоступен>";
            }
            
            FillRouteText();
        }

        private void FillRouteText()
        {
            routeText.Clear();
            foreach (RoutePoint point in _route.Points)
            {
                routeText.Text = _route.ToText();
            }
        }

        private void SetStatusText(String StatusText)
        {
            toolStripStatusLabel.Text = StatusText;
        }

        
        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void ButtonRead_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                Route route = RouteReader.GetRouteFromString(Clipboard.GetText());
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

        private void ButtonWrite_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(RouteWriter.PutToRTE(_route));
            FillControls();
            SetStatusText("Маршрут выгружен");
        }
    }
}
