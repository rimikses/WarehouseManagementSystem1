using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace WarehouseManagementSystem1.Services
{
    public static class FileService
    {
        private static string _dataPath = "Data";

        public static void EnsureDataDirectory()
        {
            if (!Directory.Exists(_dataPath))
            {
                Directory.CreateDirectory(_dataPath);
                Console.WriteLine($"✅ Создана папка: {Path.GetFullPath(_dataPath)}");
            }
        }

        public static void SaveToFile<T>(string fileName, List<T> data)
        {
            try
            {
                EnsureDataDirectory();
                string filePath = Path.Combine(_dataPath, fileName);
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(filePath, json);
                Console.WriteLine($"✅ Сохранён файл: {fileName} ({data.Count} записей)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка сохранения {fileName}: {ex.Message}");
            }
        }

        public static List<T> LoadFromFile<T>(string fileName)
        {
            try
            {
                string filePath = Path.Combine(_dataPath, fileName);

                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"⚠ Файл не найден: {fileName}");
                    return null;
                }

                string json = File.ReadAllText(filePath);
                var data = JsonConvert.DeserializeObject<List<T>>(json);
                Console.WriteLine($"✅ Загружен файл: {fileName} ({data?.Count ?? 0} записей)");
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка загрузки {fileName}: {ex.Message}");
                return null;
            }
        }

        public static bool FileExists(string fileName)
        {
            string filePath = Path.Combine(_dataPath, fileName);
            return File.Exists(filePath);
        }
    }
}