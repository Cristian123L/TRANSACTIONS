using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace Frontend
{
    public partial class FrmPuntoVenta : Form
    {
        private Guna2TextBox? txtCodigoBarras;
        private Guna2DataGridView? dgvProductos;
        private Guna2HtmlLabel? lblSubtotal;
        private Guna2HtmlLabel? lblIva;
        private Guna2HtmlLabel? lblTotal;
        private Guna2Button? btnGuardarVenta;

        public FrmPuntoVenta()
        {
            InitializeComponent();
            CrearInterfazProgramaticamente();
        }

        private void CrearInterfazProgramaticamente()
        {
            this.Text = "Punto de Venta - Sistema POS";
            this.Size = new Size(950, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Ancho incrementado a 550
            txtCodigoBarras = new Guna2TextBox
            {
                PlaceholderText = "Escanea o ingresa el código de barras y presiona Enter...",
                Location = new Point(20, 20),
                Size = new Size(550, 45),
                BorderRadius = 6,
                Font = new Font("Segoe UI", 12)
            };
            txtCodigoBarras.KeyDown += TxtCodigoBarras_KeyDown;
            this.Controls.Add(txtCodigoBarras);

            dgvProductos = new Guna2DataGridView
            {
                Location = new Point(20, 80),
                Size = new Size(890, 380),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                ColumnHeadersHeight = 45
            };
            dgvProductos.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvProductos.ThemeStyle.AlternatingRowsStyle.BackColor = Color.FromArgb(245, 245, 245);

            dgvProductos.Columns.Add("Clave", "Clave");
            dgvProductos.Columns.Add("Descripcion", "Descripción");
            dgvProductos.Columns.Add("Cantidad", "Cantidad");
            dgvProductos.Columns.Add("Precio", "Precio Unitario");
            dgvProductos.Columns.Add("Importe", "Importe");
            this.Controls.Add(dgvProductos);

            lblSubtotal = new Guna2HtmlLabel { Text = "Subtotal: $0.00", Location = new Point(700, 480), Font = new Font("Segoe UI", 12) };
            lblIva = new Guna2HtmlLabel { Text = "IVA (16%): $0.00", Location = new Point(700, 510), Font = new Font("Segoe UI", 12) };
            lblTotal = new Guna2HtmlLabel { Text = "Total: $0.00", Location = new Point(700, 540), Font = new Font("Segoe UI", 16, FontStyle.Bold) };

            this.Controls.Add(lblSubtotal);
            this.Controls.Add(lblIva);
            this.Controls.Add(lblTotal);

            // Ancho incrementado a 280
            btnGuardarVenta = new Guna2Button
            {
                Text = "COBRAR Y GUARDAR",
                Location = new Point(20, 480),
                Size = new Size(280, 55),
                BorderRadius = 8,
                FillColor = Color.FromArgb(46, 204, 113),
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnGuardarVenta.Click += BtnGuardarVenta_Click;
            this.Controls.Add(btnGuardarVenta);
        }

        private void TxtCodigoBarras_KeyDown(object? sender, KeyEventArgs e)
        {
        }

        private void BtnGuardarVenta_Click(object? sender, EventArgs e)
        {
        }

        private void FrmPuntoVenta_Load(object sender, EventArgs e)
        {

        }
    }
}