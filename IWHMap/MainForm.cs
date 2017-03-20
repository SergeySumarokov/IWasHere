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

            MercatorMap.MapImage = new Bitmap(@"\Projects\IWasHere\Resources\RU-LEN_map.jpg", false); ;

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }
    }
}
