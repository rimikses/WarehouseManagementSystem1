using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using WarehouseManagementSystem1.Models;
using WarehouseManagementSystem1.Services;

namespace WarehouseManagementSystem1
{
    public class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtCaptcha;
        private Button btnLogin;
        private Button btnCancel;
        private PictureBox captchaPictureBox;
        private Button btnRefreshCaptcha;
        private Label lblTitle;
        private Panel panelContainer;

        private string captchaCode;
        private Random random;
        public User AuthenticatedUser { get; private set; }

        public LoginForm()
        {
            random = new Random();
            captchaCode = ""; // Инициализируем пустую капчу
            CreateBeautifulLoginForm();
            GenerateCaptcha();

            this.Load += (s, e) => txtUsername.Focus();
        }

        private void CreateBeautifulLoginForm()
        {
            // УВЕЛИЧИЛИ РАЗМЕР ОКНА
            this.Text = "🔐 Вход в систему";
            this.Size = new Size(500, 600); // Было 450x550
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(33, 33, 33);
            this.Paint += Form_Paint;

            // Контейнер для содержимого
            panelContainer = new Panel();
            panelContainer.Size = new Size(450, 520); // Увеличили
            panelContainer.Location = new Point(25, 40); // Подвинули вниз для тени
            panelContainer.BackColor = Color.White;
            panelContainer.Paint += PanelContainer_Paint;
            this.Controls.Add(panelContainer);

            // Заголовок
            lblTitle = new Label();
            lblTitle.Text = "ВХОД В СИСТЕМУ";
            lblTitle.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(33, 150, 243);
            lblTitle.Location = new Point(0, 40);
            lblTitle.Size = new Size(450, 40);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            panelContainer.Controls.Add(lblTitle);

            // Подзаголовок
            var lblSubtitle = new Label();
            lblSubtitle.Text = "Система складского учета";
            lblSubtitle.Font = new Font("Segoe UI", 12);
            lblSubtitle.ForeColor = Color.FromArgb(100, 100, 100);
            lblSubtitle.Location = new Point(0, 85);
            lblSubtitle.Size = new Size(450, 30);
            lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
            panelContainer.Controls.Add(lblSubtitle);

            int y = 140;

            // Поле: Логин
            var lblUser = CreateLabel("Логин:", y);
            panelContainer.Controls.Add(lblUser);
            y += 30;

            txtUsername = CreateTextBox(y, false, "Введите логин");
            panelContainer.Controls.Add(txtUsername);
            y += 55;

            // Поле: Пароль
            var lblPass = CreateLabel("Пароль:", y);
            panelContainer.Controls.Add(lblPass);
            y += 30;

            txtPassword = CreateTextBox(y, true, "Введите пароль");
            panelContainer.Controls.Add(txtPassword);
            y += 55;

            // Капча заголовок
            var lblCaptcha = CreateLabel("Введите код с картинки:", y);
            panelContainer.Controls.Add(lblCaptcha);
            y += 30;

            // Панель для капчи - УВЕЛИЧИЛИ
            var captchaPanel = new Panel();
            captchaPanel.Location = new Point(75, y); // Сдвинули для центрирования
            captchaPanel.Size = new Size(300, 100); // Увеличили высоту
            captchaPanel.BackColor = Color.Transparent;
            panelContainer.Controls.Add(captchaPanel);

            // Картинка капчи - УВЕЛИЧИЛИ
            captchaPictureBox = new PictureBox();
            captchaPictureBox.Location = new Point(0, 0);
            captchaPictureBox.Size = new Size(200, 100); // Увеличили высоту
            captchaPictureBox.BackColor = Color.White;
            captchaPictureBox.BorderStyle = BorderStyle.FixedSingle;
            captchaPictureBox.Paint += CaptchaPictureBox_Paint;
            captchaPictureBox.Click += (s, e) => { GenerateCaptcha(); txtCaptcha.Focus(); };
            captchaPictureBox.Cursor = Cursors.Hand;
            captchaPanel.Controls.Add(captchaPictureBox);

            // Кнопка обновления капчи
            btnRefreshCaptcha = new Button();
            btnRefreshCaptcha.Text = "🔄 Новый код";
            btnRefreshCaptcha.Font = new Font("Segoe UI", 10);
            btnRefreshCaptcha.Location = new Point(210, 30);
            btnRefreshCaptcha.Size = new Size(80, 40);
            btnRefreshCaptcha.FlatStyle = FlatStyle.Flat;
            btnRefreshCaptcha.FlatAppearance.BorderSize = 1;
            btnRefreshCaptcha.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            btnRefreshCaptcha.BackColor = Color.White;
            btnRefreshCaptcha.Cursor = Cursors.Hand;
            btnRefreshCaptcha.Click += (s, e) => { GenerateCaptcha(); txtCaptcha.Focus(); };
            captchaPanel.Controls.Add(btnRefreshCaptcha);

            y += 110; // Увеличили отступ из-за увеличенной капчи

            // Поле для ввода капчи - УВЕЛИЧИЛИ
            txtCaptcha = new TextBox();
            txtCaptcha.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            txtCaptcha.Location = new Point(75, y); // Сдвинули для центрирования
            txtCaptcha.Size = new Size(300, 45); // Увеличили
            txtCaptcha.BorderStyle = BorderStyle.FixedSingle;
            txtCaptcha.BackColor = Color.FromArgb(250, 250, 250);
            txtCaptcha.TextAlign = HorizontalAlignment.Center;
            txtCaptcha.MaxLength = 6;
            panelContainer.Controls.Add(txtCaptcha);
            y += 60;

            // Кнопка ВОЙТИ
            btnLogin = new Button();
            btnLogin.Text = "🔐 ВОЙТИ В СИСТЕМУ";
            btnLogin.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnLogin.ForeColor = Color.White;
            btnLogin.BackColor = Color.FromArgb(33, 150, 243);
            btnLogin.Location = new Point(75, y); // Сдвинули для центрирования
            btnLogin.Size = new Size(300, 45);
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.Click += BtnLogin_Click;

            // Эффекты при наведении
            btnLogin.MouseEnter += (s, e) => btnLogin.BackColor = Color.FromArgb(13, 130, 223);
            btnLogin.MouseLeave += (s, e) => btnLogin.BackColor = Color.FromArgb(33, 150, 243);

            panelContainer.Controls.Add(btnLogin);
            y += 60;

            // Кнопка ОТМЕНА
            btnCancel = new Button();
            btnCancel.Text = "❌ ЗАКРЫТЬ";
            btnCancel.Font = new Font("Segoe UI", 10);
            btnCancel.ForeColor = Color.FromArgb(100, 100, 100);
            btnCancel.BackColor = Color.FromArgb(240, 240, 240);
            btnCancel.Location = new Point(75, y); // Сдвинули для центрирования
            btnCancel.Size = new Size(300, 35);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 1;
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Click += (s, e) => this.Close();

            // Эффекты при наведении
            btnCancel.MouseEnter += (s, e) => btnCancel.BackColor = Color.FromArgb(230, 230, 230);
            btnCancel.MouseLeave += (s, e) => btnCancel.BackColor = Color.FromArgb(240, 240, 240);

            panelContainer.Controls.Add(btnCancel);

            // Информация внизу - ОБНОВИЛИ
            var lblInfo = new Label();
            lblInfo.Text = "Тестовые данные: admin / admin123 | manager / manager123 | worker / worker123";
            lblInfo.Font = new Font("Segoe UI", 8);
            lblInfo.ForeColor = Color.FromArgb(150, 150, 150);
            lblInfo.Location = new Point(0, 480);
            lblInfo.Size = new Size(450, 40);
            lblInfo.TextAlign = ContentAlignment.MiddleCenter;
            panelContainer.Controls.Add(lblInfo);

            // Декоративный элемент внизу
            var decorPanel = new Panel();
            decorPanel.BackColor = Color.FromArgb(33, 150, 243);
            decorPanel.Location = new Point(0, 495);
            decorPanel.Size = new Size(450, 5);
            panelContainer.Controls.Add(decorPanel);

            // Кнопка закрытия (крестик)
            var btnClose = new Button();
            btnClose.Text = "✕";
            btnClose.Font = new Font("Segoe UI", 14);
            btnClose.ForeColor = Color.White;
            btnClose.BackColor = Color.Transparent;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 50, 50);
            btnClose.Location = new Point(455, 10);
            btnClose.Size = new Size(35, 35);
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);

            this.AcceptButton = btnLogin;
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            // Улучшенная тень формы
            using (var path = new GraphicsPath())
            {
                int radius = 20;
                Rectangle formRect = new Rectangle(0, 0, this.Width - 1, this.Height - 1); // ИЗМЕНИЛИ ИМЯ ПЕРЕМЕННОЙ

                path.AddArc(formRect.X, formRect.Y, radius, radius, 180, 90);
                path.AddArc(formRect.X + formRect.Width - radius, formRect.Y, radius, radius, 270, 90);
                path.AddArc(formRect.X + formRect.Width - radius, formRect.Y + formRect.Height - radius, radius, radius, 0, 90);
                path.AddArc(formRect.X, formRect.Y + formRect.Height - radius, radius, radius, 90, 90);
                path.CloseFigure();

                using (var pen = new Pen(Color.FromArgb(0, 0, 0, 30), 10))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }

            // Градиентный фон формы
            var gradientRect = new Rectangle(0, 0, this.Width, this.Height); // ИЗМЕНИЛИ ИМЯ ПЕРЕМЕННОЙ
            using (var brush = new LinearGradientBrush(gradientRect,
                Color.FromArgb(33, 33, 33),
                Color.FromArgb(60, 60, 60),
                45f)) // Наклонный градиент
            {
                e.Graphics.FillRectangle(brush, gradientRect);
            }
        }

        private void PanelContainer_Paint(object sender, PaintEventArgs e)
        {
            // Скругленные углы панели с улучшениями
            using (var path = new GraphicsPath())
            {
                int radius = 25; // Увеличили радиус
                Rectangle panelRect = new Rectangle(0, 0, panelContainer.Width, panelContainer.Height); // ИЗМЕНИЛИ ИМЯ ПЕРЕМЕННОЙ

                path.AddArc(panelRect.X, panelRect.Y, radius, radius, 180, 90);
                path.AddArc(panelRect.X + panelRect.Width - radius, panelRect.Y, radius, radius, 270, 90);
                path.AddArc(panelRect.X + panelRect.Width - radius, panelRect.Y + panelRect.Height - radius, radius, radius, 0, 90);
                path.AddArc(panelRect.X, panelRect.Y + panelRect.Height - radius, radius, radius, 90, 90);
                path.CloseFigure();

                panelContainer.Region = new Region(path);
            }

            // Тень панели с градиентом
            using (var pen = new Pen(Color.FromArgb(220, 220, 220), 2))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, panelContainer.Width - 1, panelContainer.Height - 1);
            }

            // Декоративная линия сверху
            using (var pen = new Pen(Color.FromArgb(33, 150, 243), 3))
            {
                e.Graphics.DrawLine(pen, 0, 0, panelContainer.Width, 0);
            }
        }

        private void CaptchaPictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (string.IsNullOrEmpty(captchaCode))
            {
                // Если капча пустая, генерируем новую
                GenerateCaptcha();
                return;
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            // Фон с текстурой
            var captchaRect = new Rectangle(0, 0, captchaPictureBox.Width, captchaPictureBox.Height); // ИЗМЕНИЛИ ИМЯ ПЕРЕМЕННОЙ

            // Градиентный фон
            using (var brush = new LinearGradientBrush(captchaRect,
                Color.FromArgb(245, 245, 245),
                Color.FromArgb(225, 225, 225),
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, captchaRect);
            }

            // Шумный фон (точки)
            for (int i = 0; i < 150; i++)
            {
                int x = random.Next(captchaRect.Width);
                int y = random.Next(captchaRect.Height);
                e.Graphics.FillRectangle(Brushes.LightGray, x, y, 1, 1);
            }

            // Текст капчи с эффектами (УЛУЧШЕННЫЙ)
            using (var font = new Font("Arial", 28, FontStyle.Bold | FontStyle.Italic))
            {
                // Смещение для каждой буквы (для затруднения OCR)
                float totalWidth = 0;
                for (int i = 0; i < captchaCode.Length; i++)
                {
                    // Измеряем ширину символа
                    SizeF charSize = e.Graphics.MeasureString(captchaCode[i].ToString(), font);

                    // Случайное смещение по вертикали
                    float yOffset = random.Next(-10, 11);
                    float x = 15 + totalWidth;
                    float y = 50 + yOffset; // Центрируем по вертикали

                    // Случайный цвет для каждой буквы
                    Color charColor = Color.FromArgb(
                        random.Next(50, 150),
                        random.Next(50, 150),
                        random.Next(50, 150)
                    );

                    // Несколько слоев для объема
                    // Тень 1
                    e.Graphics.DrawString(captchaCode[i].ToString(), font,
                        Brushes.DarkGray, x + 2, y + 2);

                    // Тень 2
                    e.Graphics.DrawString(captchaCode[i].ToString(), font,
                        Brushes.Gray, x + 1, y + 1);

                    // Основной текст
                    using (var brush = new SolidBrush(charColor))
                    {
                        e.Graphics.DrawString(captchaCode[i].ToString(), font, brush, x, y);
                    }

                    // Случайный поворот для некоторых символов
                    if (random.Next(0, 3) == 0) // 33% шанс
                    {
                        e.Graphics.TranslateTransform(x + charSize.Width / 2, y + charSize.Height / 2);
                        e.Graphics.RotateTransform(random.Next(-15, 16));
                        e.Graphics.DrawString(captchaCode[i].ToString(), font,
                            Brushes.Black, -charSize.Width / 2, -charSize.Height / 2);
                        e.Graphics.ResetTransform();
                    }

                    totalWidth += charSize.Width - 5; // Немного сжимаем интервалы
                }
            }

            // Линии-помехи (больше и разнообразнее)
            for (int i = 0; i < 8; i++) // Было 3
            {
                // Разные цвета и толщины линий
                int lineWidth = random.Next(1, 3);
                Color lineColor = Color.FromArgb(
                    random.Next(100, 180),
                    random.Next(100, 180),
                    random.Next(100, 180)
                );

                using (var pen = new Pen(lineColor, lineWidth))
                {
                    // Волнистые линии
                    int startX = random.Next(10, captchaRect.Width - 10);
                    int startY = random.Next(10, captchaRect.Height - 10);
                    int endX = random.Next(10, captchaRect.Width - 10);
                    int endY = random.Next(10, captchaRect.Height - 10);

                    // Рисуем ломаную линию
                    Point[] points = new Point[4];
                    points[0] = new Point(startX, startY);
                    points[1] = new Point((startX + endX) / 2 + random.Next(-20, 21),
                                         (startY + endY) / 2 + random.Next(-20, 21));
                    points[2] = new Point((startX + endX) / 2 + random.Next(-20, 21),
                                         (startY + endY) / 2 + random.Next(-20, 21));
                    points[3] = new Point(endX, endY);

                    e.Graphics.DrawCurve(pen, points, 0.5f);
                }
            }

            // Эллипсы-помехи
            for (int i = 0; i < 5; i++)
            {
                Color ellipseColor = Color.FromArgb(
                    50, // Полупрозрачные
                    random.Next(100, 200),
                    random.Next(100, 200),
                    random.Next(100, 200)
                );

                using (var brush = new SolidBrush(ellipseColor))
                {
                    int size = random.Next(20, 60);
                    e.Graphics.FillEllipse(brush,
                        random.Next(0, captchaRect.Width - size),
                        random.Next(0, captchaRect.Height - size),
                        size, size);
                }
            }

            // Рамка
            using (var pen = new Pen(Color.FromArgb(200, 200, 200), 2))
            {
                e.Graphics.DrawRectangle(pen, 1, 1, captchaRect.Width - 3, captchaRect.Height - 3);
            }

            // Инструкция на самой картинке
            using (var font = new Font("Arial", 8))
            {
                e.Graphics.DrawString("Нажмите для обновления", font,
                    Brushes.DimGray, 5, 5);
            }
        }

        private Label CreateLabel(string text, int y)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(66, 66, 66),
                Location = new Point(75, y), // Сдвинули для центрирования
                Size = new Size(300, 25)
            };
        }

        private TextBox CreateTextBox(int y, bool isPassword, string placeholder)
        {
            var textBox = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(75, y), // Сдвинули для центрирования
                Size = new Size(300, 40),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 250, 250),
                Text = placeholder,
                ForeColor = Color.Gray
            };

            if (isPassword)
                textBox.PasswordChar = '\0'; // Сначала без маски для placeholder

            // Эффекты при фокусе
            textBox.Enter += (s, e) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                    if (isPassword) textBox.PasswordChar = '•';
                }
                textBox.BackColor = Color.FromArgb(245, 245, 245);
                textBox.BorderStyle = BorderStyle.FixedSingle;
            };

            textBox.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Color.Gray;
                    if (isPassword) textBox.PasswordChar = '\0';
                }
                textBox.BackColor = Color.FromArgb(250, 250, 250);
                textBox.BorderStyle = BorderStyle.FixedSingle;
            };

            return textBox;
        }

        private void GenerateCaptcha()
        {
            // УСЛОЖНЕННАЯ КАПЧА: буквы и цифры, но без сложных символов
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
            captchaCode = "";
            for (int i = 0; i < 6; i++) // 6 символов
            {
                captchaCode += chars[random.Next(chars.Length)];
            }

            // Для отладки выводим в консоль
            Console.WriteLine($"Сгенерирована капча: {captchaCode}");

            // Обновляем отображение
            if (captchaPictureBox != null)
            {
                captchaPictureBox.Invalidate();
            }

            if (txtCaptcha != null)
            {
                txtCaptcha.Clear();
                txtCaptcha.Focus();
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            // 1. Проверка полей
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            // Проверяем, не является ли текст placeholder
            if (username == "Введите логин" || string.IsNullOrWhiteSpace(username) ||
                password == "Введите пароль" || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(txtCaptcha.Text))
            {
                ShowError("Заполните все поля!", txtUsername);
                return;
            }

            // 2. Проверка капчи (БЕЗ УЧЕТА РЕГИСТРА)
            string enteredCaptcha = txtCaptcha.Text.Trim();
            string generatedCaptcha = captchaCode;

            Console.WriteLine($"Введено: {enteredCaptcha}, Ожидается: {generatedCaptcha}");

            // Сравниваем без учета регистра
            if (!string.Equals(enteredCaptcha, generatedCaptcha, StringComparison.OrdinalIgnoreCase))
            {
                ShowError($"Неверный код безопасности!\nВведите символы с картинки", txtCaptcha);
                GenerateCaptcha();
                return;
            }

            // 3. Аутентификация
            try
            {
                // Показываем индикатор загрузки
                btnLogin.Text = "ПРОВЕРКА...";
                btnLogin.Enabled = false;
                btnLogin.BackColor = Color.FromArgb(255, 193, 7); // Желтый для индикации
                Application.DoEvents();

                var service = DataService.Instance;
                AuthenticatedUser = service.Authenticate(username, password);

                if (AuthenticatedUser != null)
                {
                    // Анимация успеха
                    btnLogin.BackColor = Color.FromArgb(76, 175, 80);
                    btnLogin.Text = "✓ ДОСТУП РАЗРЕШЕН";
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(500); // Увеличили задержку для эффекта

                    this.Hide();
                    var mainForm = new MainForm(AuthenticatedUser);
                    mainForm.FormClosed += (s, args) => this.Close();
                    mainForm.Show();
                }
                else
                {
                    // Ошибка
                    btnLogin.Enabled = true;
                    btnLogin.Text = "🔐 ВОЙТИ В СИСТЕМУ";
                    btnLogin.BackColor = Color.FromArgb(33, 150, 243);
                    ShowError("Неверный логин или пароль!\nПопробуйте снова.", txtUsername);
                    GenerateCaptcha();
                    txtPassword.Clear();
                    txtCaptcha.Clear();
                    txtUsername.Focus();
                }
            }
            catch (Exception ex)
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "🔐 ВОЙТИ В СИСТЕМУ";
                btnLogin.BackColor = Color.FromArgb(33, 150, 243);
                ShowError($"Ошибка системы: {ex.Message}\nОбратитесь к администратору.", txtUsername);
            }
        }

        private void ShowError(string message, Control focusControl)
        {
            // Визуальный эффект ошибки с улучшениями
            // Проверяем, является ли контрол TextBox для доступа к BorderStyle
            if (focusControl is TextBox textBoxControl)
            {
                var originalColor = textBoxControl.BackColor;
                var originalBorder = textBoxControl.BorderStyle;

                textBoxControl.BackColor = Color.FromArgb(255, 240, 240);
                textBoxControl.BorderStyle = BorderStyle.FixedSingle;

                var timer = new Timer();
                timer.Interval = 100;
                int blinkCount = 0;
                timer.Tick += (s, e) =>
                {
                    blinkCount++;
                    textBoxControl.BackColor = (blinkCount % 2 == 0)
                        ? Color.FromArgb(255, 240, 240)
                        : Color.FromArgb(255, 220, 220);

                    if (blinkCount > 6) // Увеличили количество миганий
                    {
                        timer.Stop();
                        textBoxControl.BackColor = originalColor;
                        textBoxControl.BorderStyle = originalBorder;
                        timer.Dispose();
                    }
                };
                timer.Start();

                MessageBox.Show(message, "Ошибка входа",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                focusControl.Focus();
                textBoxControl.SelectAll();
            }
            else
            {
                // Для других контролов просто показываем сообщение
                MessageBox.Show(message, "Ошибка входа",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                focusControl.Focus();
            }
        }
    }
}