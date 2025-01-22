import React, { useState, useEffect } from 'react';
import './OrdersPage.css';
import Header from '../components/HeaderCliente';

function OrdersPage() {
  const [orders, setOrders] = useState([]);

  useEffect(() => {
    fetch('https://localhost:7004/api/Pedido/usuario/1')
      .then((response) => response.json())
      .then((data) => setOrders(data));
  }, []);

  return (
    <><Header></Header>
    <section className="orders-page">
      <h2>Mis Pedidos</h2>
      <ul className="order-list">
        {orders.map((order) => (
          <li key={order.IdPedido} className="order-item">
            <h3>Pedido #{order.IdPedido}</h3>
            <p>Estado: {order.Estado}</p>
            <p>Total: ${order.Total}</p>
            <ul>
              {order.Productos.map((product) => (
                <li key={product.IdProducto}>
                  {product.NombreProducto} (Cantidad: {product.Cantidad})
                </li>
              ))}
            </ul>
          </li>
        ))}
      </ul>
    </section>
    </>
  );
}

export default OrdersPage;
