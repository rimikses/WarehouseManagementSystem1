using System;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagementSystem1.Enums;
using WarehouseManagementSystem1.Models;
using WarehouseManagementSystem1.Services;

namespace WarehouseManagementSystem1
{
    public partial class MainForm : Form
    {
        private User currentUser;
        private DataService dataService;

        private MenuStrip mainMenu;
        private StatusStrip statusBar;
        private TabControl mainTabs;
        private Label lblWelcome;
        private Label lblDateTime;
        private Timer clockTimer;

        // Объявляем пункты меню как поля класса
        private ToolStripMenuItem fileMenu;
        private ToolStripMenuItem productsMenu;
        private ToolStripMenuItem operationsMenu;
        private ToolStripMenuItem referencesMenu;
        private ToolStripMenuItem adminMenu;
        private ToolStripMenuItem helpMenu;

        public MainForm(User user)
        {
            currentUser = user ?? throw new ArgumentNullException(nameof(user));
            dataService = DataService.Instance;

            InitializeComponent();
            CreateMainForm();
            StartClock();
        }

        private void CreateMainForm()
        {
            this.Text = $"🏠 Складской учет - {currentUser.Username} ({GetRoleName(currentUser.Role)})";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // ===== 1. МЕНЮ =====
            mainMenu = new MenuStrip();
            mainMenu.BackColor = Color.FromArgb(33, 150, 243);
            mainMenu.ForeColor = Color.White;
            mainMenu.Font = new Font("Segoe UI", 10);
            this.Controls.Add(mainMenu);
            this.MainMenuStrip = mainMenu;

            // Пункт "Файл" - объявляем как поле
            fileMenu = new ToolStripMenuItem("📁 Файл");
            fileMenu.DropDownItems.Add("📊 Отчеты", null, (s, e) => ShowMessage("Функция отчетов будет реализована позже"));
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add("🚪 Выход", null, (s, e) => ExitApplication());
            mainMenu.Items.Add(fileMenu);

            // Пункт "Товары"
            productsMenu = new ToolStripMenuItem("📦 Товары");
            productsMenu.DropDownItems.Add("📋 Список товаров", null, (s, e) => ShowProductsForm());
            productsMenu.DropDownItems.Add("➕ Добавить товар", null, (s, e) => ShowAddProductForm());
            productsMenu.DropDownItems.Add("🔍 Поиск товаров", null, (s, e) => ShowSearchForm());
            mainMenu.Items.Add(productsMenu);

            // Пункт "Операции" (только для работников и выше)
            if (currentUser.Role == UserRole.Worker || currentUser.Role == UserRole.Manager || currentUser.Role == UserRole.Admin)
            {
                operationsMenu = new ToolStripMenuItem("🔄 Операции");
                operationsMenu.DropDownItems.Add("📥 Приход товара", null, (s, e) => ShowMessage("Форма прихода товара будет реализована"));
                operationsMenu.DropDownItems.Add("📤 Расход товара", null, (s, e) => ShowMessage("Форма расхода товара будет реализована"));
                operationsMenu.DropDownItems.Add("🔄 Перемещение", null, (s, e) => ShowMessage("Форма перемещения товара будет реализована"));
                mainMenu.Items.Add(operationsMenu);
            }

            // Пункт "Справочники"
            referencesMenu = new ToolStripMenuItem("📚 Справочники");
            referencesMenu.DropDownItems.Add("🏢 Поставщики", null, (s, e) => ShowMessage("Управление поставщиками будет реализовано"));
            referencesMenu.DropDownItems.Add("📁 Категории", null, (s, e) => ShowMessage("Управление категориями будет реализовано"));
            mainMenu.Items.Add(referencesMenu);

            // Пункт "Администрирование" (только для админов и менеджеров)
            if (currentUser.Role == UserRole.Admin || currentUser.Role == UserRole.Manager)
            {
                adminMenu = new ToolStripMenuItem("⚙ Администрирование");
                adminMenu.DropDownItems.Add("👥 Пользователи", null, (s, e) => ShowUsersForm());
                adminMenu.DropDownItems.Add("📈 Статистика", null, (s, e) => ShowStatistics());
                mainMenu.Items.Add(adminMenu);
            }

            // Пункт "Справка"
            helpMenu = new ToolStripMenuItem("❓ Справка");
            helpMenu.DropDownItems.Add("ℹ О программе", null, (s, e) => ShowAbout());
            helpMenu.DropDownItems.Add("📖 Руководство", null, (s, e) => ShowMessage("Руководство пользователя будет добавлено"));
            mainMenu.Items.Add(helpMenu);

            // ===== 2. ПАНЕЛЬ ПРИВЕТСТВИЯ =====
            var welcomePanel = new Panel();
            welcomePanel.BackColor = Color.FromArgb(240, 248, 255);
            welcomePanel.BorderStyle = BorderStyle.FixedSingle;
            welcomePanel.Location = new Point(10, 35);
            welcomePanel.Size = new Size(970, 70);
            this.Controls.Add(welcomePanel);

            lblWelcome = new Label();
            lblWelcome.Text = $"👋 Добро пожаловать, {currentUser.Username}!";
            lblWelcome.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblWelcome.ForeColor = Color.FromArgb(33, 150, 243);
            lblWelcome.Location = new Point(20, 15);
            lblWelcome.Size = new Size(500, 30);
            welcomePanel.Controls.Add(lblWelcome);

            var lblRole = new Label();
            lblRole.Text = $"Роль: {GetRoleName(currentUser.Role)}";
            lblRole.Font = new Font("Segoe UI", 11);
            lblRole.ForeColor = Color.DarkSlateGray;
            lblRole.Location = new Point(20, 45);
            lblRole.Size = new Size(300, 25);
            welcomePanel.Controls.Add(lblRole);

            lblDateTime = new Label();
            lblDateTime.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            lblDateTime.Font = new Font("Segoe UI", 10);
            lblDateTime.ForeColor = Color.Gray;
            lblDateTime.TextAlign = ContentAlignment.MiddleRight;
            lblDateTime.Location = new Point(700, 20);
            lblDateTime.Size = new Size(250, 30);
            welcomePanel.Controls.Add(lblDateTime);

            // ===== 3. ВКЛАДКИ =====
            mainTabs = new TabControl();
            mainTabs.Location = new Point(10, 115);
            mainTabs.Size = new Size(970, 520);
            this.Controls.Add(mainTabs);

            var dashboardTab = new TabPage("📊 Дашборд");
            CreateDashboard(dashboardTab);
            mainTabs.TabPages.Add(dashboardTab);

            // ===== 4. СТАТУС БАР =====
            statusBar = new StatusStrip();
            statusBar.BackColor = Color.FromArgb(240, 240, 240);

            // Добавляем элементы в статус бар
            var userStatus = new ToolStripStatusLabel($"Пользователь: {currentUser.Username}");
            statusBar.Items.Add(userStatus);

            var separator = new ToolStripStatusLabel();
            separator.Spring = true; // Растягиваемый разделитель
            statusBar.Items.Add(separator);

            var readyStatus = new ToolStripStatusLabel("Готово");
            statusBar.Items.Add(readyStatus);

            this.Controls.Add(statusBar);
        }

