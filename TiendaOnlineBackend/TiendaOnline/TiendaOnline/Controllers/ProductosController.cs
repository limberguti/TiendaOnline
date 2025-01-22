using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using TiendaOnlineAPI.Models;

[Route("api/[controller]")]
[ApiController]
public class ProductosController : ControllerBase
{
    private readonly IDbConnection _dbConnection;

    public ProductosController(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    // GET: api/Productos/lista
    [HttpGet("lista")]
    public async Task<IActionResult> GetProductos()
    {
        string query = @"
        SELECT 
            id_producto AS IdProducto, 
            nombre AS Nombre, 
            descripcion AS Descripcion, 
            precio AS Precio, 
            stock AS Stock 
        FROM Producto";
        var productos = await _dbConnection.QueryAsync<Producto>(query);

        if (!productos.Any())
            return NotFound(new { message = "No se encontraron productos" });

        return Ok(productos);
    }


    // GET: api/Productos/detalle/{id}
    [HttpGet("detalle/{id}")]
    public async Task<IActionResult> GetProducto(int id)
    {
        string query = "SELECT * FROM Producto WHERE id_producto = @Id";
        var producto = await _dbConnection.QueryFirstOrDefaultAsync<Producto>(query, new { Id = id });

        if (producto == null)
            return NotFound(new { message = "Producto no encontrado" });

        return Ok(producto);
    }

    // POST: api/Productos/crear
    [HttpPost("crear")]
    public async Task<IActionResult> CreateProducto([FromBody] Producto producto)
    {
        string query = "INSERT INTO Producto (nombre, descripcion, precio, stock) VALUES (@Nombre, @Descripcion, @Precio, @Stock)";
        var result = await _dbConnection.ExecuteAsync(query, producto);

        if (result > 0)
            return Ok(new { message = "Producto creado exitosamente" });

        return BadRequest(new { message = "Error al crear el producto" });
    }

    // PUT: api/Productos/actualizar/{id}
    [HttpPut("actualizar/{id}")]
    public async Task<IActionResult> UpdateProducto(int id, [FromBody] Producto producto)
    {
        string query = "UPDATE Producto SET nombre = @Nombre, descripcion = @Descripcion, precio = @Precio, stock = @Stock WHERE id_producto = @Id";
        var result = await _dbConnection.ExecuteAsync(query, new { producto.Nombre, producto.Descripcion, producto.Precio, producto.Stock, Id = id });

        if (result > 0)
            return Ok(new { message = "Producto actualizado exitosamente" });

        return BadRequest(new { message = "Error al actualizar el producto" });
    }

    // DELETE: api/Productos/eliminar/{id}
    [HttpDelete("eliminar/{id}")]
    public async Task<IActionResult> DeleteProducto(int id)
    {
        Console.WriteLine($"Solicitud DELETE recibida para ID: {id}");
        string query = "DELETE FROM Producto WHERE id_producto = @Id";
        var result = await _dbConnection.ExecuteAsync(query, new { Id = id });

        if (result > 0)
        {
            Console.WriteLine("Producto eliminado exitosamente");
            return Ok(new { message = "Producto eliminado exitosamente" });
        }

        Console.WriteLine("Error al eliminar el producto");
        return BadRequest(new { message = "Error al eliminar el producto" });
    }

}
