using Microsoft.AspNetCore.Mvc;
using ShopTemplate.Data;
using ShopTemplate.Models;
namespace ShopTemplate.Controllers
{
    public class UserController : Controller
    {
        private AppDBContext _dbContext { get; set; }

        public void DeleteSession(string sessionName="")
        {
            if (sessionName == "")
                HttpContext.Session.Clear();
            else {
                HttpContext.Session.Remove(sessionName);
            }
        }
        //private readonly DataRepository _repObj;
        public UserController(AppDBContext dbContext)
        {
            //_repObj = repObj;
            _dbContext = dbContext;

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
                DeleteSession("RegisterEmailId");
                return View();
            }
            else
            {
                return RedirectToAction("PostRegisterUser");
            }
            return View();
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
            string checkbox = frm["rememberMe"];
            string userName = "";
            if (checkbox == "on")
            {
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Append("UserId", userId, option);
                Response.Cookies.Append("Password", password, option);
            }
            else {
                DeleteCookie("UserId");
                DeleteCookie("Password");
            }
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
            catch(Exception ex)
            {
                loginStatus = false;
                TempData["loginStatus"] = false;
                TempData["loginError"] = "Some Error Occurred! Please try agiain later";

            }
            if (loginStatus) {
                if (roleId != null)
                {
                    HttpContext.Session.SetString("UserName", userName);
                    HttpContext.Session.SetString("EmailId", user.EmailId);
                    HttpContext.Session.SetString("ID", user.Id.ToString());
                    if(user.ProfilePicName is null)
                        HttpContext.Session.SetString("ProfilePic", "");
                    else
                        HttpContext.Session.SetString("ProfilePic", user.ProfilePicName );
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
                else{
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
        public Dictionary<string, object> CheckEmailExists(string emailId) {
            bool returnVal = false;
            string errorMsg = "";
            Dictionary<string, object> returnDetails= new Dictionary<string, object>();
            User user = null;
            try
            {
                user = _dbContext.Users.Where(x => x.EmailId == emailId).FirstOrDefault();
                if (user is null)
                {
                    returnVal= true;
                }
                else
                {
                    returnVal =  false;
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
                checkEmailExistDetails = CheckEmailExists(EmailId);
                bool checkEmailExistFlag = (bool)checkEmailExistDetails["status"];
                if (checkEmailExistFlag)
                {
                    HttpContext.Session.SetString("RegisterEmailId", EmailId);
                    HttpContext.Session.SetString("Password", password);
                    returnValue = true;

                }
                else {
                    TempData["errorMsg"] = checkEmailExistDetails["errorMsg"].ToString();
                    returnValue = false;
                }
            }
            catch {
                TempData["errorMsg"]  = "Some Error Occurred. Please Try Again!"; 
                returnValue = false;

            }
            if (returnValue)
                return RedirectToAction("PostRegisterUser");
            else
                return RedirectToAction("RegisterUser");
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
            User userObj = new User();
            userObj.Name = name;
            userObj.EmailId = HttpContext.Session.GetString("RegisterEmailId");
            userObj.Password = HttpContext.Session.GetString("Password");
            userObj.RoleId = 2;
            userObj.RegistrationDate = DateTime.Now;
            userObj.ProfilePicName = "";

            bool returnValue = false;
            try
            {
                _dbContext.Users.Add(userObj);
                _dbContext.SaveChanges();
                
                returnValue = true;
            }
            catch (Exception e)
            {
                TempData["errorMsg"] = "Some Error Occurred. Please Try Again!";
                returnValue = false;
            }
            
            
            if (returnValue) {

                HttpContext.Session.SetString("UserName", name);
                DeleteSession("Password");
                return RedirectToAction("Congratulation");
            }
            else
                return RedirectToAction("RegisterUser");
            
            TempData["errorMsg"] = "Some Error Occurred. Please Try Again!";
            return RedirectToAction("RegisterUser");
        }
    }
}
