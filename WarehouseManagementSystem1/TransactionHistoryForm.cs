using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WarehouseManagementSystem1.Enums;
using WarehouseManagementSystem1.Models;
using WarehouseManagementSystem1.Services;

namespace WarehouseManagementSystem1
{
    public class TransactionHistoryForm : Form
    {
        private DataGridView dataGridView1;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private ComboBox cmbTransactionType;
        private ComboBox cmbProduct;
        private Button btnFilter;
        private Button btnClearFilter;
        private Button btnExport;
        private Label lblStats;

        private List<Transaction> allTransactions = new List<Transaction>();
        private DataService dataService = DataService.Instance;

        public TransactionHistoryForm()
        {
            InitializeComponent();
            LoadAllTransactions();
            LoadFilterData();
            ApplyFilters();

            // ↓↓↓ ДОБАВЛЯЕМ ЭТОТ КОД ↓↓↓
            // Подписываемся на событие обновления данных
            DataService.Instance.DataChanged += DataService_DataChanged;
        }

        // ↓↓↓ ДОБАВЛЯЕМ ЭТОТ МЕТОД В КЛАСС ↓↓↓
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
                        LoadAllTransactions();
                        ApplyFilters();
                    });
                }
                else
                {
                    LoadAllTransactions();
                    ApplyFilters();
                }
            }
        }

        // ↓↓↓ ДОБАВЛЯЕМ В КОНЕЦ КЛАССА ↓↓↓
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Отписываемся от события при закрытии формы
            DataService.Instance.DataChanged -= DataService_DataChanged;
            base.OnFormClosing(e);
        }

        private void InitializeComponent()
        {
            this.Text = "📋 История операций";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Панель фильтров
            var filterPanel = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = Color.WhiteSmoke, BorderStyle = BorderStyle.FixedSingle };

            int y = 10;
            int x = 10;

            // Период
            var lblPeriod = new Label { Text = "Период:", Location = new Point(x, y), Size = new Size(60, 25) };
            dtpStartDate = new DateTimePicker
            {
                Location = new Point(x + 65, y),
                Size = new Size(120, 25),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddDays(-30)
            };

            var lblTo = new Label { Text = "по", Location = new Point(x + 190, y), Size = new Size(20, 25) };
            dtpEndDate = new DateTimePicker
            {
                Location = new Point(x + 215, y),
                Size = new Size(120, 25),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };
            y += 30;

            // Тип операции
            var lblType = new Label { Text = "Тип операции:", Location = new Point(x, y), Size = new Size(90, 25) };
            cmbTransactionType = new ComboBox
            {
                Location = new Point(x + 95, y),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            y += 30;

            // Товар
            var lblProduct = new Label { Text = "Товар:", Location = new Point(x, y), Size = new Size(50, 25) };
            cmbProduct = new ComboBox
            {
                Location = new Point(x + 55, y),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Вторая колонка
            x = 400;
            y = 10;

            // Кнопки фильтров
            btnFilter = new Button
            {
                Text = "🔍 Применить фильтры",
                Location = new Point(x, y),
                Size = new Size(150, 30),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnFilter.Click += BtnFilter_Click;

            btnClearFilter = new Button
            {
                Text = "❌ Очистить фильтры",
                Location = new Point(x + 160, y),
                Size = new Size(150, 30),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            btnClearFilter.Click += BtnClearFilter_Click;
            y += 35;

            btnExport = new Button
            {
                Text = "📥 Экспорт в CSV",
                Location = new Point(x, y),
                Size = new Size(150, 30),
                BackColor = Color.FromArgb(156, 39, 176),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnExport.Click += BtnExport_Click;

            // Панель статистики
            lblStats = new Label
            {
                Location = new Point(x + 320, 10),
                Size = new Size(300, 60),
                Font = new Font("Arial", 9, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };

            filterPanel.Controls.AddRange(new Control[]
            {
                lblPeriod, dtpStartDate, lblTo, dtpEndDate,
                lblType, cmbTransactionType,
                lblProduct, cmbProduct,
                btnFilter, btnClearFilter, btnExport, lblStats
            });

            // DataGridView
            dataGridView1 = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                BackgroundColor = Color.White
            };

            this.Controls.AddRange(new Control[] { dataGridView1, filterPanel });
        }

        private void LoadAllTransactions()
        {
            allTransactions = dataService.Transactions ?? new List<Transaction>();
        }

        private void LoadFilterData()
        {
            // Типы операций
            cmbTransactionType.Items.Clear();
            cmbTransactionType.Items.Add("Все типы");
            cmbTransactionType.Items.AddRange(Enum.GetValues(typeof(TransactionType)).Cast<TransactionType>().Cast<object>().ToArray());
            cmbTransactionType.SelectedIndex = 0;

            // Товары
            var products = dataService.Products ?? new List<Product>();
            cmbProduct.Items.Clear();
            cmbProduct.Items.Add("Все товары");
            cmbProduct.Items.AddRange(products.OrderBy(p => p.Name).ToArray());
            cmbProduct.SelectedIndex = 0;
        }

        private void ApplyFilters()
        {
            try
            {
                var filtered = allTransactions.AsEnumerable();

                // Фильтр по дате
                filtered = filtered.Where(t => t.TransactionDate.Date >= dtpStartDate.Value.Date &&
                                               t.TransactionDate.Date <= dtpEndDate.Value.Date);

                // Фильтр по типу операции
                if (cmbTransactionType.SelectedIndex > 0)
                {
                    var selectedType = (TransactionType)cmbTransactionType.SelectedItem;
                    filtered = filtered.Where(t => t.Type == selectedType);
                }

                // Фильтр по товару
                if (cmbProduct.SelectedIndex > 0)
                {
                    var selectedProduct = cmbProduct.SelectedItem as Product;
                    filtered = filtered.Where(t => t.ProductId == selectedProduct.Id);
                }

                // Сортировка по дате (сначала новые)
                filtered = filtered.OrderByDescending(t => t.TransactionDate);

                var result = filtered.ToList();
                dataGridView1.DataSource = result;
                ConfigureGridViewColumns();
                UpdateStats(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка фильтрации: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureGridViewColumns()
        {
            if (dataGridView1.DataSource == null) return;

            dataGridView1.Columns.Clear();

            // Создаем столбцы
            var columns = new[]
            {
                new { Name = "TransactionDate", Header = "Дата", Width = 120 },
                new { Name = "Type", Header = "Тип операции", Width = 120 },
                new { Name = "ProductName", Header = "Товар", Width = 200 },
                new { Name = "Quantity", Header = "Количество", Width = 100 },
                new { Name = "FromLocation", Header = "Откуда", Width = 150 },
                new { Name = "ToLocation", Header = "Куда", Width = 150 },
                new { Name = "DocumentNumber", Header = "Документ", Width = 120 },
                new { Name = "UserName", Header = "Пользователь", Width = 120 },
                new { Name = "Comments", Header = "Комментарий", Width = 200 }
            };

            foreach (var col in columns)
            {
                var column = new DataGridViewTextBoxColumn
                {
                    Name = col.Name,
                    HeaderText = col.Header,
                    Width = col.Width,
                    ReadOnly = true
                };

                if (col.Name == "Quantity")
                {
                    column.DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight };
                }
                else if (col.Name == "TransactionDate")
                {
                    column.DefaultCellStyle = new DataGridViewCellStyle { Format = "dd.MM.yyyy HH:mm" };
                }

                dataGridView1.Columns.Add(column);
            }

            // Заполняем данными
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var transaction = row.DataBoundItem as Transaction;
                if (transaction != null)
                {
                    // Получаем название товара
                    var product = dataService.Products?.FirstOrDefault(p => p.Id == transaction.ProductId);
                    row.Cells["ProductName"].Value = product?.Name ?? "Неизвестно";

                    // Получаем имя пользователя
                    var user = dataService.Users?.FirstOrDefault(u => u.Id == transaction.UserId);
                    row.Cells["UserName"].Value = user?.Username ?? "Неизвестно";

                    // Раскрашиваем строки по типу операции
                    switch (transaction.Type)
                    {
                        case TransactionType.Приход:
                            row.DefaultCellStyle.BackColor = Color.FromArgb(220, 255, 220); // Светло-зеленый
                            break;
                        case TransactionType.Расход:
                            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220); // Светло-красный
                            break;
                        case TransactionType.Перемещение:
                            row.DefaultCellStyle.BackColor = Color.FromArgb(220, 220, 255); // Светло-синий
                            break;
                    }
                }
            }
        }

        private void UpdateStats(List<Transaction> transactions)
        {
            var total = transactions.Count;
            var income = transactions.Count(t => t.Type == TransactionType.Приход);
            var outcome = transactions.Count(t => t.Type == TransactionType.Расход);
            var move = transactions.Count(t => t.Type == TransactionType.Перемещение);
            var totalQuantity = transactions.Sum(t => t.Quantity);

            lblStats.Text = $"📊 Статистика:\n" +
                           $"Всего операций: {total}\n" +
                           $"Приход: {income} | Расход: {outcome} | Перемещение: {move}\n" +
                           $"Общее количество: {totalQuantity}";
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void BtnClearFilter_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Now.AddDays(-30);
            dtpEndDate.Value = DateTime.Now;
            cmbTransactionType.SelectedIndex = 0;
            cmbProduct.SelectedIndex = 0;
            ApplyFilters();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "CSV файлы (*.csv)|*.csv";
                    sfd.FileName = $"операции_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        var transactions = (List<Transaction>)dataGridView1.DataSource;

                        var lines = new List<string>();
                        lines.Add("Дата;Тип операции;Товар;Количество;Откуда;Куда;Документ;Пользователь;Комментарий");

                        foreach (var t in transactions)
                        {
                            var product = dataService.Products?.FirstOrDefault(p => p.Id == t.ProductId);
                            var user = dataService.Users?.FirstOrDefault(u => u.Id == t.UserId);

                            lines.Add($"\"{t.TransactionDate:dd.MM.yyyy HH:mm}\";" +
                                     $"\"{t.Type}\";" +
                                     $"\"{product?.Name ?? "Неизвестно"}\";" +
                                     $"{t.Quantity};" +
                                     $"\"{t.FromLocation ?? ""}\";" +
                                     $"\"{t.ToLocation ?? ""}\";" +
                                     $"\"{t.DocumentNumber ?? ""}\";" +
                                     $"\"{user?.Username ?? "Неизвестно"}\";" +
                                     $"\"{t.Comments ?? ""}\"");
                        }

                        System.IO.File.WriteAllLines(sfd.FileName, lines, System.Text.Encoding.UTF8);

                        MessageBox.Show($"Экспорт завершен!\nФайл: {sfd.FileName}\nЗаписей: {transactions.Count}",
                            "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}