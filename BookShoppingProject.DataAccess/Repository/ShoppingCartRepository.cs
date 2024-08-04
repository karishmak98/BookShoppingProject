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
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _context;
        public ShoppingCartRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
        public void Update(ShoppingCart shoppingCart)
        {
            _context.Update(shoppingCart);
        }
    }
}
