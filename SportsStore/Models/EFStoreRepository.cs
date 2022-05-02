using System.Linq;

namespace SportsStore.Models
{
    public class EfStoreRepository : IStoreRepository
    {
        private readonly StoreDbContext _context;

        public EfStoreRepository(StoreDbContext context)
        {
            _context = context;
        }

        public IQueryable<Product> Products => _context.Products;
    }
}