        private void CreateDashboard(TabPage tab)
        {
            tab.BackColor = Color.White;

            // --- Заголовок ---
            var lblTitle = new Label();
            lblTitle.Text = "📊 Обзор системы";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(33, 150, 243);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Size = new Size(300, 35);
            tab.Controls.Add(lblTitle);

            // --- Панель статистики (ОНА БУДЕТ ОБНОВЛЯТЬСЯ) ---
            Panel statsPanel = null; // Объявляем, чтобы использовать в обработчике кнопки
            Label lblProducts = null, lblValue = null, lblUsers = null;

            Action UpdateStatsPanel = () =>
            {
                if (statsPanel != null) tab.Controls.Remove(statsPanel);

                statsPanel = new Panel();
                statsPanel.BorderStyle = BorderStyle.FixedSingle;
                statsPanel.BackColor = Color.AliceBlue;
                statsPanel.Location = new Point(20, 70);
                statsPanel.Size = new Size(920, 150);
                tab.Controls.Add(statsPanel);

                // --- Расчёт актуальных данных ---
                var productsCount = dataService.Products?.Count ?? 0;
                decimal totalValue = 0;
                if (dataService.Products != null)
                {
                    // ВАЖНО: Правильный расчёт! Цена * Количество
                    foreach (var product in dataService.Products)
                    {
                        totalValue += product.Price * product.Quantity;
                    }
                }
                var usersCount = dataService.Users?.Count ?? 0;

                // --- Отображение данных ---
                lblProducts = new Label();
                lblProducts.Text = $"📦 Всего товаров: {productsCount}";
                lblProducts.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                lblProducts.Location = new Point(20, 20);
                lblProducts.Size = new Size(300, 30);
                statsPanel.Controls.Add(lblProducts);

                lblValue = new Label();
                lblValue.Text = $"💰 Общая стоимость: {totalValue:C}";
                lblValue.Font = new Font("Segoe UI", 12);
                lblValue.Location = new Point(20, 60);
                lblValue.Size = new Size(300, 30);
                statsPanel.Controls.Add(lblValue);

                lblUsers = new Label();
                lblUsers.Text = $"👥 Пользователей: {usersCount}";
                lblUsers.Font = new Font("Segoe UI", 12);
                lblUsers.Location = new Point(20, 100);
                lblUsers.Size = new Size(300, 30);
                statsPanel.Controls.Add(lblUsers);

                // Добавляем информацию о поставщиках если есть
                if (dataService.Suppliers != null && dataService.Suppliers.Count > 0)
                {
                    var lblSuppliers = new Label();
                    lblSuppliers.Text = $"🏢 Поставщиков: {dataService.Suppliers.Count}";
                    lblSuppliers.Font = new Font("Segoe UI", 12);
                    lblSuppliers.Location = new Point(350, 20);
                    lblSuppliers.Size = new Size(300, 30);
                    statsPanel.Controls.Add(lblSuppliers);
                }
            };

            // Сразу обновляем панель при создании
            UpdateStatsPanel();

            // --- Быстрые действия ---
            var actionsTitle = new Label();
            actionsTitle.Text = "🚀 Быстрые действия:";
            actionsTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            actionsTitle.Location = new Point(20, 240);
            actionsTitle.Size = new Size(300, 30);
            tab.Controls.Add(actionsTitle);

            // Кнопка "Управление товарами"
            var btnProducts = new Button();
            btnProducts.Text = "📦 Управление товарами";
            btnProducts.Font = new Font("Segoe UI", 11);
            btnProducts.ForeColor = Color.White;
            btnProducts.BackColor = Color.FromArgb(33, 150, 243);
            btnProducts.Location = new Point(20, 280);
            btnProducts.Size = new Size(250, 45);
            btnProducts.Click += (s, e) => ShowProductsForm();
            tab.Controls.Add(btnProducts);

            // Кнопка "Обновить статистику" (ГЛАВНОЕ НОВОВВЕДЕНИЕ!)
            var btnRefresh = new Button();
            btnRefresh.Text = "🔄 Обновить статистику";
            btnRefresh.Font = new Font("Segoe UI", 11);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.BackColor = Color.FromArgb(255, 152, 0); // Оранжевый
            btnRefresh.Location = new Point(290, 280);
            btnRefresh.Size = new Size(250, 45);
            btnRefresh.Click += (s, e) => {
                UpdateStatsPanel(); // Вызываем обновление панели
                UpdateStatus("Статистика обновлена");
            };
            tab.Controls.Add(btnRefresh);

            // Кнопка для админов и менеджеров
            if (currentUser.Role == UserRole.Admin || currentUser.Role == UserRole.Manager)
            {
                var btnUsers = new Button();
                btnUsers.Text = "👥 Управление пользователями";
                btnUsers.Font = new Font("Segoe UI", 11);
                btnUsers.ForeColor = Color.White;
                btnUsers.BackColor = Color.FromArgb(156, 39, 176);
                btnUsers.Location = new Point(560, 280);
                btnUsers.Size = new Size(250, 45);
                btnUsers.Click += (s, e) => ShowUsersForm();
                tab.Controls.Add(btnUsers);
            }

            // --- Панель с подсказками ---
            var infoPanel = new Panel();
            infoPanel.BorderStyle = BorderStyle.FixedSingle;
            infoPanel.BackColor = Color.FromArgb(255, 253, 231);
            infoPanel.Location = new Point(20, 350);
            infoPanel.Size = new Size(920, 100);
            tab.Controls.Add(infoPanel);

            var lblInfo = new Label();
            lblInfo.Text = $"💡 Подсказка: Ваша роль '{GetRoleName(currentUser.Role)}' позволяет " +
                          GetRolePermissions(currentUser.Role) + "\n" +
                          "После добавления товара нажмите 'Обновить статистику'.";
            lblInfo.Font = new Font("Segoe UI", 10);
            lblInfo.Location = new Point(15, 15);
            lblInfo.Size = new Size(890, 70);
            infoPanel.Controls.Add(lblInfo);
        }

