using System.Linq;

namespace SportsStore.Models
{
    public class EFStoreRepository : IStoreRepository
    {
        private readonly StoreDbContext _context;

        public EFStoreRepository(StoreDbContext context)
        {
            _context = context;
        }

        public IQueryable<Product> Products => _context.Products;

        public void CreateProduct(Product p)
        {
            _context.Add(p);
            Save();
        }
        public void DeleteProduct(Product p)
        {
            _context.Remove(p);
            Save();
        }
        public void SaveProduct(Product p)
        {
            Save();
        }

        private void Save()
        {
            _context.SaveChanges();
        }
    }
}
