using System;
using System.Threading.Tasks;
namespace Lab4
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine(@"Чтобы начать парсинг введите:максимальную глубину, максимальное количество страниц и ссылку на ресурс, который вы хотели бы пропарсить.");
            string maxDepth = Console.ReadLine();
            string maxCount = Console.ReadLine();
            string baseUrl = Console.ReadLine();
            Parser parser = new Parser(int.Parse(maxDepth), int.Parse(maxCount), baseUrl);
            parser.ItemFound += (str) => Console.WriteLine(str);
            await parser.Scan(baseUrl);
            parser.CreateCsv();
            parser.wc.Dispose();
            Console.ReadLine();
        }
    }
}
