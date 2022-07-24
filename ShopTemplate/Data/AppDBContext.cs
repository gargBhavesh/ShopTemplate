using Microsoft.EntityFrameworkCore;
using ShopTemplate.Models;
namespace ShopTemplate.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
    }
}
