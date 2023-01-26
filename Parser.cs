using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System;
using System.IO;
using System.Text;
using System.Linq;

namespace Lab4
{
    class Parser
    {
        private int depth = 0;
        private int count = 0;
        private string baseUrl;
        private static readonly string relPattern = @"<a href=""([/\w-]+)"">(.*)</a>";
        private static readonly string phonePattern = @"\+7([0-9 ()-])*";
        private static readonly string mailPattern = @"[\w]*\[at\][\w]*\[dot\][\w]*";
        private Queue<Page> pagesToScan = new Queue<Page>();
        private List<Page> pages = new List<Page>();
        public HttpClient wc = new HttpClient();
        public int MaxDepth { get; set; }
        public int MaxCount { get; set; }
        public Action<string> ItemFound;
        public Parser(int maxDepth, int maxCount, string baseUrl)
        {
            InitClient();
            MaxCount = maxCount;
            MaxDepth = maxDepth;
            this.baseUrl = baseUrl;
        }
        public async Task Scan(string baseUrl = null)
        {
            var page = pagesToScan.Count != 0 ? pagesToScan.Dequeue() : new Page(baseUrl, 0);
            string link;
            depth = page.Depth;
            if (baseUrl != null)
                link = baseUrl;
            else
            {
                link = page.Link;
            }
            HttpResponseMessage message = await wc.SendAsync(new HttpRequestMessage(HttpMethod.Get, link));
            string pageText = await message.Content.ReadAsStringAsync();
            var phoneMatches = Regex.Matches(pageText, phonePattern);
            for (int i = 0; i < phoneMatches.Count; i++)
            {
                page.phones.Add(phoneMatches[i].Groups[0].Value);
                ItemFound?.Invoke(phoneMatches[i].Groups[0].Value);
            }
            var emailMatches = Regex.Matches(pageText, mailPattern);
            for (int i = 0; i < emailMatches.Count; i++)
            {
                page.emails.Add(emailMatches[i].Groups[0].Value);
                ItemFound?.Invoke(emailMatches[i].Groups[0].Value);
            }
            pages.Add(page);
            var matches = Regex.Matches(pageText, relPattern);
            for (int i = 1; i < matches.Count; i++)
                pagesToScan.Enqueue(new Page(this.baseUrl + matches[i].Groups[1].Value, depth+1));
            count++;
            if (pagesToScan.Count == 0 || depth > MaxDepth || count == MaxCount)
                return;
            await Scan();
        }

        private IEnumerable<Page> GetPages() => pages.OrderBy(x => x.Depth);

        public void CreateCsv()
        {
            using (StreamWriter sw = new StreamWriter($"{baseUrl.Replace("https://", "")}.csv", false, Encoding.UTF8))
            {
                sw.WriteLine("ССылка на страницу; email; Телефон; Глубина");
                foreach (var p in GetPages())
                    sw.WriteLine(($"{p.DepthFormat} {p.Link};{p.DepthFormat} {p.EmailFormat};{p.DepthFormat} {p.PhonesFormat};{p.Depth};\n").Replace(";;", ";"));
            }
        }

        private void InitClient()
        {
            wc.DefaultRequestHeaders.Clear();
            wc.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/104.0.5112.102 Safari/537.36 OPR/90.0.4480.84");
            wc.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            wc.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
        }
    }
}