using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using WarehouseManagementSystem1.Enums;
using WarehouseManagementSystem1.Models;

namespace WarehouseManagementSystem1.Services
{
    public class DataService
    {
        private static DataService _instance;
        public static DataService Instance => _instance ?? (_instance = new DataService());

        public List<User> Users { get; private set; }
        public List<Product> Products { get; private set; }
        public List<Category> Categories { get; private set; }
        public List<Supplier> Suppliers { get; private set; }
        public List<Transaction> Transactions { get; private set; } = new List<Transaction>(); // НОВОЕ свойство

        private string _dataPath = "Data";

        public DataService()
        {
            Console.WriteLine("=== ИНИЦИАЛИЗАЦИЯ DataService ===");
            InitializeData();
        }

        private void InitializeData()
        {
            try
            {
                Console.WriteLine("1. Проверяем папку Data...");
                if (!Directory.Exists(_dataPath))
                {
                    Directory.CreateDirectory(_dataPath);
                }

                // Проверяем, есть ли JSON файлы
                bool hasUsers = File.Exists(Path.Combine(_dataPath, "users.json"));
                bool hasProducts = File.Exists(Path.Combine(_dataPath, "products.json"));

                Console.WriteLine($"2. users.json: {hasUsers}, products.json: {hasProducts}");

                if (!hasUsers || !hasProducts)
                {
                    Console.WriteLine("3. Создаем тестовые данные...");
                    CreateDefaultData();
                    SaveAllData();
                }
                else
                {
                    Console.WriteLine("3. Загружаем данные из файлов...");
                    LoadAllData();
                }

                // Двойная проверка
                if (Users == null || Users.Count == 0)
                {
                    Console.WriteLine("4. Данные пустые, создаем заново...");
                    CreateDefaultData();
                    SaveAllData();
                }

                Console.WriteLine($"=== ГОТОВО ===");
                Console.WriteLine($"Пользователей: {Users.Count}");
                Console.WriteLine($"Товаров: {Products.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ОШИБКА: {ex.Message}");
                CreateDefaultData();
            }
        }

        private void CreateDefaultData()
        {
            // 1. Пользователи
            Users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Password = "admin123",
                    Email = "admin@warehouse.com",
                    Role = UserRole.Admin,
                    CreatedDate = DateTime.Now
                },
                new User
                {
                    Id = 2,
                    Username = "manager",
                    Password = "manager123",
                    Email = "manager@warehouse.com",
                    Role = UserRole.Manager,
                    CreatedDate = DateTime.Now
                },
                new User
                {
                    Id = 3,
                    Username = "worker",
                    Password = "worker123",
                    Email = "worker@warehouse.com",
                    Role = UserRole.Worker,
                    CreatedDate = DateTime.Now
                }
            };

            // 2. Категории
            Categories = new List<Category>
            {
                new Category { Id = 1, Name = "Электроника", Description = "Техника и гаджеты" },
                new Category { Id = 2, Name = "Офисные товары", Description = "Канцелярия и бумага" },
                new Category { Id = 3, Name = "Хозтовары", Description = "Бытовая химия и инструменты" }
            };

            // 3. Поставщики
            Suppliers = new List<Supplier>
            {
                new Supplier
                {
                    Id = 1,
                    Name = "ООО ТехноСити",
                    Phone = "+79991112233",
                    Email = "info@tech.ru",
                    Address = "г. Москва, ул. Ленина, 1"
                },
                new Supplier
                {
                    Id = 2,
                    Name = "ИП Иванов",
                    Phone = "+79994445566",
                    Email = "ivanov@mail.ru",
                    Address = "г. Москва, ул. Пушкина, 10"
                }
            };

