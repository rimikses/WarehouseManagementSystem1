using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WarehouseManagementSystem1.Models;
using WarehouseManagementSystem1.Services;

namespace WarehouseManagementSystem1
{
    public class CustomerManagerForm : Form
    {
        private DataGridView dataGridView1;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnClose;
        private DataService dataService;

        public CustomerManagerForm()
        {
            dataService = DataService.Instance;
            CreateForm();
            LoadCustomers();
        }

        private void CreateForm()
        {
            this.Text = "👥 Управление клиентами";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            // DataGridView
            dataGridView1 = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(740, 400),
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;

            // Кнопки управления
            btnAdd = new Button
            {
                Text = "➕ Добавить клиента",
                Location = new Point(20, 440),
                Size = new Size(150, 35),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White
            };
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button
            {
                Text = "✏️ Редактировать",
                Location = new Point(180, 440),
                Size = new Size(150, 35),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                Enabled = false
            };
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button
            {
                Text = "🗑️ Удалить",
                Location = new Point(340, 440),
                Size = new Size(150, 35),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                Enabled = false
            };
            btnDelete.Click += BtnDelete_Click;

            btnClose = new Button
            {
                Text = "Закрыть",
                Location = new Point(500, 440),
                Size = new Size(150, 35)
            };
            btnClose.Click += (s, e) => this.Close();

            panel.Controls.AddRange(new Control[]
            {
                dataGridView1,
                btnAdd, btnEdit, btnDelete, btnClose
            });

            this.Controls.Add(panel);
        }

        private void LoadCustomers()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("Id", "ID");
            dataGridView1.Columns.Add("Name", "Название");
            dataGridView1.Columns.Add("ContactPerson", "Контактное лицо");
            dataGridView1.Columns.Add("Phone", "Телефон");
            dataGridView1.Columns.Add("Email", "Email");

            if (dataService.Customers != null)
            {
                foreach (var customer in dataService.Customers.Where(c => c.IsActive))
                {
                    dataGridView1.Rows.Add(
                        customer.Id,
                        customer.Name,
                        customer.ContactPerson,
                        customer.Phone,
                        customer.Email
                    );
                }
            }
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            bool hasSelection = dataGridView1.SelectedRows.Count > 0;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new CustomerEditForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // TODO: Добавление клиента в DataService
                    LoadCustomers();
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int customerId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);
                var customer = dataService.Customers?.FirstOrDefault(c => c.Id == customerId);
                if (customer != null)
                {
                    using (var form = new CustomerEditForm(customer))
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            LoadCustomers();
                        }
                    }
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int customerId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);
                var customer = dataService.Customers?.FirstOrDefault(c => c.Id == customerId);
                if (customer != null)
                {
                    var result = MessageBox.Show($"Удалить клиента '{customer.Name}'?", "Подтверждение",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        customer.IsActive = false;
                        // TODO: Сохранение изменений
                        LoadCustomers();
                    }
                }
            }
        }
    }
}