using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WarehouseManagementSystem1.Models;
using WarehouseManagementSystem1.Services;

namespace WarehouseManagementSystem1
{
    public class CategoryManagerForm : Form
    {
        private DataGridView dataGridView1;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnClose;
        private TextBox txtName;
        private TextBox txtDescription;
        private DataService dataService;

        public CategoryManagerForm()
        {
            dataService = DataService.Instance;
            CreateForm();
            LoadCategories();

            // Подписываемся на событие обновления данных
            dataService.DataChanged += DataService_DataChanged;
        }

        private void CreateForm()
        {
            this.Text = "📁 Управление категориями";
            this.Size = new Size(600, 500);
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

            // Поле для описания
            var lblDescription = new Label
            {
                Text = "Описание:",
                Location = new Point(20, y),
                Size = new Size(labelWidth, 25),
                Font = new Font("Segoe UI", 10)
            };
            txtDescription = new TextBox
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

            // DataGridView для категорий
            dataGridView1 = new DataGridView
            {
                Location = new Point(20, y),
                Size = new Size(540, 200),
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
                Location = new Point(240, y),
                Size = new Size(100, 35),
                Font = new Font("Segoe UI", 10)
            };
            btnClose.Click += (s, e) => this.Close();

            panel.Controls.AddRange(new Control[]
            {
                lblName, txtName,
                lblDescription, txtDescription,
                btnAdd, btnEdit, btnDelete,
                dataGridView1,
                btnClose
            });

            this.Controls.Add(panel);
        }

        private void LoadCategories()
        {
            try
            {
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                // Настраиваем колонки
                dataGridView1.Columns.Add("Id", "ID");
                dataGridView1.Columns.Add("Name", "Название");
                dataGridView1.Columns.Add("Description", "Описание");

                // Заполняем данными
                if (dataService.Categories != null)
                {
                    foreach (var category in dataService.Categories.OrderBy(c => c.Name))
                    {
                        dataGridView1.Rows.Add(category.Id, category.Name, category.Description);
                    }
                }

                // Автоматически подгоняем ширину столбцов
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DataService_DataChanged()
        {
            // Проверяем, что форма еще открыта и видима
            if (this.IsHandleCreated && !this.IsDisposed && this.Visible)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        LoadCategories();
                    });
                }
                else
                {
                    LoadCategories();
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
                txtDescription.Text = selectedRow.Cells["Description"].Value?.ToString() ?? "";
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
                var newCategory = new Category
                {
                    Id = dataService.Categories.Any() ?
                        dataService.Categories.Max(c => c.Id) + 1 : 1,
                    Name = txtName.Text.Trim(),
                    Description = txtDescription.Text.Trim()
                };

                dataService.Categories.Add(newCategory);
                dataService.SaveToJson(); // Этот метод уже вызывает DataChanged

                // НЕ вызываем событие здесь - SaveToJson() делает это автоматически

                MessageBox.Show("Категория успешно добавлена!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearFields();
                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления категории: {ex.Message}", "Ошибка",
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
                int categoryId = Convert.ToInt32(selectedRow.Cells["Id"].Value);

                var category = dataService.Categories.FirstOrDefault(c => c.Id == categoryId);
                if (category != null)
                {
                    category.Name = txtName.Text.Trim();
                    category.Description = txtDescription.Text.Trim();

                    dataService.SaveToJson(); // Этот метод уже вызывает DataChanged

                    // НЕ вызываем событие здесь

                    MessageBox.Show("Категория успешно обновлена!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ClearFields();
                    dataGridView1.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления категории: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;

            var selectedRow = dataGridView1.SelectedRows[0];
            int categoryId = Convert.ToInt32(selectedRow.Cells["Id"].Value);
            string categoryName = selectedRow.Cells["Name"].Value?.ToString() ?? "";

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить категорию?\n\n" +
                $"📁 Название: {categoryName}\n\n" +
                $"⚠ Внимание: Удаление категории не удалит связанные товары!",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var category = dataService.Categories.FirstOrDefault(c => c.Id == categoryId);
                    if (category != null)
                    {
                        dataService.Categories.Remove(category);
                        dataService.SaveToJson(); // Этот метод уже вызывает DataChanged

                        // НЕ вызываем событие здесь

                        MessageBox.Show("Категория успешно удалена!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        ClearFields();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления категории: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название категории!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            // Проверка на уникальность имени
            var existingCategory = dataService.Categories?
                .FirstOrDefault(c => c.Name.Trim().ToLower() == txtName.Text.Trim().ToLower());

            if (existingCategory != null && dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Категория с таким названием уже существует!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            return true;
        }

        private void ClearFields()
        {
            txtName.Text = "";
            txtDescription.Text = "";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Отписываемся от события при закрытии формы
            dataService.DataChanged -= DataService_DataChanged;
            base.OnFormClosing(e);
        }
    }
}