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
    [Authorize(Roles =SD.Role_Admin+","+SD.Role_Employee)]
    public class CompanyController : Controller
    {
        private readonly IUnitofWork _unitofWork;
        public CompanyController(IUnitofWork unitofWork )
        {
            _unitofWork = unitofWork;

        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            Company company = new Company();
            if (id == null)
                return View(company);
            company = _unitofWork.Company.Get(id.GetValueOrDefault());
            return View(company);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (company == null)
                return NotFound();
            if (!ModelState.IsValid) return View(company);
            if (company.Id == 0)
                _unitofWork.Company.Add(company);
            else
                _unitofWork.Company.Update(company);
            _unitofWork.Save();
            return RedirectToAction(nameof(Index));
        }






        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _unitofWork.Company.GetAll() });
        
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var companyInDb = _unitofWork.Company.Get(id);
            if (companyInDb == null)
                return Json(new { success = false, message = "Error while delete data!!!" });
            _unitofWork.Company.Remove(companyInDb);
            _unitofWork.Save();
            return Json(new { success = true, message = "Data deleted successfully!!!" });
        }
        #endregion
    }
}
