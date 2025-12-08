using System;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagementSystem1.Services;

namespace WarehouseManagementSystem1
{
    public partial class Form1 : Form
    {
        private Button btnLaunchLogin;
        private Button btnTestData;
        private Button btnDiagnostic;
        private Button btnSimpleLogin;
        private Button btnForceCreate;  // НОВАЯ КНОПКА
        private Label lblInfo;

        public Form1()
        {
            InitializeComponent();
            CreateLauncherForm();
        }

        private void CreateLauncherForm()
        {
            // Настройка формы-лаунчера
            this.Text = "🚀 Складской учет - Лаунчер";
            this.Size = new Size(500, 450);  // УВЕЛИЧИЛИ ВЫСОТУ
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Заголовок
            var titleLabel = new Label();
            titleLabel.Text = "Добро пожаловать!";
            titleLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(33, 150, 243);
            titleLabel.Location = new Point(100, 30);
            titleLabel.Size = new Size(300, 40);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(titleLabel);

            // Подзаголовок
            var subtitleLabel = new Label();
            subtitleLabel.Text = "Система управления складом с авторизацией";
            subtitleLabel.Font = new Font("Segoe UI", 10);
            subtitleLabel.ForeColor = Color.Gray;
            subtitleLabel.Location = new Point(80, 75);
            subtitleLabel.Size = new Size(340, 25);
            subtitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(subtitleLabel);

            int y = 120; // Начальная позиция Y

            // Основная кнопка - запуск формы входа
            btnLaunchLogin = new Button();
            btnLaunchLogin.Text = "🔐  НАЧАТЬ РАБОТУ (Войти в систему)";
            btnLaunchLogin.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnLaunchLogin.ForeColor = Color.White;
            btnLaunchLogin.BackColor = Color.FromArgb(33, 150, 243);
            btnLaunchLogin.Location = new Point(100, y);
            btnLaunchLogin.Size = new Size(300, 45);
            btnLaunchLogin.FlatStyle = FlatStyle.Flat;
            btnLaunchLogin.FlatAppearance.BorderSize = 0;
            btnLaunchLogin.Cursor = Cursors.Hand;
            btnLaunchLogin.Click += BtnLaunchLogin_Click;
            this.Controls.Add(btnLaunchLogin);
            y += 60;

            // Кнопка для тестирования
            btnTestData = new Button();
            btnTestData.Text = "📊  ТЕСТ: Посмотреть данные (без входа)";
            btnTestData.Font = new Font("Segoe UI", 9);
            btnTestData.ForeColor = Color.DimGray;
            btnTestData.BackColor = Color.LightGray;
            btnTestData.Location = new Point(100, y);
            btnTestData.Size = new Size(300, 35);
            btnTestData.FlatStyle = FlatStyle.Flat;
            btnTestData.FlatAppearance.BorderSize = 0;
            btnTestData.Cursor = Cursors.Hand;
            btnTestData.Click += BtnTestData_Click;
            this.Controls.Add(btnTestData);
            y += 50;

            // Кнопка для диагностики
            btnDiagnostic = new Button();
            btnDiagnostic.Text = "🔍  ДИАГНОСТИКА DataService";
            btnDiagnostic.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnDiagnostic.ForeColor = Color.White;
            btnDiagnostic.BackColor = Color.Orange;
            btnDiagnostic.Location = new Point(100, y);
            btnDiagnostic.Size = new Size(300, 35);
            btnDiagnostic.FlatStyle = FlatStyle.Flat;
            btnDiagnostic.FlatAppearance.BorderSize = 0;
            btnDiagnostic.Cursor = Cursors.Hand;
            btnDiagnostic.Click += BtnDiagnostic_Click;
            this.Controls.Add(btnDiagnostic);
            y += 50;

            // Кнопка тестового входа (без капчи)
            btnSimpleLogin = new Button();
            btnSimpleLogin.Text = "🧪  Тестовый вход (без капчи)";
            btnSimpleLogin.Font = new Font("Segoe UI", 9);
            btnSimpleLogin.ForeColor = Color.Black;
            btnSimpleLogin.BackColor = Color.LightGreen;
            btnSimpleLogin.Location = new Point(100, y);
            btnSimpleLogin.Size = new Size(300, 35);
            btnSimpleLogin.FlatStyle = FlatStyle.Flat;
            btnSimpleLogin.FlatAppearance.BorderSize = 0;
            btnSimpleLogin.Cursor = Cursors.Hand;
            btnSimpleLogin.Click += BtnSimpleLogin_Click;
            this.Controls.Add(btnSimpleLogin);
            y += 50;

            // НОВАЯ КНОПКА: Создать тестовые данные
            btnForceCreate = new Button();
            btnForceCreate.Text = "🛠️  Создать тестовые данные";
            btnForceCreate.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnForceCreate.ForeColor = Color.White;
            btnForceCreate.BackColor = Color.Red;
            btnForceCreate.Location = new Point(100, y);
            btnForceCreate.Size = new Size(300, 35);
            btnForceCreate.FlatStyle = FlatStyle.Flat;
            btnForceCreate.FlatAppearance.BorderSize = 0;
            btnForceCreate.Cursor = Cursors.Hand;
            btnForceCreate.Click += BtnForceCreate_Click;
            this.Controls.Add(btnForceCreate);
            y += 50;

            // Информационная панель
            lblInfo = new Label();
            lblInfo.Text = "✅ DataService инициализирован\n" +
                          "📁 Данные сохраняются в папку 'Data'\n" +
                          "👤 Тестовые пользователи: admin/admin123, manager/manager123";
            lblInfo.Font = new Font("Consolas", 8);
            lblInfo.ForeColor = Color.DarkGreen;
            lblInfo.Location = new Point(50, y);
            lblInfo.Size = new Size(400, 60);
            lblInfo.TextAlign = ContentAlignment.TopCenter;
            this.Controls.Add(lblInfo);
        }

