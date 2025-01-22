import React, { useState, useEffect } from 'react';
import './CartPage.css';
import { FaTrash } from 'react-icons/fa';
import Header from '../components/HeaderCliente';

function CartPage() {
  const [cartDetails, setCartDetails] = useState([]); // Detalles del carrito
  const [cartTotal, setCartTotal] = useState(0); // Total del carrito

  // Cargar el carrito con idCarrito = 1 al inicio
  useEffect(() => {
    fetch('https://localhost:7004/api/Carrito/detalle/1')
      .then((response) => {
        if (!response.ok) throw new Error('Error al obtener los detalles del carrito');
        return response.json();
      })
      .then((data) => {
        setCartDetails(data.detalles);
        setCartTotal(data.total);
      })
      .catch((error) => console.error('Error al cargar el carrito:', error));
  }, []);

  // Actualizar la cantidad de un producto en el carrito
  const updateQuantity = (idProducto, newQuantity) => {
    fetch('https://localhost:7004/api/Carrito/actualizar-cantidad', {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ idCarrito: 1, idProducto, cantidad: newQuantity }),
    })
      .then((response) => {
        if (!response.ok) throw new Error('Error al actualizar la cantidad');
        alert('Cantidad actualizada');
        // Recargar detalles del carrito
        fetch('https://localhost:7004/api/Carrito/detalle/1')
          .then((res) => res.json())
          .then((data) => {
            setCartDetails(data.detalles);
            setCartTotal(data.total);
          });
      })
      .catch((error) => console.error('Error al actualizar cantidad:', error));
  };

  // Eliminar un producto del carrito
  const removeFromCart = (idProducto) => {
    fetch('https://localhost:7004/api/Carrito/quitar-producto', {
      method: 'DELETE',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ idCarrito: 1, idProducto }),
    })
      .then((response) => {
        if (!response.ok) throw new Error('Error al eliminar el producto');
        alert('Producto eliminado');
        // Recargar detalles del carrito
        fetch('https://localhost:7004/api/Carrito/detalle/1')
          .then((res) => res.json())
          .then((data) => {
            setCartDetails(data.detalles);
            setCartTotal(data.total);
          });
      })
      .catch((error) => console.error('Error al eliminar producto:', error));
  };

  return (
    <>
      <Header />
      <section className="cart-page">
        <h2>Detalles del Carrito</h2>
        <ul className="cart-detail-list">
          {cartDetails.map((item) => (
            <li key={item.idDetalle} className="cart-detail-item">
              <span>{item.nombreProducto}</span>
              <span>Precio: ${item.precioProducto.toFixed(2)}</span>
              <span>Subtotal: ${item.subtotal.toFixed(2)}</span>
              <input
                type="number"
                min="1"
                defaultValue={item.cantidad}
                onBlur={(e) => updateQuantity(item.idProducto, parseInt(e.target.value))}
              />
              <button
                onClick={() => removeFromCart(item.idProducto)}
                className="delete-item"
              >
                <FaTrash /> Eliminar
              </button>
            </li>
          ))}
        </ul>
        <h4>Total Carrito: ${cartTotal.toFixed(2)}</h4>
      </section>
    </>
  );
}

export default CartPage;
