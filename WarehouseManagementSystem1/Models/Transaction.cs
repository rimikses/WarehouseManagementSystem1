using System;
using System.ComponentModel.DataAnnotations;
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
        public TransactionType Type { get; set; }

        [Required]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть положительным")]
        public int Quantity { get; set; }

        // Для связи с накладными
        public int? InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }

        // Для прихода
        public int? SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

        // Для расхода
        public int? CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public string FromLocation { get; set; }
        public string ToLocation { get; set; }

        [StringLength(500)]
        public string Comments { get; set; }

        [Required]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        [StringLength(50)]
        public string DocumentNumber { get; set; }
    }
}