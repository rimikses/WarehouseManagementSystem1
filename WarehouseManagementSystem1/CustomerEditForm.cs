using System;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagementSystem1.Models;

namespace WarehouseManagementSystem1
{
    public class CustomerEditForm : Form
    {
        private Customer customer;
        private TextBox txtName;
        private TextBox txtContactPerson;
        private TextBox txtPhone;
        private TextBox txtEmail;
        private TextBox txtAddress;
        private Button btnSave;
        private Button btnCancel;

        public CustomerEditForm()
        {
            customer = new Customer();
            InitializeComponent();
        }

        public CustomerEditForm(Customer existingCustomer)
        {
            customer = existingCustomer;
            InitializeComponent();
            LoadCustomerData();
        }

        private void InitializeComponent()
        {
            this.Text = customer.Id > 0 ? "✏️ Редактирование клиента" : "➕ Новый клиент";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            int y = 20;

            // Название
            var lblName = new Label { Text = "Название*:", Location = new Point(20, y), Size = new Size(150, 25) };
            txtName = new TextBox { Location = new Point(180, y), Size = new Size(280, 25) };
            y += 35;

            // Контактное лицо
            var lblContact = new Label { Text = "Контактное лицо:", Location = new Point(20, y), Size = new Size(150, 25) };
            txtContactPerson = new TextBox { Location = new Point(180, y), Size = new Size(280, 25) };
            y += 35;

            // Телефон
            var lblPhone = new Label { Text = "Телефон:", Location = new Point(20, y), Size = new Size(150, 25) };
            txtPhone = new TextBox { Location = new Point(180, y), Size = new Size(280, 25) };
            y += 35;

            // Email
            var lblEmail = new Label { Text = "Email:", Location = new Point(20, y), Size = new Size(150, 25) };
            txtEmail = new TextBox { Location = new Point(180, y), Size = new Size(280, 25) };
            y += 35;

            // Адрес
            var lblAddress = new Label { Text = "Адрес:", Location = new Point(20, y), Size = new Size(150, 25) };
            txtAddress = new TextBox { Location = new Point(180, y), Size = new Size(280, 60), Multiline = true };
            y += 70;

            // Кнопки
            btnSave = new Button
            {
                Text = "💾 Сохранить",
                Location = new Point(150, y),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(260, y),
                Size = new Size(100, 35),
                BackColor = Color.LightGray
            };
            btnCancel.Click += (s, e) => this.Close();

            panel.Controls.AddRange(new Control[]
            {
                lblName, txtName,
                lblContact, txtContactPerson,
                lblPhone, txtPhone,
                lblEmail, txtEmail,
                lblAddress, txtAddress,
                btnSave, btnCancel
            });

            this.Controls.Add(panel);
        }

        private void LoadCustomerData()
        {
            txtName.Text = customer.Name;
            txtContactPerson.Text = customer.ContactPerson;
            txtPhone.Text = customer.Phone;
            txtEmail.Text = customer.Email;
            txtAddress.Text = customer.Address;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название клиента!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            customer.Name = txtName.Text.Trim();
            customer.ContactPerson = txtContactPerson.Text.Trim();
            customer.Phone = txtPhone.Text.Trim();
            customer.Email = txtEmail.Text.Trim();
            customer.Address = txtAddress.Text.Trim();

            // TODO: Сохранение в DataService

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}