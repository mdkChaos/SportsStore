using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public class EFProductRepository : IProductRepository
    {
        private ApplicationDbContext _context;
        public EFProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public IQueryable<Product> Products => _context.Products;

        public async Task SaveProductAsync(Product product)
        {
            if (product.ProductID == 0)
            {
                await _context.Products.AddAsync(product);
            }
            else
            {
                Product dbEntry = _context.Products
                    .FirstOrDefault(p => p.ProductID == product.ProductID);
                if (dbEntry != null)
                {
                    dbEntry.Name = product.Name;
                    dbEntry.Description = product.Description;
                    dbEntry.Price = product.Price;
                    dbEntry.Category = product.Category;
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task<Product> DeleteProductAsync(int productID)
        {
            Product dbEntry = _context.Products
                .FirstOrDefault(p => p.ProductID == productID);
            if (dbEntry != null)
            {
                _context.Products.Remove(dbEntry);
                await _context.SaveChangesAsync();
            }
            return dbEntry;
        }
    }
}
