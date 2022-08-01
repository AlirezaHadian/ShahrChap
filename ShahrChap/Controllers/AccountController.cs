using DataLayer;
using DataLayer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Utilities;
using ViewModels;

namespace ShahrChap.Controllers
{
    public class AccountController : Controller
    {
        UnitOfWork db = new UnitOfWork();
        // GET: Account
        [Route("Register")]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Register")]
        public ActionResult Register(RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
                if (!db.UserRepository.Get().Any(u => u.Email == register.EmailOrPhone.Trim().ToLower() || u.Phone == register.EmailOrPhone.Trim()))
                {
                    User user = new User()
                    {
                        UserName = register.UserName,
                        RoleID = 1,
                        Password = FormsAuthentication.HashPasswordForStoringInConfigFile(register.Password, "MD5"),
                        RegisterDate = DateTime.Now,
                    };
                    if (register.EmailOrPhone.Contains("@"))
                    {
                        user.Email = register.EmailOrPhone;
                        user.ActiveCode = Guid.NewGuid().ToString();
                        user.IsEmailActive = false;
                        db.UserRepository.Insert(user);
                        db.Save();
                        string body = PartialToStringClass.RenderPartialView("ManageEmails", "ActivationEmail", user);
                        SendEmail.Send(user.Email, "فعالسازی حساب کاربری", body);
                        return View("SuccessEmailRegister", user);
                    }
                    else
                    {
                        Random random = new Random();
                        string DigitCode = random.Next(10000, 99999).ToString();
                        user.Phone = register.EmailOrPhone;
                        user.IsPhoneActive = false;
                        user.DigitCode = DigitCode;
                        //SendSMS.SendWithPattern(register.EmailOrPhone, register.UserName, DigitCode);
                        db.UserRepository.Insert(user);
                        db.Save();
                        TempData["OTP"] = DigitCode;
                        TempData["ExpireTime"] = DateTime.Now;
                        Session["PhoneNumber"] = register.EmailOrPhone;
                        ViewBag.Phone = register.EmailOrPhone;
                        return RedirectToAction("VerifyPhone", "Account");
                    }
                }
                else
                {
                    ModelState.AddModelError("EmailOrPhone", "کاربری با این مشخصات در سایت وجود دارد!");
                }
            }
            return View(register);
        }

        public ActionResult ActiveEmail(string id)
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
        public ActionResult VerifyPhone()
        {
            ViewBag.Phone = "0" + Convert.ToInt64(Session["PhoneNumber"]);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyPhone(OTPViewModel DigitCode)
        {
            string SessionOTP = TempData["OTP"].ToString();
            var user = db.UserRepository.Get().SingleOrDefault(u => u.DigitCode == SessionOTP);

            if (DigitCode.OTP == SessionOTP)
            {
                if ((DateTime.Now - Convert.ToDateTime(TempData["ExpireTime"])).TotalSeconds < 120)
                {
                    user.IsPhoneActive = true;
                    db.UserRepository.Update(user);
                    db.Save();
                    return View("SuccessPhoneRegister", user);
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
            return View(DigitCode);
        }


        [Route("Login")]
        public ActionResult Login()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("Login")]
        public ActionResult Login(LoginViewModel login)
        {
            return View();
        }

        public ActionResult ResendOTP()
        {
            var phone = Convert.ToString(Session["PhoneNumber"]);
            var user = db.UserRepository.Get().SingleOrDefault(u => u.Phone == phone);
            Random random = new Random();
            string OTP = random.Next(10000, 99999).ToString();
            user.DigitCode = OTP;
            SendSMS.SendWithPattern(phone, user.UserName, OTP);
            db.UserRepository.Update(user);
            db.Save();
            TempData["OTP"] = OTP;
            TempData["ExpireTime"] = DateTime.Now;
            return RedirectToAction("VerifyPhone", "Account");
        }
    }
}