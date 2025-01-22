namespace TiendaOnlineAPI.Models
{
    public class Inventario
    {
        public int IdInventario { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public string TipoMovimiento { get; set; } // 'entrada' o 'salida'
        public DateTime FechaMovimiento { get; set; }
    }
}
