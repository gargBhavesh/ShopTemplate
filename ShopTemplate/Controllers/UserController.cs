using Microsoft.AspNetCore.Mvc;
using ShopTemplate.Data;
using ShopTemplate.Models;
using ShopTemplate.Services;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.WebUtilities;

namespace ShopTemplate.Controllers
{
    public class UserController : Controller
    {
        private AppDBContext _dbContext { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;

        //private readonly DataRepository _repObj;
        public UserController(AppDBContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            //_repObj = repObj;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;

        }
        public void DeleteSession(string sessionName = "")
        {
            if (sessionName == "")
                HttpContext.Session.Clear();
            else
            {
                HttpContext.Session.Remove(sessionName);
            }
        }
        public IActionResult RegisterUser()
        {
            if (HttpContext.Session.GetString("RegisterEmailId") != null)
            {
                return RedirectToAction("PostRegisterUser");
            }
            else {
                DeleteSession();
                return View();
            }

        }
        public IActionResult Login()
        {
            DeleteSession();
            return View();
        }
        public IActionResult Congratulation()
        {
            string EmailId = HttpContext.Session.GetString("RegisterEmailId");

            if (EmailId != null)
            {
                HttpContext.Session.SetString("EmailId", HttpContext.Session.GetString("RegisterEmailId"));
                TempData["EmailId"] = EmailId;
                DeleteSession("RegisterEmailId");
                return View();
            }
            else
            {
                return RedirectToAction("PostRegisterUser");
            }
        }
        public IActionResult SignOut()
        {
            DeleteSession();
            return RedirectToAction("Login");
        }
        public void DeleteCookie(string cookieName) {

            string CookieValue = Request.Cookies.TryGetValue(cookieName, out string strVal) ? strVal : null;
            if (CookieValue != null)
            {
                Response.Cookies.Delete(cookieName);
            }
        }

        [HttpPost]
        public ActionResult Login(IFormCollection frm)
        {
            bool loginStatus = false;
            string userId = frm["Email"];
            string password = frm["Password"];
            string userName = "";

            byte? roleId = null;
            User user = null;
            try {
                user = _dbContext.Users.Where(x => x.EmailId == userId && x.Password == password).FirstOrDefault();
                if (user is null)
                {
                    loginStatus = false;
                    TempData["loginStatus"] = false;
                    TempData["loginError"] = "Invalid Credentials";
                }
                else
                {
                    loginStatus = true;
                    userName = user.Name;
                    roleId = user.RoleId;
                }
            }
            catch (Exception ex)
            {
                loginStatus = false;
                TempData["loginStatus"] = false;
                TempData["loginError"] = "Some Error Occurred! Please try agiain later";

            }
            if (loginStatus) {
                if (roleId != null)
                {
                    HttpContext.Session.SetString("EmailId", user.EmailId);
                    HttpContext.Session.SetString("ID", user.Id.ToString());
                    if (user.EmailVerificationStatus != null)
                        HttpContext.Session.SetString("EmailVerificationStatus", user.EmailVerificationStatus.ToString());
                    else {
                        HttpContext.Session.SetString("EmailVerificationStatus", "0");
                    }
                    
                    

                    if (userName == "") {
                        
                        HttpContext.Session.SetString("Password", password);
                        HttpContext.Session.SetString("ProfilePic", "");
                        
                        return RedirectToAction("PostRegisterUser");
                    }

                    HttpContext.Session.SetString("UserName", userName);
                    
                    if (user.ProfilePicName is null)
                        HttpContext.Session.SetString("ProfilePic", "");
                    else
                        HttpContext.Session.SetString("ProfilePic", user.ProfilePicName);
                    if (roleId == 1)
                    {
                        TempData["loginStatus"] = true;
                        return RedirectToAction("AdminHome", "Admin");
                    }
                    else
                    {
                        TempData["loginStatus"] = true;
                        return RedirectToAction("Index", "Home");
                    }
                }
                else {
                    TempData["loginStatus"] = false;
                    TempData["loginError"] = "Some Error Occurred! Please try agiain later";
                    return RedirectToAction("Login");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public Dictionary<string, object> CheckEmailIsUnique(string emailId) {
            bool returnVal = false;
            string errorMsg = "";
            Dictionary<string, object> returnDetails = new Dictionary<string, object>();
            User user = null;
            try
            {
                user = _dbContext.Users.Where(x => x.EmailId == emailId).FirstOrDefault();
                if (user is null)
                {
                    returnVal = true;
                }
                else
                {
                    returnVal = false;
                    errorMsg = "Email Already Exists";
                }
            }
            catch {
                returnVal = false;
                errorMsg = "Some Error Occurred! Please try again Later";
            }
            returnDetails.Add("status", (object)returnVal);
            returnDetails.Add("errorMsg", (object)errorMsg);
            return returnDetails;


        }
        [HttpPost]
        public IActionResult RegisterUser(IFormCollection form) {

            bool returnValue = false;
            string EmailId = form["EmailId"];
            string password = form["Password"];
            Dictionary<string, object> checkEmailExistDetails = new Dictionary<string, object>();
            try
            {
                checkEmailExistDetails = CheckEmailIsUnique(EmailId);
                bool checkEmailExistFlag = (bool)checkEmailExistDetails["status"];
                if (checkEmailExistFlag)
                {
                    try {
                        User user = new User();
                        user.EmailId = EmailId;
                        user.Password = password;
                        user.Name = "";
                        user.RoleId = 2;
                        user.RegistrationDate = DateTime.Now;
                        user.ProfilePicName = "";
                        _dbContext.Users.Add(user);
                        _dbContext.SaveChanges();
                        HttpContext.Session.SetString("RegisterEmailId", EmailId);
                        HttpContext.Session.SetString("Password", password);
                        HttpContext.Session.SetString("ProfilePic", "");
                        HttpContext.Session.SetString("ID", user.Id.ToString());
                        returnValue = true;
                    }
                    catch {
                        TempData["errorMsg"] = "Some Error Occurred. Please Try Again!";
                        returnValue = false;

                    }
                }
                else {
                    TempData["errorMsg"] = checkEmailExistDetails["errorMsg"].ToString();
                    returnValue = false;
                }
            }
            catch {
                TempData["errorMsg"] = "Some Error Occurred. Please Try Again!";
                returnValue = false;
            }
            if (returnValue)
                return RedirectToAction("PostRegisterUser");
            else
                return RedirectToAction("RegisterUser");
        }

        public IActionResult ForgotPassword() {
            DeleteSession();
            
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ForgotPassword(IFormCollection form) {
            string email = form["Email"];
            string password = "";
            bool returnVal = false;
            User user = null;
            user = _dbContext.Users.Where(x => x.EmailId == email).FirstOrDefault();
            if (!(user is null))
            {
                password = user.Password;
                string encryptedPassword = EncodePasswordToBase64(password);
                ForgotPasswordRequest request = new ForgotPasswordRequest();
                request.ToEmail = email;
                String hostName, scheme;
                hostName = _httpContextAccessor.HttpContext.Request.Host.ToString();
                scheme = _httpContextAccessor.HttpContext.Request.Scheme.ToString();
                System.Collections.Specialized.NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);
                string url = scheme + "://" + hostName + "/User/ResetPassword";
                var param = new Dictionary<string, string>() { { "email", email },{ "code" , encryptedPassword } };

                var newUrl = new Uri(QueryHelpers.AddQueryString(url, param));
                request.ResetLink = newUrl.AbsoluteUri;


                return RedirectToAction("SendPasswordResetEmail", "Mail", request);
            }
            else
            {
                returnVal = false;
                TempData["errorMsg"] = "User Not Found";
            }
            
                return RedirectToAction("ForgotPassword");
            

        }

        public IActionResult ResetPassword(string email,string code) {
            string password = DecodeFrom64(code);
            User user = new User();
            try
            {
                user = _dbContext.Users.Where(x => x.EmailId == email && x.Password == password).FirstOrDefault();
                if (user != null)
                {

                    HttpContext.Session.SetString("EmailId", email);
                    HttpContext.Session.SetString("Password", password);
                    return View();
                }
                else
                {
                    TempData["errorMsg"] = "Invalid Token";
                    return RedirectToAction("ForgotPassword");
                }
            }
            catch {
                TempData["errorMsg"] = "Some Error Occurred! Please try again later";
                return RedirectToAction("ForgotPassword");
            }
        }
        public static string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte =  Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch 
            {
                return password; 
            }
        }

        
        public string DecodeFrom64(string encodedData)
        {
            UTF8Encoding encoder = new  UTF8Encoding();
            Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }

        [HttpPost]
        public IActionResult ResetPassword(IFormCollection form)
        {
            string password = form["Password"];
            bool returnVal = false;
            string email = HttpContext.Session.GetString("EmailId");
            string  currPassword = HttpContext.Session.GetString("Password");
            string userName = "";
            
            byte? roleId ;
            User user = null;
            user = _dbContext.Users.Where(x => x.EmailId == email && x.Password == currPassword).FirstOrDefault();
            if (!(user is null))
            {
                DeleteSession("Password");
                user.Password = password;
                _dbContext.SaveChanges();
                userName = user.Name;
                roleId = user.RoleId;
                if (roleId != null)
                {
                    if (userName == "")
                    {
                        HttpContext.Session.SetString("RegisterEmailId", user.EmailId);
                        HttpContext.Session.SetString("Password", password);
                        HttpContext.Session.SetString("ProfilePic", "");
                        HttpContext.Session.SetString("ID", user.Id.ToString());
                        return RedirectToAction("PostRegisterUser");
                    }
                    HttpContext.Session.SetString("UserName", userName);
                    HttpContext.Session.SetString("EmailId", user.EmailId);
                    HttpContext.Session.SetString("ID", user.Id.ToString());
                    if (user.ProfilePicName is null)
                        HttpContext.Session.SetString("ProfilePic", "");
                    else
                        HttpContext.Session.SetString("ProfilePic", user.ProfilePicName);
                    if (roleId == 1)
                    {
                        TempData["loginStatus"] = true;
                        return RedirectToAction("AdminHome", "Admin");
                    }
                    else
                    {
                        TempData["loginStatus"] = true;
                        return RedirectToAction("Index", "Home");
                    }
                }
                else {
                    returnVal = false;
                    TempData["errorMsg"] = "Invalid User";
                }
            }
            else
            {
                returnVal = false;
                TempData["errorMsg"] = "User Not Found";
            }
            return RedirectToAction("ForgotPassword");
        }


        public IActionResult ForgotPasswordConfirmation()
        {
            if (TempData.ContainsKey("errorMsg"))
            {
                TempData.Remove("errorMsg");
            }
            return View();
        }
        public IActionResult PostRegisterUser() {

            string EmailId = HttpContext.Session.GetString("RegisterEmailId");
            if (EmailId != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("RegisterUser");
            }
                
        }
        [HttpPost]
        public IActionResult SaveRegisterUser(IFormCollection form)
        {
            string name = form["Name"];
            bool returnVal = false;
            string EmailId = HttpContext.Session.GetString("RegisterEmailId");
            string Password = HttpContext.Session.GetString("Password");
            User user = null;
            if (EmailId == null)
            {
                returnVal = false;
                TempData["errorMsg"] = "Some Error Occurred. Please Try Again!";
            }
            try
            {
                user = _dbContext.Users.Where(x => x.EmailId == EmailId && x.Password==Password).FirstOrDefault();
                if (!(user is null))
                {
                    user.Name = name;
                    _dbContext.SaveChanges();
                }
                else {
                    returnVal = false;
                    TempData["errorMsg"] = "User Not Found";
                }
                returnVal = true;
            }
            catch (Exception e)
            {
                TempData["errorMsg"] = "Some Error Occurred. Please Try Again!";
                returnVal = false;
            }
            if (returnVal)
            {

                HttpContext.Session.SetString("UserName", name);
                DeleteSession("Password");
                return RedirectToAction("Congratulation");
            }
            else {
                TempData["errorMsg"] = "Some Error Occurred. Please Try Again!";
                return RedirectToAction("RegisterUser");
            }
        }
    }
}
