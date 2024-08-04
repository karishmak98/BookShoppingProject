using BookShoppingProject.DataAccess.Repository.IRepository;
using BookShoppingProject.Models;
using BookShoppingProject.Models.ViewModels;
using BookShoppingProject.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookShoppingProject_11.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitofWork _unitofWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitofWork unitofWork,IWebHostEnvironment webHostEnvironment)
        {
            _unitofWork = unitofWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(), //for create
                CategoryList = _unitofWork.Category.GetAll().Select(cl => new SelectListItem()
                {
                    Text = cl.Name,
                    Value = cl.Id.ToString()
                }),
                CoverTypeList = _unitofWork.CoverType.GetAll().Select(ct => new SelectListItem()
                {
                    Text = ct.Name,
                    Value = ct.Id.ToString()
                })

            };
            //Create
            if (id == null)
                return View(productVM);  //productVM have product,category,covertype tables.
            //Edit
            productVM.Product = _unitofWork.Product.Get(id.GetValueOrDefault()); //find product from db.
            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var webrootpath = _webHostEnvironment.WebRootPath;        //for image path upto wwwroot client path.//
                var files = HttpContext.Request.Form.Files;                 //To access file upload control,it have files array.
                if (files.Count > 0)                                      //if file choose ki file upload control se//
                {
                    var fileName = Guid.NewGuid().ToString();                 //file name cant be duplicate.Generate random file name.
                    var uploads = Path.Combine(webrootpath, @"images\products");              //image save in products folder(in server)
                    var extension = Path.GetExtension(files[0].FileName);               //files[0] --for one file from file upload control.To take extension of one file.
                    if (productVM.Product.Id != 0)                                    //edit code 
                    {
                        var imageExists = _unitofWork.Product.       //it have old image path in DB.
                            Get(productVM.Product.Id).ImageURL;
                        productVM.Product.ImageURL = imageExists;
                    }
                    if (productVM.Product.ImageURL != null)    //means image is not null .image change while update
                    {
                        var imagePath = Path.Combine
                            (webrootpath, productVM.Product.ImageURL.TrimStart('\\'));//image path from DB
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);//Delete imagepath from product folder and from DB also.
                        }

                    }
                    using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension),
                        FileMode.Create))                           //to access image it actually save file from client to product folder.

                    {
                        files[0].CopyTo(fileStream);                 //copy image from client and paste in product folder.
                        //but not saving in DB.
                    }
                    //To save in DB
                    productVM.Product.ImageURL = @"\images\products\" + fileName + extension;
                }
                else                                                   //if file not select
                {
                    if (productVM.Product.Id != 0)
                    {
                        var imageExists = _unitofWork.Product.             //image not change while update.
                                Get(productVM.Product.Id).ImageURL;        // if DB have image it will pick up that image,
                        productVM.Product.ImageURL = imageExists;         //otherwise update imageurl=null.
                    }

                }
                //Save code
                if (productVM.Product.Id == 0)
                    _unitofWork.Product.Add(productVM.Product);
                else
                    _unitofWork.Product.Update(productVM.Product);
                _unitofWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                productVM = new ProductVM()   //it have product details.
                {
                    CategoryList = _unitofWork.Category.GetAll().Select(cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.Id.ToString()
                    }),
                    CoverTypeList = _unitofWork.CoverType.GetAll().Select(ct => new SelectListItem()
                    {
                        Text = ct.Name,
                        Value = ct.Id.ToString()
                    }),

                };
                //for edit textbox must have data.
                if(productVM.Product.Id!=0)
                {
                    productVM.Product = _unitofWork.Product.Get(productVM.Product.Id);

                } 
                return View(productVM);
            }
            
                                                                                                                                                                                                                                

            

        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitofWork.Product.
                GetAll(includeProperties:"Category,CoverType");
            return Json(new { data = productList });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var productInDb = _unitofWork.Product.Get(id);
            if (productInDb == null)  //if id is wrong.
                return Json(new { success = false, message = "Error while delete data!!!" });
            if(productInDb.ImageURL!=" ")  //if image is there
            {
                var webrootpath = _webHostEnvironment.WebRootPath;    //path upto wwwroot.
                var imagePath = Path.Combine(webrootpath, productInDb.ImageURL.TrimStart('\\'));     //path upto product folder.
                if(System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);    //delete image from DB and product folder.
                }
            }
            _unitofWork.Product.Remove(productInDb);  //rest data will delete
            _unitofWork.Save();
            return Json(new { success = true, message = "data deleted successfully!!!" });


        }
        #endregion
    }
}
