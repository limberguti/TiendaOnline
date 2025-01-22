import React from 'react';
import { Link } from 'react-router-dom';
import './Header.css';
import { FaShoppingCart, FaBox, FaStore, FaUser } from 'react-icons/fa';

function Header() {
  return (
    <header className="header">
      <h1 className="title"><FaStore /> Tienda Online</h1>
      <nav className="nav">
        <Link to="/"><FaBox /> Productos</Link>
        <Link to="/products"><FaUser /> Modo Usuario</Link>
        
      </nav>
    </header>
  );
}

export default Header;