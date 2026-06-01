using System;

namespace IndependentWork20
{
    // =================================================================
    // 1. ПАТЕРН STRATEGY (Стратегії накладання фільтрів на зображення)
    // =================================================================

    // Інтерфейс стратегії обробки зображення
    public interface IDataProcessorStrategy
    {
        void Process(string data);
    }

    // Стратегія 1: Чорно-білий фільтр
    public class GrayscaleFilterStrategy : IDataProcessorStrategy
    {
        public void Process(string data)
        {
            Console.WriteLine($"[STRATEGY] 🔲 Applying GrayscaleFilter... Converting '{data}' pixels to shades of gray.");
        }
    }

    // Стратегія 2: Фільтр Сепія (ефект старовини)
    public class SepiaFilterStrategy : IDataProcessorStrategy
    {
        public void Process(string data)
        {
            Console.WriteLine($"[STRATEGY] 🟫 Applying SepiaFilter... Adding warm brown tones to '{data}'.");
        }
    }

    // Стратегія 3: Розмиття зображення
    public class BlurFilterStrategy : IDataProcessorStrategy
    {
        public void Process(string data)
        {
            Console.WriteLine($"[STRATEGY] 🌫️ Applying BlurFilter... Smoothing pixels for '{data}' using Gaussian blur matrix.");
        }
    }

    // Контекст обробки (в нашому випадку — графічний редактор / процесор зображень)
    public class ImageDataContext
    {
        private IDataProcessorStrategy _strategy;

        // Конструктор приймає початкову стратегію
        public ImageDataContext(IDataProcessorStrategy strategy)
        {
            _strategy = strategy;
        }

        // Зміна фільтра в рантаймі
        public void SetStrategy(IDataProcessorStrategy strategy)
        {
            _strategy = strategy;
        }

        // Запуск обробки поточним фільтром
        public void ExecuteProcessing(string data)
        {
            if (_strategy == null)
            {
                Console.WriteLine("[ERROR] Filter strategy is not set!");
                return;
            }
            _strategy.Process(data);
        }
    }

    // =================================================================
    // 2. ПАТЕРН OBSERVER (Сповіщення про готовність зображення)
    // =================================================================

    // Суб'єкт (Publisher), який сповіщає, що зображення оброблено
    public class DataPublisher
    {
        // Подія, на яку підписуватимуться спостерігачі
        public event Action<string> DataProcessed;

        // Метод публікації події
        public void PublishDataProcessed(string data)
        {
            Console.WriteLine($"[PUBLISHER] 📢 Image processing finished! Notifying UI and storage handlers for: '{data}'");
            
            // Виклик події для всіх підписників
            DataProcessed?.Invoke(data);
        }
    }

    // Спостерігач 1: Імітація відображення на екрані (UI)
    public class ConsoleOutputObserver
    {
        public void OnDataProcessed(string data)
        {
            Console.WriteLine($"   [OBSERVER] 💻 ConsoleOutput: Refreshing screen matrix. Rendering updated image '{data}' on display.");
        }
    }

    // Спостерігач 2: Зберігач обробленого зображення на диск
    public class ImageSaverObserver
    {
        public void OnDataProcessed(string data)
        {
            Console.WriteLine($"   [OBSERVER] 💾 ImageSaver: Writing metadata. Image '{data}' successfully written to /outputs/ folder.");
        }
    }

    // =================================================================
    // 3. МЕТОД MAIN (Тестування варіанту)
    // =================================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Студент: Крупка Іван | Варіант: Обробка зображень ===");
            Console.WriteLine("Демонстрація патернів Strategy + Observer\n");

            // Назва файлу зображення, що імітує вхідні дані
            string imageName = "photo_2026.png";

            // 1. Створюємо контекст (початковий фільтр — Grayscale) та видавця подій
            var imageContext = new ImageDataContext(new GrayscaleFilterStrategy());
            var publisher = new DataPublisher();

            // 2. Створюємо спостерігачів згідно з вашим варіантом
            var consoleUI = new ConsoleOutputObserver();
            var imageSaver = new ImageSaverObserver();

            // 3. Підписуємо спостерігачів на подію закінчення обробки
            publisher.DataProcessed += consoleUI.OnDataProcessed;
            publisher.DataProcessed += imageSaver.OnDataProcessed;

            // --- ТЕСТ 1: Ефект Чорно-білого фото ---
            Console.WriteLine("--- Етап 1: Застосування GrayscaleFilter ---");
            imageContext.ExecuteProcessing(imageName);
            publisher.PublishDataProcessed(imageName);
            Console.WriteLine();

            // --- ТЕСТ 2: Динамічна зміна фільтра на Сепію ---
            Console.WriteLine("--- Етап 2: Зміна фільтра на SepiaFilter ---");
            imageContext.SetStrategy(new SepiaFilterStrategy());
            imageContext.ExecuteProcessing(imageName);
            publisher.PublishDataProcessed(imageName);
            Console.WriteLine();

            // --- ТЕСТ 3: Динамічна зміна на Розмиття + Демонстрація гнучкості спостерігачів ---
            Console.WriteLine("--- Етап 3: Зміна фільтра на BlurFilter & Відписка збереження на диск ---");
            imageContext.SetStrategy(new BlurFilterStrategy());
            imageContext.ExecuteProcessing(imageName);

            // Наприклад, користувач просто дивиться прев'ю, автоматично зберігати на диск не потрібно
            publisher.DataProcessed -= imageSaver.OnDataProcessed;

            // Сповіщаємо знову — тепер відпрацює лише вивід на екран
            publisher.PublishDataProcessed(imageName);
            Console.WriteLine();

            Console.WriteLine("Програму успішно виконано.");
            Console.ReadKey();
        }
    }
}