using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WarehouseManagementSystem1.Enums;

namespace WarehouseManagementSystem1.Models
{
    [Table("Transactions")]
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        // Ссылка на товар (пока без ForeignKey для простоты)
        // Позже добавим связь

        [Required]
        public TransactionType Type { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть положительным")]
        public int Quantity { get; set; }

        [StringLength(200)]
        public string FromLocation { get; set; } // Откуда

        [StringLength(200)]
        public string ToLocation { get; set; }   // Куда

        [StringLength(500)]
        public string Comments { get; set; }

        public int UserId { get; set; } // Кто выполнил операцию

        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string DocumentNumber { get; set; } // Номер накладной
    }
}