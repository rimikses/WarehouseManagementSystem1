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
    public partial class InvoiceTransactionForm : Form
    {
        private TabControl tabControl;
        private ComboBox cmbInvoiceType;
        private DateTimePicker dtpInvoiceDate;
        private ComboBox cmbSupplier;
        private ComboBox cmbCustomer;
        private TextBox txtContractNumber;
        private TextBox txtWaybillNumber;
        private TextBox txtVehicleNumber;
        private TextBox txtDriverName;
        private TextBox txtNotes;

        private DataGridView gridInvoiceItems;
        private Button btnAddItem;
        private Button btnRemoveItem;
        private ComboBox cmbProduct;
        private NumericUpDown numQuantity;
        private NumericUpDown numPrice;

        private Button btnSave;
        private Button btnPrint;
        private Button btnCancel;

        private User currentUser;
        private Invoice currentInvoice;
        private List<InvoiceItem> invoiceItems = new List<InvoiceItem>();
        private Label lblTotal;

        public InvoiceTransactionForm(User user, InvoiceType invoiceType)
        {
            currentUser = user;
            currentInvoice = new Invoice
            {
                Type = invoiceType,
                InvoiceNumber = GenerateInvoiceNumber(invoiceType),
                InvoiceDate = DateTime.Now,
                CreatedBy = user.Username
            };

            InitializeComponent();
            LoadData();
            UpdateTotals();
        }

        private void InitializeComponent()
        {
            this.Text = "📄 Оформление накладной";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterParent;

            tabControl = new TabControl { Dock = DockStyle.Fill };

            // Вкладка 1: Основные данные
            var tabBasic = new TabPage("📋 Основные данные");
            CreateBasicTab(tabBasic);
            tabControl.TabPages.Add(tabBasic);

            // Вкладка 2: Товары
            var tabItems = new TabPage("📦 Товары");
            CreateItemsTab(tabItems);
            tabControl.TabPages.Add(tabItems);

            // Вкладка 3: Печать
            var tabPrint = new TabPage("🖨️ Печать");
            CreatePrintTab(tabPrint);
            tabControl.TabPages.Add(tabPrint);

            // Панель кнопок
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            btnSave = new Button
            {
                Text = "💾 Сохранить накладную",
                Location = new Point(20, 15),
                Size = new Size(180, 35),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSave.Click += BtnSave_Click;

            btnPrint = new Button
            {
                Text = "🖨️ Сохранить и распечатать",
                Location = new Point(210, 15),
                Size = new Size(200, 35),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnPrint.Click += BtnPrint_Click;

            btnCancel = new Button
            {
                Text = "❌ Отмена",
                Location = new Point(420, 15),
                Size = new Size(120, 35),
                BackColor = Color.LightGray,
                Font = new Font("Segoe UI", 10)
            };
            btnCancel.Click += (s, e) => this.Close();

            buttonPanel.Controls.AddRange(new Control[] { btnSave, btnPrint, btnCancel });

            this.Controls.AddRange(new Control[] { tabControl, buttonPanel });
        }

        private void CreateBasicTab(TabPage tab)
        {
            int y = 20;
            int labelWidth = 180;

            // Тип накладной
            var lblType = new Label
            {
                Text = "Тип накладной:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10)
            };
            cmbInvoiceType = new ComboBox
            {
                Location = new Point(210, y),
                Size = new Size(300, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbInvoiceType.Items.AddRange(Enum.GetValues(typeof(InvoiceType)).Cast<InvoiceType>().Cast<object>().ToArray());
            cmbInvoiceType.SelectedItem = currentInvoice.Type;
            cmbInvoiceType.SelectedIndexChanged += CmbInvoiceType_SelectedIndexChanged;
            y += 40;

            // Номер и дата
            var lblNumber = new Label
            {
                Text = "Номер накладной:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, 25)
            };
            var txtInvoiceNumber = new TextBox
            {
                Location = new Point(210, y),
                Size = new Size(150, 30),
                Text = currentInvoice.InvoiceNumber,
                ReadOnly = true,
                BackColor = Color.LightGray
            };

            var lblDate = new Label
            {
                Text = "Дата:",
                Location = new Point(370, y),
                Size = new Size(50, 25)
            };
            dtpInvoiceDate = new DateTimePicker
            {
                Location = new Point(430, y),
                Size = new Size(150, 30),
                Value = currentInvoice.InvoiceDate,
                Format = DateTimePickerFormat.Short
            };
            y += 40;

            // Поставщик (для приходной накладной)
            var lblSupplier = new Label
            {
                Text = "Поставщик:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, 25)
            };
            cmbSupplier = new ComboBox
            {
                Location = new Point(210, y),
                Size = new Size(300, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "Name"
            };

            // Кнопка добавления нового поставщика
            var btnAddSupplier = new Button
            {
                Text = "+",
                Location = new Point(520, y),
                Size = new Size(30, 30),
                BackColor = Color.LightGray
            };
            btnAddSupplier.Click += (s, e) => ShowSupplierManager();
            y += 40;

            // Клиент (для расходной накладной)
            var lblCustomer = new Label
            {
                Text = "Клиент/Покупатель:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, 25)
            };
            cmbCustomer = new ComboBox
            {
                Location = new Point(210, y),
                Size = new Size(300, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "Name"
            };

            var btnAddCustomer = new Button
            {
                Text = "+",
                Location = new Point(520, y),
                Size = new Size(30, 30),
                BackColor = Color.LightGray
            };
            btnAddCustomer.Click += (s, e) => ShowCustomerManager();
            y += 40;

            // Договор и ТТН
            var lblContract = new Label
            {
                Text = "Номер договора:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, 25)
            };
            txtContractNumber = new TextBox
            {
                Location = new Point(210, y),
                Size = new Size(300, 30)
            };
            y += 40;

            var lblWaybill = new Label
            {
                Text = "Номер ТТН:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, 25)
            };
            txtWaybillNumber = new TextBox
            {
                Location = new Point(210, y),
                Size = new Size(300, 30)
            };
            y += 40;

            // Транспорт
            var lblVehicle = new Label
            {
                Text = "Номер машины:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, 25)
            };
            txtVehicleNumber = new TextBox
            {
                Location = new Point(210, y),
                Size = new Size(300, 30)
            };
            y += 40;

            var lblDriver = new Label
            {
                Text = "Водитель:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, 25)
            };
            txtDriverName = new TextBox
            {
                Location = new Point(210, y),
                Size = new Size(300, 30)
            };
            y += 40;

            // Ответственные лица
            var lblReceived = new Label
            {
                Text = "Принял (ФИО):",
                Location = new Point(20, y),
                Size = new Size(labelWidth, 25)
            };
            var txtReceivedBy = new TextBox
            {
                Location = new Point(210, y),
                Size = new Size(300, 30)
            };
            y += 40;

            var lblReleased = new Label
            {
                Text = "Отпустил (ФИО):",
                Location = new Point(20, y),
                Size = new Size(labelWidth, 25)
            };
            var txtReleasedBy = new TextBox
            {
                Location = new Point(210, y),
                Size = new Size(300, 30)
            };
            y += 40;

            // Примечания
            var lblNotes = new Label
            {
                Text = "Примечания:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, 25)
            };
            txtNotes = new TextBox
            {
                Location = new Point(210, y),
                Size = new Size(300, 100),
                Multiline = true
            };

            tab.Controls.AddRange(new Control[]
            {
                lblType, cmbInvoiceType,
                lblNumber, txtInvoiceNumber, lblDate, dtpInvoiceDate,
                lblSupplier, cmbSupplier, btnAddSupplier,
                lblCustomer, cmbCustomer, btnAddCustomer,
                lblContract, txtContractNumber,
                lblWaybill, txtWaybillNumber,
                lblVehicle, txtVehicleNumber,
                lblDriver, txtDriverName,
                lblReceived, txtReceivedBy,
                lblReleased, txtReleasedBy,
                lblNotes, txtNotes
            });
        }

        private void CreateItemsTab(TabPage tab)
        {
            // Таблица товаров
            gridInvoiceItems = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(700, 300),
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            ConfigureItemsGrid();

            // Панель добавления товара
            var addPanel = new Panel
            {
                Location = new Point(20, 340),
                Size = new Size(700, 100),
                BorderStyle = BorderStyle.FixedSingle
            };

            int x = 10;
            int yAdd = 20;

            var lblProduct = new Label { Text = "Товар:", Location = new Point(x, yAdd), Size = new Size(60, 25) };
            cmbProduct = new ComboBox
            {
                Location = new Point(x + 65, yAdd),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "Name"
            };

            var lblQuantity = new Label { Text = "Кол-во:", Location = new Point(x + 280, yAdd), Size = new Size(60, 25) };
            numQuantity = new NumericUpDown
            {
                Location = new Point(x + 345, yAdd),
                Size = new Size(80, 25),
                Minimum = 1,
                Maximum = 10000
            };

            var lblPrice = new Label { Text = "Цена:", Location = new Point(x + 440, yAdd), Size = new Size(60, 25) };
            numPrice = new NumericUpDown
            {
                Location = new Point(x + 505, yAdd),
                Size = new Size(100, 25),
                DecimalPlaces = 2,
                Minimum = 0,
                Maximum = 1000000
            };

            btnAddItem = new Button
            {
                Text = "➕ Добавить",
                Location = new Point(x + 620, yAdd - 5),
                Size = new Size(70, 35),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White
            };
            btnAddItem.Click += BtnAddItem_Click;

            btnRemoveItem = new Button
            {
                Text = "🗑️ Удалить",
                Location = new Point(20, 450),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                Enabled = false
            };
            btnRemoveItem.Click += BtnRemoveItem_Click;

            // Итоги
            lblTotal = new Label
            {
                Text = "Итого: 0.00 руб.",
                Location = new Point(400, 450),
                Size = new Size(300, 35),
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.DarkGreen,
                TextAlign = ContentAlignment.MiddleRight
            };

            addPanel.Controls.AddRange(new Control[] { lblProduct, cmbProduct, lblQuantity, numQuantity, lblPrice, numPrice, btnAddItem });

            tab.Controls.AddRange(new Control[] { gridInvoiceItems, addPanel, btnRemoveItem, lblTotal });
        }

        private void CreatePrintTab(TabPage tab)
        {
            // Предварительный просмотр накладной
            var webBrowser = new WebBrowser
            {
                Dock = DockStyle.Fill,
                ScriptErrorsSuppressed = true
            };

            // Кнопки печати
            var btnPreview = new Button
            {
                Text = "👁️ Предварительный просмотр",
                Location = new Point(20, 20),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White
            };
            btnPreview.Click += (s, e) => GenerateInvoicePreview(webBrowser);

            var btnPrintDirect = new Button
            {
                Text = "🖨️ Печать накладной",
                Location = new Point(240, 20),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White
            };
            btnPrintDirect.Click += BtnPrintDirect_Click;

            var btnExportPDF = new Button
            {
                Text = "📄 Экспорт в PDF",
                Location = new Point(460, 20),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(156, 39, 176),
                ForeColor = Color.White
            };
            btnExportPDF.Click += BtnExportPDF_Click;

            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.WhiteSmoke
            };
            panel.Controls.AddRange(new Control[] { btnPreview, btnPrintDirect, btnExportPDF });

            tab.Controls.AddRange(new Control[] { panel, webBrowser });
        }

        private void ConfigureItemsGrid()
        {
            gridInvoiceItems.Columns.Clear();
            gridInvoiceItems.Columns.Add("ProductName", "Товар");
            gridInvoiceItems.Columns.Add("Quantity", "Кол-во");
            gridInvoiceItems.Columns.Add("Price", "Цена");
            gridInvoiceItems.Columns.Add("Amount", "Сумма");
            gridInvoiceItems.Columns.Add("Unit", "Ед.изм.");

            gridInvoiceItems.Columns["Price"].DefaultCellStyle.Format = "C2";
            gridInvoiceItems.Columns["Amount"].DefaultCellStyle.Format = "C2";
            gridInvoiceItems.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gridInvoiceItems.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gridInvoiceItems.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void LoadData()
        {
            var dataService = DataService.Instance;

            // Загрузка поставщиков
            if (dataService.Suppliers != null)
            {
                cmbSupplier.DataSource = dataService.Suppliers
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.Name)
                    .ToList();
            }

            // Загрузка клиентов
            if (dataService.Customers != null)
            {
                cmbCustomer.DataSource = dataService.Customers
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .ToList();
            }

            // Загрузка товаров
            if (dataService.Products != null)
            {
                cmbProduct.DataSource = dataService.Products
                    .OrderBy(p => p.Name)
                    .ToList();
            }

            UpdateFormVisibility();
        }

        private void UpdateFormVisibility()
        {
            bool isIncome = currentInvoice.Type == InvoiceType.Приходная;
            bool isOutcome = currentInvoice.Type == InvoiceType.Расходная;

            cmbSupplier.Enabled = isIncome;
            cmbCustomer.Enabled = isOutcome;
        }

        private void UpdateTotals()
        {
            decimal total = invoiceItems.Sum(i => i.Amount);
            lblTotal.Text = $"Итого: {total:F2} руб.";
        }

        private string GenerateInvoiceNumber(InvoiceType type)
        {
            string prefix = type == InvoiceType.Приходная ? "ПН" :
                           type == InvoiceType.Расходная ? "РН" : "ВН";

            return $"{prefix}-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInvoice())
            {
                SaveInvoice(false);
                MessageBox.Show("Накладная сохранена успешно!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            if (ValidateInvoice())
            {
                SaveInvoice(true);
                MessageBox.Show("Накладная сохранена и отправлена на печать!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private bool ValidateInvoice()
        {
            if (invoiceItems.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы один товар в накладную!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (currentInvoice.Type == InvoiceType.Приходная && cmbSupplier.SelectedItem == null)
            {
                MessageBox.Show("Выберите поставщика для приходной накладной!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (currentInvoice.Type == InvoiceType.Расходная && cmbCustomer.SelectedItem == null)
            {
                MessageBox.Show("Выберите клиента для расходной накладной!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void SaveInvoice(bool print)
        {
            // Сохранение данных формы в currentInvoice
            currentInvoice.InvoiceDate = dtpInvoiceDate.Value;
            currentInvoice.ContractNumber = txtContractNumber.Text;
            currentInvoice.WaybillNumber = txtWaybillNumber.Text;
            currentInvoice.VehicleNumber = txtVehicleNumber.Text;
            currentInvoice.DriverName = txtDriverName.Text;
            currentInvoice.Notes = txtNotes.Text;

            if (currentInvoice.Type == InvoiceType.Приходная)
            {
                currentInvoice.Supplier = cmbSupplier.SelectedItem as Supplier;
            }
            else if (currentInvoice.Type == InvoiceType.Расходная)
            {
                currentInvoice.Customer = cmbCustomer.SelectedItem as Customer;
            }

            // Расчет итогов
            currentInvoice.TotalQuantity = invoiceItems.Sum(i => i.Quantity);
            currentInvoice.TotalAmount = invoiceItems.Sum(i => i.Amount);
            currentInvoice.VAT = currentInvoice.TotalAmount * 0.20m; // 20% НДС
            currentInvoice.TotalWithVAT = currentInvoice.TotalAmount + currentInvoice.VAT;

            // TODO: Сохранение в DataService
            // var dataService = DataService.Instance;
            // dataService.AddInvoice(currentInvoice);

            if (print)
            {
                PrintInvoice();
            }
        }

        private void PrintInvoice()
        {
            // Генерация HTML для печати
            string html = GenerateInvoiceHtml();

            // Использование WebBrowser для печати
            var printForm = new Form { Size = new Size(800, 600) };
            var browser = new WebBrowser
            {
                Dock = DockStyle.Fill,
                DocumentText = html
            };
            printForm.Controls.Add(browser);

            // Даем время на загрузку
            browser.DocumentCompleted += (s, e) =>
            {
                browser.ShowPrintDialog();
                printForm.Close();
            };

            printForm.ShowDialog();
        }

        private string GenerateInvoiceHtml()
        {
            // Упрощенная HTML накладная
            return $@"
            <html>
            <body>
                <h2>Накладная № {currentInvoice.InvoiceNumber}</h2>
                <p>Дата: {currentInvoice.InvoiceDate:dd.MM.yyyy}</p>
                <p>Тип: {currentInvoice.Type}</p>
                <table border='1' style='width:100%'>
                    <tr>
                        <th>Товар</th>
                        <th>Кол-во</th>
                        <th>Цена</th>
                        <th>Сумма</th>
                    </tr>
                    {string.Join("", invoiceItems.Select(item => $@"
                    <tr>
                        <td>{item.Product?.Name ?? "Товар"}</td>
                        <td>{item.Quantity}</td>
                        <td>{item.Price:F2}</td>
                        <td>{item.Amount:F2}</td>
                    </tr>"))}
                </table>
                <h3>Итого: {currentInvoice.TotalAmount:F2} руб.</h3>
            </body>
            </html>";
        }

        private void BtnAddItem_Click(object sender, EventArgs e)
        {
            var product = cmbProduct.SelectedItem as Product;
            if (product == null)
            {
                MessageBox.Show("Выберите товар!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var item = new InvoiceItem
            {
                Product = product,
                ProductId = product.Id,
                Quantity = (int)numQuantity.Value,
                Price = numPrice.Value,
                Unit = "шт."
            };

            invoiceItems.Add(item);
            RefreshItemsGrid();
            UpdateTotals();

            // Сброс полей
            numQuantity.Value = 1;
            numPrice.Value = product.Price;
        }

        private void BtnRemoveItem_Click(object sender, EventArgs e)
        {
            if (gridInvoiceItems.SelectedRows.Count > 0)
            {
                int index = gridInvoiceItems.SelectedRows[0].Index;
                if (index >= 0 && index < invoiceItems.Count)
                {
                    invoiceItems.RemoveAt(index);
                    RefreshItemsGrid();
                    UpdateTotals();
                }
            }
        }

        private void RefreshItemsGrid()
        {
            gridInvoiceItems.Rows.Clear();
            foreach (var item in invoiceItems)
            {
                gridInvoiceItems.Rows.Add(
                    item.Product?.Name ?? "Неизвестно",
                    item.Quantity,
                    item.Price,
                    item.Amount,
                    item.Unit
                );
            }

            btnRemoveItem.Enabled = invoiceItems.Count > 0;
        }

        private void CmbInvoiceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbInvoiceType.SelectedItem is InvoiceType selectedType)
            {
                currentInvoice.Type = selectedType;
                UpdateFormVisibility();
            }
        }

        private void ShowSupplierManager()
        {
            using (var form = new SupplierManagerForm())
            {
                form.ShowDialog();
                LoadData();
            }
        }

        private void ShowCustomerManager()
        {
            using (var form = new CustomerManagerForm())
            {
                form.ShowDialog();
                LoadData();
            }
        }

        private void GenerateInvoicePreview(WebBrowser browser)
        {
            string html = GenerateInvoiceHtml();
            browser.DocumentText = html;
        }

        private void BtnPrintDirect_Click(object sender, EventArgs e)
        {
            PrintInvoice();
        }

        private void BtnExportPDF_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Экспорт в PDF будет реализован в следующей версии", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnRemoveItem_Click_1(object sender, EventArgs e)
        {
            // Простая реализация удаления
            if (gridInvoiceItems.SelectedRows.Count > 0)
            {
                invoiceItems.RemoveAt(gridInvoiceItems.SelectedRows[0].Index);
                RefreshItemsGrid();
                UpdateTotals();
            }
        }
    }
}