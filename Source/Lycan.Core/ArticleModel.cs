using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lycan.Core
{
    public class ArticleModel
    {
        public int ArticleID { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Link { get; set; }
        public string PostDate { get; set; }
        public string User { get; set; }
        public string ReplyTo { get; set; }
        public int Edits { get; set; }
    }
}
