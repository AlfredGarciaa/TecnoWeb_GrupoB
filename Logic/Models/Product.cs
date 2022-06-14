using System;

namespace Logic.Models
{
    //CARACTERISTICAS DE LOS PRODUCTOS
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
    }
}