using System;
using System.Drawing;
using System.Linq; // ← ДОБАВЬТЕ ЭТУ СТРОКУ!
using System.Windows.Forms;
using WarehouseManagementSystem1.Models;
using WarehouseManagementSystem1.Services;

namespace WarehouseManagementSystem1
{
    public partial class LoginForm : Form
    {
        // Элементы управления
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Label lblCaptcha;
        private TextBox txtCaptcha;
        private Button btnLogin;
        private Button btnCancel;
        private Button btnRefreshCaptcha;

        // Данные формы
        private string captchaCode;
        private Random random;
        public User AuthenticatedUser { get; private set; }

        public LoginForm()
        {
            random = new Random();
            CreateLoginForm();
            GenerateAndDisplayCaptcha();
        }

        private void CreateLoginForm()
        {
            // Основные настройки формы
            this.Text = "🔐 Вход в систему складского учета";
            this.Size = new Size(450, 380);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9);

            // Заголовок
            var lblTitle = new Label();
            lblTitle.Text = "АВТОРИЗАЦИЯ ПОЛЬЗОВАТЕЛЯ";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(33, 150, 243);
            lblTitle.Location = new Point(80, 20);
            lblTitle.Size = new Size(290, 30);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(lblTitle);

            // Разделительная линия
            var separator = new Label();
            separator.BorderStyle = BorderStyle.Fixed3D;
            separator.Location = new Point(25, 60);
            separator.Size = new Size(400, 2);
            this.Controls.Add(separator);

            int y = 80; // Начальная позиция по вертикали

            // Поле: Логин
            var lblUser = new Label();
            lblUser.Text = "Логин:";
            lblUser.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblUser.Location = new Point(50, y);
            lblUser.Size = new Size(100, 25);
            this.Controls.Add(lblUser);

            txtUsername = new TextBox();
            txtUsername.Font = new Font("Segoe UI", 10);
            txtUsername.Location = new Point(150, y);
            txtUsername.Size = new Size(220, 25);
            txtUsername.BorderStyle = BorderStyle.FixedSingle;
            txtUsername.Text = "admin"; // Для теста
            this.Controls.Add(txtUsername);
            y += 40;

            // Поле: Пароль
            var lblPass = new Label();
            lblPass.Text = "Пароль:";
            lblPass.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblPass.Location = new Point(50, y);
            lblPass.Size = new Size(100, 25);
            this.Controls.Add(lblPass);

            txtPassword = new TextBox();
            txtPassword.Font = new Font("Segoe UI", 10);
            txtPassword.Location = new Point(150, y);
            txtPassword.Size = new Size(220, 25);
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.PasswordChar = '•';
            txtPassword.Text = "admin123"; // Для теста
            this.Controls.Add(txtPassword);
            y += 45;

            // Заголовок капчи
            var lblCaptchaTitle = new Label();
            lblCaptchaTitle.Text = "Защита от роботов:";
            lblCaptchaTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblCaptchaTitle.Location = new Point(50, y);
            lblCaptchaTitle.Size = new Size(140, 25);
            this.Controls.Add(lblCaptchaTitle);

            // Панель для капчи
            var captchaPanel = new Panel();
            captchaPanel.Location = new Point(150, y - 5);
            captchaPanel.Size = new Size(220, 60);
            captchaPanel.BorderStyle = BorderStyle.FixedSingle;
            captchaPanel.BackColor = Color.FromArgb(250, 250, 250);
            this.Controls.Add(captchaPanel);

            // Поле с капчей (внутри панели)
            lblCaptcha = new Label();
            lblCaptcha.Location = new Point(10, 15);
            lblCaptcha.Size = new Size(120, 30);
            lblCaptcha.Font = new Font("Arial", 16, FontStyle.Bold);
            lblCaptcha.TextAlign = ContentAlignment.MiddleCenter;
            captchaPanel.Controls.Add(lblCaptcha);

            // Кнопка обновления капчи
            btnRefreshCaptcha = new Button();
            btnRefreshCaptcha.Text = "🔄 Обновить";
            btnRefreshCaptcha.Font = new Font("Segoe UI", 9);
            btnRefreshCaptcha.Location = new Point(140, 15);
            btnRefreshCaptcha.Size = new Size(70, 30);
            btnRefreshCaptcha.FlatStyle = FlatStyle.Flat;
            btnRefreshCaptcha.FlatAppearance.BorderSize = 1;
            btnRefreshCaptcha.FlatAppearance.BorderColor = Color.Gray;
            btnRefreshCaptcha.BackColor = Color.White;
            btnRefreshCaptcha.Click += (s, e) =>
            {
                GenerateAndDisplayCaptcha();
                txtCaptcha.Clear();
                txtCaptcha.Focus();
            };
            captchaPanel.Controls.Add(btnRefreshCaptcha);
            y += 70;

