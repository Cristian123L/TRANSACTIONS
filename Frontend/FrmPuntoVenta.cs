using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using Entities;
using DataBase;

namespace Frontend
{
    public partial class FrmPuntoVenta : Form
    {
        // Controles de la interfaz
        private Guna2TextBox? txtCodigoBarras;
        private Guna2DataGridView? dgvProductos;
        private Guna2HtmlLabel? lblSubtotal;
        private Guna2HtmlLabel? lblIva;
        private Guna2HtmlLabel? lblTotal;
        private Guna2Button? btnGuardarVenta;

        // Lógica de negocio y estado
        private VentaRepository repository;
        private List<Producto> listaDetalles;
        private decimal subtotalVenta = 0m;
        private decimal ivaVenta = 0m;
        private decimal totalVenta = 0m;

        public FrmPuntoVenta()
        {
            InitializeComponent();
            repository = new VentaRepository();
            listaDetalles = new List<Producto>();
            CrearInterfazProgramaticamente();
        }

        private void CrearInterfazProgramaticamente()
        {
            this.Text = "Punto de Venta - Sistema POS";
            this.Size = new Size(950, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

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

        // Evento: Al presionar Enter en el TextBox
        private void TxtCodigoBarras_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtCodigoBarras != null)
            {
                string codigo = txtCodigoBarras.Text.Trim();
                if (!string.IsNullOrEmpty(codigo))
                {
                    AgregarProducto(codigo);
                    txtCodigoBarras.Clear();
                }
            }
        }

        // Evento: Al hacer clic en el botón de Cobrar
        private void BtnGuardarVenta_Click(object? sender, EventArgs e)
        {
            if (listaDetalles.Count == 0)
            {
                MessageBox.Show("No hay productos en la venta actual.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Ejecuta la transacción en la base de datos
                repository.GuardarVentaConTransaccion(subtotalVenta, ivaVenta, totalVenta, listaDetalles);

                MessageBox.Show("Venta guardada y stock actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Limpiar la pantalla para la siguiente venta
                listaDetalles.Clear();
                ActualizarGridYTotales();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error en Transacción", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AgregarProducto(string codigo)
        {
            // Busca en la base de datos a través de la capa DataBase
            Producto? prod = repository.BuscarProductoPorCodigo(codigo);

            if (prod != null)
            {
                listaDetalles.Add(prod);
                ActualizarGridYTotales();
            }
            else
            {
                MessageBox.Show("El producto no existe en la base de datos.", "No Encontrado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarGridYTotales()
        {
            if (dgvProductos == null || lblSubtotal == null || lblIva == null || lblTotal == null) return;

            dgvProductos.Rows.Clear();
            subtotalVenta = 0m;

            foreach (var item in listaDetalles)
            {
                dgvProductos.Rows.Add(item.Clave, item.Descripcion, item.Cantidad, item.Precio.ToString("C"), item.Importe.ToString("C"));

                // Cálculo de Subtotal: Sumatoria de los importes
                subtotalVenta += item.Importe;
            }

            // Cálculo de IVA: Subtotal * 0.16
            ivaVenta = subtotalVenta * 0.16m;

            // Cálculo de Total: Subtotal + IVA
            totalVenta = subtotalVenta + ivaVenta;

            lblSubtotal.Text = $"Subtotal: {subtotalVenta:C}";
            lblIva.Text = $"IVA (16%): {ivaVenta:C}";
            lblTotal.Text = $"Total: {totalVenta:C}";
        }
    }
}