        private void BtnLaunchLogin_Click(object sender, EventArgs e)
        {
            // Скрываем лаунчер
            this.Hide();

            // Создаем и показываем форму входа
            var loginForm = new LoginForm();
            loginForm.FormClosed += (s, args) =>
            {
                // Когда форма входа закроется, показываем лаунчер снова
                this.Show();
            };

            loginForm.Show();
        }

        private void BtnTestData_Click(object sender, EventArgs e)
        {
            var testForm = new DataTestForm();
            testForm.ShowDialog();
        }

        private void BtnDiagnostic_Click(object sender, EventArgs e)
        {
            try
            {
                var service = DataService.Instance;
                string info = "=== ДИАГНОСТИКА DataService ===\n\n";

                // 1. Проверяем коллекции
                info += $"1. Users == null? {(service.Users == null ? "ДА ❌" : "НЕТ ✅")}\n";
                info += $"2. Users.Count: {service.Users?.Count ?? 0}\n";
                info += $"3. Products.Count: {service.Products?.Count ?? 0}\n";
                info += $"4. Categories.Count: {service.Categories?.Count ?? 0}\n\n";

                // 2. Проверяем папку Data
                string dataPath = "Data";
                info += $"5. Папка Data существует? {System.IO.Directory.Exists(dataPath)}\n";

                if (System.IO.Directory.Exists(dataPath))
                {
                    // ИЩЕМ XML ФАЙЛЫ
                    var xmlFiles = System.IO.Directory.GetFiles(dataPath, "*.xml");
                    info += $"6. XML-файлов в Data/: {xmlFiles.Length}\n";
                    foreach (var file in xmlFiles)
                    {
                        var fileInfo = new System.IO.FileInfo(file);
                        info += $"   • {System.IO.Path.GetFileName(file)} ({fileInfo.Length} байт)\n";
                    }
                }
                else
                {
                    info += $"6. Папка Data НЕ существует!\n";
                }

                // 3. Прямой доступ к Users
                info += "\n7. Прямой доступ к service.Users:\n";
                if (service.Users != null && service.Users.Count > 0)
                {
                    foreach (var user in service.Users)
                    {
                        info += $"   • {user.Id}: {user.Username} / {user.Password} ({user.Role})\n";
                    }

                    var testAuth = service.Authenticate("admin", "admin123");
                    info += $"\n8. Тест аутентификации admin/admin123: ";
                    info += testAuth != null ? "✅ УСПЕХ" : "❌ ОШИБКА";
                }
                else
                {
                    info += "   Коллекция Users ПУСТАЯ!\n";
                }

                MessageBox.Show(info, "Диагностика", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка диагностики:\n{ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSimpleLogin_Click(object sender, EventArgs e)
        {
            var testForm = new Form();
            testForm.Text = "🧪 Тестовый вход (без капчи)";
            testForm.Size = new Size(350, 250);
            testForm.StartPosition = FormStartPosition.CenterScreen;
            testForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            testForm.MaximizeBox = false;
            testForm.BackColor = Color.White;

            var lblTitle = new Label();
            lblTitle.Text = "Тестовая аутентификация";
            lblTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTitle.ForeColor = Color.DarkBlue;
            lblTitle.Location = new Point(80, 20);
            lblTitle.Size = new Size(200, 30);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            testForm.Controls.Add(lblTitle);

            var lblUser = new Label { Text = "Логин:", Location = new Point(50, 70), Size = new Size(80, 25) };
            var lblPass = new Label { Text = "Пароль:", Location = new Point(50, 110), Size = new Size(80, 25) };

            var txtUser = new TextBox
            {
                Location = new Point(130, 70),
                Size = new Size(150, 25),
                Text = "admin",
                BorderStyle = BorderStyle.FixedSingle
            };

            var txtPass = new TextBox
            {
                Location = new Point(130, 110),
                Size = new Size(150, 25),
                Text = "admin123",
                PasswordChar = '*',
                BorderStyle = BorderStyle.FixedSingle
            };

            var btnLogin = new Button
            {
                Text = "ВОЙТИ (тест)",
                Location = new Point(100, 160),
                Size = new Size(150, 35),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            testForm.Controls.Add(lblUser);
            testForm.Controls.Add(lblPass);
            testForm.Controls.Add(txtUser);
            testForm.Controls.Add(txtPass);
            testForm.Controls.Add(btnLogin);

            btnLogin.Click += (sender2, e2) =>
            {
                var service = DataService.Instance;
                string debugInfo = $"Перед аутентификацией:\n";
                debugInfo += $"• Users.Count: {service.Users?.Count ?? 0}\n";

                if (service.Users != null)
                {
                    debugInfo += "Все пользователи в системе:\n";
                    foreach (var user in service.Users)
                    {
                        debugInfo += $"• {user.Username} / {user.Password}\n";
                    }
                }

                var authUser = service.Authenticate(txtUser.Text, txtPass.Text);
                if (authUser != null)
                {
                    debugInfo += $"\n✅ УСПЕХ! Найден пользователь: {authUser.Username}";
                    MessageBox.Show(debugInfo, "Тест - УСПЕХ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    testForm.Close();
                }
                else
                {
                    debugInfo += $"\n❌ ОШИБКА! Пользователь не найден.";
                    MessageBox.Show(debugInfo, "Тест - ОШИБКА", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            testForm.ShowDialog();
        }

        // НОВЫЙ МЕТОД: Обработчик кнопки создания тестовых данных
        private void BtnForceCreate_Click(object sender, EventArgs e)
        {
            try
            {
                var service = DataService.Instance;
                service.ForceCreateTestData();

                string info = "✅ Тестовые данные созданы!\n\n";
                info += $"• Пользователей: {service.Users.Count}\n";
                info += $"• Товаров: {service.Products.Count}\n";
                info += $"• Категорий: {service.Categories.Count}\n\n";

                info += "Созданные пользователи:\n";
                foreach (var user in service.Users)
                {
                    info += $"• {user.Username} / {user.Password} ({user.Role})\n";
                }

                MessageBox.Show(info, "Данные созданы",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                lblInfo.Text = "✅ Тестовые данные созданы!\n" +
                              "👤 admin/admin123\n" +
                              "👤 manager/manager123\n" +
                              "👤 worker/worker123";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}