using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WarehouseManagementSystem1.Models; // ДОБАВЬТЕ ЭТУ СТРОКУ!
using WarehouseManagementSystem1.Services;

namespace WarehouseManagementSystem1
{
    public partial class AddEditProductForm : Form
    {
        private System.ComponentModel.IContainer components = null; // Добавлено здесь

        private TextBox txtArticle;
        private TextBox txtName;
        private TextBox txtCategory;
        private NumericUpDown nudQuantity;
        private NumericUpDown nudPrice;
        private TextBox txtDescription;
        private Button btnSave;
        private Button btnCancel;
        private Button btnGenerateArticle;

        private Product _product;
        private bool _isEditMode;

        public AddEditProductForm()
        {
            _isEditMode = false;
            InitializeComponent();
        }

        public AddEditProductForm(Product product)
        {
            _product = product;
            _isEditMode = true;
            InitializeComponent();
            LoadProductData();
        }

        private void InitializeComponent()
        {
            this.Text = _isEditMode ? "Редактировать товар" : "Добавить товар";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            int y = 20;
            int labelWidth = 120;
            int controlWidth = 300;

            // Артикул
            var lblArticle = new Label { Text = "Артикул*:", Location = new Point(0, y), Size = new Size(labelWidth, 25), TextAlign = ContentAlignment.MiddleRight };
            txtArticle = new TextBox { Location = new Point(labelWidth + 10, y), Size = new Size(controlWidth - 100, 25) };
            btnGenerateArticle = new Button { Text = "Сгенерировать", Location = new Point(labelWidth + controlWidth - 90, y), Size = new Size(90, 25) };
            btnGenerateArticle.Click += BtnGenerateArticle_Click;
            if (_isEditMode) txtArticle.Enabled = false;
            y += 35;

            // Название
            var lblName = new Label { Text = "Название*:", Location = new Point(0, y), Size = new Size(labelWidth, 25), TextAlign = ContentAlignment.MiddleRight };
            txtName = new TextBox { Location = new Point(labelWidth + 10, y), Size = new Size(controlWidth, 25) };
            y += 35;

            // Категория
            var lblCategory = new Label { Text = "Категория*:", Location = new Point(0, y), Size = new Size(labelWidth, 25), TextAlign = ContentAlignment.MiddleRight };
            txtCategory = new TextBox { Location = new Point(labelWidth + 10, y), Size = new Size(controlWidth, 25) };
            y += 35;

            // Количество
            var lblQuantity = new Label { Text = "Количество:", Location = new Point(0, y), Size = new Size(labelWidth, 25), TextAlign = ContentAlignment.MiddleRight };
            nudQuantity = new NumericUpDown { Location = new Point(labelWidth + 10, y), Size = new Size(controlWidth, 25), Minimum = 0, Maximum = 1000000 };
            y += 35;

            // Цена
            var lblPrice = new Label { Text = "Цена*:", Location = new Point(0, y), Size = new Size(labelWidth, 25), TextAlign = ContentAlignment.MiddleRight };
            nudPrice = new NumericUpDown { Location = new Point(labelWidth + 10, y), Size = new Size(controlWidth, 25), Minimum = 0, Maximum = 10000000, DecimalPlaces = 2 };
            y += 35;

            // Описание
            var lblDescription = new Label { Text = "Описание:", Location = new Point(0, y), Size = new Size(labelWidth, 25), TextAlign = ContentAlignment.MiddleRight };
            txtDescription = new TextBox { Location = new Point(labelWidth + 10, y), Size = new Size(controlWidth, 60), Multiline = true };
            y += 70;

            // Кнопки
            btnSave = new Button { Text = "Сохранить", Location = new Point(labelWidth + 10, y), Size = new Size(120, 35) };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button { Text = "Отмена", Location = new Point(labelWidth + 140, y), Size = new Size(120, 35) };
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            panel.Controls.AddRange(new Control[]
            {
                lblArticle, txtArticle, btnGenerateArticle,
                lblName, txtName,
                lblCategory, txtCategory,
                lblQuantity, nudQuantity,
                lblPrice, nudPrice,
                lblDescription, txtDescription,
                btnSave, btnCancel
            });

            this.Controls.Add(panel);
        }

        private void LoadProductData()
        {
            txtArticle.Text = _product.Article;
            txtName.Text = _product.Name;
            txtCategory.Text = _product.Category;
            nudQuantity.Value = _product.Quantity;
            nudPrice.Value = _product.Price;
            txtDescription.Text = _product.Description ?? "";
        }

        private void BtnGenerateArticle_Click(object sender, EventArgs e)
        {
            txtArticle.Text = $"ART{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                var products = DataService.LoadProducts(); // Это теперь работает

                if (!_isEditMode)
                {
                    // Проверка уникальности артикула
                    if (products.Any(p => p.Article == txtArticle.Text))
                    {
                        MessageBox.Show("Товар с таким артикулом уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    products.Add(new Product
                    {
                        Article = txtArticle.Text,
                        Name = txtName.Text,
                        Category = txtCategory.Text,
                        Quantity = (int)nudQuantity.Value,
                        Price = nudPrice.Value,
                        Description = txtDescription.Text
                    });
                }
                else
                {
                    // Обновление существующего товара
                    var existingProduct = products.FirstOrDefault(p => p.Article == _product.Article);
                    if (existingProduct != null)
                    {
                        existingProduct.Name = txtName.Text;
                        existingProduct.Category = txtCategory.Text;
                        existingProduct.Quantity = (int)nudQuantity.Value;
                        existingProduct.Price = nudPrice.Value;
                        existingProduct.Description = txtDescription.Text;
                    }
                }

                DataService.SaveProducts(products);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtArticle.Text))
            {
                MessageBox.Show("Введите артикул товара!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtArticle.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название товара!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCategory.Text))
            {
                MessageBox.Show("Введите категорию товара!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCategory.Focus();
                return false;
            }

            if (nudPrice.Value <= 0)
            {
                MessageBox.Show("Цена должна быть больше 0!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nudPrice.Focus();
                return false;
            }

            return true;
        }

        // Добавлено: метод Dispose ВНУТРИ класса
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    // Освободить управляемые ресурсы
                    if (components != null)
                        components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}