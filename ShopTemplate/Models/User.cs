
using System.ComponentModel.DataAnnotations;


namespace ShopTemplate.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string EmailId { get; set; }
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
        public byte? RoleId { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime RegistrationDate { get; set; }
        public string? ProfilePicName { get; set; }

    }
}
