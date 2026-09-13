using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Edelstahl.WinApp.Forms.Commercial
{
    public partial class FrmConfirmarPedidoV2 : Form
    {
        public FrmConfirmarPedidoV2()
        {
            InitializeComponent();

            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            Text = "Confirmar Pedido";

            Width = 1500;
            Height = 850;

            StartPosition =
                FormStartPosition.CenterScreen;

            BackColor =
                Color.FromArgb(240, 242, 245);

            MinimumSize =
                new Size(1400, 800);
        }
    }
    
}
