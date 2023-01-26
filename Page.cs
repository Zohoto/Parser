using System.Text;
using System.Collections.Generic;

namespace Lab4
{
    class Page
    {
        public List<string> phones = new List<string>();
        public List<string> emails = new List<string>();
        public string Link { get; set; }
        public int Depth { get; }
        public string PhonesFormat
        {
            get
            {
                string str = "";
                foreach (var item in phones)
                    str += item + " ";
                return str;
            }
        }
        public string EmailFormat
        {
            get
            {
                string str = "";
                foreach (var item in emails)
                    str += item + " ";
                return str;
            }
        }
        public string DepthFormat
        {
            get
            {
                string result = "";
                for (int i = 0; i < Depth; i++)
                    result += "|--";
                return result;
            }
        }
        public Page(string link, int depth)
        {
            Link = link;
            Depth = depth;
        }
    }
}
