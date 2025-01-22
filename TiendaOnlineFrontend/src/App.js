import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Header from './components/Header';
import ProductsPage from './pages/ProductsPage';
import CartPage from './pages/CartPage';
import OrdersPage from './pages/OrdersPage';
import ProductsPageUser from './pages/ProductsPageUser';
import Footer from './components/Footer';
import './App.css';

function App() {
  return (
    <Router>
      <div className="App">
        
        <main>
          <Routes>
            <Route path="/" element={<ProductsPage />} />
            <Route path="/cart" element={<CartPage />} />
            <Route path="/orders" element={<OrdersPage />} />
            <Route path="/products" element={<ProductsPageUser />} />
          </Routes>
        </main>
        <Footer />
      </div>
    </Router>
  );
}

export default App;