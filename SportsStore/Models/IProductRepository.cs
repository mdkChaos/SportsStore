using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public interface IProductRepository
    {
        IQueryable<Product> Products { get; }
        Task SaveProductAsync(Product product);
        Task<Product> DeleteProductAsync(int productID);
    }
}
