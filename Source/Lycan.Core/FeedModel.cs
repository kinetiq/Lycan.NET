using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lycan.Core
{
    public class FeedModel
    {
        public string Subject { get; set; }
        public string ArticleCount { get; set; }
        public List<ArticleModel> Articles { get; set; }

        public FeedModel()
        {
            Articles = new List<ArticleModel>();
        } 
    }
}
