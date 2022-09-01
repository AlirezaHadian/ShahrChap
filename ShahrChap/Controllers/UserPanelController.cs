using DataLayer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ViewModels;
using Utilities;
using DataLayer;

namespace ShahrChap.Controllers
{
    public class UserPanelController : Controller
    {
        UnitOfWork db = new UnitOfWork();
        // GET: UserPanel
        [Authorize]
        public ActionResult Index()
        {
            var username = User.Identity.Name;
            var user = db.UserRepository.Get().SingleOrDefault(u => u.UserName == username);

            if (user != null)
            {
                return View("Index", user);
            }
            return HttpNotFound();
        }
        public ActionResult Address(int id)
        {
            ViewBag.Address = db.UserAddressRepository.Get().Where(u => u.UserID == id).ToList();
            ViewBag.UserId = id;
            return PartialView();
        }


        public ActionResult EnableEmail(int id)
        {
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        public ActionResult EnableEmail(EnableEmailViewModel enableEmail, int id)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                var user = db.UserRepository.Get().SingleOrDefault(u => u.UserID == id);
                if (user != null)
                {
                    user.Email = enableEmail.Email;
                    user.ActiveCode = Guid.NewGuid().ToString();
                    user.IsEmailActive = false;
                    db.UserRepository.Update(user);
                    db.Save();
                    string body = PartialToStringClass.RenderPartialView("ManageEmails", "EnableEmail", user);
                    SendEmail.Send(user.Email, "فعالسازی ایمیل", body);
                    return View("SuccessEnableEmail", user);
                }
                return HttpNotFound();
            }
            return PartialView("EnableEmail", enableEmail);
        }
        public ActionResult EnablePhone(int id)
        {
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EnablePhone(EnablePhoneViewModel enablePhone, int id)
        {
            if (ModelState.IsValid)
            {
                var user = db.UserRepository.Get().SingleOrDefault(u => u.UserID == id);
                if (user != null)
                {
                    user.Phone = enablePhone.Phone;
                    user.IsPhoneActive = false;
                    Random random = new Random();
                    string DigitCode = random.Next(10000, 99999).ToString();
                    user.Phone = enablePhone.Phone;
                    SendSMS.SendWithPattern(enablePhone.Phone, user.UserName, DigitCode);
                    db.UserRepository.Update(user);
                    db.Save();
                    Session["OTP"] = DigitCode;
                    Session["ExpireTime"] = DateTime.Now;
                    Session["PhoneNumber"] = user.Phone;
                    return RedirectToAction("VerifyPhone", "UserPanel");
                }
                return HttpNotFound();
            }
            return PartialView("EnablePhone", enablePhone);
        }
        public ActionResult ActiveUserPanelEmail(string id)
        {
            var user = db.UserRepository.Get().SingleOrDefault(u => u.ActiveCode == id);
            if (user == null)
            {
                return HttpNotFound();
            }
            user.IsEmailActive = true;
            user.ActiveCode = Guid.NewGuid().ToString();
            db.UserRepository.Update(user);
            db.Save();
            ViewBag.Username = user.UserName;
            return View();
        }
        public ActionResult VerifyUserPanelPhone()
        {
            ViewBag.Phone = Convert.ToString(Session["PhoneNumber"]);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyUserPanelPhone(OTPViewModel DigitCode)
        {
            string OTP = Session["OTP"].ToString();
            string Phone = Session["PhoneNumber"].ToString();
            var user = db.UserRepository.Get().SingleOrDefault(u => u.Phone == Phone);
            ViewBag.Phone = Phone;
            if (DigitCode.OTP == OTP)
            {
                if ((DateTime.Now - Convert.ToDateTime(Session["ExpireTime"])).TotalSeconds < 120)
                {
                    user.IsPhoneActive = true;
                    ViewBag.UserName = user.UserName;
                    db.UserRepository.Update(user);
                    db.Save();
                    return View("SuccessEnablePhone", user);
                }
                else
                {
                    ModelState.AddModelError("OTP", "کد فعالسازی وارد شده منقضی شده است");
                }
            }
            else
            {
                ModelState.AddModelError("OTP", "کد فعالسازی وارد شده معتبر نمی باشد");
            }
            return View();
        }
        public ActionResult ChangePassword(string id)
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel changePassword,string id)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                var user = db.UserRepository.Get().Single(u => u.UserName == User.Identity.Name);
                string oldPasswordHash = FormsAuthentication.HashPasswordForStoringInConfigFile(changePassword.OldPassword, "MD5");
                if(user.Password == oldPasswordHash)
                {
                    string hashNewPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(changePassword.Password, "MD5");
                    user.Password = hashNewPassword;
                    db.Save();
                    ViewBag.Success = true;
                }
                else
                {
                    ModelState.AddModelError("OldPassword", "کلمه عبور فعلی درست نمی باشد");
                }
            }
            return View();
        }
        public ActionResult ResendOTP()
        {
            ResendCode();
            return RedirectToAction("VerifyPhone", "Account");
        }

        //Address
        public ActionResult AddAddress(int id)
        {
            List<province> provinces = db.ProvinceRepository.Get().ToList();
            ViewBag.ProvinceList = new SelectList(provinces, "provinceId", "provinceName");
            return View();
        }
        [HttpPost]
        public ActionResult AddAddress(AddAddressViewModel address)
        {
            var username = User.Identity.Name;
            var user = db.UserRepository.Get().SingleOrDefault(u => u.UserName == username);
            if (ModelState.IsValid)
            {
                UserAddress userAddress = new UserAddress()
                {
                    FullName = address.FullName,
                    FullAddress = address.FullAddress,
                    PosteCode = address.PostCode,
                    ProvinceID = address.Province,
                    CityID = address.City,
                    UserID = user.UserID
                };
                db.UserAddressRepository.Insert(userAddress);
                db.Save();
                return RedirectToAction("Index");
            }
            return View();
        }
        public void DeleteAddress(int id)
        {
            var address = db.UserAddressRepository.GetById(id);
            db.UserAddressRepository.Delete(address);
            db.Save();
        }
        public void ResendCode()
        {
            var phone = Convert.ToString(Session["PhoneNumber"]);
            var user = db.UserRepository.Get().SingleOrDefault(u => u.Phone == phone);
            Random random = new Random();
            string OTP = random.Next(10000, 99999).ToString();
            SendSMS.SendWithPattern(phone, user.UserName, OTP);
            Session["OTP"] = OTP;
            Session["ExpireTime"] = DateTime.Now;
        }
        public ActionResult GetCityList(int provinceId)
        {
            List<city> cities = db.CityRepository.Get().Where(c => c.province_id == provinceId).ToList();
            ViewBag.CityList = new SelectList(cities, "cityId", "cityName");
            return PartialView("DisplayCities");
        }
    }
}