        // ===== МЕТОДЫ ДЛЯ МЕНЮ =====

        private void ShowProductsForm()
        {
            var productsForm = new ProductsForm(currentUser);
            productsForm.ShowDialog();
            UpdateStatus("Готово");
        }

        private void ShowAddProductForm()
        {
            var addForm = new AddEditProductForm(null, currentUser);
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Товар успешно добавлен!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            UpdateStatus("Готово");
        }

        private void ShowSearchForm()
        {
            // Создаем простую форму поиска
            var searchForm = new Form();
            searchForm.Text = "🔍 Поиск товаров";
            searchForm.Size = new Size(600, 400);
            searchForm.StartPosition = FormStartPosition.CenterParent;
            searchForm.ShowDialog();
            UpdateStatus("Готово");
        }

        private void ShowUsersForm()
        {
            if (currentUser.Role != UserRole.Admin && currentUser.Role != UserRole.Manager)
            {
                MessageBox.Show("Доступ запрещен! Только администраторы и менеджеры могут управлять пользователями.",
                    "Ошибка доступа", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Показываем простой список пользователей
            var usersText = "👥 Список пользователей:\n\n";
            if (dataService.Users != null)
            {
                foreach (var user in dataService.Users)
                {
                    usersText += $"• {user.Username} ({GetRoleName(user.Role)})\n";
                }
            }

            MessageBox.Show(usersText, "Пользователи системы",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            UpdateStatus("Готово");
        }

        private void ShowStatistics()
        {
            var statsForm = new Form();
            statsForm.Text = "📈 Статистика";
            statsForm.Size = new Size(500, 400);
            statsForm.StartPosition = FormStartPosition.CenterParent;

            // Добавляем простую статистику
            var lblStats = new Label();
            lblStats.Text = $"Статистика склада:\n\n" +
                           $"📦 Товаров: {dataService.Products?.Count ?? 0}\n" +
                           $"👥 Пользователей: {dataService.Users?.Count ?? 0}\n" +
                           $"🏢 Поставщиков: {dataService.Suppliers?.Count ?? 0}\n" +
                           $"📁 Категорий: {dataService.Categories?.Count ?? 0}";
            lblStats.Font = new Font("Segoe UI", 12);
            lblStats.Location = new Point(20, 20);
            lblStats.Size = new Size(460, 200);
            statsForm.Controls.Add(lblStats);

            statsForm.ShowDialog();
            UpdateStatus("Готово");
        }

        // ===== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ =====

        private void StartClock()
        {
            clockTimer = new Timer();
            clockTimer.Interval = 1000;
            clockTimer.Tick += (s, e) =>
            {
                lblDateTime.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            };
            clockTimer.Start();
        }

        private string GetRoleName(UserRole role)
        {
            switch (role)
            {
                case UserRole.Admin:
                    return "Администратор";
                case UserRole.Manager:
                    return "Менеджер";
                case UserRole.Worker:
                    return "Кладовщик";
                case UserRole.Viewer:
                    return "Наблюдатель";
                default:
                    return "Неизвестно";
            }
        }

        private string GetRolePermissions(UserRole role)
        {
            switch (role)
            {
                case UserRole.Admin:
                    return "полный доступ ко всем функциям системы.";
                case UserRole.Manager:
                    return "управление товарами, пользователями и просмотр отчетов.";
                case UserRole.Worker:
                    return "проведение операций с товарами (приход, расход, перемещение).";
                case UserRole.Viewer:
                    return "только просмотр информации без возможности изменений.";
                default:
                    return "ограниченный доступ.";
            }
        }

        private void UpdateStatus(string message)
        {
            if (statusBar.Items.Count > 2)
            {
                statusBar.Items[2].Text = message;
            }
        }

        private void ShowMessage(string message)
        {
            MessageBox.Show(message, "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            UpdateStatus("Готово");
        }

        private void ShowAbout()
        {
            MessageBox.Show(
                "🏭 Система складского учета\n\n" +
                $"Версия: 1.0.0\n" +
                $"Текущий пользователь: {currentUser.Username}\n" +
                $"Роль: {GetRoleName(currentUser.Role)}\n" +
                $"Дата: {DateTime.Now:dd.MM.yyyy}\n\n" +
                $"© {DateTime.Now.Year} Складской учет", 
                "О программе",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void ExitApplication()
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти из системы?",
                "Подтверждение выхода",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            clockTimer?.Stop();
            base.OnFormClosing(e);
        }
    }
}