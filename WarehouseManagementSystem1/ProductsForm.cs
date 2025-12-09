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
    public partial class ProductsForm : Form
    {
        private DataService dataService;
        private User currentUser;
        private List<Product> allProducts;

        // Элементы управления
        private DataGridView productsGrid;
        private TextBox txtSearch;
        private ComboBox cmbCategoryFilter;
        private Button btnAddProduct;
        private Button btnEditProduct;
        private Button btnDeleteProduct;
        private Button btnRefresh;
        private Label lblStats;

        public ProductsForm(User user)
        {
            currentUser = user;
            dataService = DataService.Instance;
            allProducts = dataService.Products ?? new List<Product>();

            InitializeComponent();
            CreateProductsForm();
            LoadProducts();
        }

        private void CreateProductsForm()
        {
            // Основные настройки формы
            this.Text = "📦 Управление товарами";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // ===== 1. ПАНЕЛЬ ИНСТРУМЕНТОВ =====
            var toolPanel = new Panel();
            toolPanel.BackColor = Color.FromArgb(240, 240, 240);
            toolPanel.BorderStyle = BorderStyle.FixedSingle;
            toolPanel.Location = new Point(10, 10);
            toolPanel.Size = new Size(1065, 80);
            this.Controls.Add(toolPanel);

            // Заголовок
            var lblTitle = new Label();
            lblTitle.Text = "📦 Управление товарами";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(33, 150, 243);
            lblTitle.Location = new Point(15, 20);
            lblTitle.Size = new Size(250, 30);
            toolPanel.Controls.Add(lblTitle);

            // Кнопка "Обновить"
            btnRefresh = new Button();
            btnRefresh.Text = "🔄 Обновить";
            btnRefresh.Font = new Font("Segoe UI", 10);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.BackColor = Color.FromArgb(33, 150, 243);
            btnRefresh.Location = new Point(280, 20);
            btnRefresh.Size = new Size(120, 35);
            btnRefresh.Click += (s, e) => LoadProducts();
            toolPanel.Controls.Add(btnRefresh);

            // Кнопка "Добавить"
            btnAddProduct = new Button();
            btnAddProduct.Text = "➕ Добавить товар";
            btnAddProduct.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnAddProduct.ForeColor = Color.White;
            btnAddProduct.BackColor = Color.FromArgb(76, 175, 80); // Зеленый
            btnAddProduct.Location = new Point(410, 20);
            btnAddProduct.Size = new Size(150, 35);
            btnAddProduct.Click += BtnAddProduct_Click;
            btnAddProduct.Enabled = currentUser.Role == UserRole.Admin || currentUser.Role == UserRole.Manager;
            toolPanel.Controls.Add(btnAddProduct);

            // Кнопка "Редактировать"
            btnEditProduct = new Button();
            btnEditProduct.Text = "✏️ Редактировать";
            btnEditProduct.Font = new Font("Segoe UI", 10);
            btnEditProduct.ForeColor = Color.White;
            btnEditProduct.BackColor = Color.FromArgb(255, 152, 0); // Оранжевый
            btnEditProduct.Location = new Point(570, 20);
            btnEditProduct.Size = new Size(150, 35);
            btnEditProduct.Click += BtnEditProduct_Click;
            btnEditProduct.Enabled = currentUser.Role == UserRole.Admin || currentUser.Role == UserRole.Manager;
            toolPanel.Controls.Add(btnEditProduct);

            // Кнопка "Удалить"
            btnDeleteProduct = new Button();
            btnDeleteProduct.Text = "🗑️ Удалить";
            btnDeleteProduct.Font = new Font("Segoe UI", 10);
            btnDeleteProduct.ForeColor = Color.White;
            btnDeleteProduct.BackColor = Color.FromArgb(244, 67, 54); // Красный
            btnDeleteProduct.Location = new Point(730, 20);
            btnDeleteProduct.Size = new Size(120, 35);
            btnDeleteProduct.Click += BtnDeleteProduct_Click;
            btnDeleteProduct.Enabled = currentUser.Role == UserRole.Admin;
            toolPanel.Controls.Add(btnDeleteProduct);

            // Статистика
            lblStats = new Label();
            lblStats.Font = new Font("Segoe UI", 10);
            lblStats.ForeColor = Color.DarkSlateGray;
            lblStats.Location = new Point(860, 20);
            lblStats.Size = new Size(200, 35);
            lblStats.TextAlign = ContentAlignment.MiddleRight;
            toolPanel.Controls.Add(lblStats);

            // ===== 2. ПАНЕЛЬ ФИЛЬТРОВ =====
            var filterPanel = new Panel();
            filterPanel.BorderStyle = BorderStyle.FixedSingle;
            filterPanel.Location = new Point(10, 100);
            filterPanel.Size = new Size(1065, 70);
            this.Controls.Add(filterPanel);

            // Поиск по названию
            var lblSearch = new Label();
            lblSearch.Text = "🔍 Поиск:";
            lblSearch.Font = new Font("Segoe UI", 10);
            lblSearch.Location = new Point(15, 20);
            lblSearch.Size = new Size(70, 25);
            filterPanel.Controls.Add(lblSearch);

            txtSearch = new TextBox();
            txtSearch.Location = new Point(90, 20);
            txtSearch.Size = new Size(200, 25);
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.TextChanged += TxtSearch_TextChanged;
            filterPanel.Controls.Add(txtSearch);

            // Фильтр по категории
            var lblCategory = new Label();
            lblCategory.Text = "📁 Категория:";
            lblCategory.Font = new Font("Segoe UI", 10);
            lblCategory.Location = new Point(310, 20);
            lblCategory.Size = new Size(90, 25);
            filterPanel.Controls.Add(lblCategory);

            cmbCategoryFilter = new ComboBox();
            cmbCategoryFilter.Location = new Point(405, 20);
            cmbCategoryFilter.Size = new Size(200, 25);
            cmbCategoryFilter.Font = new Font("Segoe UI", 10);
            cmbCategoryFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategoryFilter.SelectedIndexChanged += CmbCategoryFilter_SelectedIndexChanged;
            filterPanel.Controls.Add(cmbCategoryFilter);

            // Фильтр по наличию
            var lblStock = new Label();
            lblStock.Text = "📊 Наличие:";
            lblStock.Font = new Font("Segoe UI", 10);
            lblStock.Location = new Point(620, 20);
            lblStock.Size = new Size(80, 25);
            filterPanel.Controls.Add(lblStock);

            var cmbStockFilter = new ComboBox();
            cmbStockFilter.Location = new Point(705, 20);
            cmbStockFilter.Size = new Size(150, 25);
            cmbStockFilter.Font = new Font("Segoe UI", 10);
            cmbStockFilter.Items.AddRange(new string[] { "Все", "В наличии", "Нет в наличии", "Мало (<10)" });
            cmbStockFilter.SelectedIndex = 0;
            cmbStockFilter.SelectedIndexChanged += (s, e) => ApplyFilters();
            filterPanel.Controls.Add(cmbStockFilter);

            // Кнопка сброса фильтров
            var btnResetFilters = new Button();
            btnResetFilters.Text = "❌ Сбросить";
            btnResetFilters.Font = new Font("Segoe UI", 10);
            btnResetFilters.Location = new Point(870, 20);
            btnResetFilters.Size = new Size(100, 25);
            btnResetFilters.Click += (s, e) =>
            {
                txtSearch.Text = "";
                cmbCategoryFilter.SelectedIndex = -1;
                cmbStockFilter.SelectedIndex = 0;
                LoadProducts();
            };
            filterPanel.Controls.Add(btnResetFilters);

            // ===== 3. ТАБЛИЦА ТОВАРОВ =====
            productsGrid = new DataGridView();
            productsGrid.Location = new Point(10, 180);
            productsGrid.Size = new Size(1065, 470);
            productsGrid.AllowUserToAddRows = false;
            productsGrid.ReadOnly = true;
            productsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            productsGrid.RowHeadersVisible = false;
            productsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            productsGrid.MultiSelect = false;
            productsGrid.CellDoubleClick += ProductsGrid_CellDoubleClick;

            // Настраиваем колонки
            SetupDataGridColumns();

            this.Controls.Add(productsGrid);
        }

        private void SetupDataGridColumns()
        {
            productsGrid.Columns.Clear();

            // ID
            productsGrid.Columns.Add("Id", "ID");
            productsGrid.Columns["Id"].Width = 50;
            productsGrid.Columns["Id"].ReadOnly = true;

            // Название
            productsGrid.Columns.Add("Name", "Название");
            productsGrid.Columns["Name"].Width = 200;

            // Цена
            var priceColumn = new DataGridViewTextBoxColumn();
            priceColumn.Name = "Price";
            priceColumn.HeaderText = "Цена";
            priceColumn.DefaultCellStyle.Format = "C";
            priceColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            productsGrid.Columns.Add(priceColumn);

            // Количество
            productsGrid.Columns.Add("Quantity", "Кол-во");
            productsGrid.Columns["Quantity"].Width = 80;
            productsGrid.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Категория
            productsGrid.Columns.Add("Category", "Категория");
            productsGrid.Columns["Category"].Width = 120;

            // Артикул
            productsGrid.Columns.Add("SKU", "Артикул");
            productsGrid.Columns["SKU"].Width = 120;

            // Местоположение
            productsGrid.Columns.Add("Location", "Место");
            productsGrid.Columns["Location"].Width = 120;

            // Штрихкод
            productsGrid.Columns.Add("Barcode", "Штрихкод");
            productsGrid.Columns["Barcode"].Width = 120;

            // Обновлено
            var dateColumn = new DataGridViewTextBoxColumn();
            dateColumn.Name = "LastUpdated";
            dateColumn.HeaderText = "Обновлено";
            dateColumn.DefaultCellStyle.Format = "dd.MM.yyyy";
            dateColumn.Width = 100;
            productsGrid.Columns.Add(dateColumn);
        }

        private void LoadProducts()
        {
            try
            {
                // 1. ОБНОВЛЯЕМ список товаров из DataService
                allProducts = dataService.Products ?? new List<Product>();

                // 2. ЗАПОМИНАЕМ, какая категория была выбрана ДО обновления
                string previouslySelectedCategory = null;
                if (cmbCategoryFilter.SelectedIndex > 0)
                {
                    previouslySelectedCategory = cmbCategoryFilter.SelectedItem?.ToString();
                }

                // 3. ПЕРЕЗАПОЛНЯЕМ список категорий (с новыми данными)
                LoadCategoriesFilter();

                // 4. ПЫТАЕМСЯ ВОССТАНОВИТЬ ВЫБОР КАТЕГОРИИ
                if (!string.IsNullOrEmpty(previouslySelectedCategory))
                {
                    // Ищем эту категорию в обновлённом списке
                    for (int i = 0; i < cmbCategoryFilter.Items.Count; i++)
                    {
                        if (cmbCategoryFilter.Items[i].ToString() == previouslySelectedCategory)
                        {
                            cmbCategoryFilter.SelectedIndex = i; // Восстанавливаем выбор
                            break;
                        }
                    }
                }

                // 5. ПРИМЕНЯЕМ ФИЛЬТРЫ (они учтут либо восстановленную категорию, либо "Все")
                ApplyFilters();

                // 6. Обновляем статистику
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров:\n{ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCategoriesFilter()
        {
            cmbCategoryFilter.Items.Clear();
            cmbCategoryFilter.Items.Add("Все категории");

            if (allProducts != null)
            {
                var categories = allProducts
                    .Where(p => !string.IsNullOrEmpty(p.Category))
                    .Select(p => p.Category)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();

                foreach (var category in categories)
                {
                    cmbCategoryFilter.Items.Add(category);
                }
            }

            cmbCategoryFilter.SelectedIndex = 0;
        }

        private void ApplyFilters()
        {
            if (allProducts == null) return;

            var filteredProducts = allProducts.AsEnumerable();

            // Фильтр по поиску
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string searchText = txtSearch.Text.ToLower();
                filteredProducts = filteredProducts.Where(p =>
                    (p.Name != null && p.Name.ToLower().Contains(searchText)) ||
                    (p.SKU != null && p.SKU.ToLower().Contains(searchText)) ||
                    (p.Barcode != null && p.Barcode.Contains(searchText)) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchText)));
            }

            // Фильтр по категории
            if (cmbCategoryFilter.SelectedIndex > 0 && cmbCategoryFilter.SelectedItem != null)
            {
                string selectedCategory = cmbCategoryFilter.SelectedItem.ToString();
                filteredProducts = filteredProducts.Where(p => p.Category == selectedCategory);
            }

            // Фильтр по наличию (нужно добавить ComboBox для этого)
            // Пока пропускаем

            // Сортировка по ID
            filteredProducts = filteredProducts.OrderBy(p => p.Id);

            // Отображаем в таблице
            DisplayProducts(filteredProducts.ToList());
        }

        private void DisplayProducts(List<Product> products)
        {
            productsGrid.Rows.Clear();

            foreach (var product in products)
            {
                int rowIndex = productsGrid.Rows.Add(
                    product.Id,
                    product.Name,
                    product.Price,
                    product.Quantity,
                    product.Category,
                    product.SKU,
                    product.Location,
                    product.Barcode,
                    product.LastUpdated
                );

                // Подсветка товаров с низким запасом
                if (product.Quantity < 10)
                {
                    productsGrid.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightPink;
                    productsGrid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.DarkRed;
                }
                else if (product.Quantity == 0)
                {
                    productsGrid.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                    productsGrid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.DarkGray;
                }
            }

            // Обновляем статистику
            UpdateStatistics(products.Count);
        }

        private void UpdateStatistics(int? filteredCount = null)
        {
            int totalCount = allProducts?.Count ?? 0;
            int displayCount = filteredCount ?? totalCount;

            // Рассчитываем общую стоимость
            decimal totalValue = 0;
            if (allProducts != null)
            {
                foreach (var product in allProducts)
                {
                    totalValue += product.Price * product.Quantity;
                }
            }

            lblStats.Text = $"📊 Показано: {displayCount} из {totalCount} | 💰 Стоимость: {totalValue:C}";
        }

        // ===== ОБРАБОТЧИКИ СОБЫТИЙ =====

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void CmbCategoryFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ProductsGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && productsGrid.Rows[e.RowIndex].Cells["Id"].Value != null)
            {
                int productId = Convert.ToInt32(productsGrid.Rows[e.RowIndex].Cells["Id"].Value);
                ShowProductDetails(productId);
            }
        }

        private void BtnAddProduct_Click(object sender, EventArgs e)
        {
            var addForm = new AddEditProductForm(null, currentUser);
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Товар успешно добавлен! Вернитесь на вкладку 'Дашборд' для обновления статистики.", "Успех");
                // Просто закрываем эту форму или оставляем открытой
                // this.Close(); // <- Можно раскомментировать, если хотим сразу закрыть ProductsForm
            }
        }

        private void BtnEditProduct_Click(object sender, EventArgs e)
        {
            if (productsGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите товар для редактирования!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int productId = Convert.ToInt32(productsGrid.SelectedRows[0].Cells["Id"].Value);
            var product = allProducts.FirstOrDefault(p => p.Id == productId);

            if (product != null)
            {
                var editForm = new AddEditProductForm(product, currentUser);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadProducts();
                    MessageBox.Show("Товар успешно обновлен!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnDeleteProduct_Click(object sender, EventArgs e)
        {
            if (productsGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите товар для удаления!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int productId = Convert.ToInt32(productsGrid.SelectedRows[0].Cells["Id"].Value);
            var product = allProducts.FirstOrDefault(p => p.Id == productId);

            if (product != null)
            {
                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить товар?\n\n" +
                    $"Название: {product.Name}\n" +
                    $"Артикул: {product.SKU}\n" +
                    $"Количество: {product.Quantity} шт.",
                    "Подтверждение удаления",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Удаляем товар
                    allProducts.Remove(product);
                    dataService.DeleteProduct(productId);

                    LoadProducts();
                    MessageBox.Show("Товар успешно удален!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ShowProductDetails(int productId)
        {
            var product = allProducts.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                string details = $"📦 Детали товара\n\n" +
                               $"ID: {product.Id}\n" +
                               $"Название: {product.Name}\n" +
                               $"Описание: {product.Description ?? "(нет)"}\n" +
                               $"Цена: {product.Price:C}\n" +
                               $"Количество: {product.Quantity} шт.\n" +
                               $"Категория: {product.Category}\n" +
                               $"Артикул: {product.SKU}\n" +
                               $"Штрихкод: {product.Barcode}\n" +
                               $"Место: {product.Location}\n" +
                               $"Обновлено: {product.LastUpdated:dd.MM.yyyy HH:mm}";

                MessageBox.Show(details, "Информация о товаре",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}