using DataLayer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShahrChap.Areas.Admin.Controllers
{
    public class PurchasesController : Controller
    {
        UnitOfWork db = new UnitOfWork();
        // GET: Admin/Purchases
        public ActionResult Factors()
        {
            var list = db.Factor_DetailsRepository.Get().OrderByDescending(f => f.Factors.Date);
            return View(list);
        }

        public ActionResult UserAddress(int id)
        {
            int addressId = (int)db.FactorsRepository.GetById(id).AddressID;
            var addressDetail = db.User_AddressRepository.GetById(addressId);
            return View(addressDetail);
        }
        public ActionResult Orders()
        {
            var orderlist = db.Order_DetailsRepository.Get().OrderByDescending(f => f.OrderDate);
            return View(orderlist);
        }

        public ActionResult OrderDetail(int id)
        {
            var detail = db.Order_DetailsRepository.GetById(id);
            ViewBag.OrderFiles = db.Order_FilesRepository.Get().Where(f => f.OT_ID == id).ToList();
            return View(detail);
        }


        public ActionResult DownloadFile(int id)
        {
            string fileName = db.Order_FilesRepository.GetById(id).FileName;
            string path = AppDomain.CurrentDomain.BaseDirectory + "/Images/OrderFiles/";
            byte[] fileBytes = System.IO.File.ReadAllBytes(path + fileName);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}