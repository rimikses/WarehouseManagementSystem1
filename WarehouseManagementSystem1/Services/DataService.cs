using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using WarehouseManagementSystem1.Enums;
using WarehouseManagementSystem1.Models;

namespace WarehouseManagementSystem1.Services
{
    public class DataService
    {
        public List<Customer> Customers { get; private set; }
        public List<Invoice> Invoices { get; private set; }

        private static DataService _instance;
        public static DataService Instance => _instance ?? (_instance = new DataService());

        // ДОБАВЛЯЕМ событие для обновления UI
        public event Action DataChanged; // ← ВСТАВЛЯЕМ ЭТУ СТРОКУ

        // ДОБАВЛЕНО: Статические методы для обратной совместимости
        public static List<Product> LoadProducts()
        {
            return Instance.Products ?? new List<Product>();
        }

        public static void SaveProducts(List<Product> products)
        {
            Instance.Products = products;
            Instance.SaveAllData();
        }

        public List<User> Users { get; private set; }
        public List<Product> Products { get; set; } // Изменено на set
        public List<Category> Categories { get; private set; }
        public List<Supplier> Suppliers { get; private set; }
        public List<Transaction> Transactions { get; private set; } = new List<Transaction>();

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
                    Article = "NB-HP-001", // ДОБАВЛЕНО
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
                    Article = "MS-LG-MX3", // ДОБАВЛЕНО
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
                    Article = "PAP-A4-500", // ДОБАВЛЕНО
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

                SaveToFile("users.json", Users);
                SaveToFile("products.json", Products);
                SaveToFile("categories.json", Categories);
                SaveToFile("suppliers.json", Suppliers);
                SaveToFile("transactions.json", Transactions);

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
                Transactions = LoadFromFile<Transaction>("transactions.json") ?? new List<Transaction>();

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

        // ДОБАВЛЕНО: Метод экспорта
        public static void ExportToCsv(List<Product> products, string filePath)
        {
            try
            {
                var lines = new List<string>();

                // Заголовок
                lines.Add("Артикул;Название;Категория;Количество;Цена;Описание;Стоимость");

                // Данные
                foreach (var product in products)
                {
                    var line = $"\"{product.Article}\";" +
                               $"\"{product.Name}\";" +
                               $"\"{product.Category}\";" +
                               $"{product.Quantity};" +
                               $"{product.Price:F2};" +
                               $"\"{product.Description ?? ""}\";" +
                               $"{(product.Quantity * product.Price):F2}";
                    lines.Add(line);
                }

                File.WriteAllLines(filePath, lines, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка экспорта: {ex.Message}");
            }
        }

        // Остальные методы остаются без изменений
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
            DataChanged?.Invoke(); // ← ДОБАВЛЯЕМ ВЫЗОВ СОБЫТИЯ
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

            // ИЗМЕНЯЕМ генерацию ID для надежности:
            int maxId = 0;
            if (Products.Any())
            {
                maxId = Products.Max(p => p.Id);

                // ДОБАВЛЯЕМ проверку на дубликаты ID
                while (Products.Any(p => p.Id == maxId + 1))
                {
                    maxId++;
                }
            }

            product.Id = maxId + 1;
            product.LastUpdated = DateTime.Now;
            Products.Add(product);
            SaveAllData();
            DataChanged?.Invoke(); // ← ДОБАВЛЯЕМ ВЫЗОВ СОБЫТИЯ
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
                    DataChanged?.Invoke(); // ← ДОБАВЛЯЕМ ВЫЗОВ СОБЫТИЯ
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
                DataChanged?.Invoke(); // ← ДОБАВЛЯЕМ ВЫЗОВ СОБЫТИЯ
            }
        }

        public Product GetProductById(int productId)
        {
            return Products?.FirstOrDefault(p => p.Id == productId);
        }

        public User GetUserById(int userId)
        {
            return Users?.FirstOrDefault(u => u.Id == userId);
        }

        public List<Product> GetProductsByLocation(string location)
        {
            return Products?.Where(p => p.Location == location).ToList()
                   ?? new List<Product>();
        }

        public List<Transaction> GetTransactionsByProduct(int productId)
        {
            return Transactions?.Where(t => t.ProductId == productId)
                               .OrderByDescending(t => t.TransactionDate)
                               .ToList()
                   ?? new List<Transaction>();
        }

        public bool ProcessTransaction(Transaction transaction)
        {
            try
            {
                var product = Products?.FirstOrDefault(p => p.Id == transaction.ProductId);
                if (product == null)
                {
                    MessageBox.Show("Ошибка: товар не найден!", "Ошибка операции",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (transaction.Type == TransactionType.Расход || transaction.Type == TransactionType.Перемещение)
                {
                    if (product.Quantity < transaction.Quantity)
                    {
                        MessageBox.Show($"Ошибка: недостаточно товара на складе!\nДоступно: {product.Quantity}, требуется: {transaction.Quantity}",
                            "Ошибка операции", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }

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

                        // ДОБАВЛЯЕМ проверку целевого местоположения
                        if (string.IsNullOrWhiteSpace(transaction.ToLocation))
                        {
                            MessageBox.Show("Для перемещения укажите целевое местоположение!",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }

                        // Проверяем, существует ли товар в целевом месте
                        var targetProduct = Products.FirstOrDefault(p =>
                            p.Article == product.Article && p.Location == transaction.ToLocation);

                        if (targetProduct != null)
                        {
                            // Если товар уже есть в целевом месте, увеличиваем количество
                            targetProduct.Quantity += transaction.Quantity;
                            targetProduct.LastUpdated = DateTime.Now;
                        }
                        else
                        {
                            // Если товара нет в целевом месте, создаем новую запись
                            var newProduct = new Product
                            {
                                Id = Products.Any() ? Products.Max(p => p.Id) + 1 : 1,
                                Article = product.Article,
                                Name = product.Name,
                                Description = product.Description,
                                Price = product.Price,
                                Quantity = transaction.Quantity,
                                Category = product.Category,
                                SKU = product.SKU,
                                Barcode = product.Barcode,
                                Location = transaction.ToLocation,
                                LastUpdated = DateTime.Now
                            };
                            Products.Add(newProduct);
                        }
                        break;
                }

                product.LastUpdated = DateTime.Now;

                transaction.Id = Transactions.Any() ? Transactions.Max(t => t.Id) + 1 : 1;
                transaction.TransactionDate = DateTime.Now;
                Transactions.Add(transaction);

                SaveAllData();
                DataChanged?.Invoke(); // ← ДОБАВЛЯЕМ ВЫЗОВ СОБЫТИЯ

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