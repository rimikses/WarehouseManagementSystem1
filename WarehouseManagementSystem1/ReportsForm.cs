using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WarehouseManagementSystem1.Models;
using WarehouseManagementSystem1.Services;

namespace WarehouseManagementSystem1
{
    public class ReportsForm : Form
    {
        private DataGridView gridReport;
        private Button btnGenerate;
        private ComboBox cmbReportType;
        private Label lblTitle;

        public ReportsForm()
        {
            InitializeComponent();
            GenerateReport();

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
                        GenerateReport();
                    });
                }
                else
                {
                    GenerateReport();
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
            this.Text = "📈 Отчеты по складу";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Верхняя панель
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.WhiteSmoke, Padding = new Padding(10) };

            lblTitle = new Label
            {
                Text = "Выберите тип отчета:",
                Location = new Point(20, 10),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10)
            };

            cmbReportType = new ComboBox
            {
                Location = new Point(220, 10),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbReportType.Items.AddRange(new object[]
            {
                "Товары с низким запасом",
                "Самые дорогие товары",
                "Сводка по категориям"
            });
            cmbReportType.SelectedIndex = 0;
            cmbReportType.SelectedIndexChanged += (s, e) => GenerateReport();

            btnGenerate = new Button
            {
                Text = "🔄 Сформировать отчет",
                Location = new Point(440, 10),
                Size = new Size(180, 30),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnGenerate.Click += (s, e) => GenerateReport();

            topPanel.Controls.AddRange(new Control[] { lblTitle, cmbReportType, btnGenerate });

            // DataGridView для отчета
            gridReport = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                BackgroundColor = Color.White
            };

            this.Controls.AddRange(new Control[] { gridReport, topPanel });
        }

        private void GenerateReport()
        {
            try
            {
                var dataService = DataService.Instance;
                var products = dataService.Products ?? new List<Product>();

                gridReport.Columns.Clear();
                gridReport.Rows.Clear();

                switch (cmbReportType.SelectedIndex)
                {
                    case 0: // Товары с низким запасом
                        GenerateLowStockReport(products);
                        break;
                    case 1: // Самые дорогие товары
                        GenerateExpensiveProductsReport(products);
                        break;
                    case 2: // Сводка по категориям
                        GenerateCategorySummaryReport(products);
                        break;
                    default:
                        MessageBox.Show("Выберите тип отчета", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка формирования отчета: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateLowStockReport(List<Product> products)
        {
            lblTitle.Text = "📉 Товары с низким запасом (менее 10 единиц)";

            var lowStockProducts = products
                .Where(p => p.Quantity < 10)
                .OrderBy(p => p.Quantity)
                .ToList();

            // Настраиваем колонки
            gridReport.Columns.Add("Article", "Артикул");
            gridReport.Columns.Add("Name", "Название");
            gridReport.Columns.Add("Category", "Категория");
            gridReport.Columns.Add("Quantity", "Количество");
            gridReport.Columns.Add("Price", "Цена");
            gridReport.Columns.Add("TotalValue", "Общая стоимость");

            foreach (var product in lowStockProducts)
            {
                gridReport.Rows.Add(
                    product.Article,
                    product.Name,
                    product.Category,
                    product.Quantity,
                    product.Price.ToString("C"),
                    (product.Quantity * product.Price).ToString("C")
                );
            }

            // Подсветка строк
            foreach (DataGridViewRow row in gridReport.Rows)
            {
                row.DefaultCellStyle.BackColor = Color.LightPink;
            }
        }

        private void GenerateExpensiveProductsReport(List<Product> products)
        {
            lblTitle.Text = "💰 10 самых дорогих товаров";

            var expensiveProducts = products
                .OrderByDescending(p => p.Price)
                .Take(10)
                .ToList();

            gridReport.Columns.Add("Name", "Название");
            gridReport.Columns.Add("Price", "Цена");
            gridReport.Columns.Add("Quantity", "Количество");
            gridReport.Columns.Add("Category", "Категория");
            gridReport.Columns.Add("TotalValue", "Общая стоимость");

            foreach (var product in expensiveProducts)
            {
                gridReport.Rows.Add(
                    product.Name,
                    product.Price.ToString("C"),
                    product.Quantity,
                    product.Category,
                    (product.Quantity * product.Price).ToString("C")
                );
            }
        }

        private void GenerateCategorySummaryReport(List<Product> products)
        {
            lblTitle.Text = "🏷️ Сводка по категориям";

            var categorySummary = products
                .GroupBy(p => p.Category)
                .Select(g => new CategorySummary
                {
                    Категория = g.Key,
                    КоличествоТоваров = g.Count(),
                    ОбщееКоличество = g.Sum(p => p.Quantity),
                    СредняяЦена = g.Average(p => p.Price),
                    ОбщаяСтоимость = g.Sum(p => p.Quantity * p.Price)
                })
                .OrderByDescending(c => c.ОбщаяСтоимость)
                .ToList();

            gridReport.Columns.Add("Category", "Категория");
            gridReport.Columns.Add("ProductCount", "Кол-во товаров");
            gridReport.Columns.Add("TotalQuantity", "Общее количество");
            gridReport.Columns.Add("AvgPrice", "Средняя цена");
            gridReport.Columns.Add("TotalValue", "Общая стоимость");

            foreach (var category in categorySummary)
            {
                gridReport.Rows.Add(
                    category.Категория,
                    category.КоличествоТоваров,
                    category.ОбщееКоличество,
                    category.СредняяЦена.ToString("C"),
                    category.ОбщаяСтоимость.ToString("C")
                );
            }
        }

        // Вспомогательный класс для сводки по категориям
        private class CategorySummary
        {
            public string Категория { get; set; }
            public int КоличествоТоваров { get; set; }
            public int ОбщееКоличество { get; set; }
            public decimal СредняяЦена { get; set; }
            public decimal ОбщаяСтоимость { get; set; }
        }
    }
}