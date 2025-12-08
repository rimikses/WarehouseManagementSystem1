using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WarehouseManagementSystem1.Services;

namespace WarehouseManagementSystem1
{
    public partial class DataTestForm : Form
    {
        private TabControl tabControl;
        private DataGridView gridProducts;
        private DataGridView gridUsers;
        private Button btnClose;
        private DataService dataService;

        public DataTestForm()
        {
            dataService = DataService.Instance;
            CreateTestForm();
            LoadAllData(); // Теперь данные загружаются ПОСЛЕ создания формы
        }

        private void CreateTestForm()
        {
            // Настройки формы
            this.Text = "📊 Тестовый просмотр данных";
            this.Size = new Size(850, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Вкладки
            tabControl = new TabControl();
            tabControl.Location = new Point(10, 10);
            tabControl.Size = new Size(820, 450);
            tabControl.SelectedIndexChanged += (s, e) =>
            {
                // При переключении вкладок обновляем данные
                if (tabControl.SelectedTab != null)
                {
                    LoadAllData();
                }
            };
            this.Controls.Add(tabControl);

            // Вкладка 1: Товары
            var tabProducts = new TabPage("📦 Товары");
            tabProducts.Name = "tabProducts";
            CreateProductsGrid(tabProducts);
            tabControl.TabPages.Add(tabProducts);

            // Вкладка 2: Пользователи
            var tabUsers = new TabPage("👥 Пользователи");
            tabUsers.Name = "tabUsers";
            CreateUsersGrid(tabUsers);
            tabControl.TabPages.Add(tabUsers);

            // Кнопка закрытия
            btnClose = new Button();
            btnClose.Text = "Закрыть";
            btnClose.Font = new Font("Segoe UI", 10);
            btnClose.ForeColor = Color.White;
            btnClose.BackColor = Color.FromArgb(33, 150, 243);
            btnClose.Location = new Point(375, 470);
            btnClose.Size = new Size(100, 35);
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private void CreateProductsGrid(TabPage tab)
        {
            gridProducts = new DataGridView();
            gridProducts.Name = "gridProducts";
            gridProducts.Dock = DockStyle.Fill;
            gridProducts.AllowUserToAddRows = false;
            gridProducts.ReadOnly = true;
            gridProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridProducts.RowHeadersVisible = false;

            // Очищаем старые колонки, если есть
            gridProducts.Columns.Clear();

            // Колонки для товаров
            gridProducts.Columns.Add("Id", "ID");
            gridProducts.Columns.Add("Name", "Название");
            gridProducts.Columns.Add("Price", "Цена");
            gridProducts.Columns.Add("Quantity", "Кол-во");
            gridProducts.Columns.Add("Category", "Категория");
            gridProducts.Columns.Add("Location", "Место");

            tab.Controls.Add(gridProducts);
        }

        private void CreateUsersGrid(TabPage tab)
        {
            gridUsers = new DataGridView();
            gridUsers.Name = "gridUsers";
            gridUsers.Dock = DockStyle.Fill;
            gridUsers.AllowUserToAddRows = false;
            gridUsers.ReadOnly = true;
            gridUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridUsers.RowHeadersVisible = false;

            // Очищаем старые колонки
            gridUsers.Columns.Clear();

            // Колонки для пользователей
            gridUsers.Columns.Add("Id", "ID");
            gridUsers.Columns.Add("Username", "Логин");
            gridUsers.Columns.Add("Email", "Email");
            gridUsers.Columns.Add("Role", "Роль");

            tab.Controls.Add(gridUsers);
        }

        private void LoadAllData()
        {
            try
            {
                // ЗАГРУЗКА ТОВАРОВ
                if (gridProducts != null)
                {
                    gridProducts.Rows.Clear();
                    var products = dataService.Products;

                    if (products != null && products.Any())
                    {
                        foreach (var product in products)
                        {
                            gridProducts.Rows.Add(
                                product.Id,
                                product.Name,
                                $"{product.Price:C}",
                                product.Quantity,
                                product.Category ?? "(нет)",
                                product.Location ?? "(не указано)"
                            );
                        }

                        // Подсветка товаров с низким запасом
                        foreach (DataGridViewRow row in gridProducts.Rows)
                        {
                            if (row.Cells["Quantity"].Value != null)
                            {
                                if (int.TryParse(row.Cells["Quantity"].Value.ToString(), out int qty) && qty < 10)
                                {
                                    row.DefaultCellStyle.BackColor = Color.LightPink;
                                    row.DefaultCellStyle.ForeColor = Color.DarkRed;
                                }
                            }
                        }
                    }
                    else
                    {
                        gridProducts.Rows.Add("Нет данных", "", "", "", "", "");
                    }
                }

                // ЗАГРУЗКА ПОЛЬЗОВАТЕЛЕЙ
                if (gridUsers != null)
                {
                    gridUsers.Rows.Clear();
                    var users = dataService.Users;

                    if (users != null && users.Any())
                    {
                        foreach (var user in users)
                        {
                            gridUsers.Rows.Add(
                                user.Id,
                                user.Username,
                                user.Email ?? "(нет)",
                                user.Role
                            );
                        }
                    }
                    else
                    {
                        gridUsers.Rows.Add("Нет данных", "", "", "");
                    }
                }

                // Обновляем заголовок с количеством
                this.Text = $"📊 Тестовый просмотр данных (Товаров: {dataService.Products?.Count ?? 0}, Пользователей: {dataService.Users?.Count ?? 0})";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных:\n{ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Дополнительный метод для принудительного обновления
        public void RefreshData()
        {
            LoadAllData();
        }
    }
}