using System;
using System.Drawing;
using System.Windows.Forms;

namespace WarehouseManagementSystem1
{
    public class InvoiceArchiveForm : Form
    {
        public InvoiceArchiveForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "📋 Архив накладных";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            var label = new Label
            {
                Text = "Архив накладных будет реализован в следующей версии",
                Font = new Font("Arial", 14),
                Location = new Point(100, 100),
                Size = new Size(600, 100),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var btnClose = new Button
            {
                Text = "Закрыть",
                Location = new Point(350, 250),
                Size = new Size(100, 35)
            };
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { label, btnClose });
        }
    }
}