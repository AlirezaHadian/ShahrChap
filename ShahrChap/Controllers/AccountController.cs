using DataLayer;
using DataLayer.Context;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
        public async Task<ActionResult> Register(RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
                var isCaptchaValid = await IsCaptchaValid(register.GoogleCaptchaToken, "register");
                if (isCaptchaValid)
                {
                    if (!db.UserRepository.Get().Any(u => u.UserName == register.UserName.Trim().ToLower()))
                    {
                        if (!db.UserRepository.Get().Any(u => u.Email == register.EmailOrPhone.Trim().ToLower() || u.Phone == register.EmailOrPhone.Trim()))
                        {
                            if (register.EmailOrPhone.Contains("@"))
                            {
                                Users user = new Users()
                                {
                                    UserName = register.UserName,
                                    RoleID = 1,
                                    Password = FormsAuthentication.HashPasswordForStoringInConfigFile(register.Password, "MD5"),
                                    RegisterDate = DateTime.Now,
                                    Email = register.EmailOrPhone,
                                    IsPhoneActive = false,
                                    IsEmailActive = false
                                };
                                db.UserRepository.Insert(user);
                                db.Save();
                                string body = PartialToStringClass.RenderPartialView("ManageEmails", "ActivationEmail", user);
                                SendEmail.Send(user.Email, "فعالسازی حساب کاربری", body);
                                return View("SuccessEmailRegister", user);
                            }
                            else
                            {
                                Users user = new Users()
                                {
                                    UserName = register.UserName,
                                    RoleID = 1,
                                    Password = FormsAuthentication.HashPasswordForStoringInConfigFile(register.Password, "MD5"),
                                    RegisterDate = DateTime.Now,
                                    Phone = register.EmailOrPhone,
                                    IsPhoneActive = false,
                                    IsEmailActive = false
                                };
                                Random random = new Random();
                                string DigitCode = random.Next(10000, 99999).ToString();
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
                    else
                    {
                        ModelState.AddModelError("UserName", "کاربری با این مشخصات در سایت وجود دارد!");
                    }
                }
                else
                {
                    ModelState.AddModelError("EmailOrPhone", "کپچا نامعتبر می باشد");
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
        public async Task<ActionResult> Login(LoginViewModel login, string returnUrl = "/")
        {
            if (ModelState.IsValid)
            {
                var isCaptchaValid = await IsCaptchaValid(login.GoogleCaptchaToken, "login");
                if (isCaptchaValid == true)
                {
                    string HashPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(login.Password, "MD5");
                    var user = db.UserRepository.Get().SingleOrDefault(u => u.Email == login.EmailOrPhone || u.Phone == login.EmailOrPhone);
                    if (user != null && user.Password == HashPassword)
                    {
                        if (user.IsEmailActive || user.IsPhoneActive)
                        {
                            FormsAuthentication.SetAuthCookie(user.UserName, login.RemmemberMe);
                            if (Session["ShopCart"] != null)
                            {
                                List<ShopCartItem> list = new List<ViewModels.ShopCartItem>();
                                var sessions = Session;
                                list = sessions["ShopCart"] as List<ViewModels.ShopCartItem>;
                                foreach(var item in list)
                                {
                                    var product = db.ProductsRepository.GetById(item.ProductID);
                                    ShopCart shop = new ShopCart()
                                    {
                                        ProductID = item.ProductID,
                                        Count = item.Count,
                                        Price = product.Price,
                                        UserID = user.UserID,
                                        Sum = product.Price * item.Count
                                    };
                                    db.ShopCartRepository.Insert(shop);
                                }
                                db.Save();
                            }
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
            else if (isCaptchaValid == false)
            {
                ModelState.AddModelError("EmailOrPhone", "کپچا نامعتبر می باشد");
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
    //این اکشن با دریافت شماره موبایل یا ایمیل کاربر یا کد اعتبارسنجی ارسال میکند یا لینک فعالسازی 
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

    //این اکشن با وارد کردن شماره تلفن و اعتبارسنجی به کاربر امکان تغییر رمز عبور فراموش شده را میدهد
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

    //این اکشن بعد از اعتبارسنجی ایمیل یا شماره موبایل، کاربر میتواند رمز فراموش شده خود را تغییر دهد
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
    private async Task<bool> IsCaptchaValid(string response, string action)
    {
        try
        {
            var secret = "6LedXs0eAAAAAM1ch6BbS09NM9JZqupvgNIAtmoQ";
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                    {
                        {"secret", secret},
                        {"response", response},
                        {"remoteip", Request.UserHostAddress}
                    };

                var content = new FormUrlEncodedContent(values);
                var verify = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
                var captchaResponseJson = await verify.Content.ReadAsStringAsync();
                var captchaResult = JsonConvert.DeserializeObject<CaptchaResponseViewModel>(captchaResponseJson);
                return captchaResult.Success
                       && captchaResult.Action == action
                       && captchaResult.Score > 0.5;
            }
        }
        catch (Exception ex)
        {
            return false;
        }

    }
}
}