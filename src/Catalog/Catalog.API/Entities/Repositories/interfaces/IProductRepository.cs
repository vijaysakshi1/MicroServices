using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.API.Entities.Repositories.interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProducts();
        Task<Product> GetProduct(string id);

        Task<IEnumerable<Product>> GetProductByName(string name);
        Task<IEnumerable<Product>> GetProductCategory(string categoryName);

        Task Create(Product product);
        Task<bool> Update(Product product);
        Task<bool> Delete(string id);


    }
}