            // Поле для ввода капчи
            var lblCaptchaInput = new Label();
            lblCaptchaInput.Text = "Введите код:";
            lblCaptchaInput.Font = new Font("Segoe UI", 10);
            lblCaptchaInput.Location = new Point(50, y);
            lblCaptchaInput.Size = new Size(100, 25);
            this.Controls.Add(lblCaptchaInput);

            txtCaptcha = new TextBox();
            txtCaptcha.Font = new Font("Segoe UI", 10);
            txtCaptcha.Location = new Point(150, y);
            txtCaptcha.Size = new Size(100, 25);
            txtCaptcha.BorderStyle = BorderStyle.FixedSingle;
            txtCaptcha.TextAlign = HorizontalAlignment.Center;
            this.Controls.Add(txtCaptcha);
            y += 40;

            // Панель для кнопок
            var buttonPanel = new Panel();
            buttonPanel.Location = new Point(25, y);
            buttonPanel.Size = new Size(400, 50);
            this.Controls.Add(buttonPanel);

            // Кнопка ВОЙТИ
            btnLogin = new Button();
            btnLogin.Text = "ВОЙТИ В СИСТЕМУ";
            btnLogin.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnLogin.ForeColor = Color.White;
            btnLogin.BackColor = Color.FromArgb(33, 150, 243);
            btnLogin.Location = new Point(50, 10);
            btnLogin.Size = new Size(150, 35);
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.Click += BtnLogin_Click;
            buttonPanel.Controls.Add(btnLogin);

            // Кнопка ОТМЕНА
            btnCancel = new Button();
            btnCancel.Text = "ОТМЕНА";
            btnCancel.Font = new Font("Segoe UI", 10);
            btnCancel.ForeColor = Color.Black;
            btnCancel.BackColor = Color.LightGray;
            btnCancel.Location = new Point(220, 10);
            btnCancel.Size = new Size(150, 35);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.DialogResult = DialogResult.Cancel;
            buttonPanel.Controls.Add(btnCancel);

            // Назначение клавиш по умолчанию
            this.AcceptButton = btnLogin;
            this.CancelButton = btnCancel;
        }

        // Генерация и отображение капчи
        private void GenerateAndDisplayCaptcha()
        {
            // Генерируем случайный 4-значный код (только цифры)
            captchaCode = random.Next(1000, 9999).ToString();

            // Отображаем капчу
            if (lblCaptcha != null)
            {
                lblCaptcha.Text = captchaCode;
                lblCaptcha.ForeColor = Color.DarkBlue;
                lblCaptcha.BackColor = Color.LightGray;
                lblCaptcha.Font = new Font("Arial", 18, FontStyle.Bold);
            }
        }

        // Обработчик нажатия кнопки "Войти" - УПРОЩЕННАЯ ВЕРСИЯ
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            // 1. Проверка заполнения полей
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtCaptcha.Text))
            {
                MessageBox.Show("Заполните все поля!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Проверка капчи
            string enteredCaptcha = txtCaptcha.Text.Trim();
            string correctCaptcha = captchaCode;

            if (enteredCaptcha != correctCaptcha)
            {
                MessageBox.Show("Неверный код с картинки!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                GenerateAndDisplayCaptcha();
                txtCaptcha.Clear();
                txtCaptcha.Focus();
                return;
            }

            // 3. Аутентификация
            try
            {
                var service = DataService.Instance;
                AuthenticatedUser = service.Authenticate(txtUsername.Text, txtPassword.Text);

                if (AuthenticatedUser != null)
                {
                    // Успешный вход
                    Console.WriteLine($"✅ Найден: {AuthenticatedUser.Username}");

                    // ВАЖНО: Сначала создаем MainForm, потом закрываем LoginForm
                    this.Hide(); // Скрываем форму входа

                    var mainForm = new MainForm(AuthenticatedUser);
                    mainForm.FormClosed += (s, args) =>
                    {
                        // Когда MainForm закроется, закрываем LoginForm
                        this.Close();
                    };

                    mainForm.Show();
                }
                else
                {
                    // Неверные данные
                    Console.WriteLine($"❌ Пользователь не найден");

                    MessageBox.Show("Неверный логин или пароль!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    GenerateAndDisplayCaptcha();
                    txtPassword.Clear();
                    txtCaptcha.Clear();
                    txtUsername.Focus();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка: {ex.Message}");
                MessageBox.Show($"Ошибка системы: {ex.Message}", "Ошибка");
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            txtUsername.SelectAll();
            txtUsername.Focus();
        }
    }
}