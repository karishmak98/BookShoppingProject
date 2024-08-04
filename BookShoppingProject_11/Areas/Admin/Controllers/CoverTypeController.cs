using BookShoppingProject.DataAccess.Repository.IRepository;
using BookShoppingProject.Models;
using BookShoppingProject.Utility;
using Dapper;
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
    public class CoverTypeController : Controller
    {
        private readonly IUnitofWork _unitofWork;
        public CoverTypeController(IUnitofWork unitofWork )
        {
            _unitofWork = unitofWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();
            if (id == null)
                return View(coverType);
            var param = new DynamicParameters();
            param.Add("@Id", id.GetValueOrDefault());
            coverType = _unitofWork.SP_Call.OneRecord<CoverType>(SD.Proc_GetCoverType, param);
           // coverType=_unitofWork.CoverType.Get(id.GetValueOrDefault());
            if (coverType == null)
                return NotFound();
            return View(coverType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (coverType == null)
                return NotFound();
            if (!ModelState.IsValid) return View(coverType);
            
            var param = new DynamicParameters();
            param.Add("@Name", coverType.Name);
            if (coverType.Id == 0)
                //_unitofWork.CoverType.Add(coverType);
                _unitofWork.SP_Call.Execute(SD.Proc_CoverType_Create, param);

            else
            {
                param.Add("@Id", coverType.Id);
                _unitofWork.SP_Call.Execute(SD.Proc_CoverType_Update, param);
            }
            //_unitofWork.CoverType.Update(coverType);
            //_unitofWork.Save();
            return RedirectToAction("Index");
        }

        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            //var coverType = _unitofWork.CoverType.GetAll();  //without SP
            var coverType = _unitofWork.SP_Call.List<CoverType>(SD.Proc_GetCoverTypes);
            return Json(new { data = coverType });
        }
        [HttpDelete]
         public IActionResult Delete(int id)
        {
            var param = new DynamicParameters();
            param.Add("@Id", id);
            var coverTypeInDb = _unitofWork.SP_Call.OneRecord<CoverType>(SD.Proc_GetCoverType, param);
            //var coverTypeInDb = _unitofWork.CoverType.Get(id);
            if (coverTypeInDb == null)
                return Json(new { success = false, message = "Error while delete data!!!" });
            
            _unitofWork.SP_Call.Execute(SD.Proc_CoverType_Delete, param); //call SP
            //_unitofWork.CoverType.Remove(coverTypeInDb);//without SP//
            //_unitofWork.Save();
            return Json(new { success = true, message = "Data Successfully deleted!!!" });
        }
        #endregion
    }
}
