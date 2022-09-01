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
                        SendSMS.SendWithPattern(register.EmailOrPhone, register.UserName, DigitCode);
                        db.UserRepository.Insert(user);
                        db.Save();
                        Session["OTP"] = DigitCode;
                        Session["ExpireTime"] = DateTime.Now;
                        Session["PhoneNumber"] = user.Phone;
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
            ViewBag.Phone = Convert.ToString(Session["PhoneNumber"]);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyPhone(OTPViewModel DigitCode)
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
        public ActionResult Login(LoginViewModel login, string returnUrl = "/")
        {
            if (ModelState.IsValid)
            {
                string HashPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(login.Password, "MD5");
                var user = db.UserRepository.Get().SingleOrDefault(u => u.Email == login.EmailOrPhone || u.Phone == login.EmailOrPhone);
                if (user != null && user.Password==HashPassword)
                {
                    if (user.IsEmailActive || user.IsPhoneActive)
                    {
                        FormsAuthentication.SetAuthCookie(user.UserName, login.RemmemberMe);
                        return Redirect("/");
                    }
                    else
                    {
                        ModelState.AddModelError("EmailOrPhone", "حساب کاربری شما فعال نشده است.");
                    }
                }
                else
                {
                    ModelState.AddModelError("EmailOrPhone", "کاربری با مشخصات وارد شده یافت نشد.");
                }
            }
            return View();
        }

        public ActionResult ResendOTP(string id = "")
        {
            ResendCode();
            if (id == "")
            {
                return RedirectToAction("VerifyPhone", "Account");
            }
            else if (id == "ForgotPass")
            {
                return RedirectToAction("ForgotPasswordWithPhone", "Account");
            }
            return HttpNotFound();
        }
        //Enter phone or email to send verification email or sms
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordViewModel forgotPassword)
        {
            if (ModelState.IsValid)
            {
                var user = db.UserRepository.Get().SingleOrDefault(u => u.Email == forgotPassword.EmailOrPhone || u.Phone == forgotPassword.EmailOrPhone);
                if (user != null)
                {
                    if (user.IsEmailActive || user.IsPhoneActive)
                    {
                        if (forgotPassword.EmailOrPhone.Contains("@"))
                        {
                            string body = PartialToStringClass.RenderPartialView("ManageEmails", "RecoveryPassword", user);
                            SendEmail.Send(user.Email, "بازیابی کلمه عبور", body);
                            return View("SuccessForgotPassword", user);
                        }
                        else
                        {
                            Random random = new Random();
                            string DigitCode = random.Next(10000, 99999).ToString();
                            SendSMS.SendWithPattern(user.Phone, user.UserName, DigitCode);
                            Session["OTP"] = DigitCode;
                            Session["ExpireTime"] = DateTime.Now;
                            Session["PhoneNumber"] = user.Phone;
                            return RedirectToAction("ForgotPasswordWithPhone", "Account");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("EmailOrPhone", "حساب کاربری شما فعال نشده است.");
                    }
                }
                else
                {
                    ModelState.AddModelError("EmailOrPhone", "کاربری با مشخصات وارد شده یافت نشد.");
                }
            }
            return View();
        }

        //Validate phone number to redirect to channge Password
        public ActionResult ForgotPasswordWithPhone()
        {
            ViewBag.Phone = Convert.ToString(Session["PhoneNumber"]);
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPasswordWithPhone(OTPViewModel validatePhone)
        {
            string ValidateCode = Session["OTP"].ToString();
            string Phone = Session["PhoneNumber"].ToString();
            var user = db.UserRepository.Get().SingleOrDefault(u => u.Phone == Phone);

            if (validatePhone.OTP == ValidateCode)
            {
                if ((DateTime.Now - Convert.ToDateTime(Session["ExpireTime"])).TotalSeconds < 120)
                {
                    return RedirectToAction("RecoveryPassword", "Account", new { id = user.UserID });
                }
                else
                {
                    ModelState.AddModelError("OTP", "کد اعتبارسنجی وارد شده منقضی شده است");
                }
            }
            else
            {
                ModelState.AddModelError("OTP", "کد اعتبارسنجی وارد شده معتبر نمی باشد");
            }
            return View();
        }

        //After validate user, Change the password
        public ActionResult RecoveryPassword(string id)
        {
            return View();
        }
        [HttpPost]
        public ActionResult RecoveryPassword(RecoveryPasswordViewModel recoveryPassword, string id)
        {
            if (ModelState.IsValid)
            {
                var user = db.UserRepository.Get().SingleOrDefault(u => u.ActiveCode == id || u.UserID.ToString() == id);
                if (user != null)
                {
                    if (id == user.ActiveCode)
                    {
                        user.ActiveCode = Guid.NewGuid().ToString();
                    }
                    user.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(recoveryPassword.Password, "MD5");
                    db.UserRepository.Update(user);
                    db.Save();
                    return Redirect("/Login?recovery=true");
                }
                else
                {
                    return HttpNotFound();
                }
            }
            return View();
        }
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return Redirect("/");
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
    }
}