using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WarehouseManagementSystem1.Models;
using WarehouseManagementSystem1.Services;

namespace WarehouseManagementSystem1
{
    public class SupplierManagerForm : Form
    {
        private DataGridView dataGridView1;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnClose;
        private TextBox txtName;
        private TextBox txtPhone;
        private TextBox txtEmail;
        private TextBox txtAddress;
        private DataService dataService;

        public SupplierManagerForm()
        {
            dataService = DataService.Instance;
            CreateForm();
            LoadSuppliers();

            // Подписываемся на событие обновления данных
            dataService.DataChanged += DataService_DataChanged;
        }

        private void CreateForm()
        {
            this.Text = "🏢 Управление поставщиками";
            this.Size = new Size(700, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            int y = 20;
            int labelWidth = 100;

            // Поле для названия
            var lblName = new Label
            {
                Text = "Название*:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10)
            };
            txtName = new TextBox
            {
                Location = new Point(130, y),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 10)
            };
            y += 35;

            // Поле для телефона
            var lblPhone = new Label
            {
                Text = "Телефон:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10)
            };
            txtPhone = new TextBox
            {
                Location = new Point(130, y),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 10)
            };
            y += 35;

            // Поле для email
            var lblEmail = new Label
            {
                Text = "Email:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10)
            };
            txtEmail = new TextBox
            {
                Location = new Point(130, y),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 10)
            };
            y += 35;

            // Поле для адреса
            var lblAddress = new Label
            {
                Text = "Адрес:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10)
            };
            txtAddress = new TextBox
            {
                Location = new Point(130, y),
                Size = new Size(300, 60),
                Multiline = true,
                Font = new Font("Segoe UI", 10)
            };
            y += 70;

            // Кнопки управления
            btnAdd = new Button
            {
                Text = "➕ Добавить",
                Location = new Point(130, y),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button
            {
                Text = "✏️ Редактировать",
                Location = new Point(240, y),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                Enabled = false,
                Font = new Font("Segoe UI", 10)
            };
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button
            {
                Text = "🗑️ Удалить",
                Location = new Point(370, y),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                Enabled = false,
                Font = new Font("Segoe UI", 10)
            };
            btnDelete.Click += BtnDelete_Click;
            y += 50;

            // DataGridView для поставщиков
            dataGridView1 = new DataGridView
            {
                Location = new Point(20, y),
                Size = new Size(640, 200),
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
            dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;
            y += 220;

            // Кнопка закрытия
            btnClose = new Button
            {
                Text = "Закрыть",
                Location = new Point(280, y),
                Size = new Size(100, 35),
                Font = new Font("Segoe UI", 10)
            };
            btnClose.Click += (s, e) => this.Close();

            panel.Controls.AddRange(new Control[]
            {
                lblName, txtName,
                lblPhone, txtPhone,
                lblEmail, txtEmail,
                lblAddress, txtAddress,
                btnAdd, btnEdit, btnDelete,
                dataGridView1,
                btnClose
            });

            this.Controls.Add(panel);
        }

        private void LoadSuppliers()
        {
            try
            {
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                // Настраиваем колонки
                dataGridView1.Columns.Add("Id", "ID");
                dataGridView1.Columns.Add("Name", "Название");
                dataGridView1.Columns.Add("Phone", "Телефон");
                dataGridView1.Columns.Add("Email", "Email");
                dataGridView1.Columns.Add("Address", "Адрес");

                // Заполняем данными
                if (dataService.Suppliers != null)
                {
                    foreach (var supplier in dataService.Suppliers.OrderBy(s => s.Name))
                    {
                        dataGridView1.Rows.Add(
                            supplier.Id,
                            supplier.Name,
                            supplier.Phone,
                            supplier.Email,
                            supplier.Address
                        );
                    }
                }

                // Автоматически подгоняем ширину столбцов
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки поставщиков: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DataService_DataChanged()
        {
            if (this.IsHandleCreated && !this.IsDisposed && this.Visible)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        LoadSuppliers();
                    });
                }
                else
                {
                    LoadSuppliers();
                }
            }
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            bool hasSelection = dataGridView1.SelectedRows.Count > 0;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;

            if (hasSelection)
            {
                var selectedRow = dataGridView1.SelectedRows[0];
                txtName.Text = selectedRow.Cells["Name"].Value?.ToString() ?? "";
                txtPhone.Text = selectedRow.Cells["Phone"].Value?.ToString() ?? "";
                txtEmail.Text = selectedRow.Cells["Email"].Value?.ToString() ?? "";
                txtAddress.Text = selectedRow.Cells["Address"].Value?.ToString() ?? "";
            }
            else
            {
                ClearFields();
            }
        }

        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                BtnEdit_Click(null, null);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                var newSupplier = new Supplier
                {
                    Id = dataService.Suppliers.Any() ?
                        dataService.Suppliers.Max(s => s.Id) + 1 : 1,
                    Name = txtName.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Address = txtAddress.Text.Trim()
                };

                dataService.Suppliers.Add(newSupplier);
                dataService.SaveToJson();

                MessageBox.Show("Поставщик успешно добавлен!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearFields();
                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления поставщика: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;
            if (!ValidateInput()) return;

            try
            {
                var selectedRow = dataGridView1.SelectedRows[0];
                int supplierId = Convert.ToInt32(selectedRow.Cells["Id"].Value);

                var supplier = dataService.Suppliers.FirstOrDefault(s => s.Id == supplierId);
                if (supplier != null)
                {
                    supplier.Name = txtName.Text.Trim();
                    supplier.Phone = txtPhone.Text.Trim();
                    supplier.Email = txtEmail.Text.Trim();
                    supplier.Address = txtAddress.Text.Trim();

                    dataService.SaveToJson();

                    MessageBox.Show("Поставщик успешно обновлен!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ClearFields();
                    dataGridView1.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления поставщика: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;

            var selectedRow = dataGridView1.SelectedRows[0];
            int supplierId = Convert.ToInt32(selectedRow.Cells["Id"].Value);
            string supplierName = selectedRow.Cells["Name"].Value?.ToString() ?? "";

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить поставщика?\n\n" +
                $"🏢 Название: {supplierName}",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var supplier = dataService.Suppliers.FirstOrDefault(s => s.Id == supplierId);
                    if (supplier != null)
                    {
                        dataService.Suppliers.Remove(supplier);
                        dataService.SaveToJson();

                        MessageBox.Show("Поставщик успешно удален!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        ClearFields();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления поставщика: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название поставщика!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            return true;
        }

        private void ClearFields()
        {
            txtName.Text = "";
            txtPhone.Text = "";
            txtEmail.Text = "";
            txtAddress.Text = "";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Отписываемся от события при закрытии формы
            dataService.DataChanged -= DataService_DataChanged;
            base.OnFormClosing(e);
        }
    }
}