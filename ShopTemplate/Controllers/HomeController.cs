using Microsoft.AspNetCore.Mvc;
using ShopTemplate.Models;
using System.Diagnostics;
using ShopTemplate.Data;
using System;
using System.Web;
using System.IO;
using Microsoft.AspNetCore.Hosting;
namespace ShopTemplate.Controllers
    
{
    public class HomeController : Controller
    {
        private AppDBContext _dbContext { get; set; }
        
        private IWebHostEnvironment _env;

        

        //private readonly DataRepository _repObj;
        public HomeController(AppDBContext dbContext, IWebHostEnvironment Environment)
        {
            //_repObj = repObj;
            this._dbContext = dbContext;
            this._env = Environment;
        }

        public IActionResult Index()
        {
            string EmailId = HttpContext.Session.GetString("EmailId");
            if (EmailId == null)
            {
                return RedirectToAction("Login", "User");
            }
            else {
                TempData["UserName"] = HttpContext.Session.GetString("UserName");
                TempData["EmailId"] = HttpContext.Session.GetString("EmailId");
                TempData["ProfilePic"] = HttpContext.Session.GetString("ProfilePic");
                TempData["ID"] = HttpContext.Session.GetString("ID");
                return View();
            }
        }
        

        [HttpPost]
        public Dictionary<string, object> updateProfileDetails(string name) {
            bool returnVal = false;
            string errorMsg = "";
            Dictionary<string, object> returnDetails = new Dictionary<string, object>();
            string EmailId =  HttpContext.Session.GetString("EmailId");
            User user = null;
            if (EmailId == null)
            {
                returnVal = false;
                errorMsg = "Session Error! Please Login Again";
            }
            try {
                user = _dbContext.Users.Where(x => x.EmailId == EmailId).FirstOrDefault();
                if (!(user is null))
                {
                    user.Name = name;
                    _dbContext.SaveChanges();
                    returnVal = true;
                    HttpContext.Session.SetString("UserName", name);
                }
                else {
                    returnVal = false;
                    errorMsg = "User Cannot be found! Please check again";
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

        public bool validateCurrentPassword(User user,string currPassword) {
            if (user.Password == currPassword)
            {
                return true;
            }
            else {
                return false;
            }
        }

        [HttpPost]
        public Dictionary<string, object> updatePasswordDetails([FromBody] UpdatePasswordModel passwordDetails) {

            
            bool returnVal = false;
            string errorMsg = "";
            Dictionary<string, object> returnDetails = new Dictionary<string, object>();
            string EmailId =  HttpContext.Session.GetString("EmailId");
            User user = null;
            if (EmailId == null)
            {
                returnVal = false;
                errorMsg = "Session Error! Please Login Again";
            }
            try {
                user = _dbContext.Users.Where(x => x.EmailId == EmailId).FirstOrDefault();

                if (!(user is null))
                {
                    bool currPassCheck =  validateCurrentPassword(user, passwordDetails.CurrPassword);
                    if (currPassCheck)
                    {
                        user.Password = passwordDetails.NewPassword;
                        _dbContext.SaveChanges();
                        returnVal = true;
                    }
                    else {
                        returnVal = false;
                        errorMsg = "Current Password is Incorrect";
                    }
                }
                else {
                    returnVal = false;
                    errorMsg = "User Cannot be found! Please check again";
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
        public IActionResult MyProfile() {
            string EmailId = HttpContext.Session.GetString("EmailId");
            if (EmailId == null)
            {
                return RedirectToAction("Login", "User");
            }
            else
            {

                TempData["UserName"] = HttpContext.Session.GetString("UserName");
                TempData["EmailId"] = HttpContext.Session.GetString("EmailId");
                TempData["ProfilePic"] = HttpContext.Session.GetString("ProfilePic");
                TempData["ID"] = HttpContext.Session.GetString("ID");
                return View();
            }
            
        }
        [HttpPost]
        public Dictionary<string, object> updateEmail([FromBody] UpdateEmailModel updateObj)
        {
            bool returnVal = false;
            string errorMsg = "";
            Dictionary<string, object> returnDetails = new Dictionary<string, object>();
            string EmailId = HttpContext.Session.GetString("EmailId");
            User user = null;
            if (EmailId == null)
            {
                returnVal = false;
                errorMsg = "Session Error! Please Login Again";
            }
            try
            {
                user = _dbContext.Users.Where(x => x.EmailId == EmailId).FirstOrDefault();

                if (!(user is null))
                {
                    bool currPassCheck = validateCurrentPassword(user, updateObj.CurrentPassword);
                    if (currPassCheck)
                    {
                        returnDetails = CheckEmailIsUnique(updateObj.NewEmail);
                        if ((bool)returnDetails["status"])
                        {
                            user.EmailId = updateObj.NewEmail;
                            _dbContext.SaveChanges();
                            returnVal = true;
                        }
                        else {
                            returnVal = false;
                            errorMsg = returnDetails["errorMsg"].ToString();
                        }
                    }
                    else
                    {
                        returnVal = false;
                        errorMsg = "Current Password is Incorrect";
                    }
                }
                else
                {
                    returnVal = false;
                    errorMsg = "User Cannot be found! Please check again";
                }
            }
            catch
            {
                returnVal = false;
                errorMsg = "Some Error Occurred! Please try again Later";
            }
            if (returnVal) {
                HttpContext.Session.SetString("EmailId", updateObj.NewEmail);
            }
            if (returnDetails.ContainsKey("status"))
            {
                returnDetails["status"] = (object)returnVal;
                returnDetails["errorMsg"] = (object)errorMsg;
            }
            else {
                returnDetails.Add("status", (object)returnVal);
                returnDetails.Add("errorMsg", (object)errorMsg);
            }
            return returnDetails;
        }
        public Dictionary<string, object> CheckEmailIsUnique(string emailId)
        {
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
            catch
            {
                returnVal = false;
                errorMsg = "Some Error Occurred! Please try again Later";
            }
            returnDetails.Add("status", (object)returnVal);
            returnDetails.Add("errorMsg", (object)errorMsg);
            return returnDetails;


        }


        public async Task<ActionResult> uploadProfilePic() {

            bool returnVal = false;
            string errorMsg = "";
            Dictionary<string, object> returnDetails = new Dictionary<string, object>();
            string EmailId = HttpContext.Session.GetString("EmailId");
            string profilePic = "";
            User user = null;
            if (EmailId == null)
            {
                returnVal = false;
                errorMsg = "Session Error! Please Login Again";
            }
            try
            {
                user = _dbContext.Users.Where(x => x.EmailId == EmailId).FirstOrDefault();
                if (!(user is null))
                {
                    var files = Request.Form.Files;
                    foreach (IFormFile source in files) {

                        string FileName = source.FileName;
                        string uploadsFolder = Path.Combine(_env.WebRootPath, "Images\\ProfilePic");
                        string uploadedFileExtension = FileName.Split(".")[FileName.Split(".").Length - 1];
                        string filePath = Path.Combine(uploadsFolder, user.Id.ToString() + "." + uploadedFileExtension);
                        profilePic = user.Id.ToString() + "." + uploadedFileExtension;
                        try
                        {
                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);
                            }
                            using (FileStream fs = new FileStream(filePath, FileMode.Create))
                            {
                                await source.CopyToAsync(fs);   
                            }
                            user.ProfilePicName = profilePic;
                            _dbContext.SaveChanges();
                            returnVal = true;
                        }
                        catch {

                            returnVal = false;
                            errorMsg = "Some Error Occurred! File did not upload";
                        }
                    }
                }
                else
                {
                    returnVal = false;
                    errorMsg = "User Cannot be found! Please check again";
                }
            }
            catch
            {
                returnVal = false;
                errorMsg = "Some Error Occurred! Please try again Later";
            }
            if (returnVal) {
                HttpContext.Session.SetString("ProfilePic", profilePic);
                TempData["ProfilePic"] = HttpContext.Session.GetString("ProfilePic");
            }
            if (returnDetails.ContainsKey("status"))
            {
                returnDetails["status"] = (object)returnVal;
                returnDetails["errorMsg"] = (object)errorMsg;
                returnDetails["ProfilePic"] = (object)profilePic;
            }
            else
            {
                returnDetails.Add("status", (object)returnVal);
                returnDetails.Add("errorMsg", (object)errorMsg);
                returnDetails.Add("ProfilePic", (object)profilePic);
            }
            return Ok(returnDetails);
        }

        public Dictionary<string, object> removeProfilePic (){
            bool returnVal = false;
            string errorMsg = "";
            Dictionary<string, object> returnDetails = new Dictionary<string, object>();
            string EmailId = HttpContext.Session.GetString("EmailId");
            string profilePic = "";
            User user = null;
            if (EmailId == null)
            {
                returnVal = false;
                errorMsg = "Session Error! Please Login Again";
            }
            try
            {
                user = _dbContext.Users.Where(x => x.EmailId == EmailId).FirstOrDefault();
                if (!(user is null))
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "Images\\ProfilePic");
                    string filePath = Path.Combine(uploadsFolder, user.ProfilePicName);
                    try
                    {
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                        user.ProfilePicName = "";
                        _dbContext.SaveChanges();
                        returnVal = true;
                    }
                    catch
                    {
                        returnVal = false;
                        errorMsg = "Some Error Occurred! File did not upload";
                    }
                }
                else
                {
                    returnVal = false;
                    errorMsg = "User Cannot be found! Please check again";
                }
            }
            catch
            {
                returnVal = false;
                errorMsg = "Some Error Occurred! Please try again Later";
            }
            if (returnVal) {
                HttpContext.Session.SetString("ProfilePic", profilePic);
                TempData["ProfilePic"] = HttpContext.Session.GetString("ProfilePic");
            }
            if (returnDetails.ContainsKey("status"))
            {
                returnDetails["status"] = (object)returnVal;
                returnDetails["errorMsg"] = (object)errorMsg;
            }
            else
            {
                returnDetails.Add("status", (object)returnVal);
                returnDetails.Add("errorMsg", (object)errorMsg);
            }
            return returnDetails;
            
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
    public class UpdatePasswordModel
    {
        public string NewPassword { get; set; }
        public string CurrPassword { get; set; }
    }
    public class UpdateEmailModel {
        public string NewEmail { get; set; }

        public string CurrentPassword { get; set; }
    }
}