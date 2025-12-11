using System;
using System.Collections.Generic;
using WarehouseManagementSystem1.Enums;

namespace WarehouseManagementSystem1.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } // Номер накладной
        public InvoiceType Type { get; set; } // Тип накладной
        public DateTime InvoiceDate { get; set; } = DateTime.Now;
        public DateTime? ShipmentDate { get; set; } // Дата отгрузки

        // Для прихода
        public int? SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

        // Для расхода
        public int? CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        // Общие поля
        public string ContractNumber { get; set; } // Номер договора
        public string WaybillNumber { get; set; } // Номер ТТН
        public string VehicleNumber { get; set; } // Номер машины
        public string DriverName { get; set; } // Водитель
        public string Notes { get; set; } // Примечания

        // Позиции в накладной
        public virtual List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

        // Суммы
        public decimal TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VAT { get; set; } // НДС
        public decimal TotalWithVAT { get; set; }

        // Ответственные лица
        public string CreatedBy { get; set; } // Кто создал
        public string ReceivedBy { get; set; } // Кто принял
        public string ReleasedBy { get; set; } // Кто отпустил
        public string Accountant { get; set; } // Бухгалтер

        public bool IsPrinted { get; set; } = false;
        public DateTime? PrintedDate { get; set; }
    }

    public class InvoiceItem
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Amount => Quantity * Price;
        public string Unit { get; set; } = "шт."; // Единица измерения
        public string Notes { get; set; }
    }
}