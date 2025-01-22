import React from 'react';
import { Link } from 'react-router-dom';
import './Header.css';
import { FaShoppingCart, FaBox, FaStore, FaUserAlt } from 'react-icons/fa';

function Header() {
  return (
    <header className="header">
      <h1 className="title"><FaStore /> Tienda Online</h1>
      <nav className="nav">
        <Link to="/products"><FaBox /> Productos</Link>
        <Link to="/cart"><FaShoppingCart /> Carrito</Link>
        <Link to="/orders"><FaBox /> Pedidos</Link>
        <Link to="/"><FaUserAlt /> Modo Administrador</Link>
      </nav>
    </header>
  );
}

export default Header;