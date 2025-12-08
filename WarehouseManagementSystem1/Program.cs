using System;
using System.Windows.Forms;

namespace WarehouseManagementSystem1
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Запускаем с лаунчера
            Application.Run(new Form1());
        }
    }
}