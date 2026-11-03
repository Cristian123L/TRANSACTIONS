using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Entities;

namespace DataBase
{
    public class VentaRepository
    {
        private string connectionString = "Server=localhost;Database=POS_Local;Uid=root;Pwd=root123;";

        public Producto? BuscarProductoPorCodigo(string clave)
        {
            Producto? p = null;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "SELECT Clave, Descripcion, Precio FROM PRODUCTOS WHERE Clave = @clave";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@clave", clave);

                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        p = new Producto
                        {
                            Clave = reader["Clave"].ToString() ?? string.Empty,
                            Descripcion = reader["Descripcion"].ToString() ?? string.Empty,
                            Precio = Convert.ToDecimal(reader["Precio"]),
                            Cantidad = 1
                        };
                        // Fórmula: Importe = Precio * Cantidad
                        p.Importe = p.Precio * p.Cantidad;
                    }
                }
            }
            return p;
        }

        public void GuardarVentaConTransaccion(decimal subtotal, decimal iva, decimal total, List<Producto> detalles)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Inicio de la transacción
                using (MySqlTransaction tr = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Insertar la venta
                        string queryVenta = "INSERT INTO VENTAS (Subtotal, Iva, Total) VALUES (@sub, @iva, @tot); SELECT LAST_INSERT_ID();";
                        MySqlCommand cmdVenta = new MySqlCommand(queryVenta, conn, tr);
                        cmdVenta.Parameters.AddWithValue("@sub", subtotal);
                        cmdVenta.Parameters.AddWithValue("@iva", iva);
                        cmdVenta.Parameters.AddWithValue("@tot", total);

                        int idVenta = Convert.ToInt32(cmdVenta.ExecuteScalar());

                        foreach (var item in detalles)
                        {
                            // 2. Insertar los detalles
                            string queryDetalle = "INSERT INTO DETALLES_VENTA (IdVenta, ClaveProducto, Cantidad, Precio, Importe) VALUES (@idVenta, @clave, @cant, @precio, @importe)";
                            MySqlCommand cmdDetalle = new MySqlCommand(queryDetalle, conn, tr);
                            cmdDetalle.Parameters.AddWithValue("@idVenta", idVenta);
                            cmdDetalle.Parameters.AddWithValue("@clave", item.Clave);
                            cmdDetalle.Parameters.AddWithValue("@cant", item.Cantidad);
                            cmdDetalle.Parameters.AddWithValue("@precio", item.Precio);
                            cmdDetalle.Parameters.AddWithValue("@importe", item.Importe);
                            cmdDetalle.ExecuteNonQuery();

                            // 3. Actualizar el stock
                            string queryStock = "UPDATE PRODUCTOS SET Stock = Stock - @cant WHERE Clave = @clave";
                            MySqlCommand cmdStock = new MySqlCommand(queryStock, conn, tr);
                            cmdStock.Parameters.AddWithValue("@cant", item.Cantidad);
                            cmdStock.Parameters.AddWithValue("@clave", item.Clave);
                            cmdStock.ExecuteNonQuery();
                        }

                        // Si las tres operaciones son exitosas, confirmamos los cambios
                        tr.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Si ocurre un error, revertimos absolutamente todo
                        tr.Rollback();
                        throw new Exception("Error en la transacción, se aplicó Rollback: " + ex.Message);
                    }
                }
            }
        }
    }
}