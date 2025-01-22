using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using TiendaOnlineAPI.Models;

namespace TiendaOnlineAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarritoController : ControllerBase
    {
        private readonly IDbConnection _dbConnection;

        public CarritoController(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // POST: api/Carrito/crear/{idUsuario}
        [HttpPost("crear/{idUsuario}")]
        public async Task<IActionResult> CrearCarrito(int idUsuario)
        {
            string checkCarritoQuery = "SELECT id_carrito FROM Carrito WHERE id_usuario = @IdUsuario";
            var idCarritoExistente = await _dbConnection.ExecuteScalarAsync<int?>(checkCarritoQuery, new { IdUsuario = idUsuario });

            if (idCarritoExistente.HasValue)
                return BadRequest(new { message = "Ya existe un carrito activo para este usuario", IdCarrito = idCarritoExistente.Value });

            string query = "INSERT INTO Carrito (id_usuario) VALUES (@IdUsuario); SELECT LAST_INSERT_ID();";
            var idCarrito = await _dbConnection.ExecuteScalarAsync<int>(query, new { IdUsuario = idUsuario });

            return Ok(new { IdCarrito = idCarrito, message = "Carrito creado exitosamente" });
        }

        // POST: api/Carrito/agregar-producto
        [HttpPost("agregar-producto")]
        public async Task<IActionResult> AddToCarrito([FromBody] DetalleCarrito detalle)
        {
            string checkInventoryQuery = "SELECT stock FROM Producto WHERE id_producto = @IdProducto";
            int stockDisponible = await _dbConnection.ExecuteScalarAsync<int>(checkInventoryQuery, new { detalle.IdProducto });

            if (stockDisponible < detalle.Cantidad)
                return BadRequest(new { message = "Stock insuficiente para el producto solicitado" });

            string checkQuery = "SELECT COUNT(*) FROM DetalleCarrito WHERE id_carrito = @IdCarrito AND id_producto = @IdProducto";
            int exists = await _dbConnection.ExecuteScalarAsync<int>(checkQuery, detalle);

            if (exists > 0)
            {
                string updateQuery = "UPDATE DetalleCarrito SET cantidad = cantidad + @Cantidad WHERE id_carrito = @IdCarrito AND id_producto = @IdProducto";
                await _dbConnection.ExecuteAsync(updateQuery, detalle);
            }
            else
            {
                string insertQuery = "INSERT INTO DetalleCarrito (id_carrito, id_producto, cantidad) VALUES (@IdCarrito, @IdProducto, @Cantidad)";
                await _dbConnection.ExecuteAsync(insertQuery, detalle);
            }

            string updateStockQuery = "UPDATE Producto SET stock = stock - @Cantidad WHERE id_producto = @IdProducto";
            await _dbConnection.ExecuteAsync(updateStockQuery, detalle);

            return Ok(new { message = "Producto agregado al carrito y stock actualizado" });
        }

        // GET: api/Carrito/detalle/{idCarrito}
        [HttpGet("detalle/{idCarrito}")]
        public async Task<IActionResult> GetCarritoById(int idCarrito)
        {
            string query = @"
            SELECT 
                dc.id_detalle AS IdDetalle,
                dc.id_carrito AS IdCarrito,
                dc.id_producto AS IdProducto,
                dc.cantidad AS Cantidad,
                dc.fecha_agregado AS FechaAgregado,
                p.nombre AS NombreProducto,
                p.precio AS PrecioProducto,
                (dc.cantidad * p.precio) AS Subtotal
            FROM DetalleCarrito dc
            INNER JOIN Producto p ON dc.id_producto = p.id_producto
            WHERE dc.id_carrito = @IdCarrito";

            var detalles = await _dbConnection.QueryAsync<DetalleCarrito>(query, new { IdCarrito = idCarrito });

            if (!detalles.Any())
                return NotFound(new { message = "El carrito está vacío" });

            var total = detalles.Sum(x => x.Subtotal);

            return Ok(new { Detalles = detalles, Total = total });
        }

        // GET: api/Carrito/listar
        [HttpGet("listar")]
        public async Task<IActionResult> ListarCarritos()
        {
            string query = @"
            SELECT 
                c.id_carrito AS IdCarrito,
                c.id_usuario AS IdUsuario,
                u.nombre AS NombreUsuario,
                COUNT(dc.id_detalle) AS TotalProductos,
                SUM(dc.cantidad * p.precio) AS TotalCarrito
            FROM Carrito c
            LEFT JOIN Usuario u ON c.id_usuario = u.id_usuario
            LEFT JOIN DetalleCarrito dc ON c.id_carrito = dc.id_carrito
            LEFT JOIN Producto p ON dc.id_producto = p.id_producto
            GROUP BY c.id_carrito, c.id_usuario, u.nombre";

            var carritos = await _dbConnection.QueryAsync<dynamic>(query);

            if (!carritos.Any())
                return NotFound(new { message = "No hay carritos disponibles" });

            return Ok(carritos);
        }

        // DELETE: api/Carrito/vaciar/{idCarrito}
        [HttpDelete("vaciar/{idCarrito}")]
        public async Task<IActionResult> ClearCarrito(int idCarrito)
        {
            string getDetalleQuery = "SELECT id_producto, cantidad FROM DetalleCarrito WHERE id_carrito = @IdCarrito";
            var detalles = await _dbConnection.QueryAsync<DetalleCarrito>(getDetalleQuery, new { IdCarrito = idCarrito });

            foreach (var item in detalles)
            {
                string revertStockQuery = "UPDATE Producto SET stock = stock + @Cantidad WHERE id_producto = @IdProducto";
                await _dbConnection.ExecuteAsync(revertStockQuery, new { item.Cantidad, item.IdProducto });
            }

            string deleteDetalleQuery = "DELETE FROM DetalleCarrito WHERE id_carrito = @IdCarrito";
            await _dbConnection.ExecuteAsync(deleteDetalleQuery, new { IdCarrito = idCarrito });

            return Ok(new { message = "Carrito vaciado y stock restaurado" });
        }

        // DELETE: api/Carrito/quitar-producto
        [HttpDelete("quitar-producto")]
        public async Task<IActionResult> RemoveProducto([FromBody] DetalleCarrito detalle)
        {
            string checkQuery = "SELECT cantidad FROM DetalleCarrito WHERE id_carrito = @IdCarrito AND id_producto = @IdProducto";
            var cantidadActual = await _dbConnection.ExecuteScalarAsync<int?>(checkQuery, new { detalle.IdCarrito, detalle.IdProducto });

            if (!cantidadActual.HasValue)
                return NotFound(new { message = "El producto no existe en el carrito" });

            string revertStockQuery = "UPDATE Producto SET stock = stock + @Cantidad WHERE id_producto = @IdProducto";
            await _dbConnection.ExecuteAsync(revertStockQuery, new { detalle.Cantidad, detalle.IdProducto });

            string deleteQuery = "DELETE FROM DetalleCarrito WHERE id_carrito = @IdCarrito AND id_producto = @IdProducto";
            await _dbConnection.ExecuteAsync(deleteQuery, detalle);

            return Ok(new { message = "Producto eliminado del carrito y stock restaurado" });
        }
    }
}
