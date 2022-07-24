using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using ShopTemplate.Data;
using ShopTemplate.Models;
namespace ShopTemplate
{
    public class DataRepository
    {

        private AppDBContext _dbContext { get; set; }

        public DataRepository(AppDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        public byte? ValidateCredentials(string userId, string password) {

            User user = null;
            user = _dbContext.Users.Find(userId);
            byte? roleId = null;
            if (user.Password == password)
            {
                roleId = user.RoleId;
            }

            return roleId;
        }

    }
}
