using System;
using System.Drawing;
using System.Windows.Forms;
using WarehouseManagementSystem1.Services;

namespace WarehouseManagementSystem1
{
    public partial class Form1 : Form
    {
        private Button btnLaunchLogin;
        private Button btnAbout;
        private Label lblInfo;
        private PictureBox logoPictureBox;

        public Form1()
        {
            InitializeComponent();
            CreateLauncherForm();
        }

        private void CreateLauncherForm()
        {
            // Настройка формы-лаунчера
            this.Text = "🚀 Система складского учета";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Логотип
            logoPictureBox = new PictureBox();
            logoPictureBox.Size = new Size(150, 150);
            logoPictureBox.Location = new Point(175, 30);
            logoPictureBox.BackColor = Color.Transparent;

            // Создаем простой логотип графически
            var logoImage = new Bitmap(150, 150);
            using (var g = Graphics.FromImage(logoImage))
            {
                g.Clear(Color.Transparent);

                // Внешний круг
                g.FillEllipse(new SolidBrush(Color.FromArgb(33, 150, 243)), 10, 10, 130, 130);

                // Внутренний круг
                g.FillEllipse(new SolidBrush(Color.White), 30, 30, 90, 90);

                // Иконка склада
                using (var font = new Font("Arial", 40, FontStyle.Bold))
                {
                    g.DrawString("🏭", font, new SolidBrush(Color.FromArgb(33, 150, 243)), 45, 40);
                }
            }
            logoPictureBox.Image = logoImage;
            this.Controls.Add(logoPictureBox);

            // Заголовок
            var titleLabel = new Label();
            titleLabel.Text = "СИСТЕМА СКЛАДСКОГО УЧЕТА";
            titleLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(33, 150, 243);
            titleLabel.Location = new Point(70, 190);
            titleLabel.Size = new Size(360, 40);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(titleLabel);

            // Версия
            var versionLabel = new Label();
            versionLabel.Text = "Версия 2.0.0";
            versionLabel.Font = new Font("Segoe UI", 10);
            versionLabel.ForeColor = Color.Gray;
            versionLabel.Location = new Point(200, 230);
            versionLabel.Size = new Size(100, 25);
            versionLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(versionLabel);

            int y = 270;

            // Основная кнопка - запуск формы входа
            btnLaunchLogin = new Button();
            btnLaunchLogin.Text = "🔐  ВОЙТИ В СИСТЕМУ";
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

            // Кнопка "О программе"
            btnAbout = new Button();
            btnAbout.Text = "ℹ  О ПРОГРАММЕ";
            btnAbout.Font = new Font("Segoe UI", 9);
            btnAbout.ForeColor = Color.DimGray;
            btnAbout.BackColor = Color.LightGray;
            btnAbout.Location = new Point(100, y);
            btnAbout.Size = new Size(300, 35);
            btnAbout.FlatStyle = FlatStyle.Flat;
            btnAbout.FlatAppearance.BorderSize = 0;
            btnAbout.Cursor = Cursors.Hand;
            btnAbout.Click += BtnAbout_Click;
            this.Controls.Add(btnAbout);

            // Информационная панель (внизу)
            lblInfo = new Label();
            lblInfo.Text = "© 2024 Система складского учета. Все права защищены.";
            lblInfo.Font = new Font("Consolas", 8);
            lblInfo.ForeColor = Color.DarkGray;
            lblInfo.Location = new Point(50, 370);
            lblInfo.Size = new Size(400, 20);
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

        private void BtnAbout_Click(object sender, EventArgs e)
        {
            var aboutText = "🏭 Система складского учета\n\n" +
                           "Версия: 2.0.0\n" +
                           "Разработчик: Команда разработки\n\n" +
                           "Функции системы:\n" +
                           "• Управление товарами и складом\n" +
                           "• Проведение операций (приход/расход/перемещение)\n" +
                           "• Отчетность и аналитика\n" +
                           "• Управление пользователями\n\n" +
                           "© 2024 Все права защищены.";

            MessageBox.Show(aboutText, "О программе",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}