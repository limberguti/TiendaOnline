namespace TiendaOnlineAPI.Models
{
    public class Pedido
    {
        public int IdPedido { get; set; }
        public int IdUsuario { get; set; }
        public string Estado { get; set; } // pendiente, enviado, entregado, cancelado
        public decimal Total { get; set; }
        public DateTime FechaPedido { get; set; }
    }
}
