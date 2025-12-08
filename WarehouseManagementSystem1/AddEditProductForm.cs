using System;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagementSystem1.Enums;
using WarehouseManagementSystem1.Models;
using WarehouseManagementSystem1.Services;
using System.Linq;

namespace WarehouseManagementSystem1
{
    public partial class AddEditProductForm : Form
    {
        private Product product;
        private User currentUser;
        private DataService dataService;
        private bool isEditMode;

        public AddEditProductForm(Product productToEdit, User user)
        {
            product = productToEdit;
            currentUser = user;
            dataService = DataService.Instance;
            isEditMode = productToEdit != null;

            InitializeComponent();
            CreateForm();

            if (isEditMode)
            {
                LoadProductData();
            }
        }

        private void CreateForm()
        {
            this.Text = isEditMode ? "✏️ Редактирование товара" : "➕ Добавление товара";
            this.Size = new Size(500, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Создаем элементы управления
            var lblName = new Label { Text = "Название:", Location = new Point(30, 30), Size = new Size(100, 25) };
            var txtName = new TextBox { Location = new Point(140, 30), Size = new Size(300, 25), Name = "txtName" };

            var lblDesc = new Label { Text = "Описание:", Location = new Point(30, 70), Size = new Size(100, 25) };
            var txtDescription = new TextBox { Location = new Point(140, 70), Size = new Size(300, 60), Multiline = true, Height = 60, Name = "txtDescription" };

            var lblPrice = new Label { Text = "Цена:", Location = new Point(30, 150), Size = new Size(100, 25) };
            var txtPrice = new TextBox { Location = new Point(140, 150), Size = new Size(150, 25), Name = "txtPrice" };

            var lblQuantity = new Label { Text = "Количество:", Location = new Point(30, 190), Size = new Size(100, 25) };
            var txtQuantity = new TextBox { Location = new Point(140, 190), Size = new Size(150, 25), Name = "txtQuantity", Text = "0" };

            var lblCategory = new Label { Text = "Категория:", Location = new Point(30, 230), Size = new Size(100, 25) };
            var cmbCategory = new ComboBox { Location = new Point(140, 230), Size = new Size(200, 25), Name = "cmbCategory", DropDownStyle = ComboBoxStyle.DropDown };

            var lblSKU = new Label { Text = "Артикул:", Location = new Point(30, 270), Size = new Size(100, 25) };
            var txtSKU = new TextBox { Location = new Point(140, 270), Size = new Size(200, 25), Name = "txtSKU" };

            var lblBarcode = new Label { Text = "Штрихкод:", Location = new Point(30, 310), Size = new Size(100, 25) };
            var txtBarcode = new TextBox { Location = new Point(140, 310), Size = new Size(200, 25), Name = "txtBarcode" };

            var lblLocation = new Label { Text = "Место:", Location = new Point(30, 350), Size = new Size(100, 25) };
            var txtLocation = new TextBox { Location = new Point(140, 350), Size = new Size(200, 25), Name = "txtLocation" };

            // Загружаем категории
            LoadCategories(cmbCategory);

            // Кнопки
            var btnSave = new Button
            {
                Text = isEditMode ? "💾 Сохранить" : "➕ Добавить",
                Location = new Point(140, 390),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Name = "btnSave"
            };

            var btnCancel = new Button
            {
                Text = "❌ Отмена",
                Location = new Point(270, 390),
                Size = new Size(120, 35),
                BackColor = Color.LightGray,
                ForeColor = Color.Black,
                Name = "btnCancel"
            };

            // Добавляем элементы на форму
            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblDesc);
            this.Controls.Add(txtDescription);
            this.Controls.Add(lblPrice);
            this.Controls.Add(txtPrice);
            this.Controls.Add(lblQuantity);
            this.Controls.Add(txtQuantity);
            this.Controls.Add(lblCategory);
            this.Controls.Add(cmbCategory);
            this.Controls.Add(lblSKU);
            this.Controls.Add(txtSKU);
            this.Controls.Add(lblBarcode);
            this.Controls.Add(txtBarcode);
            this.Controls.Add(lblLocation);
            this.Controls.Add(txtLocation);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);

            // Обработчики событий
            btnSave.Click += (sender, e) => SaveProduct(
                txtName.Text,
                txtDescription.Text,
                txtPrice.Text,
                txtQuantity.Text,
                cmbCategory.Text,
                txtSKU.Text,
                txtBarcode.Text,
                txtLocation.Text
            );

            btnCancel.Click += (sender, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            // Настройка клавиш
            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private void LoadCategories(ComboBox cmbCategory)
        {
            cmbCategory.Items.Clear();

            // Добавляем стандартные категории
            cmbCategory.Items.Add("Электроника");
            cmbCategory.Items.Add("Офисные товары");
            cmbCategory.Items.Add("Хозтовары");
            cmbCategory.Items.Add("Мебель");
            cmbCategory.Items.Add("Инструменты");

            if (dataService.Products != null)
            {
                // Добавляем уникальные категории из существующих товаров
                foreach (var product in dataService.Products)
                {
                    if (!string.IsNullOrEmpty(product.Category) && !cmbCategory.Items.Contains(product.Category))
                    {
                        cmbCategory.Items.Add(product.Category);
                    }
                }
            }

            if (cmbCategory.Items.Count > 0)
            {
                cmbCategory.SelectedIndex = 0;
            }
        }

        private void LoadProductData()
        {
            if (product == null) return;

            // Находим элементы управления по имени
            foreach (Control control in this.Controls)
            {
                if (control.Name == "txtName") control.Text = product.Name;
                if (control.Name == "txtDescription") control.Text = product.Description;
                if (control.Name == "txtPrice") control.Text = product.Price.ToString();
                if (control.Name == "txtQuantity") control.Text = product.Quantity.ToString();
                if (control.Name == "cmbCategory") ((ComboBox)control).Text = product.Category;
                if (control.Name == "txtSKU") control.Text = product.SKU;
                if (control.Name == "txtBarcode") control.Text = product.Barcode;
                if (control.Name == "txtLocation") control.Text = product.Location;
            }
        }

        private void SaveProduct(string name, string description, string priceText, string quantityText,
                                string category, string sku, string barcode, string location)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введите название товара!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!decimal.TryParse(priceText, out decimal price) || price <= 0)
            {
                MessageBox.Show("Введите корректную цену!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(quantityText, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Введите корректное количество!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (isEditMode)
            {
                // Обновляем существующий товар
                product.Name = name;
                product.Description = description;
                product.Price = price;
                product.Quantity = quantity;
                product.Category = category;
                product.SKU = sku;
                product.Barcode = barcode;
                product.Location = location;
                product.LastUpdated = DateTime.Now;

                dataService.UpdateProduct(product);
            }
            else
            {
                // Создаем новый товар
                var newProduct = new Product
                {
                    Name = name,
                    Description = description,
                    Price = price,
                    Quantity = quantity,
                    Category = category,
                    SKU = sku,
                    Barcode = barcode,
                    Location = location,
                    LastUpdated = DateTime.Now
                };

                dataService.AddProduct(newProduct);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}