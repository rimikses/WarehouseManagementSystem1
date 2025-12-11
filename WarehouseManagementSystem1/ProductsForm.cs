using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WarehouseManagementSystem1.Models;
using WarehouseManagementSystem1.Services;

namespace WarehouseManagementSystem1
{
    public class ProductsForm : Form
    {
        private DataGridView dataGridView1;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnEdit;
        private Button btnRefresh;
        private TextBox txtSearch;
        private ComboBox cmbCategoryFilter;
        private ComboBox cmbSortBy;
        private Button btnExport;
        private Label lblStats;
        private User currentUser;
        private NumericUpDown numMinPrice;
        private NumericUpDown numMaxPrice;
        private NumericUpDown numMinQuantity;
        private NumericUpDown numMaxQuantity;
        private Button btnClearFilters;
        private Label lblFilterCount;

        // Коллекция всех товаров для фильтрации
        private List<Product> allProducts = new List<Product>();

        public ProductsForm()
        {
            InitializeComponent();
            LoadAllProducts();
            ApplyFiltersAndSort();
            UpdateStats();
        }

        public ProductsForm(User user)
        {
            currentUser = user;
            InitializeComponent();
            LoadAllProducts();
            ApplyFiltersAndSort();
            UpdateStats();

            // ↓↓↓ ДОБАВЛЯЕМ ЭТОТ КОД ПОСЛЕ UpdateStats() ↓↓↓
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
                        LoadAllProducts();
                        ApplyFiltersAndSort();
                        UpdateStats();
                    });
                }
                else
                {
                    LoadAllProducts();
                    ApplyFiltersAndSort();
                    UpdateStats();
                }
            }

        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Отписываемся от события при закрытии формы
            DataService.Instance.DataChanged -= DataService_DataChanged;
            base.OnFormClosing(e);
        }


        private void InitializeComponent()
        {
            this.Text = "📦 Управление товарами";
            this.Size = new Size(1300, 750);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Панель фильтров
            var filterPanel = new Panel { Dock = DockStyle.Top, Height = 130, BackColor = Color.WhiteSmoke, BorderStyle = BorderStyle.FixedSingle };

            int y = 10;
            int x = 10;

            // 🔍 Поиск по названию и артикулу
            var lblSearch = new Label { Text = "Поиск:", Location = new Point(x, y), Size = new Size(50, 25), TextAlign = ContentAlignment.MiddleLeft };
            txtSearch = new TextBox
            {
                Location = new Point(x + 55, y),
                Size = new Size(200, 25),
                Text = "Название, артикул, описание..." // ЗАМЕНИЛИ PlaceholderText на Text
            };

            // Добавляем обработчики для имитации placeholder
            txtSearch.ForeColor = Color.Gray;
            txtSearch.Enter += (s, e) =>
            {
                if (txtSearch.Text == "Название, артикул, описание...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = Color.Black;
                }
            };
            txtSearch.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Название, артикул, описание...";
                    txtSearch.ForeColor = Color.Gray;
                }
            };

            txtSearch.TextChanged += TxtSearch_TextChanged;
            y += 30;

            // 🏷️ Фильтр по категории
            var lblCategory = new Label { Text = "Категория:", Location = new Point(x, y), Size = new Size(70, 25), TextAlign = ContentAlignment.MiddleLeft };
            cmbCategoryFilter = new ComboBox
            {
                Location = new Point(x + 75, y),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCategoryFilter.SelectedIndexChanged += CmbCategoryFilter_SelectedIndexChanged;
            y += 30;

            // 💰 Фильтр по цене
            var lblPrice = new Label { Text = "Цена от:", Location = new Point(x, y), Size = new Size(60, 25), TextAlign = ContentAlignment.MiddleLeft };
            numMinPrice = new NumericUpDown
            {
                Location = new Point(x + 65, y),
                Size = new Size(80, 25),
                Minimum = 0,
                Maximum = 1000000,
                DecimalPlaces = 2,
                Value = 0
            };
            numMinPrice.ValueChanged += Filter_ValueChanged;

            var lblPriceTo = new Label { Text = "до:", Location = new Point(x + 150, y), Size = new Size(30, 25), TextAlign = ContentAlignment.MiddleLeft };
            numMaxPrice = new NumericUpDown
            {
                Location = new Point(x + 185, y),
                Size = new Size(80, 25),
                Minimum = 0,
                Maximum = 1000000,
                DecimalPlaces = 2,
                Value = 1000000
            };
            numMaxPrice.ValueChanged += Filter_ValueChanged;
            y += 30;

            // 📦 Фильтр по количеству
            var lblQuantity = new Label { Text = "Кол-во от:", Location = new Point(x, y), Size = new Size(70, 25), TextAlign = ContentAlignment.MiddleLeft };
            numMinQuantity = new NumericUpDown
            {
                Location = new Point(x + 75, y),
                Size = new Size(80, 25),
                Minimum = 0,
                Maximum = 10000,
                Value = 0
            };
            numMinQuantity.ValueChanged += Filter_ValueChanged;

            var lblQuantityTo = new Label { Text = "до:", Location = new Point(x + 160, y), Size = new Size(30, 25), TextAlign = ContentAlignment.MiddleLeft };
            numMaxQuantity = new NumericUpDown
            {
                Location = new Point(x + 195, y),
                Size = new Size(80, 25),
                Minimum = 0,
                Maximum = 10000,
                Value = 10000
            };
            numMaxQuantity.ValueChanged += Filter_ValueChanged;

            // Вторая колонка фильтров (x = 300)
            x = 300;
            y = 10;

            // 📊 Сортировка
            var lblSort = new Label { Text = "Сортировка:", Location = new Point(x, y), Size = new Size(80, 25), TextAlign = ContentAlignment.MiddleLeft };
            cmbSortBy = new ComboBox
            {
                Location = new Point(x + 85, y),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbSortBy.Items.AddRange(new[] {
                "Название (А-Я)", "Название (Я-А)",
                "Цена (по возрастанию)", "Цена (по убыванию)",
                "Количество (по возрастанию)", "Количество (по убыванию)",
                "Артикул (А-Я)", "Артикул (Я-А)",
                "Дата обновления (сначала новые)"
            });
            cmbSortBy.SelectedIndex = 0;
            cmbSortBy.SelectedIndexChanged += CmbSortBy_SelectedIndexChanged;
            y += 30;

            // Кнопки управления фильтрами
            btnClearFilters = new Button
            {
                Text = "❌ Очистить фильтры",
                Location = new Point(x, y),
                Size = new Size(150, 30),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            btnClearFilters.Click += BtnClearFilters_Click;

            lblFilterCount = new Label
            {
                Location = new Point(x + 160, y),
                Size = new Size(150, 30),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Arial", 9, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };

            // Третья колонка - кнопки действий
            x = 600;
            y = 10;

            btnAdd = new Button
            {
                Text = "➕ Добавить товар",
                Location = new Point(x, y),
                Size = new Size(140, 35),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button
            {
                Text = "✏️ Редактировать",
                Location = new Point(x + 150, y),
                Size = new Size(140, 35),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            btnEdit.Click += BtnEdit_Click;
            y += 40;

            btnDelete = new Button
            {
                Text = "🗑️ Удалить",
                Location = new Point(x, y),
                Size = new Size(140, 35),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = new Button
            {
                Text = "🔄 Обновить",
                Location = new Point(x + 150, y),
                Size = new Size(140, 35),
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat
            };
            btnRefresh.Click += BtnRefresh_Click;

            btnExport = new Button
            {
                Text = "📥 Экспорт в CSV",
                Location = new Point(x + 300, 10),
                Size = new Size(140, 65),
                BackColor = Color.FromArgb(156, 39, 176),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnExport.Click += BtnExport_Click;

            // Добавляем все контролы на панель фильтров
            filterPanel.Controls.AddRange(new Control[] {
                lblSearch, txtSearch,
                lblCategory, cmbCategoryFilter,
                lblPrice, numMinPrice, lblPriceTo, numMaxPrice,
                lblQuantity, numMinQuantity, lblQuantityTo, numMaxQuantity,
                lblSort, cmbSortBy,
                btnClearFilters, lblFilterCount,
                btnAdd, btnEdit, btnDelete, btnRefresh, btnExport
            });

            // DataGridView
            dataGridView1 = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
            dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;

            // Панель статистики
            var statsPanel = new Panel { Dock = DockStyle.Bottom, Height = 40, BackColor = Color.LightGray };
            lblStats = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Arial", 10, FontStyle.Bold) };
            statsPanel.Controls.Add(lblStats);

            this.Controls.AddRange(new Control[] { dataGridView1, statsPanel, filterPanel });
        }

        private void LoadAllProducts()
        {
            try
            {
                allProducts = DataService.LoadProducts();
                LoadCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                allProducts = new List<Product>();
            }
        }

        private void LoadCategories()
        {
            try
            {
                var categories = allProducts
                    .Select(p => p.Category)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();

                cmbCategoryFilter.Items.Clear();
                cmbCategoryFilter.Items.Add("Все категории");
                cmbCategoryFilter.Items.AddRange(categories.ToArray());
                cmbCategoryFilter.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyFiltersAndSort()
        {
            if (allProducts == null || allProducts.Count == 0)
            {
                dataGridView1.DataSource = null;
                UpdateStats();
                return;
            }

            try
            {
                // 1. Применяем все фильтры
                var filtered = allProducts.AsEnumerable();

                // Поиск по тексту (игнорируем placeholder текст)
                var searchText = txtSearch.Text;
                if (searchText != "Название, артикул, описание..." && !string.IsNullOrWhiteSpace(searchText))
                {
                    searchText = searchText.ToLower();
                    filtered = filtered.Where(p =>
                        (p.Name != null && p.Name.ToLower().Contains(searchText)) ||
                        (p.Article != null && p.Article.ToLower().Contains(searchText)) ||
                        (p.Description != null && p.Description.ToLower().Contains(searchText)));
                }

                // Фильтр по категории
                if (cmbCategoryFilter.SelectedIndex > 0)
                {
                    var selectedCategory = cmbCategoryFilter.SelectedItem.ToString();
                    filtered = filtered.Where(p => p.Category == selectedCategory);
                }

                // Фильтр по цене
                filtered = filtered.Where(p => p.Price >= numMinPrice.Value && p.Price <= numMaxPrice.Value);

                // Фильтр по количеству
                filtered = filtered.Where(p => p.Quantity >= (int)numMinQuantity.Value && p.Quantity <= (int)numMaxQuantity.Value);

                // 2. Применяем сортировку
                switch (cmbSortBy.SelectedIndex)
                {
                    case 0: filtered = filtered.OrderBy(p => p.Name); break; // Название (А-Я)
                    case 1: filtered = filtered.OrderByDescending(p => p.Name); break; // Название (Я-А)
                    case 2: filtered = filtered.OrderBy(p => p.Price); break; // Цена (возрастание)
                    case 3: filtered = filtered.OrderByDescending(p => p.Price); break; // Цена (убывание)
                    case 4: filtered = filtered.OrderBy(p => p.Quantity); break; // Количество (возрастание)
                    case 5: filtered = filtered.OrderByDescending(p => p.Quantity); break; // Количество (убывание)
                    case 6: filtered = filtered.OrderBy(p => p.Article); break; // Артикул (А-Я)
                    case 7: filtered = filtered.OrderByDescending(p => p.Article); break; // Артикул (Я-А)
                    case 8: filtered = filtered.OrderByDescending(p => p.LastUpdated); break; // Дата обновления
                }

                var result = filtered.ToList();
                dataGridView1.DataSource = result;
                ConfigureGridViewColumns();
                UpdateStats();

                // Обновляем счетчик отфильтрованных записей
                lblFilterCount.Text = $"Найдено: {result.Count} из {allProducts.Count}";
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
            var articleColumn = new DataGridViewTextBoxColumn
            {
                Name = "Article",
                HeaderText = "Артикул",
                Width = 120,
                DataPropertyName = "Article"
            };

            var nameColumn = new DataGridViewTextBoxColumn
            {
                Name = "Name",
                HeaderText = "Название",
                Width = 200,
                DataPropertyName = "Name",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };

            var categoryColumn = new DataGridViewTextBoxColumn
            {
                Name = "Category",
                HeaderText = "Категория",
                Width = 130,
                DataPropertyName = "Category"
            };

            var quantityColumn = new DataGridViewTextBoxColumn
            {
                Name = "Quantity",
                HeaderText = "Кол-во",
                Width = 80,
                DataPropertyName = "Quantity",
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
            };

            var priceColumn = new DataGridViewTextBoxColumn
            {
                Name = "Price",
                HeaderText = "Цена",
                Width = 100,
                DataPropertyName = "Price",
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            };

            var locationColumn = new DataGridViewTextBoxColumn
            {
                Name = "Location",
                HeaderText = "Место",
                Width = 120,
                DataPropertyName = "Location"
            };

            var lastUpdatedColumn = new DataGridViewTextBoxColumn
            {
                Name = "LastUpdated",
                HeaderText = "Обновлено",
                Width = 120
            };

            // Добавляем столбцы
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] {
                articleColumn, nameColumn, categoryColumn,
                quantityColumn, priceColumn, locationColumn, lastUpdatedColumn
            });

            // Заполняем данные
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var product = row.DataBoundItem as Product;
                if (product != null)
                {
                    // Заполняем колонку "Обновлено"
                    row.Cells["LastUpdated"].Value = product.LastUpdated.ToString("dd.MM.yyyy HH:mm");

                    // Подсветка товаров с малым количеством
                    if (product.Quantity < 10)
                    {
                        row.Cells["Quantity"].Style.BackColor = Color.LightPink;
                        row.Cells["Quantity"].Style.ForeColor = Color.DarkRed;
                    }
                    else if (product.Quantity > 100)
                    {
                        row.Cells["Quantity"].Style.BackColor = Color.LightGreen;
                    }
                }
            }

            // Добавляем столбец со стоимостью как вычисляемое поле
            var totalValueColumn = new DataGridViewTextBoxColumn
            {
                Name = "TotalValue",
                HeaderText = "Стоимость",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            };
            dataGridView1.Columns.Add(totalValueColumn);

            // Заполняем столбец Стоимость
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var product = row.DataBoundItem as Product;
                if (product != null)
                {
                    row.Cells["TotalValue"].Value = product.Quantity * product.Price;
                }
            }
        }

        private void UpdateStats()
        {
            try
            {
                var products = dataGridView1.DataSource as List<Product>;
                if (products == null || products.Count == 0)
                {
                    lblStats.Text = "📊 Статистика: Нет данных для отображения";
                    return;
                }

                var totalProducts = products.Count;
                var totalQuantity = products.Sum(p => p.Quantity);
                var totalValue = products.Sum(p => p.Quantity * p.Price);
                var avgPrice = products.Average(p => p.Price);
                var minPrice = products.Min(p => p.Price);
                var maxPrice = products.Max(p => p.Price);

                lblStats.Text = $"📊 Статистика: Товаров: {totalProducts} | Кол-во: {totalQuantity} | Стоимость: {totalValue:C2} | Цена: {avgPrice:C2} (от {minPrice:C2} до {maxPrice:C2})";
            }
            catch (Exception ex)
            {
                lblStats.Text = $"Ошибка расчета статистики: {ex.Message}";
            }
        }

        // ==================== ОБРАБОТЧИКИ СОБЫТИЙ ====================

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            // Игнорируем изменение текста если это placeholder
            if (txtSearch.ForeColor == Color.Gray) return;

            // Задержка для поиска (дебаунсинг)
            var timer = new Timer { Interval = 300 };
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                timer.Dispose();
                ApplyFiltersAndSort();
            };
            timer.Start();
        }

        private void CmbCategoryFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFiltersAndSort();
        }

        private void CmbSortBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFiltersAndSort();
        }

        private void Filter_ValueChanged(object sender, EventArgs e)
        {
            ApplyFiltersAndSort();
        }

        private void BtnClearFilters_Click(object sender, EventArgs e)
        {
            // Сброс всех фильтров
            txtSearch.Text = "Название, артикул, описание...";
            txtSearch.ForeColor = Color.Gray;
            cmbCategoryFilter.SelectedIndex = 0;
            numMinPrice.Value = 0;
            numMaxPrice.Value = 1000000;
            numMinQuantity.Value = 0;
            numMaxQuantity.Value = 10000;
            cmbSortBy.SelectedIndex = 0;

            ApplyFiltersAndSort();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new AddEditProductForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadAllProducts(); // Перезагружаем все товары
                    ApplyFiltersAndSort();
                    MessageBox.Show("Товар успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var product = (Product)dataGridView1.SelectedRows[0].DataBoundItem;
                using (var form = new AddEditProductForm(product))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadAllProducts(); // Перезагружаем все товары
                        ApplyFiltersAndSort();
                        MessageBox.Show("Товар успешно обновлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var product = (Product)dataGridView1.SelectedRows[0].DataBoundItem;

                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить товар?\n\n" +
                    $"📦 Название: {product.Name}\n" +
                    $"🏷️ Артикул: {product.Article}\n" +
                    $"📊 Количество: {product.Quantity}\n" +
                    $"💰 Цена: {product.Price:C}",
                    "Подтверждение удаления",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        var products = DataService.LoadProducts();
                        products.RemoveAll(p => p.Article == product.Article);
                        DataService.SaveProducts(products);

                        LoadAllProducts();
                        ApplyFiltersAndSort();
                        MessageBox.Show("Товар успешно удален", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadAllProducts();
            ApplyFiltersAndSort();
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            bool hasSelection = dataGridView1.SelectedRows.Count > 0;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
        }

        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                BtnEdit_Click(sender, e);
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "CSV файлы (*.csv)|*.csv";
                    sfd.FileName = $"товары_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        var products = (List<Product>)dataGridView1.DataSource;
                        DataService.ExportToCsv(products, sfd.FileName);

                        if (MessageBox.Show(
                            $"Экспорт завершен успешно!\n\n" +
                            $"Файл: {sfd.FileName}\n" +
                            $"Записей: {products.Count}\n\n" +
                            $"Открыть файл?",
                            "Экспорт завершен",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(sfd.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Горячие клавиши
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.F:
                    txtSearch.Focus();
                    if (txtSearch.Text == "Название, артикул, описание...")
                    {
                        txtSearch.Text = "";
                        txtSearch.ForeColor = Color.Black;
                    }
                    txtSearch.SelectAll();
                    return true;
                case Keys.F5:
                    BtnRefresh_Click(null, null);
                    return true;
                case Keys.Control | Keys.N:
                    BtnAdd_Click(null, null);
                    return true;
                case Keys.Delete when btnDelete.Enabled:
                    BtnDelete_Click(null, null);
                    return true;
                case Keys.Enter when btnEdit.Enabled:
                    BtnEdit_Click(null, null);
                    return true;
                case Keys.Control | Keys.E:
                    BtnClearFilters_Click(null, null);
                    return true;
                case Keys.Control | Keys.S:
                    BtnExport_Click(null, null);
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}