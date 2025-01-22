namespace TiendaOnlineAPI.Models
{
    public class DetalleCarrito
    {
        public int IdDetalle { get; set; }
        public int IdCarrito { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public DateTime FechaAgregado { get; set; }

        // Campos adicionales para vista del detalle
        public string NombreProducto { get; set; }
        public decimal PrecioProducto { get; set; }
        public decimal Subtotal { get; set; }
    }
}
