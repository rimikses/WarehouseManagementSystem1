using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WarehouseManagementSystem1.Models
{
   [Table("WarehouseLocations")]
    public class WarehouseLocation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } // А1, Б2, В3

        [Required]
        [StringLength(200)]
        public string Name { get; set; } // Стеллаж А, Холодильник 1

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public decimal Capacity { get; set; } // Вместимость

        [Required]
        public decimal CurrentLoad { get; set; } = 0; // Текущая загрузка

        [StringLength(50)]
        public string TemperatureZone { get; set; } // Температурный режим

        public bool IsActive { get; set; } = true;

        // Навигация
        public virtual ICollection<Product> Products { get; set; }
    }
}
