using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WarehouseManagementSystem1.Enums;

namespace WarehouseManagementSystem1.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Required]
        public TransactionType Type { get; set; } // Приход, Расход, Перемещение

        [Required]
        public int ProductId { get; set; }

        // Навигационное свойство (ссылка на товар)
        public virtual Product Product { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть положительным")]
        public int Quantity { get; set; }

        // Для прихода/расхода: откуда/куда. Для перемещения: откуда и куда.
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }

        [StringLength(500)]
        public string Comments { get; set; }

        [Required]
        public int UserId { get; set; } // Кто выполнил операцию

        // Навигационное свойство на пользователя (опционально, но полезно)
        public virtual User User { get; set; }

        // Уникальный номер документа (накладной, ордера)
        [StringLength(50)]
        public string DocumentNumber { get; set; }
    }
}