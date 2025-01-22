import React, { useState, useEffect } from 'react';
import './ProductsPage.css';
import { FaEdit, FaTrash, FaPlusCircle, FaCartPlus } from 'react-icons/fa';
import Header from '../components/Header';
function ProductsPage() {
  const [products, setProducts] = useState([]);
  const [newProduct, setNewProduct] = useState({ nombre: '', descripcion: '', precio: 0, stock: 0 });
  const [updateProduct, setUpdateProduct] = useState(null);
  const [showModal, setShowModal] = useState(false);
  
  // Cargar los productos al inicio
  useEffect(() => {
    fetch('https://localhost:7004/api/Productos/lista')
      .then((response) => {
        console.log('Cargando productos - Estado de la respuesta:', response.status);
        return response.json();
      })
      .then((data) => {
        console.log('Productos cargados:', data);
        setProducts(data);
      })
      .catch((error) => console.error('Error al cargar productos:', error));
  }, []);

  // Crear producto
  const handleCreateProduct = () => {
    console.log('Datos del producto a crear:', newProduct);

    fetch('https://localhost:7004/api/Productos/crear', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(newProduct),
    })
      .then((response) => {
        console.log('Estado de respuesta al crear producto:', response.status);
        if (!response.ok) throw new Error('Error al crear el producto');
        return response.json();
      })
      .then(() => {
        alert('Producto creado exitosamente');
        setNewProduct({ nombre: '', descripcion: '', precio: 0, stock: 0 });
        fetch('https://localhost:7004/api/Productos/lista')
          .then((res) => res.json())
          .then((data) => setProducts(data));
      })
      .catch((error) => console.error('Error al crear producto:', error));
  };

  // Abrir el modal para actualizar
  const handleOpenModal = (product) => {
    console.log('Abriendo modal para producto:', product);
    setUpdateProduct(product);
    setShowModal(true);
  };

  // Actualizar producto
  const handleUpdateProduct = () => {
    console.log('Datos del producto a actualizar:', updateProduct);

    fetch(`https://localhost:7004/api/Productos/actualizar/${updateProduct.idProducto}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(updateProduct),
    })
      .then((response) => {
        console.log('Estado de respuesta al actualizar producto:', response.status);
        if (!response.ok) throw new Error('Error al actualizar el producto');
        return response.json();
      })
      .then(() => {
        alert('Producto actualizado exitosamente');
        setShowModal(false);
        fetch('https://localhost:7004/api/Productos/lista')
          .then((res) => res.json())
          .then((data) => setProducts(data));
      })
      .catch((error) => console.error('Error al actualizar producto:', error));
  };

  // Eliminar producto
  const handleDeleteProduct = (id) => {
    console.log('Producto a eliminar - ID:', id);
  
    fetch(`https://localhost:7004/api/Productos/eliminar/${id}`, {
      method: 'DELETE',
      headers: { Accept: '*/*' }, // Simula las cabeceras de Postman
    })
      .then((response) => {
        console.log('Estado de respuesta al eliminar producto:', response.status);
        if (!response.ok) {
          return response.json().then((data) => {
            throw new Error(`Error al eliminar producto: ${data.message || response.statusText}`);
          });
        }
        alert('Producto eliminado exitosamente');
        setProducts(products.filter((product) => product.id !== id)); // Actualiza la lista local
      })
      .catch((error) => console.error('Error al eliminar producto:', error));
  };
  

  // Agregar al carrito
  const handleAddToCart = (productId) => {
    console.log('Producto a agregar al carrito - ID:', productId);

    fetch('https://localhost:7004/api/Carrito/agregar-producto', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ idUsuario: 1, idProducto: productId, cantidad: 1 }),
    })
      .then((response) => {
        console.log('Estado de respuesta al agregar al carrito:', response.status);
        if (!response.ok) {
          return response.json().then((data) => {
            throw new Error(`Error al agregar al carrito: ${data.message || response.statusText}`);
          });
        }
        alert('Producto agregado al carrito exitosamente');
      })
      .catch((error) => console.error('Error al agregar al carrito:', error));
  };

  return (
    <> <Header></Header>
    <section className="products-page">
      <h2>Productos</h2>

      {/* Crear producto */}
      <div className="new-product">
        <h3>Crear Producto</h3>
        <input
          type="text"
          placeholder="Nombre"
          value={newProduct.nombre}
          onChange={(e) => setNewProduct({ ...newProduct, nombre: e.target.value })}
        />
        <input
          type="text"
          placeholder="Descripción"
          value={newProduct.descripcion}
          onChange={(e) => setNewProduct({ ...newProduct, descripcion: e.target.value })}
        />
        <input
          type="number"
          placeholder="Precio"
          value={newProduct.precio}
          onChange={(e) => setNewProduct({ ...newProduct, precio: parseFloat(e.target.value) })}
        />
        <input
          type="number"
          placeholder="Stock"
          value={newProduct.stock}
          onChange={(e) => setNewProduct({ ...newProduct, stock: parseInt(e.target.value) })}
        />
        <button onClick={handleCreateProduct}>
          <FaPlusCircle /> Crear Producto
        </button>
      </div>

      {/* Lista de productos */}
      <div className="product-list">
        {products.map((product) => (
          <div key={product.idProducto} className="product-item">
            <h3>{product.nombre}</h3>
            <p>ID: {product.idProducto}</p>
            <p>{product.descripcion}</p>
            <p>Precio: ${product.precio}</p>
            <p>Stock: {product.stock}</p>
            {/* <button onClick={() => handleAddToCart(product.idProducto)}>
              <FaCartPlus /> Agregar al Carrito
            </button> */}
            <button onClick={() => handleOpenModal(product)}>
              <FaEdit /> Actualizar
            </button>
            <button onClick={() => handleDeleteProduct(product.idProducto)}>
              <FaTrash /> Eliminar
            </button>
          </div>
        ))}
      </div>

      {/* Modal de actualización */}
      {showModal && (
        <div className="modal">
          <div className="modal-content">
            <h3>Actualizar Producto</h3>
            <input
              type="text"
              placeholder="Nombre"
              value={updateProduct.nombre}
              onChange={(e) => setUpdateProduct({ ...updateProduct, nombre: e.target.value })}
            />
            <input
              type="text"
              placeholder="Descripción"
              value={updateProduct.descripcion}
              onChange={(e) => setUpdateProduct({ ...updateProduct, descripcion: e.target.value })}
            />
            <input
              type="number"
              placeholder="Precio"
              value={updateProduct.precio}
              onChange={(e) => setUpdateProduct({ ...updateProduct, precio: parseFloat(e.target.value) })}
            />
            <input
              type="number"
              placeholder="Stock"
              value={updateProduct.stock}
              onChange={(e) => setUpdateProduct({ ...updateProduct, stock: parseInt(e.target.value) })}
            />
            <button onClick={handleUpdateProduct}>Guardar</button>
            <button onClick={() => setShowModal(false)}>Cancelar</button>
          </div>
        </div>
      )}
    </section>
    </>
  );
}

export default ProductsPage;
