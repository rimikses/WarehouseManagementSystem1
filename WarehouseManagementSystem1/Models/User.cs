using System;
using WarehouseManagementSystem1.Enums;

namespace WarehouseManagementSystem1.Models
{
    public class User
    {
        public User()
        {
            // Конструктор по умолчанию для сериализации
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}