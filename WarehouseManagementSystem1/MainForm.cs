using System;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagementSystem1.Enums;
using WarehouseManagementSystem1.Models;
using WarehouseManagementSystem1.Services;
using System.Collections.Generic;
using System.Linq;

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
        private ToolStripMenuItem reportsMenu;
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

            // ↓↓↓ ДОБАВЛЯЕМ ЭТОТ КОД ↓↓↓
            // Подписываемся на событие обновления данных
            dataService.DataChanged += DataService_DataChanged;
        }

        // ↓↓↓ ДОБАВЛЯЕМ ЭТОТ МЕТОД В КЛАСС (можно после конструктора) ↓↓↓
        private void DataService_DataChanged()
        {
            // Проверяем, что форма еще открыта и видима
            if (this.IsHandleCreated && !this.IsDisposed && this.Visible)
            {
                // Вызываем обновление в основном потоке
                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        // Обновляем только дашборд (первую вкладку)
                        if (mainTabs.TabPages.Count > 0 && mainTabs.SelectedIndex == 0)
                        {
                            RefreshDashboard();
                        }
                    });
                }
                else
                {
                    // Обновляем только дашборд (первую вкладку)
                    if (mainTabs.TabPages.Count > 0 && mainTabs.SelectedIndex == 0)
                    {
                        RefreshDashboard();
                    }
                }
            }
        }

        // ↓↓↓ В методе OnFormClosing ДОБАВЛЯЕМ отписку (примерно строка 900) ↓↓↓
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            clockTimer?.Stop();

            // Отписываемся от события
            dataService.DataChanged -= DataService_DataChanged;

            // Сохраняем данные при закрытии
            try
            {
                dataService.SaveToJson();
            }
            catch
            {
                // Игнорируем ошибки при сохранении при выходе
            }

            base.OnFormClosing(e);
        }

        private void CreateMainForm()
        {
            this.Text = $"🏠 Складской учет - {currentUser.Username} ({GetRoleName(currentUser.Role)})";
            this.Size = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = true;

            // ===== 1. МЕНЮ =====
            mainMenu = new MenuStrip();
            mainMenu.BackColor = Color.FromArgb(33, 150, 243);
            mainMenu.ForeColor = Color.White;
            mainMenu.Font = new Font("Segoe UI", 10);
            this.Controls.Add(mainMenu);
            this.MainMenuStrip = mainMenu;

            // Пункт "Файл"
            fileMenu = new ToolStripMenuItem("📁 Файл");
            fileMenu.DropDownItems.Add("📊 Отчеты", null, (s, e) => ShowReportsForm());
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add("🚪 Выход из системы", null, (s, e) => ExitApplication());
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
                // 1. Сначала создаем operationsMenu
                operationsMenu = new ToolStripMenuItem("🔄 Операции");

                // 2. Теперь добавляем в нее элементы
                operationsMenu.DropDownItems.Add("📄 Оформление накладной", null, (s, e) => ShowInvoiceForm());
                operationsMenu.DropDownItems.Add(new ToolStripSeparator());
                operationsMenu.DropDownItems.Add("📥 Приход товара", null, (s, e) => ShowTransactionForm(TransactionType.Приход));
                operationsMenu.DropDownItems.Add("📤 Расход товара", null, (s, e) => ShowTransactionForm(TransactionType.Расход));
                operationsMenu.DropDownItems.Add("🔄 Перемещение товара", null, (s, e) => ShowTransactionForm(TransactionType.Перемещение));
                operationsMenu.DropDownItems.Add(new ToolStripSeparator());
                operationsMenu.DropDownItems.Add("📋 История операций", null, (s, e) => ShowTransactionHistory());

                // 3. Добавляем подменю "Справочники"
                var referencesSubMenu = new ToolStripMenuItem("📚 Справочники");
                referencesSubMenu.DropDownItems.Add("🏢 Поставщики", null, (s, e) => ShowSuppliersForm());
                referencesSubMenu.DropDownItems.Add("👥 Клиенты", null, (s, e) => ShowCustomersForm());
                referencesSubMenu.DropDownItems.Add("📁 Категории", null, (s, e) => ShowCategoriesForm());
                referencesSubMenu.DropDownItems.Add("📍 Местоположения", null, (s, e) => ShowLocationsForm());
                operationsMenu.DropDownItems.Add(referencesSubMenu);

                operationsMenu.DropDownItems.Add(new ToolStripSeparator());
                operationsMenu.DropDownItems.Add("📋 Архив накладных", null, (s, e) => ShowInvoicesArchive());

                // 4. Добавляем operationsMenu в главное меню
                mainMenu.Items.Add(operationsMenu);
            }


            // Пункт "Отчеты"
            reportsMenu = new ToolStripMenuItem("📈 Отчеты");
            reportsMenu.DropDownItems.Add("📊 Общий отчет", null, (s, e) => ShowReportsForm());
            reportsMenu.DropDownItems.Add("📉 Товары с низким запасом", null, (s, e) => ShowLowStockReport());
            reportsMenu.DropDownItems.Add("💰 Самые дорогие товары", null, (s, e) => ShowExpensiveProductsReport());
            reportsMenu.DropDownItems.Add("🏷️ Сводка по категориям", null, (s, e) => ShowCategorySummaryReport());
            mainMenu.Items.Add(reportsMenu);

            // Пункт "Справочники"
            referencesMenu = new ToolStripMenuItem("📚 Справочники");
            referencesMenu.DropDownItems.Add("🏢 Поставщики", null, (s, e) => ShowSuppliersForm());
            referencesMenu.DropDownItems.Add("📁 Категории", null, (s, e) => ShowCategoriesForm());
            referencesMenu.DropDownItems.Add("📍 Местоположения", null, (s, e) => ShowLocationsForm());
            mainMenu.Items.Add(referencesMenu);

            // Пункт "Администрирование" (только для админов и менеджеров)
            if (currentUser.Role == UserRole.Admin || currentUser.Role == UserRole.Manager)
            {
                adminMenu = new ToolStripMenuItem("⚙ Администрирование");
                adminMenu.DropDownItems.Add("👥 Пользователи", null, (s, e) => ShowUsersForm());
                adminMenu.DropDownItems.Add("⚙ Настройки системы", null, (s, e) => ShowSystemSettings());
                adminMenu.DropDownItems.Add("🔄 Резервное копирование", null, (s, e) => ShowBackupForm());
                mainMenu.Items.Add(adminMenu);
            }

            // Пункт "Справка"
            helpMenu = new ToolStripMenuItem("❓ Справка");
            helpMenu.DropDownItems.Add("ℹ О программе", null, (s, e) => ShowAbout());
            helpMenu.DropDownItems.Add("📖 Руководство пользователя", null, (s, e) => ShowUserManual());
            helpMenu.DropDownItems.Add("🐛 Техническая поддержка", null, (s, e) => ShowSupportForm());
            mainMenu.Items.Add(helpMenu);

            // ===== 2. ПАНЕЛЬ ПРИВЕТСТВИЯ =====
            var welcomePanel = new Panel();
            welcomePanel.BackColor = Color.FromArgb(240, 248, 255);
            welcomePanel.BorderStyle = BorderStyle.FixedSingle;
            welcomePanel.Location = new Point(10, 35);
            welcomePanel.Size = new Size(1065, 80);
            this.Controls.Add(welcomePanel);

            lblWelcome = new Label();
            lblWelcome.Text = $"👋 Добро пожаловать, {currentUser.Username}!";
            lblWelcome.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblWelcome.ForeColor = Color.FromArgb(33, 150, 243);
            lblWelcome.Location = new Point(20, 20);
            lblWelcome.Size = new Size(500, 35);
            welcomePanel.Controls.Add(lblWelcome);

            var lblRole = new Label();
            lblRole.Text = $"Роль: {GetRoleName(currentUser.Role)} | Права: {GetRolePermissions(currentUser.Role)}";
            lblRole.Font = new Font("Segoe UI", 10);
            lblRole.ForeColor = Color.DarkSlateGray;
            lblRole.Location = new Point(20, 50);
            lblRole.Size = new Size(600, 25);
            welcomePanel.Controls.Add(lblRole);

            // Панель с быстрыми действиями в приветствии
            var quickActionsPanel = new Panel();
            quickActionsPanel.BackColor = Color.Transparent;
            quickActionsPanel.Location = new Point(650, 15);
            quickActionsPanel.Size = new Size(400, 50);
            welcomePanel.Controls.Add(quickActionsPanel);

            // Кнопка быстрого прихода
            if (currentUser.Role == UserRole.Worker || currentUser.Role == UserRole.Manager || currentUser.Role == UserRole.Admin)
            {
                var btnQuickIncome = new Button
                {
                    Text = "📥 Быстрый приход",
                    Location = new Point(0, 0),
                    Size = new Size(120, 30),
                    BackColor = Color.FromArgb(76, 175, 80),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 8, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat
                };
                btnQuickIncome.Click += (s, e) => ShowTransactionForm(TransactionType.Приход);
                quickActionsPanel.Controls.Add(btnQuickIncome);
            }

            // Кнопка быстрого добавления товара
            var btnQuickAdd = new Button
            {
                Text = "➕ Новый товар",
                Location = new Point(125, 0),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnQuickAdd.Click += (s, e) => ShowAddProductForm();
            quickActionsPanel.Controls.Add(btnQuickAdd);

            // Кнопка обновления
            var btnRefreshDashboard = new Button
            {
                Text = "🔄 Обновить",
                Location = new Point(250, 0),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnRefreshDashboard.Click += (s, e) => RefreshDashboard();
            quickActionsPanel.Controls.Add(btnRefreshDashboard);

            lblDateTime = new Label();
            lblDateTime.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            lblDateTime.Font = new Font("Segoe UI", 10);
            lblDateTime.ForeColor = Color.Gray;
            lblDateTime.TextAlign = ContentAlignment.MiddleRight;
            lblDateTime.Location = new Point(650, 50);
            lblDateTime.Size = new Size(400, 30);
            welcomePanel.Controls.Add(lblDateTime);

            // ===== 3. ВКЛАДКИ =====
            mainTabs = new TabControl();
            mainTabs.Location = new Point(10, 125);
            mainTabs.Size = new Size(1065, 565);
            mainTabs.SelectedIndexChanged += (s, e) => OnTabChanged();
            this.Controls.Add(mainTabs);

            // Вкладка Дашборд
            var dashboardTab = new TabPage("📊 Дашборд");
            CreateDashboard(dashboardTab);
            mainTabs.TabPages.Add(dashboardTab);

            // Вкладка Быстрый доступ (только если есть права)
            if (currentUser.Role == UserRole.Worker || currentUser.Role == UserRole.Manager || currentUser.Role == UserRole.Admin)
            {
                var quickAccessTab = new TabPage("⚡ Быстрый доступ");
                CreateQuickAccessTab(quickAccessTab);
                mainTabs.TabPages.Add(quickAccessTab);
            }

            // ===== 4. СТАТУС БАР =====
            statusBar = new StatusStrip();
            statusBar.BackColor = Color.FromArgb(240, 240, 240);
            statusBar.Font = new Font("Segoe UI", 9);

            // Добавляем элементы в статус бар
            var userStatus = new ToolStripStatusLabel($"👤 {currentUser.Username} ({GetRoleName(currentUser.Role)})");
            statusBar.Items.Add(userStatus);

            var separator = new ToolStripStatusLabel();
            separator.Spring = true;
            separator.Text = "";
            statusBar.Items.Add(separator);

            var dbStatus = new ToolStripStatusLabel($"📁 Данных: {dataService.Products?.Count ?? 0} товаров, {dataService.Transactions?.Count ?? 0} операций");
            statusBar.Items.Add(dbStatus);

            var separator2 = new ToolStripStatusLabel();
            separator2.Width = 20;
            statusBar.Items.Add(separator2);

            var readyStatus = new ToolStripStatusLabel("✅ Готово");
            readyStatus.Name = "statusReady";
            statusBar.Items.Add(readyStatus);

            this.Controls.Add(statusBar);
        }
        private void ShowInvoiceForm()
        {
            using (var form = new InvoiceTypeSelectionForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // Преобразуем InvoiceType в TransactionType
                    TransactionType transactionType;
                    switch (form.SelectedType)
                    {
                        case InvoiceType.Приходная:
                            transactionType = TransactionType.Приход;
                            break;
                        case InvoiceType.Расходная:
                            transactionType = TransactionType.Расход;
                            break;
                        case InvoiceType.Внутренняя:
                            transactionType = TransactionType.Перемещение;
                            break;
                        default:
                            transactionType = TransactionType.Приход;
                            break;
                    }

                    ShowTransactionForm(transactionType);
                }
            }
        }
        private void ShowCustomersForm()
        {
            using (var form = new CustomerManagerForm())
            {
                form.ShowDialog();
            }
        }

        private void ShowInvoicesArchive()
        {
            using (var form = new InvoiceArchiveForm())
            {
                form.ShowDialog();
            }
        }

        private void CreateDashboard(TabPage tab)
        {
            tab.BackColor = Color.White;
            tab.Padding = new Padding(10);

            // Заголовок дашборда
            var lblTitle = new Label();
            lblTitle.Text = "📊 Обзор системы";
            lblTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(33, 150, 243);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Size = new Size(400, 40);
            tab.Controls.Add(lblTitle);

            // Панель статистики
            Panel statsPanel = null;
            UpdateStatsPanelDelegate updateStats = () =>
            {
                if (statsPanel != null) tab.Controls.Remove(statsPanel);

                statsPanel = new Panel();
                statsPanel.BorderStyle = BorderStyle.FixedSingle;
                statsPanel.BackColor = Color.AliceBlue;
                statsPanel.Location = new Point(20, 70);
                statsPanel.Size = new Size(1020, 150);
                tab.Controls.Add(statsPanel);

                // Расчёт актуальных данных
                var productsCount = dataService.Products?.Count ?? 0;
                decimal totalValue = 0;
                int totalQuantity = 0;
                int lowStockCount = 0;

                if (dataService.Products != null)
                {
                    foreach (var product in dataService.Products)
                    {
                        totalValue += product.Price * product.Quantity;
                        totalQuantity += product.Quantity;
                        if (product.Quantity < 10) lowStockCount++;
                    }
                }

                var usersCount = dataService.Users?.Count ?? 0;
                var transactionsCount = dataService.Transactions?.Count ?? 0;
                var categoriesCount = dataService.Categories?.Count ?? 0;
                var suppliersCount = dataService.Suppliers?.Count ?? 0;

                // Карточка 1: Товары
                var card1 = CreateStatCard("📦 Товары", $"{productsCount}", "шт.", Color.FromArgb(33, 150, 243), new Point(20, 20));
                statsPanel.Controls.Add(card1);

                // Карточка 2: Стоимость
                var card2 = CreateStatCard("💰 Стоимость", $"{totalValue:C}", "общая", Color.FromArgb(76, 175, 80), new Point(200, 20));
                statsPanel.Controls.Add(card2);

                // Карточка 3: Мало товара
                var card3 = CreateStatCard("⚠ Мало на складе", $"{lowStockCount}", "товаров", Color.FromArgb(255, 152, 0), new Point(380, 20));
                statsPanel.Controls.Add(card3);

                // Карточка 4: Операции
                var card4 = CreateStatCard("🔄 Операции", $"{transactionsCount}", "шт.", Color.FromArgb(156, 39, 176), new Point(560, 20));
                statsPanel.Controls.Add(card4);

                // Карточка 5: Пользователи
                var card5 = CreateStatCard("👥 Пользователи", $"{usersCount}", "чел.", Color.FromArgb(244, 67, 54), new Point(740, 20));
                statsPanel.Controls.Add(card5);

                // Карточка 6: Поставщики
                var card6 = CreateStatCard("🏢 Поставщики", $"{suppliersCount}", "комп.", Color.FromArgb(0, 150, 136), new Point(20, 100));
                statsPanel.Controls.Add(card6);

                // Карточка 7: Категории
                var card7 = CreateStatCard("📁 Категории", $"{categoriesCount}", "шт.", Color.FromArgb(121, 85, 72), new Point(200, 100));
                statsPanel.Controls.Add(card7);

                // Карточка 8: Общее количество
                var card8 = CreateStatCard("📊 Общее кол-во", $"{totalQuantity}", "единиц", Color.FromArgb(96, 125, 139), new Point(380, 100));
                statsPanel.Controls.Add(card8);
            };

            // Сразу обновляем панель
            updateStats();

            // Быстрые действия
            var actionsTitle = new Label();
            actionsTitle.Text = "🚀 Быстрые действия:";
            actionsTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            actionsTitle.Location = new Point(20, 240);
            actionsTitle.Size = new Size(300, 30);
            tab.Controls.Add(actionsTitle);

            int actionY = 280;
            int actionX = 20;

            // Кнопка "Управление товарами"
            var btnProducts = CreateActionButton("📦 Управление товарами",
                Color.FromArgb(33, 150, 243), new Point(actionX, actionY));
            btnProducts.Click += (s, e) => ShowProductsForm();
            tab.Controls.Add(btnProducts);
            actionX += 220;

            // Кнопка "Провести операцию"
            if (currentUser.Role == UserRole.Worker || currentUser.Role == UserRole.Manager || currentUser.Role == UserRole.Admin)
            {
                var btnTransaction = CreateActionButton("🔄 Провести операцию",
                    Color.FromArgb(76, 175, 80), new Point(actionX, actionY));
                btnTransaction.Click += (s, e) => ShowTransactionForm(TransactionType.Приход);
                tab.Controls.Add(btnTransaction);
                actionX += 220;
            }

            // Кнопка "История операций"
            if (currentUser.Role == UserRole.Manager || currentUser.Role == UserRole.Admin)
            {
                var btnHistory = CreateActionButton("📋 История операций",
                    Color.FromArgb(156, 39, 176), new Point(actionX, actionY));
                btnHistory.Click += (s, e) => ShowTransactionHistory();
                tab.Controls.Add(btnHistory);
                actionX += 220;
            }

            // Кнопка "Обновить статистику"
            var btnRefresh = CreateActionButton("🔄 Обновить статистику",
                Color.FromArgb(255, 152, 0), new Point(actionX, actionY));
            btnRefresh.Click += (s, e) =>
            {
                updateStats();
                UpdateStatus("Статистика обновлена");
            };
            tab.Controls.Add(btnRefresh);

            // Панель с последними операциями
            if (dataService.Transactions != null && dataService.Transactions.Count > 0)
            {
                var recentPanel = new Panel();
                recentPanel.BorderStyle = BorderStyle.FixedSingle;
                recentPanel.BackColor = Color.FromArgb(255, 253, 231);
                recentPanel.Location = new Point(20, 340);
                recentPanel.Size = new Size(1020, 180);
                tab.Controls.Add(recentPanel);

                var recentTitle = new Label();
                recentTitle.Text = "📝 Последние операции:";
                recentTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                recentTitle.Location = new Point(10, 10);
                recentTitle.Size = new Size(300, 25);
                recentPanel.Controls.Add(recentTitle);

                // Таблица последних операций
                var recentGrid = new DataGridView();
                recentGrid.Location = new Point(10, 40);
                recentGrid.Size = new Size(1000, 130);
                recentGrid.AllowUserToAddRows = false;
                recentGrid.ReadOnly = true;
                recentGrid.RowHeadersVisible = false;
                recentGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                recentGrid.BackgroundColor = Color.White;

                // Наполняем данными
                var recentTransactions = dataService.Transactions
                    .OrderByDescending(t => t.TransactionDate)
                    .Take(10)
                    .ToList();

                recentGrid.Columns.Add("Date", "Дата");
                recentGrid.Columns.Add("Type", "Тип");
                recentGrid.Columns.Add("Product", "Товар");
                recentGrid.Columns.Add("Quantity", "Кол-во");
                recentGrid.Columns.Add("User", "Пользователь");

                foreach (var transaction in recentTransactions)
                {
                    var product = dataService.Products?.FirstOrDefault(p => p.Id == transaction.ProductId);
                    var user = dataService.Users?.FirstOrDefault(u => u.Id == transaction.UserId);

                    recentGrid.Rows.Add(
                        transaction.TransactionDate.ToString("dd.MM.yyyy HH:mm"),
                        GetTransactionTypeName(transaction.Type),
                        product?.Name ?? "Неизвестно",
                        transaction.Quantity,
                        user?.Username ?? "Неизвестно"
                    );
                }

                recentPanel.Controls.Add(recentGrid);
            }

            // Информационная панель
            var infoPanel = new Panel();
            infoPanel.BorderStyle = BorderStyle.FixedSingle;
            infoPanel.BackColor = Color.FromArgb(240, 248, 255);
            infoPanel.Location = new Point(20, 530);
            infoPanel.Size = new Size(1020, 80);
            tab.Controls.Add(infoPanel);

            var lblInfo = new Label();
            lblInfo.Text = $"💡 Подсказка: Ваша роль '{GetRoleName(currentUser.Role)}' позволяет " +
                          GetRolePermissions(currentUser.Role) + "\n" +
                          "Используйте вкладки и меню для навигации по системе.";
            lblInfo.Font = new Font("Segoe UI", 10);
            lblInfo.Location = new Point(15, 15);
            lblInfo.Size = new Size(990, 50);
            infoPanel.Controls.Add(lblInfo);
        }

        private void CreateQuickAccessTab(TabPage tab)
        {
            tab.BackColor = Color.White;
            tab.Padding = new Padding(10);

            var title = new Label();
            title.Text = "⚡ Быстрый доступ к операциям";
            title.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(33, 150, 243);
            title.Location = new Point(20, 20);
            title.Size = new Size(400, 40);
            tab.Controls.Add(title);

            int y = 80;
            int x = 20;

            // Быстрый приход
            var incomeCard = CreateQuickAccessCard("📥 Быстрый приход",
                "Добавление товара на склад",
                Color.FromArgb(76, 175, 80),
                new Point(x, y));
            incomeCard.Click += (s, e) => ShowTransactionForm(TransactionType.Приход);
            tab.Controls.Add(incomeCard);
            x += 260;

            // Быстрый расход
            var outcomeCard = CreateQuickAccessCard("📤 Быстрый расход",
                "Списание товара со склада",
                Color.FromArgb(244, 67, 54),
                new Point(x, y));
            outcomeCard.Click += (s, e) => ShowTransactionForm(TransactionType.Расход);
            tab.Controls.Add(outcomeCard);
            x += 260;

            // Перемещение
            var moveCard = CreateQuickAccessCard("🔄 Перемещение",
                "Перемещение товара между местами",
                Color.FromArgb(33, 150, 243),
                new Point(x, y));
            moveCard.Click += (s, e) => ShowTransactionForm(TransactionType.Перемещение);
            tab.Controls.Add(moveCard);
            x += 260;

            // Новая строка
            x = 20;
            y += 140;

            // Добавление товара
            var addProductCard = CreateQuickAccessCard("➕ Новый товар",
                "Добавить новый товар в систему",
                Color.FromArgb(255, 152, 0),
                new Point(x, y));
            addProductCard.Click += (s, e) => ShowAddProductForm();
            tab.Controls.Add(addProductCard);
            x += 260;

            // История операций
            var historyCard = CreateQuickAccessCard("📋 История",
                "Просмотр истории операций",
                Color.FromArgb(156, 39, 176),
                new Point(x, y));
            historyCard.Click += (s, e) => ShowTransactionHistory();
            tab.Controls.Add(historyCard);
            x += 260;

            // Отчеты
            var reportsCard = CreateQuickAccessCard("📊 Отчеты",
                "Формирование отчетов",
                Color.FromArgb(0, 150, 136),
                new Point(x, y));
            reportsCard.Click += (s, e) => ShowReportsForm();
            tab.Controls.Add(reportsCard);

            // Информационная панель
            var infoPanel = new Panel();
            infoPanel.BorderStyle = BorderStyle.FixedSingle;
            infoPanel.BackColor = Color.FromArgb(255, 253, 231);
            infoPanel.Location = new Point(20, 350);
            infoPanel.Size = new Size(1020, 100);
            tab.Controls.Add(infoPanel);

            var lblInfo = new Label();
            lblInfo.Text = "💡 Быстрый доступ позволяет выполнять часто используемые операции в один клик.\n" +
                          "Для доступа ко всем функциям используйте меню в верхней части окна.";
            lblInfo.Font = new Font("Segoe UI", 10);
            lblInfo.Location = new Point(15, 15);
            lblInfo.Size = new Size(990, 70);
            infoPanel.Controls.Add(lblInfo);
        }

        // ===== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ДЛЯ СОЗДАНИЯ КОМПОНЕНТОВ =====

        private Panel CreateStatCard(string title, string value, string unit, Color color, Point location)
        {
            var panel = new Panel();
            panel.BackColor = Color.White;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Location = location;
            panel.Size = new Size(170, 70);

            // Левая цветная полоса
            var colorStrip = new Panel();
            colorStrip.BackColor = color;
            colorStrip.Dock = DockStyle.Left;
            colorStrip.Width = 5;
            panel.Controls.Add(colorStrip);

            // Заголовок
            var lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblTitle.ForeColor = Color.DarkSlateGray;
            lblTitle.Location = new Point(10, 10);
            lblTitle.Size = new Size(150, 20);
            panel.Controls.Add(lblTitle);

            // Значение
            var lblValue = new Label();
            lblValue.Text = value;
            lblValue.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblValue.ForeColor = color;
            lblValue.Location = new Point(10, 30);
            lblValue.Size = new Size(120, 25);
            panel.Controls.Add(lblValue);

            // Единица измерения
            var lblUnit = new Label();
            lblUnit.Text = unit;
            lblUnit.Font = new Font("Segoe UI", 8);
            lblUnit.ForeColor = Color.Gray;
            lblUnit.Location = new Point(130, 38);
            lblUnit.Size = new Size(40, 20);
            panel.Controls.Add(lblUnit);

            return panel;
        }

        private Button CreateActionButton(string text, Color color, Point location)
        {
            return new Button
            {
                Text = text,
                Location = location,
                Size = new Size(200, 50),
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleCenter
            };
        }

        private Panel CreateQuickAccessCard(string title, string description, Color color, Point location)
        {
            var panel = new Panel();
            panel.BackColor = Color.White;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Location = location;
            panel.Size = new Size(240, 120);
            panel.Cursor = Cursors.Hand;

            // Верхняя цветная полоса
            var headerPanel = new Panel();
            headerPanel.BackColor = color;
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 40;
            panel.Controls.Add(headerPanel);

            // Заголовок в заголовке
            var lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            headerPanel.Controls.Add(lblTitle);

            // Описание
            var lblDesc = new Label();
            lblDesc.Text = description;
            lblDesc.Font = new Font("Segoe UI", 9);
            lblDesc.ForeColor = Color.DarkSlateGray;
            lblDesc.Location = new Point(10, 50);
            lblDesc.Size = new Size(220, 60);
            lblDesc.TextAlign = ContentAlignment.MiddleCenter;
            panel.Controls.Add(lblDesc);

            // Эффект при наведении
            panel.MouseEnter += (s, e) => panel.BackColor = Color.FromArgb(245, 245, 245);
            panel.MouseLeave += (s, e) => panel.BackColor = Color.White;

            return panel;
        }

        // ===== ОСНОВНЫЕ МЕТОДЫ ДЛЯ МЕНЮ =====

        private void ShowProductsForm()
        {
            var productsForm = new ProductsForm(currentUser);
            productsForm.ShowDialog();
            UpdateStatus("Готово");
        }

        private void ShowAddProductForm()
        {
            var addForm = new AddEditProductForm();
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Товар успешно добавлен!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateStatus("Товар добавлен");
            }
        }

        private void ShowSearchForm()
        {
            // Упрощенная форма поиска
            var searchForm = new Form();
            searchForm.Text = "🔍 Поиск товаров";
            searchForm.Size = new Size(500, 300);
            searchForm.StartPosition = FormStartPosition.CenterParent;

            var lblSearch = new Label { Text = "Введите название или артикул:", Location = new Point(50, 50), Size = new Size(200, 25) };
            var txtSearch = new TextBox { Location = new Point(50, 80), Size = new Size(400, 25) };
            var btnSearch = new Button { Text = "Искать", Location = new Point(50, 120), Size = new Size(100, 35) };

            btnSearch.Click += (s, e) =>
            {
                // В реальной системе здесь будет вызов поиска
                MessageBox.Show("Поиск будет реализован в следующей версии", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            searchForm.Controls.AddRange(new Control[] { lblSearch, txtSearch, btnSearch });
            searchForm.ShowDialog();
            UpdateStatus("Готово");
        }

        private InvoiceType ConvertToInvoiceType(TransactionType transactionType)
        {
            switch (transactionType)
            {
                case TransactionType.Приход:
                    return InvoiceType.Приходная;
                case TransactionType.Расход:
                    return InvoiceType.Расходная;
                default:
                    return InvoiceType.Внутренняя;
            }
        }

        private void ShowTransactionForm(TransactionType transactionType)
        {
            InvoiceType invoiceType = ConvertToInvoiceType(transactionType);

            using (var form = new InvoiceTransactionForm(currentUser, invoiceType))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    UpdateStatus($"Накладная оформлена успешно");
                    RefreshDashboard();
                }
            }
        }

        private void ShowTransactionHistory()
        {
            using (var form = new TransactionHistoryForm())
            {
                form.ShowDialog();
            }
            UpdateStatus("Готово");
        }

        private void ShowReportsForm()
        {
            using (var form = new ReportsForm())
            {
                form.ShowDialog();
            }
            UpdateStatus("Отчеты сформированы");
        }

        private void ShowLowStockReport()
        {
            MessageBox.Show("Отчет 'Товары с низким запасом' будет реализован в следующей версии",
                "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowExpensiveProductsReport()
        {
            MessageBox.Show("Отчет 'Самые дорогие товары' будет реализован в следующей версии",
                "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowCategorySummaryReport()
        {
            MessageBox.Show("Отчет 'Сводка по категориям' будет реализован в следующей версии",
                "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

       
        private void ShowCategoriesForm()
        {
            using (var form = new CategoryManagerForm())
            {
                form.ShowDialog();
            }
            UpdateStatus("Готово");
        }

        private void ShowSuppliersForm()
        {
            using (var form = new SupplierManagerForm())
            {
                form.ShowDialog();
            }
            UpdateStatus("Готово");
        }

        private void ShowLocationsForm()
        {
            MessageBox.Show("Управление местоположениями будет реализовано в следующей версии",
                "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowUsersForm()
        {
            if (currentUser.Role != UserRole.Admin && currentUser.Role != UserRole.Manager)
            {
                MessageBox.Show("Доступ запрещен! Только администраторы и менеджеры могут управлять пользователями.",
                    "Ошибка доступа", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Простой список пользователей
            var usersText = "👥 Список пользователей:\n\n";
            if (dataService.Users != null)
            {
                foreach (var user in dataService.Users)
                {
                    usersText += $"• {user.Username} ({GetRoleName(user.Role)}) - {user.Email ?? "нет email"}\n";
                }
            }

            MessageBox.Show(usersText, "Пользователи системы",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            UpdateStatus("Готово");
        }

        private void ShowSystemSettings()
        {
            MessageBox.Show("Настройки системы будут реализованы в следующей версии",
                "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowBackupForm()
        {
            MessageBox.Show("Резервное копирование будет реализовано в следующей версии",
                "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowUserManual()
        {
            var manualText = "📖 Руководство пользователя\n\n" +
                           "1. Управление товарами:\n" +
                           "   - 📦 Просмотр списка товаров\n" +
                           "   - ➕ Добавление новых товаров\n" +
                           "   - ✏️ Редактирование существующих\n\n" +
                           "2. Операции:\n" +
                           "   - 📥 Приход товара на склад\n" +
                           "   - 📤 Расход товара со склада\n" +
                           "   - 🔄 Перемещение между местами\n\n" +
                           "3. Отчеты:\n" +
                           "   - 📊 Общая статистика\n" +
                           "   - 📉 Анализ запасов\n\n" +
                           "4. Справочники:\n" +
                           "   - 🏢 Поставщики\n" +
                           "   - 📁 Категории\n\n" +
                           "Для получения помощи обращайтесь к администратору системы.";

            MessageBox.Show(manualText, "Руководство пользователя",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowSupportForm()
        {
            var supportForm = new Form();
            supportForm.Text = "🐛 Техническая поддержка";
            supportForm.Size = new Size(500, 400);
            supportForm.StartPosition = FormStartPosition.CenterParent;

            var lblTitle = new Label
            {
                Text = "Обращение в техническую поддержку",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(50, 20),
                Size = new Size(400, 30)
            };

            var lblMessage = new Label
            {
                Text = "Опишите вашу проблему:",
                Location = new Point(50, 70),
                Size = new Size(200, 25)
            };

            var txtMessage = new TextBox
            {
                Location = new Point(50, 100),
                Size = new Size(400, 150),
                Multiline = true
            };

            var btnSend = new Button
            {
                Text = "📧 Отправить обращение",
                Location = new Point(150, 270),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            btnSend.Click += (s, e) =>
            {
                MessageBox.Show("Ваше обращение отправлено. Мы свяжемся с вами в ближайшее время.",
                    "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                supportForm.Close();
            };

            supportForm.Controls.AddRange(new Control[] { lblTitle, lblMessage, txtMessage, btnSend });
            supportForm.ShowDialog();
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

        private void OnTabChanged()
        {
            if (mainTabs.SelectedTab != null)
            {
                UpdateStatus($"Активна вкладка: {mainTabs.SelectedTab.Text}");
            }
        }

        private void RefreshDashboard()
        {
            if (mainTabs.TabPages.Count > 0)
            {
                // Пересоздаем вкладку дашборда
                mainTabs.TabPages[0].Controls.Clear();
                CreateDashboard(mainTabs.TabPages[0]);
                UpdateStatus("Дашборд обновлен");
            }
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

        private string GetTransactionTypeName(TransactionType type)
        {
            switch (type)
            {
                case TransactionType.Приход:
                    return "Приход";
                case TransactionType.Расход:
                    return "Расход";
                case TransactionType.Перемещение:
                    return "Перемещение";
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
            var statusItem = statusBar.Items["statusReady"] as ToolStripStatusLabel;
            if (statusItem != null)
            {
                statusItem.Text = $"✅ {message}";
            }
        }

        private void ShowAbout()
        {
            var aboutText = $"🏭 Система складского учета\n\n" +
                           $"Версия: 2.0.0\n" +
                           $"Пользователь: {currentUser.Username}\n" +
                           $"Роль: {GetRoleName(currentUser.Role)}\n" +
                           $"Дата: {DateTime.Now:dd.MM.yyyy}\n" +
                           $"Время: {DateTime.Now:HH:mm:ss}\n\n" +
                           $"Функции системы:\n" +
                           $"• Управление товарами\n" +
                           $"• Проведение операций\n" +
                           $"• Формирование отчетов\n" +
                           $"• История операций\n\n" +
                           $"© {DateTime.Now.Year} Складской учет";

            MessageBox.Show(aboutText, "О программе",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        // Делегат для обновления панели статистики
        private delegate void UpdateStatsPanelDelegate();

    }
}