using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using TiendaOnlineAPI.Models;

namespace TiendaOnlineAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private readonly IDbConnection _dbConnection;

        public PedidoController(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // POST: api/Pedido/crear
        [HttpPost("crear")]
        public async Task<IActionResult> CrearPedido(int idCarrito)
        {
            string queryCarrito = @"
                SELECT 
                    dc.id_producto AS IdProducto,
                    dc.cantidad AS Cantidad,
                    p.precio AS PrecioUnitario,
                    (dc.cantidad * p.precio) AS Subtotal
                FROM DetalleCarrito dc
                INNER JOIN Producto p ON dc.id_producto = p.id_producto
                WHERE dc.id_carrito = @IdCarrito";

            var productos = await _dbConnection.QueryAsync<DetallePedido>(queryCarrito, new { IdCarrito = idCarrito });

            if (!productos.Any())
                return BadRequest(new { message = "El carrito está vacío" });

            decimal total = productos.Sum(p => p.Subtotal);

            string queryPedido = "INSERT INTO Pedido (id_usuario, estado, total) VALUES ((SELECT id_usuario FROM Carrito WHERE id_carrito = @IdCarrito), 'entregado', @Total); SELECT LAST_INSERT_ID();";
            int idPedido = await _dbConnection.ExecuteScalarAsync<int>(queryPedido, new { IdCarrito = idCarrito, Total = total });

            foreach (var producto in productos)
            {
                string queryDetallePedido = "INSERT INTO DetallePedido (id_pedido, id_producto, cantidad, precio_unitario) VALUES (@IdPedido, @IdProducto, @Cantidad, @PrecioUnitario)";
                await _dbConnection.ExecuteAsync(queryDetallePedido, new
                {
                    IdPedido = idPedido,
                    producto.IdProducto,
                    producto.Cantidad,
                    producto.PrecioUnitario
                });
            }

            string queryVaciarCarrito = "DELETE FROM DetalleCarrito WHERE id_carrito = @IdCarrito";
            await _dbConnection.ExecuteAsync(queryVaciarCarrito, new { IdCarrito = idCarrito });

            return Ok(new { IdPedido = idPedido, Total = total, message = "Pedido creado exitosamente" });
        }

        // GET: api/Pedido/usuario/{idUsuario}
        [HttpGet("usuario/{idUsuario}")]
        public async Task<IActionResult> ObtenerPedidosPorUsuario(int idUsuario)
        {
            string queryPedidos = @"
                SELECT 
                    p.id_pedido AS IdPedido,
                    p.id_usuario AS IdUsuario,
                    p.estado AS Estado,
                    p.total AS Total,
                    p.fecha_pedido AS FechaPedido,
                    dp.id_producto AS IdProducto,
                    pr.nombre AS NombreProducto,
                    dp.cantidad AS Cantidad,
                    dp.precio_unitario AS PrecioUnitario,
                    dp.subtotal AS Subtotal
                FROM Pedido p
                INNER JOIN DetallePedido dp ON p.id_pedido = dp.id_pedido
                INNER JOIN Producto pr ON dp.id_producto = pr.id_producto
                WHERE p.id_usuario = @IdUsuario
                ORDER BY p.fecha_pedido DESC";

            var pedidos = await _dbConnection.QueryAsync<dynamic>(queryPedidos, new { IdUsuario = idUsuario });

            if (!pedidos.Any())
                return NotFound(new { message = "No se encontraron pedidos para este usuario" });

            var pedidosAgrupados = pedidos
                .GroupBy(p => new { p.IdPedido, p.Estado, p.Total, p.FechaPedido })
                .Select(g => new
                {
                    g.Key.IdPedido,
                    g.Key.Estado,
                    g.Key.Total,
                    g.Key.FechaPedido,
                    Productos = g.Select(p => new
                    {
                        p.IdProducto,
                        p.NombreProducto,
                        p.Cantidad,
                        p.PrecioUnitario,
                        p.Subtotal
                    })
                });

            return Ok(pedidosAgrupados);
        }

        // GET: api/Pedido/todos
        [HttpGet("todos")]
        public async Task<IActionResult> ObtenerTodosLosPedidos()
        {
            string queryTodosPedidos = @"
                SELECT 
                    p.id_pedido AS IdPedido,
                    p.id_usuario AS IdUsuario,
                    p.estado AS Estado,
                    p.total AS Total,
                    p.fecha_pedido AS FechaPedido,
                    dp.id_producto AS IdProducto,
                    pr.nombre AS NombreProducto,
                    dp.cantidad AS Cantidad,
                    dp.precio_unitario AS PrecioUnitario,
                    dp.subtotal AS Subtotal
                FROM Pedido p
                INNER JOIN DetallePedido dp ON p.id_pedido = dp.id_pedido
                INNER JOIN Producto pr ON dp.id_producto = pr.id_producto
                ORDER BY p.fecha_pedido DESC";

            var pedidos = await _dbConnection.QueryAsync<dynamic>(queryTodosPedidos);

            if (!pedidos.Any())
                return NotFound(new { message = "No se encontraron pedidos" });

            var pedidosAgrupados = pedidos
                .GroupBy(p => new { p.IdPedido, p.Estado, p.Total, p.FechaPedido })
                .Select(g => new
                {
                    g.Key.IdPedido,
                    g.Key.Estado,
                    g.Key.Total,
                    g.Key.FechaPedido,
                    Productos = g.Select(p => new
                    {
                        p.IdProducto,
                        p.NombreProducto,
                        p.Cantidad,
                        p.PrecioUnitario,
                        p.Subtotal
                    })
                });

            return Ok(pedidosAgrupados);
        }
    }
}
