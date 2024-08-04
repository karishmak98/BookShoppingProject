using BookShoppingProject.DataAccess.Repository.IRepository;
using BookShoppingProject.Models;
using BookShoppingProject.Models.ViewModels;
using BookShoppingProject.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookShoppingProject_11.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitofWork _unitofWork;

        public HomeController(ILogger<HomeController> logger, IUnitofWork unitofWork)
        {
            _logger = logger;
            _unitofWork = unitofWork;
        }

        public IActionResult Index()
        {
            var productList = _unitofWork.Product.GetAll(includeProperties:"Category,CoverType");
            //session
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim!=null)
            {
                var count = _unitofWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_session, count);
            }
            return View(productList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Details(int id)
        {
            var productInDb = _unitofWork.Product.FirstOrDefault(p => p.Id == id,includeProperties: "Category,CoverType");
            if (productInDb == null)
                return NotFound();
            var shoppingCart = new ShoppingCart()
            {
                Product = productInDb,
                ProductId=productInDb.Id

            };
            return View(shoppingCart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCartobj)
        {
            if(ModelState.IsValid)
            {
                shoppingCartobj.Id = 0;   
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                shoppingCartobj.ApplicationUserId = claim.Value;
                var shoppingCartFromDb = _unitofWork.ShoppingCart.FirstOrDefault(u => u.ApplicationUserId == claim.Value 
                && u.ProductId == shoppingCartobj.ProductId);
                if(shoppingCartFromDb==null)
                {
                    //add to cart
                    _unitofWork.ShoppingCart.Add(shoppingCartobj);
                }
                else
                {
                    //update to cart
                    shoppingCartFromDb.Count += shoppingCartobj.Count;
                }
                _unitofWork.Save();

                //session
                var count = _unitofWork.ShoppingCart.GetAll(u=>u.ApplicationUserId==claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_session,count);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var productInDb = _unitofWork.Product.FirstOrDefault(p => p.Id == shoppingCartobj.ProductId,
                    includeProperties: "Category,CoverType");
                
                var shoppingCart = new ShoppingCart()
                {
                    Product = productInDb,
                    ProductId = productInDb.Id

                };
                return View(shoppingCart);
            }
        }
    }
}
