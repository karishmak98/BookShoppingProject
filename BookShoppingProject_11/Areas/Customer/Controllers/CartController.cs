using BookShoppingProject.DataAccess.Repository.IRepository;
using BookShoppingProject.Models;
using BookShoppingProject.Models.ViewModels;
using BookShoppingProject.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookShoppingProject_11.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitofWork _unitofWork;
        public CartController(IUnitofWork unitofWork)
        {
            _unitofWork = unitofWork;
        }
        [BindProperty]
        public ShoppinCartVM ShoppingCartVM { get; set; }
        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim==null)
            {
                ShoppingCartVM = new ShoppinCartVM()
                {
                    ListCart=new List<ShoppingCart>()
                };
                return View(ShoppingCartVM);
            }

            ShoppingCartVM = new ShoppinCartVM()
            {
                OrderHeader = new OrderHeader(),
                ListCart = _unitofWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product")
                
            };
            ShoppingCartVM.OrderHeader.OrderTotal = 0;
           // ShoppingCartVM.OrderHeader.ApplicationUser = _unitofWork.ApplicationUser.FirstOrDefault(u=>u.Id==claim.Value,includeProperties:"Company");
            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, 
                    list.Product.Price50, list.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Count * list.Price);
                if(list.Product.Discription.Length>100)
                {
                    list.Product.Discription = list.Product.Discription.Substring(0, 99) + "....";
                }
                
            }
            return View(ShoppingCartVM);
        }

        public IActionResult Plus(int cartId)
        {
            var cart = _unitofWork.ShoppingCart.FirstOrDefault(sc => sc.Id == cartId, includeProperties: "Product");
            cart.Count += 1;
            _unitofWork.Save();
            return RedirectToAction(nameof(Index));

        }

        public IActionResult Minus(int cartId)
        {
            var cart = _unitofWork.ShoppingCart.FirstOrDefault(sc => sc.Id == cartId, includeProperties: "Product");
            if (cart.Count == 0)
            {
                _unitofWork.ShoppingCart.Remove(cart);
                _unitofWork.Save();
            }
            cart.Count -= 1;
            _unitofWork.Save();
            return RedirectToAction(nameof(Index));

        }

        public IActionResult Delete(int cartId)
        {
            var cart = _unitofWork.ShoppingCart.FirstOrDefault(sc => sc.Id == cartId, includeProperties: "Product");
            _unitofWork.ShoppingCart.Remove(cart);
            _unitofWork.Save();
            return RedirectToAction("Index");
        }
        public IActionResult Summary()
        {
            var claimIdentity = (ClaimsIdentity)(User.Identity);
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM = new ShoppinCartVM()
            {
                OrderHeader = new OrderHeader(),
                ListCart = _unitofWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,includeProperties:"Product")
            };
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitofWork.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value, includeProperties: "Company");
            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count,list.Product.Price,list.Product.Price50,list.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Count * list.Price);
                list.Product.Discription = SD.ConvertToRawHtml(list.Product.Discription);
            }
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
         
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]

        public IActionResult SummaryPost(string stripeToken)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitofWork.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value, includeProperties: "Company");
            ShoppingCartVM.ListCart = _unitofWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value, includeProperties: "Product");
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusPending;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
            _unitofWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitofWork.Save();
            foreach(var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
                OrderDetails orderDetails = new OrderDetails()
                {
                    ProductId = list.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price=list.Price,
                    Count=list.Count
                };
                ShoppingCartVM.OrderHeader.OrderTotal += (orderDetails.Price * orderDetails.Count);
                _unitofWork.OrderDetail.Add(orderDetails);
                _unitofWork.Save();
            }
            _unitofWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
            _unitofWork.Save();
            HttpContext.Session.SetInt32(SD.Ss_session, 0);
            
            #region Stripe Payment
            if(stripeToken==null)
            {
                ShoppingCartVM.OrderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;
            }
            else
            {
                //Payment Process
                var options = new ChargeCreateOptions()
                {
                    Amount=Convert.ToInt32(ShoppingCartVM.OrderHeader.OrderTotal),
                    Currency="inr",
                    Description="Order Id : " + ShoppingCartVM.OrderHeader.Id,
                    Source=stripeToken
                };
                //Payment
                var service = new ChargeService();
                Charge charge = service.Create(options);
                if (charge.BalanceTransactionId == null)
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
                ShoppingCartVM.OrderHeader.TransactionId = charge.BalanceTransactionId;
                if(charge.Status.ToLower()=="succeeded")
                {
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;
                    ShoppingCartVM.OrderHeader.PaymentDate = DateTime.Now;
                }
                _unitofWork.Save();
            }
            #endregion
           
            return RedirectToAction("OrderConfirmation","Cart",new { id=ShoppingCartVM.OrderHeader.Id});
        }

        public IActionResult OrderConfirmation(int id)
        {
            
            return View(id);
        }
            



    }
}
