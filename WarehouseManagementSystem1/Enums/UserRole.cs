using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagementSystem1.Enums
{
    public enum UserRole
    {
        /// <summary>
        /// Администратор - полный доступ
        /// </summary>
        Admin = 1,

        /// <summary>
        /// Менеджер - управление товарами
        /// </summary>
        Manager = 2,

        /// <summary>
        /// Кладовщик - проведение операций
        /// </summary>
        Worker = 3,

        /// <summary>
        /// Наблюдатель - только просмотр
        /// </summary>
        Viewer = 4
    }
}
