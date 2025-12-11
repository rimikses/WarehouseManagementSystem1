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
    public class TransactionForm : Form
    {
        private ComboBox cmbTransactionType;
        private ComboBox cmbProduct;
        private NumericUpDown numQuantity;
        private TextBox txtFromLocation;
        private TextBox txtToLocation;
        private TextBox txtDocumentNumber;
        private TextBox txtComments;
        private Button btnProcess;
        private Button btnCancel;
        private Label lblCurrentQuantity;
        private User currentUser;
        private DataService dataService;

        public TransactionForm(User user, TransactionType initialType = TransactionType.Приход)
        {
            currentUser = user;
            dataService = DataService.Instance;

            InitializeComponent();

            // Проверяем, есть ли товары
            if (dataService.Products == null || dataService.Products.Count == 0)
            {
                MessageBox.Show("В системе нет товаров для проведения операций.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return;
            }

            LoadProducts();
            cmbTransactionType.SelectedItem = initialType;
            UpdateCurrentQuantity();
        }

        private void InitializeComponent()
        {
            this.Text = "🔄 Проведение операции";
            this.Size = new Size(600, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            int y = 20;
            int labelWidth = 180;
            int controlWidth = 300;

            // Тип операции
            var lblType = new Label
            {
                Text = "Тип операции*:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            cmbTransactionType = new ComboBox
            {
                Location = new Point(labelWidth + 30, y),
                Size = new Size(controlWidth, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            cmbTransactionType.Items.AddRange(new object[] { TransactionType.Приход, TransactionType.Расход, TransactionType.Перемещение });
            cmbTransactionType.SelectedIndexChanged += CmbTransactionType_SelectedIndexChanged;
            y += 40;

            // Товар
            var lblProduct = new Label
            {
                Text = "Товар*:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10)
            };
            cmbProduct = new ComboBox
            {
                Location = new Point(labelWidth + 30, y),
                Size = new Size(controlWidth, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                DisplayMember = "Name"
            };
            cmbProduct.SelectedIndexChanged += CmbProduct_SelectedIndexChanged;
            y += 40;

            // Текущее количество
            lblCurrentQuantity = new Label
            {
                Text = "Текущее количество: 0",
                Location = new Point(20, y),
                Size = new Size(labelWidth + controlWidth + 10, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.DarkBlue
            };
            y += 35;

            // Количество
            var lblQuantity = new Label
            {
                Text = "Количество*:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10)
            };
            numQuantity = new NumericUpDown
            {
                Location = new Point(labelWidth + 30, y),
                Size = new Size(controlWidth, 30),
                Minimum = 1,
                Maximum = 10000,
                Font = new Font("Segoe UI", 10)
            };
            y += 40;

            // Откуда
            var lblFrom = new Label
            {
                Text = "Откуда:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10)
            };
            txtFromLocation = new TextBox
            {
                Location = new Point(labelWidth + 30, y),
                Size = new Size(controlWidth, 30),
                Font = new Font("Segoe UI", 10)
            };
            y += 40;

            // Куда
            var lblTo = new Label
            {
                Text = "Куда:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10)
            };
            txtToLocation = new TextBox
            {
                Location = new Point(labelWidth + 30, y),
                Size = new Size(controlWidth, 30),
                Font = new Font("Segoe UI", 10)
            };
            y += 40;

            // Номер документа
            var lblDoc = new Label
            {
                Text = "Номер документа:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10)
            };
            txtDocumentNumber = new TextBox
            {
                Location = new Point(labelWidth + 30, y),
                Size = new Size(controlWidth, 30),
                Font = new Font("Segoe UI", 10),
                Text = "ОРД-" + DateTime.Now.ToString("yyyyMMdd-") + new Random().Next(1000, 9999)
            };
            y += 40;

            // Комментарий
            var lblComments = new Label
            {
                Text = "Комментарий:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10)
            };
            txtComments = new TextBox
            {
                Location = new Point(labelWidth + 30, y),
                Size = new Size(controlWidth, 80),
                Multiline = true,
                Font = new Font("Segoe UI", 10)
            };
            y += 100;

            // Информационная панель
            var infoPanel = new Panel
            {
                Location = new Point(20, y),
                Size = new Size(540, 60),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.AliceBlue
            };

            var lblInfo = new Label
            {
                Text = "ℹ Для операции 'Перемещение' укажите 'Откуда' и 'Куда'.\n" +
                      "Для 'Прихода' укажите 'Куда', для 'Расхода' - 'Откуда'.",
                Location = new Point(10, 10),
                Size = new Size(520, 40),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.DarkSlateGray
            };
            infoPanel.Controls.Add(lblInfo);
            y += 70;

            // Кнопки
            btnProcess = new Button
            {
                Text = "✅ Провести операцию",
                Location = new Point(150, y),
                Size = new Size(180, 45),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnProcess.Click += BtnProcess_Click;

            btnCancel = new Button
            {
                Text = "❌ Отмена",
                Location = new Point(350, y),
                Size = new Size(100, 45),
                BackColor = Color.LightGray,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            panel.Controls.AddRange(new Control[]
            {
                lblType, cmbTransactionType,
                lblProduct, cmbProduct,
                lblCurrentQuantity,
                lblQuantity, numQuantity,
                lblFrom, txtFromLocation,
                lblTo, txtToLocation,
                lblDoc, txtDocumentNumber,
                lblComments, txtComments,
                infoPanel,
                btnProcess, btnCancel
            });

            this.Controls.Add(panel);
            UpdateLocationFields();
        }

        private void LoadProducts()
        {
            try
            {
                var products = dataService.Products ?? new List<Product>();

                if (products.Count == 0)
                {
                    MessageBox.Show("В системе нет товаров. Сначала добавьте товары.", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    return;
                }

                cmbProduct.DataSource = products.OrderBy(p => p.Name).ToList();
                cmbProduct.DisplayMember = "Name";
                cmbProduct.ValueMember = "Id";

                // Устанавливаем первый товар по умолчанию
                if (cmbProduct.Items.Count > 0)
                {
                    cmbProduct.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbTransactionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateLocationFields();
            UpdateCurrentQuantity();
        }

        private void CmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCurrentQuantity();

            var product = cmbProduct.SelectedItem as Product;

            // ДОБАВЛЯЕМ проверку на null:
            if (product == null) return;

            // ДОБАВЛЯЕМ проверку для cmbTransactionType.SelectedItem:
            if (cmbTransactionType.SelectedItem != null)
            {
                var selectedType = (TransactionType)cmbTransactionType.SelectedItem;

                if (selectedType == TransactionType.Приход)
                {
                    txtToLocation.Text = product.Location ?? "";
                }
                else if (selectedType == TransactionType.Расход)
                {
                    txtFromLocation.Text = product.Location ?? "";
                }
                else if (selectedType == TransactionType.Перемещение)
                {
                    txtFromLocation.Text = product.Location ?? "";
                    txtToLocation.Text = "";
                }
            }
            else
            {
                // Если тип операции не выбран, заполняем оба поля
                txtFromLocation.Text = product.Location ?? "";
                txtToLocation.Text = product.Location ?? "";
            }
        }

        private void UpdateLocationFields()
        {
            // ДОБАВЛЯЕМ эту проверку:
            if (cmbTransactionType.SelectedItem == null)
            {
                // Устанавливаем значения по умолчанию
                txtFromLocation.Text = "";
                txtToLocation.Text = "";
                return;
            }

            var transactionType = (TransactionType)cmbTransactionType.SelectedItem;

            txtFromLocation.Enabled = true;
            txtToLocation.Enabled = true;

            if (transactionType == TransactionType.Приход)
            {
                txtFromLocation.Text = "Поставщик";
                txtFromLocation.Enabled = false;
                txtToLocation.Text = "";
                txtToLocation.Enabled = true;
            }
            else if (transactionType == TransactionType.Расход)
            {
                txtFromLocation.Text = "";
                txtFromLocation.Enabled = true;
                txtToLocation.Text = "Клиент";
                txtToLocation.Enabled = false;
            }
            else if (transactionType == TransactionType.Перемещение)
            {
                txtFromLocation.Text = "";
                txtToLocation.Text = "";
                txtFromLocation.Enabled = true;
                txtToLocation.Enabled = true;
            }
        }

        private void UpdateCurrentQuantity()
        {
            var product = cmbProduct.SelectedItem as Product;

            // ДОБАВЛЯЕМ проверку на null:
            if (product != null)
            {
                lblCurrentQuantity.Text = $"Текущее количество: {product.Quantity}";
                lblCurrentQuantity.ForeColor = product.Quantity < 10 ? Color.Red : Color.DarkBlue;

                // ТАКЖЕ ДОБАВЛЯЕМ проверку для cmbTransactionType.SelectedItem:
                if (cmbTransactionType.SelectedItem != null &&
                    (TransactionType)cmbTransactionType.SelectedItem == TransactionType.Расход)
                {
                    numQuantity.Maximum = Math.Max(product.Quantity, 1);
                }
                else
                {
                    numQuantity.Maximum = 10000;
                }
            }
            else
            {
                // Устанавливаем значения по умолчанию
                lblCurrentQuantity.Text = "Текущее количество: 0";
                lblCurrentQuantity.ForeColor = Color.Black;
                numQuantity.Maximum = 10000;
            }
        }

        private void BtnProcess_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                var product = cmbProduct.SelectedItem as Product;

                // Проверка на null
                if (product == null)
                {
                    MessageBox.Show("Ошибка: товар не найден!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var transaction = new Transaction
                {
                    Type = (TransactionType)cmbTransactionType.SelectedItem,
                    ProductId = product.Id,
                    Quantity = (int)numQuantity.Value,
                    FromLocation = txtFromLocation.Text.Trim(),
                    ToLocation = txtToLocation.Text.Trim(),
                    DocumentNumber = txtDocumentNumber.Text.Trim(),
                    Comments = txtComments.Text.Trim(),
                    UserId = currentUser.Id
                };

                bool success = dataService.ProcessTransaction(transaction);

                if (success)
                {
                    MessageBox.Show(
                        $"✅ Операция успешно проведена!\n\n" +
                        $"Тип: {transaction.Type}\n" +
                        $"Товар: {product.Name}\n" +
                        $"Количество: {transaction.Quantity}\n" +
                        $"Документ: {transaction.DocumentNumber}",
                        "Успех",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка проведения операции:\n{ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            // 1. Проверка выбора товара
            if (cmbProduct.SelectedItem == null)
            {
                MessageBox.Show("Выберите товар!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbProduct.Focus();
                return false;
            }

            var product = cmbProduct.SelectedItem as Product;
            if (product == null)
            {
                MessageBox.Show("Ошибка данных товара!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // 2. Проверка выбора типа операции (ДОБАВЛЯЕМ ЭТУ ПРОВЕРКУ)
            if (cmbTransactionType.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип операции!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbTransactionType.Focus();
                return false;
            }

            // 3. Проверка количества
            if (numQuantity.Value <= 0)
            {
                MessageBox.Show("Количество должно быть больше 0!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numQuantity.Focus();
                return false;
            }

            var transactionType = (TransactionType)cmbTransactionType.SelectedItem;

            // 4. Проверка для расхода
            if (transactionType == TransactionType.Расход)
            {
                if (numQuantity.Value > product.Quantity)
                {
                    MessageBox.Show($"Недостаточно товара на складе!\n" +
                                   $"Доступно: {product.Quantity}\n" +
                                   $"Запрошено: {numQuantity.Value}",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            // 5. Проверка полей местоположения
            if (transactionType == TransactionType.Приход)
            {
                if (string.IsNullOrWhiteSpace(txtToLocation.Text))
                {
                    MessageBox.Show("Укажите место, куда поступил товар!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtToLocation.Focus();
                    return false;
                }
            }
            else if (transactionType == TransactionType.Расход)
            {
                if (string.IsNullOrWhiteSpace(txtFromLocation.Text))
                {
                    MessageBox.Show("Укажите место, откуда взят товар!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtFromLocation.Focus();
                    return false;
                }
            }
            else if (transactionType == TransactionType.Перемещение)
            {
                if (string.IsNullOrWhiteSpace(txtFromLocation.Text))
                {
                    MessageBox.Show("Укажите, откуда перемещается товар!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtFromLocation.Focus();
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtToLocation.Text))
                {
                    MessageBox.Show("Укажите, куда перемещается товар!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtToLocation.Focus();
                    return false;
                }

                if (txtFromLocation.Text.Trim() == txtToLocation.Text.Trim())
                {
                    MessageBox.Show("Места 'Откуда' и 'Куда' не должны совпадать!",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            // 6. Проверка номера документа (ДОБАВЛЯЕМ ЭТУ ПРОВЕРКУ)
            if (string.IsNullOrWhiteSpace(txtDocumentNumber.Text))
            {
                MessageBox.Show("Введите номер документа!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDocumentNumber.Focus();
                return false;
            }

            return true;
        }
    }
}