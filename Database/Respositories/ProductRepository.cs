using DB.Exceptions;
using DB.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DB
{
    public class ProductRepository
    {
        private PIDbContext _context;

        public ProductRepository(PIDbContext context)
        {
            _context = context;
        }

        public Product GetById(Guid id)
        {
            try
            {
                return _context.Set<Product>().Find(id);
            }
            catch (Exception ex)
            {
                string err = "HUBO FALLAS al obtener el producto de la base de datos";
                // Console.WriteLine(err);
                // Console.WriteLine(ex.Message + ex.StackTrace); // debido al uso de log, no creo que so ya sea necesario. Pendiente hacer pruebas
                throw new DatabaseException($"{err} : {ex.Message}");
            }
            Log.Information($"DB-Repository Layer: El ID={id.ToString()} fue obtenido satisfactoriamente");
        }
        public async Task<List<Product>> GetAllProducts()
        {
            try
            {
                return await _context.Set<Product>().ToListAsync();
            }
            catch (Exception ex)
            {
                string err = "HUBO FALLAS al obtener los productos de la base de datos";
                // Console.WriteLine(err);
                // Console.WriteLine(ex.Message + ex.StackTrace);
                throw new DatabaseException($"{err} : {ex.Message}");
            }
            Log.Information($"DB-Repository Layer: Los products fueron obtenidos satisfactoriamente");
        }

        public Product CreateProduct(Product product)
        {
            try
            {
                _context.Set<Product>().Add(product);
                return product;
            }
            catch (Exception ex)
            {
                string err = "HUBO FALLAS al crear el producto en la base de datos";
                // Console.WriteLine(err);
                // Console.WriteLine(ex.Message + ex.StackTrace);
                throw new DatabaseException($"{err} : {ex.Message}");
            }
            Log.Information($"DB-Repository Layer: Un nuevo producto fue almacenado en dataset satisfactoriamente");
        }

        public Product UpdateProduct(Product product)
        {
            try
            {
                _context.Entry(product).State = EntityState.Modified;
                return product;
            }
            catch (Exception ex)
            {
                string err = "HUBO FALLAS al actualizar el producto en la base de datos";
                Console.WriteLine(err);
                Console.WriteLine(ex.Message + ex.StackTrace);
                throw new DatabaseException($"{err} : {ex.Message}");
            }
            Log.Information($"DB-Repository Layer:Los datos del producto fueron actualizados satisfactoriamente");
        }

        public Product DeleteProduct(Product product)
        {
            try
            {
                _context.Set<Product>().Remove(product);
                return product;
            }
            catch (Exception ex)
            {
                string err = "HUBO FALLAS al eliminar el producto en la base de datos";
                Console.WriteLine(err);
                Console.WriteLine(ex.Message + ex.StackTrace);
                throw new DatabaseException($"{err} : {ex.Message}");
            }
            Log.Information($"DB-Repository Layer: El producto fue eliminados del dataset satisfactoriamente");
        }
    }
}