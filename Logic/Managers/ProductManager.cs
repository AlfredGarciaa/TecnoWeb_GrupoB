using System;
using System.Collections.Generic;
using DB;
using Logic.Models;
using Services;
using DB.Models;
using Logic.Exceptions;
using Serilog;

namespace Logic
{
    public class ProductManager : IProductManager
    {
        public List<Logic.Models.Product> Products { get; set; }
        private PriceService _getPriceService;
        private IUnitOfWork _uow;

        public ProductManager(PriceService getPriceService, IUnitOfWork uow)
        {
            _getPriceService = getPriceService;
            _uow = uow;
            Products = new List<Logic.Models.Product>()
            {
                new Logic.Models.Product() { Name = "Polera", Type = "SOCCER", Code = "SOCCER-001",  Price = 45, Stock = 100 },
                new Logic.Models.Product() { Name = "Corto", Type = "SOCCER", Code = "SOCCER-002",  Price = 30, Stock = 50 },
                new Logic.Models.Product() { Name = "Tennis", Type = "BASQUET", Code = "BASQUET-001",  Price = 120, Stock = 250 },
                new Logic.Models.Product() { Name = "Balon", Type = "BASKET", Code = "BASKET-002", Price = 50, Stock = 20 }
            };
        }
        
        public List<Logic.Models.Product> GetProducts() {
            Log.Information("Logic Layer: Se procesa la solicitud de GET Products");

            double newPriceGenerate;
            List <DB.Models.Product> products = _uow.ProductRepository.GetAllProducts().Result;

            List<Logic.Models.Product> productsConverted = new List<Models.Product>();
            foreach (DB.Models.Product item in products)
            {
                newPriceGenerate = _getPriceService.GetPriceServiceAsync().Result;
                productsConverted.Add(new Logic.Models.Product() { Id = item.Id, Name = item.Name, Type = item.Type, Code = item.Code, Stock = item.Stock, Price = newPriceGenerate });
            }
            return productsConverted;
        }

        
        public Logic.Models.Product PostProduct(Logic.Models.Product product)
        {
            Log.Information("Logic Layer: Se procesa la solicitud de POST Product");

            if (product.Stock >= 0)
            {
                DB.Models.Product productConverted = new DB.Models.Product()
                {
                    Name = product.Name,
                    Type = product.Type,
                    Code = getNewCode(product.Type),
                    Stock = product.Stock,
                    Id = new Guid()
                };
                productConverted = _uow.ProductRepository.CreateProduct(productConverted);
                _uow.Save();
            }
            else
            {
                throw new InvalidStockException($"El stock '{product.Stock}' no es valido");
            }

            Log.Information($"Nuevo producto ingresado: Id={product.Id},Name={product.Name} ,Type={product.Type} ,Code={product.Code}, Stock={product.Stock}");
            return product;
        }

        public string getNewCode(string typeProduct)
        {
            string newNumberOfCode = "";
            int nextNumber = 0;
            if (typeProduct != "SOCCER" && typeProduct != "BASKET")
            {
                throw new InvalidTypeException($"El tipo de producto '{typeProduct}' no es valido");
            }
            List<Logic.Models.Product> listOneType = GetProducts().FindAll(product => product.Type == typeProduct);

            if (listOneType.Count == 0)
            {
                return typeProduct + "-001";
            }
            else
            {
                string lastCode = listOneType[listOneType.Count - 1].Code;
                nextNumber = Int32.Parse(lastCode.Split('-')[1]) + 1;
            }

            if (nextNumber < 10)
            {
                newNumberOfCode = "-00" + nextNumber;
            }
            else if (nextNumber < 100)
            {
                newNumberOfCode = "-0" + nextNumber;
            }
            else
            {
                newNumberOfCode = "-" + nextNumber;
            }
            // implementar excepcion si hay más de 999 
            return typeProduct + newNumberOfCode;
        }

        public Logic.Models.Product PutProduct(Logic.Models.Product product)
        {
            Log.Information($"Logic Layer: Se procesa la solicitud de PUT Product with ID={product.Id.ToString()}");
            DB.Models.Product productFound = _uow.ProductRepository.GetById(product.Id);

            productFound.Name = product.Name;
            if (product.Type != productFound.Type)
            {
                productFound.Code = getNewCode(product.Type);
            }
            productFound.Type = product.Type;
            if(product.Stock >= 0)
            {
                productFound.Stock = product.Stock;
            }
            else
            {
                throw new InvalidStockException($"El stock '{product.Stock}' no es valido");
            }
            _uow.ProductRepository.UpdateProduct(productFound);
            _uow.Save();

            return product;
        }
        public Logic.Models.Product DeleteProduct(Guid productId)
        {
            Log.Information($"Logic Layer: Se procesa la solicitud de DELETE Product with ID={productId.ToString()}");
            DB.Models.Product productFound = _uow.ProductRepository.GetById(productId);

            _uow.ProductRepository.DeleteProduct(productFound);
            _uow.Save();

            return new Logic.Models.Product() { Name = productFound.Name, Type = productFound.Type, Id = productFound.Id, Code = productFound.Code, Stock = productFound.Stock };
        }
    }
}
