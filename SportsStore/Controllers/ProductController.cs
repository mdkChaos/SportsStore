using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using System.Linq;

namespace SportsStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _repository;
        public int pageSize = 4;
        public ProductController(IProductRepository repository)
        {
            _repository = repository;
        }

        public ViewResult List(int productPage = 1) => View(
            _repository.Products
            .OrderBy(p => p.ProductID)
            .Skip((productPage - 1) * pageSize)
            .Take(pageSize));
    }
}
