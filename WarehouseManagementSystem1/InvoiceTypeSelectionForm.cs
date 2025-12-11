using System;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagementSystem1.Enums;

namespace WarehouseManagementSystem1
{
    public partial class InvoiceTypeSelectionForm : Form
    {
        public InvoiceType SelectedType { get; private set; }

        public InvoiceTypeSelectionForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Выберите тип накладной";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(40) };

            var lblTitle = new Label
            {
                Text = "📄 Выберите тип операции",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(0, 20),
                Size = new Size(320, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };

            int y = 80;

            // Приходная накладная
            var btnIncome = new Button
            {
                Text = "📥 Приход от поставщика",
                Location = new Point(40, y),
                Size = new Size(280, 50),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Tag = InvoiceType.Приходная
            };
            btnIncome.Click += BtnType_Click;
            y += 60;

            // Расходная накладная
            var btnOutcome = new Button
            {
                Text = "📤 Расход клиенту",
                Location = new Point(40, y),
                Size = new Size(280, 50),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Tag = InvoiceType.Расходная
            };
            btnOutcome.Click += BtnType_Click;
            y += 60;

            // Внутренняя накладная
            var btnInternal = new Button
            {
                Text = "🔄 Внутреннее перемещение",
                Location = new Point(40, y),
                Size = new Size(280, 50),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Tag = InvoiceType.Внутренняя
            };
            btnInternal.Click += BtnType_Click;

            var btnCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(140, 200),
                Size = new Size(120, 35),
                BackColor = Color.LightGray
            };
            btnCancel.Click += (s, e) => this.Close();

            panel.Controls.AddRange(new Control[] { lblTitle, btnIncome, btnOutcome, btnInternal, btnCancel });
            this.Controls.Add(panel);
        }

        private void BtnType_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            SelectedType = (InvoiceType)button.Tag;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}