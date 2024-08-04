using BookShoppingProject.DataAccess.Data;
using BookShoppingProject.DataAccess.Repository.IRepository;
using BookShoppingProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingProject.DataAccess.Repository
{
    public class OrderDetailRepository : Repository<OrderDetails>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderDetailRepository(ApplicationDbContext context):base(context)       
        {
            _context = context;
        }
        public void Update(OrderDetails orderDetails)
        {
            _context.Update(orderDetails);
        }
    }
}
