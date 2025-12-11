namespace WarehouseManagementSystem1.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactPerson { get; set; } // Контактное лицо
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string INN { get; set; } // ИНН
        public string KPP { get; set; } // КПП
        public string BankDetails { get; set; } // Банковские реквизиты
        public bool IsActive { get; set; } = true;
    }
}