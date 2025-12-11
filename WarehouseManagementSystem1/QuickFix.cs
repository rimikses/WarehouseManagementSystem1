using System.Windows.Forms;
using WarehouseManagementSystem1.Models;
using WarehouseManagementSystem1.Services;

namespace WarehouseManagementSystem1
{
    public class QuickFix
    {
        // Если нужно быстро запустить систему, используйте этот метод
        public static void RunTest()
        {
            // 1. Создаем тестовые данные
            var service = DataService.Instance;
            service.ForceCreateTestData();

            // 2. Проверяем аутентификацию
            var user = service.Authenticate("admin", "admin123");

            if (user != null)
            {
                MessageBox.Show($"✅ Аутентификация успешна!\nПользователь: {user.Username}\nРоль: {user.Role}");

                // 3. Запускаем главную форму
                Application.Run(new MainForm(user));
            }
            else
            {
                MessageBox.Show("❌ Ошибка аутентификации");
            }
        }
    }
}