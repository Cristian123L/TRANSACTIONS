using System;
using System.Collections.Generic;
using Entities;
using DataBase;

namespace Backend
{
    public class VentaService
    {
        private VentaRepository repository = new VentaRepository();

        public Producto? ObtenerProducto(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;

            return repository.BuscarProductoPorCodigo(codigo);
        }

        public void ProcesarVenta(decimal subtotal, decimal iva, decimal total, List<Producto> detalles)
        {
            if (detalles == null || detalles.Count == 0)
                throw new Exception("No se puede procesar una venta sin productos.");

            if (total <= 0)
                throw new Exception("El total de la venta debe ser mayor a cero.");

            // Envía los datos validados a la capa DataBase para ejecutar la transacción SQL
            repository.GuardarVentaConTransaccion(subtotal, iva, total, detalles);
        }
    }
}