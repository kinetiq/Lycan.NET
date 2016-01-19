using System;
using System.Xml.Linq;
using Ether.Outcomes;
using Lycan.Core;
using Lycan.Xml;

namespace Lycan.UI.Feeds
{
    public class FeedReader
    {
        public IOutcome<FeedModel> read(string feedUrl)
        {
            var xmlOutcome = GetXml(feedUrl);

            if (xmlOutcome.Failure)
                return Outcomes.Failure<FeedModel>()
                               .WithMessagesFrom(xmlOutcome);


            return GetFeed(xmlOutcome.Value);
        }

        private IOutcome<FeedModel> GetFeed(XDocument doc)
        {
            var parser = new FeedParser();
            FeedModel result;

            try
            {
                result = parser.Parse(doc);
            }
            catch (Exception ex)
            {
                return Outcomes.Failure<FeedModel>()
                               .WithMessage("Error parsing XDoc")
                               .FromException(ex);
            }

            return Outcomes.Success<FeedModel>()
                           .WithValue(result);
        }

        private IOutcome<XDocument> GetXml(string feedUrl)
        {
            XDocument doc;

            try
            {
                doc = XDocument.Load(feedUrl);
            }
            catch (Exception ex)
            {
                return Outcomes.Failure<XDocument>()
                               .WithMessage("Error reading feed")
                               .FromException(ex);
            }

            return Outcomes.Success<XDocument>()
                           .WithValue(doc);
        }
    }
}
