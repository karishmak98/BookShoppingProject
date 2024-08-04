using BookShoppingProject.DataAccess.Repository.IRepository;
using BookShoppingProject.Models;
using BookShoppingProject.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShoppingProject_11.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class CategoryController : Controller
    {
        
        private readonly IUnitofWork _unitofwork;
        public CategoryController(IUnitofWork unitofwork)
        {
            _unitofwork = unitofwork;        
        }
        
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            Category category = new Category();
            //create
            if (id == null)
                return View(category);
            //edit
            var categoryInDb = _unitofwork.Category.Get(id.GetValueOrDefault());
            if (categoryInDb == null)
                return NotFound();
            return View(categoryInDb);
            

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (category == null)
                return NotFound();
            if (!ModelState.IsValid) return View(category);
            if (category.Id == 0)
                _unitofwork.Category.Add(category);
            else
                _unitofwork.Category.Update(category);
            _unitofwork.Save();
            return RedirectToAction("Index");

            
        }
        
         #region APIs
        [HttpGet]
        
        public IActionResult GetAll()
        {
            var categoryList = _unitofwork.Category.GetAll();
            return Json(new { data = categoryList });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var categoryInDb = _unitofwork.Category.Get( id);
            if (categoryInDb == null)
                return Json(new { success = false, message = "Error while delete data!!!" });
            _unitofwork.Category.Remove(categoryInDb);
            _unitofwork.Save();
            return Json(new { success = true, message = "Data Successfully deleted!!!" });


        }

        #endregion

    }
}
