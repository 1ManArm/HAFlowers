using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Serialization;

namespace HAFlowers
{
    internal class Program
    {
        public class Flower
        {
            public string Name { get; set; }
            public double Height { get; set; }
            public int HealthLevel { get; set; }

            public void Grow(double growthAmount)
            {
                Height += growthAmount;
                Console.WriteLine($"{Name} has grown. New height: {Height}");
            }

            public void ChangeHealth(int healthChange)
            {
                HealthLevel += healthChange;
                Console.WriteLine($"{Name} health level has changed. New health level: {HealthLevel}");
            }
        }
        interface IFlowerInfo
        {
            void DisplayInfo();
        }

        public class Garden : IFlowerInfo
        {
            public List<Flower> Flowers { get; set; } = new List<Flower>();
            public event Action<Flower> FlowerGrowthEvent;

            public void AddFlower(Flower flower)
            {
                Flowers.Add(flower);
                flower.Grow(0);
            }

            public void WaterFlower(Flower flower)
            {
                flower.Grow(0.5);
            }

            public void FertilizeFlower(Flower flower)
            {
                flower.ChangeHealth(1);
            }

            public void DisplayInfo()
            {
                foreach (var flower in Flowers)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Name: " + flower.Name);
                    Console.WriteLine($"Height: " + flower.Height);
                    Console.WriteLine($"Health Level: " + flower.HealthLevel);
                    Console.WriteLine();
                }
            }

            public IEnumerable<Flower> GetFlowersWithHeightGreaterThan(double height)
            {
                return Flowers.Where(f => f.Height > height);
            }

            public IEnumerable<Flower> GetFlowersSortedByHealthLevel()
            {
                return Flowers.OrderByDescending(f => f.HealthLevel);
            }

            public void SaveFlowersToJson(string filePath)
            {
                string jsonString = JsonSerializer.Serialize(Flowers);
                File.WriteAllText(filePath, jsonString);
            }

            public void LoadFlowersFromJson(string filePath)
            {
                string jsonString = File.ReadAllText(filePath);
                Flowers = JsonSerializer.Deserialize<List<Flower>>(jsonString);
            }

            public void SaveFlowersToXml(string filePath)
            {
                var serializer = new XmlSerializer(typeof(List<Flower>));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    serializer.Serialize(stream, Flowers);
                }
            }
            public void LoadFlowersFromXml(string filePath)
            {
                var serializer = new XmlSerializer(typeof(List<Flower>));
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    Flowers = serializer.Deserialize(stream) as List<Flower>;
                }
            }
        }

            static void Main(string[] args)
        {
            Garden garden = new Garden();

            // Создаем несколько цветов по умолчанию
            Flower flower1 = new Flower { Name = "Rose", Height = 10, HealthLevel = 5 };
            Flower flower2 = new Flower { Name = "Tulip", Height = 8, HealthLevel = 4 };
            Flower flower3 = new Flower { Name = "Daisy", Height = 6, HealthLevel = 3 };

            // Добавляем цветы в сад
            garden.AddFlower(flower1);
            garden.AddFlower(flower2);
            garden.AddFlower(flower3);

            // Поливаем цветы
            Action<Flower> waterAction = f => garden.WaterFlower(f);
            foreach (var flower in garden.Flowers)
            {
                waterAction(flower);
            }

            // Удобряем цветы
            Action<Flower> fertilizeAction = f => garden.FertilizeFlower(f);
            foreach (var flower in garden.Flowers)
            {
                fertilizeAction(flower);
            }

            // Отображаем информацию о цветах
            garden.DisplayInfo();

            // Провоцируем рост цветов и обрабатываем событие
            garden.FlowerGrowthEvent += f => Console.WriteLine($"{f.Name} has grown. New height: {f.Height}");
            garden.Flowers[0].Grow(2.5);

            // Фильтрация и сортировка цветов
            var tallFlowers = garden.GetFlowersWithHeightGreaterThan(8);
            var sortedFlowersByHealth = garden.GetFlowersSortedByHealthLevel();

            // Сохранение и загрузка данных
            garden.SaveFlowersToJson("flowers.json");
            garden.SaveFlowersToXml("flowers.xml");
            garden.LoadFlowersFromJson("flowers.json");
            garden.LoadFlowersFromXml("flowers.xml");
        }
    }
}