            // 4. Товары
            Products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Ноутбук HP Pavilion",
                    Description = "15-дюймовый, Intel Core i5, 8GB RAM",
                    Price = 54999.99m,
                    Quantity = 15,
                    Category = "Электроника",
                    SKU = "NB-HP-001",
                    Barcode = "1234567890123",
                    Location = "Стеллаж А1-01",
                    LastUpdated = DateTime.Now
                },
                new Product
                {
                    Id = 2,
                    Name = "Мышь Logitech MX Master 3",
                    Description = "Беспроводная, лазерная мышь",
                    Price = 7499.50m,
                    Quantity = 42,
                    Category = "Электроника",
                    SKU = "MS-LG-MX3",
                    Barcode = "2345678901234",
                    Location = "Стеллаж А1-02",
                    LastUpdated = DateTime.Now
                },
                new Product
                {
                    Id = 3,
                    Name = "Бумага А4 Svetocopy",
                    Description = "Пачка 500 листов, 80г/м²",
                    Price = 450.00m,
                    Quantity = 200,
                    Category = "Офисные товары",
                    SKU = "PAP-A4-500",
                    Barcode = "3456789012345",
                    Location = "Стеллаж Б2-01",
                    LastUpdated = DateTime.Now
                }
            };
        }

        private void SaveAllData()
        {
            try
            {
                if (!Directory.Exists(_dataPath))
                    Directory.CreateDirectory(_dataPath);

                // Сохраняем все коллекции
                SaveToFile("users.json", Users);
                SaveToFile("products.json", Products);
                SaveToFile("categories.json", Categories);
                SaveToFile("suppliers.json", Suppliers);
                SaveToFile("transactions.json", Transactions); // НОВОЕ: сохраняем транзакции

                Console.WriteLine("✅ Все данные сохранены");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка сохранения: {ex.Message}");
            }
        }

        private void SaveToFile<T>(string fileName, List<T> data)
        {
            try
            {
                var filePath = Path.Combine(_dataPath, fileName);
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка сохранения {fileName}: {ex.Message}");
            }
        }

        private void LoadAllData()
        {
            try
            {
                Users = LoadFromFile<User>("users.json") ?? new List<User>();
                Products = LoadFromFile<Product>("products.json") ?? new List<Product>();
                Categories = LoadFromFile<Category>("categories.json") ?? new List<Category>();
                Suppliers = LoadFromFile<Supplier>("suppliers.json") ?? new List<Supplier>();
                Transactions = LoadFromFile<Transaction>("transactions.json") ?? new List<Transaction>(); // НОВОЕ: загружаем транзакции

                Console.WriteLine("✅ Данные загружены");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка загрузки: {ex.Message}");
                CreateDefaultData();
            }
        }

        private List<T> LoadFromFile<T>(string fileName)
        {
            var filePath = Path.Combine(_dataPath, fileName);

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"⚠ {fileName} не найден");
                return null;
            }

            try
            {
                var json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<T>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка загрузки {fileName}: {ex.Message}");
                return null;
            }
        }

        // ===== ПУБЛИЧНЫЕ МЕТОДЫ =====

        public User Authenticate(string username, string password)
        {
            if (Users == null) return null;
            foreach (var user in Users)
            {
                if (user.Username == username && user.Password == password)
                    return user;
            }
            return null;
        }

        public void ForceCreateTestData()
        {
            Console.WriteLine("🔄 Создание тестовых данных...");
            CreateDefaultData();
            SaveAllData();
            Console.WriteLine($"✅ Создано: {Users.Count} пользователей, {Products.Count} товаров");
        }

        public string GetDebugInfo()
        {
            return $"DataService:\n" +
                   $"• Users: {Users?.Count ?? 0}\n" +
                   $"• Products: {Products?.Count ?? 0}\n" +
                   $"• Transactions: {Transactions?.Count ?? 0}\n" +
                   $"• Папка Data: {Path.GetFullPath(_dataPath)}";
        }

        public void SaveToJson()
        {
            SaveAllData();
        }

        public void AddProduct(Product product)
        {
            if (Products == null) Products = new List<Product>();

            int maxId = Products.Any() ? Products.Max(p => p.Id) : 0;
            product.Id = maxId + 1;
            product.LastUpdated = DateTime.Now;
            Products.Add(product);
            SaveAllData();
        }

        public void UpdateProduct(Product updatedProduct)
        {
            if (Products == null) return;

            for (int i = 0; i < Products.Count; i++)
            {
                if (Products[i].Id == updatedProduct.Id)
                {
                    updatedProduct.LastUpdated = DateTime.Now;
                    Products[i] = updatedProduct;
                    SaveAllData();
                    return;
                }
            }
        }

        public void DeleteProduct(int productId)
        {
            if (Products == null) return;

            var productToRemove = Products.FirstOrDefault(p => p.Id == productId);
            if (productToRemove != null)
            {
                Products.Remove(productToRemove);
                SaveAllData();
            }
        }

        // ===== МЕТОДЫ ДЛЯ ОПЕРАЦИЙ (НОВЫЕ) =====

        public bool ProcessTransaction(Transaction transaction)
        {
            try
            {
                // 1. Находим товар
                var product = Products?.FirstOrDefault(p => p.Id == transaction.ProductId);
                if (product == null)
                {
                    MessageBox.Show("Ошибка: товар не найден!", "Ошибка операции",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // 2. Проверяем остаток для РАСХОДА или ПЕРЕМЕЩЕНИЯ
                if (transaction.Type == TransactionType.Расход || transaction.Type == TransactionType.Перемещение)
                {
                    if (product.Quantity < transaction.Quantity)
                    {
                        MessageBox.Show($"Ошибка: недостаточно товара на складе!\nДоступно: {product.Quantity}, требуется: {transaction.Quantity}",
                            "Ошибка операции", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }

                // 3. Выполняем операцию
                switch (transaction.Type)
                {
                    case TransactionType.Приход:
                        product.Quantity += transaction.Quantity;
                        transaction.ToLocation = transaction.ToLocation ?? product.Location;
                        break;
                    case TransactionType.Расход:
                        product.Quantity -= transaction.Quantity;
                        transaction.FromLocation = transaction.FromLocation ?? product.Location;
                        break;
                    case TransactionType.Перемещение:
                        product.Quantity -= transaction.Quantity;
                        transaction.FromLocation = transaction.FromLocation ?? product.Location;
                        break;
                }

                product.LastUpdated = DateTime.Now;

                // 4. Записываем операцию в журнал
                transaction.Id = Transactions.Any() ? Transactions.Max(t => t.Id) + 1 : 1;
                transaction.TransactionDate = DateTime.Now;
                Transactions.Add(transaction);

                // 5. Сохраняем всё
                SaveAllData();

                Console.WriteLine($"✅ Операция проведена: {transaction.Type} товара '{product.Name}' x{transaction.Quantity}");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Критическая ошибка:\n{ex.Message}", "Ошибка системы",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public List<Transaction> GetProductTransactions(int productId)
        {
            return Transactions
                ?.Where(t => t.ProductId == productId)
                .OrderByDescending(t => t.TransactionDate)
                .ToList() ?? new List<Transaction>();
        }

        public List<Transaction> GetTransactionsByDate(DateTime startDate, DateTime endDate)
        {
            return Transactions
                ?.Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .OrderByDescending(t => t.TransactionDate)
                .ToList() ?? new List<Transaction>();
        }
    }
}