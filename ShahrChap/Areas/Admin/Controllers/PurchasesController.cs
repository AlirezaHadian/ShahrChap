using DataLayer;
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
            var list = db.FactorsRepository.Get().OrderByDescending(f => f.Date);
            return View(list);
        }
        public ActionResult UserAddress(int id)
        {
            var addressDetail = db.User_AddressRepository.GetById(id);
            return View(addressDetail);
        }

        public ActionResult FactorDetail(int id)
        {
            List<Factor_Details> factorDetail = db.Factor_DetailsRepository.Get().Where(f=> f.FactorID == id).ToList();
            return View(factorDetail);
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


        public ActionResult Done(int orderId)
        {
            Order_Details order = db.Order_DetailsRepository.GetById(orderId);
            if(order.IsDone == false)
            {
                order.IsDone = true;
            }
            else
            {
                order.IsDone = false;
            }
            db.Order_DetailsRepository.Update(order);
            db.Save();
            return RedirectToAction("Orders", new { id = orderId });
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