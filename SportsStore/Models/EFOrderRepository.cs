using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SportsStore.Models
{
    public class EFOrderRepository : IOrderRepository
    {

        private readonly StoreDbContext _dbContext;

        public EFOrderRepository(StoreDbContext ctx)
        {
            _dbContext = ctx;
        }
        public IQueryable<Order> Orders => _dbContext.Orders.Include(o => o.Lines).ThenInclude(l => l.Product);
        public void SaveOrder(Order order)
        {
            _dbContext.AttachRange(order.Lines.Select(x => x.Product));
            if (order.OrderId == 0)
            {
                _dbContext.Orders.Add(order);
            }
            _dbContext.SaveChanges();
        }
    }
}
