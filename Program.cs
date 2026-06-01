using System;

namespace IndependentWork20
{
    // =================================================================
    // 1. ПАТЕРН STRATEGY (Стратегії обробки даних звіту)
    // =================================================================

    // Інтерфейс стратегії
    public interface IDataProcessorStrategy
    {
        void Process(string data);
    }

    // Стратегія 1: Шифрування даних
    public class EncryptDataStrategy : IDataProcessorStrategy
    {
        public void Process(string data)
        {
            // Імітація шифрування (простий реверс рядка для наочності)
            char[] arr = data.ToCharArray();
            Array.Reverse(arr);
            string encrypted = new string(arr);
            Console.WriteLine($"[STRATEGY] 🔐 Encrypting data... Result: '{encrypted}'");
        }
    }

    // Стратегія 2: Стиснення даних
    public class CompressDataStrategy : IDataProcessorStrategy
    {
        public void Process(string data)
        {
            Console.WriteLine($"[STRATEGY] 🗜️ Compressing data... Deflate ratio: 42%. Original size: {data.Length} chars.");
        }
    }

    // Стратегія 3: Логування даних перед збереженням
    public class LogDataStrategy : IDataProcessorStrategy
    {
        public void Process(string data)
        {
            Console.WriteLine($"[STRATEGY] 📝 Logging data context execution for: '{data}'");
        }
    }

    // Контекст, який використовує стратегію (динамічна зміна поведінки в рантаймі)
    public class DataContext
    {
        private IDataProcessorStrategy _strategy;

        // Передача стратегії через конструктор
        public DataContext(IDataProcessorStrategy strategy)
        {
            _strategy = strategy;
        }

        // Зміна стратегії "на льоту"
        public void SetStrategy(IDataProcessorStrategy strategy)
        {
            _strategy = strategy;
        }

        // Виконання алгоритму стратегії
        public void ExecuteProcessing(string data)
        {
            if (_strategy == null)
            {
                Console.WriteLine("[ERROR] Strategy is not set!");
                return;
            }
            _strategy.Process(data);
        }
    }

    // =================================================================
    // 2. ПАТЕРН OBSERVER (Сповіщення через події C#)
    // =================================================================

    // Суб'єкт (Subject / Publisher), який генерує події
    public class DataPublisher
    {
        // Подія на основі вбудованого делегату Action<string>
        public event Action<string> DataProcessed;

        // Метод для виклику події та сповіщення підписників
        public void PublishDataProcessed(string data)
        {
            Console.WriteLine($"[PUBLISHER] 📢 State changed! Notifying observers about processed data: '{data}'");
            
            // Викликаємо подію, якщо є хоча б один підписник (?.Invoke)
            DataProcessed?.Invoke(data);
        }
    }

    // Спостерігач 1: Консольний логер
    public class ConsoleLoggerObserver
    {
        public void OnDataProcessed(string data)
        {
            Console.WriteLine($"   [OBSERVER] 💻 ConsoleLogger: Received update. Internal log saved for '{data}'.");
        }
    }

    // Спостерігач 2: Зберігач у файл
    public class FileSaverObserver
    {
        public void OnDataProcessed(string data)
        {
            Console.WriteLine($"   [OBSERVER] 💾 FileSaver: Simulated append to 'audit_trail.txt' with payload '{data}'.");
        }
    }

    // Спостерігач 3: Відправник аналітики
    public class AnalyticsSenderObserver
    {
        public void OnDataProcessed(string data)
        {
            Console.WriteLine($"   [OBSERVER] 📊 AnalyticsSender: Metric sent to cloud server. String payload length = {data.Length}.");
        }
    }

    // =================================================================
    // 3. МЕТОД MAIN (Клієнтський код)
    // =================================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Студент: Крупка Іван | Самостійна робота №20 ===");
            Console.WriteLine("Демонстрація патернів Strategy + Observer (Events)\n");

            // --- Крок 1: Ініціалізація Контексту та Видавця ---
            var context = new DataContext(new LogDataStrategy()); // Початкова стратегія - логування
            var publisher = new DataPublisher();

            // --- Крок 2: Створення та підписка спостерігачів (Observer) ---
            var consoleObs = new ConsoleLoggerObserver();
            var fileObs = new FileSaverObserver();
            var analyticsObs = new AnalyticsSenderObserver();

            // Підписуємо методи спостерігачів на подію через оператор +=
            publisher.DataProcessed += consoleObs.OnDataProcessed;
            publisher.DataProcessed += fileObs.OnDataProcessed;
            publisher.DataProcessed += analyticsObs.OnDataProcessed;

            string payload = "Report_No20_Data";

            // --- Крок 3: Робота зі Стратегією 1 (Логування) ---
            Console.WriteLine("--- Етап 1: Використання LogDataStrategy ---");
            context.ExecuteProcessing(payload);
            publisher.PublishDataProcessed(payload);
            Console.WriteLine();

            // --- Крок 4: Зміна стратегії в рантаймі на Стиснення ---
            Console.WriteLine("--- Етап 2: Динамічна зміна на CompressDataStrategy ---");
            context.SetStrategy(new CompressDataStrategy());
            context.ExecuteProcessing(payload);
            publisher.PublishDataProcessed(payload);
            Console.WriteLine();

            // --- Крок 5: Зміна стратегії на Шифрування + Демонстрація відписки ---
            Console.WriteLine("--- Етап 3: Динамічна зміна на EncryptDataStrategy & Відписка файлового менеджера ---");
            context.SetStrategy(new EncryptDataStrategy());
            context.ExecuteProcessing(payload);

            // Показуємо гнучкість Observer: відписуємо FileSaverObserver
            publisher.DataProcessed -= fileObs.OnDataProcessed;
            
            // Публікуємо знову — FileSaver вже не отримає сповіщення
            publisher.PublishDataProcessed(payload);
            Console.WriteLine();

            Console.WriteLine("Роботу завершено успішно.");
            Console.ReadKey();
        }
    }
}