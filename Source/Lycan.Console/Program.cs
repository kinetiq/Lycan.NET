using System;
using Lycan.UI.Feeds;
using Lycan.UI.Sheets;

namespace Lycan.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            var reader = new FeedReader();

            var feedOutcome = reader.read(Constants.FeedUrl);

            if (feedOutcome.Failure)
            {
                Console.Write(feedOutcome.ToMultiLine(Environment.NewLine));
                PressAnyKeyToContinue();

                return;
            }

            var feed = feedOutcome.Value;
            var updater = new SheetUpdater(Constants.CsvPath, feed);

            var updateOutcome = updater.UpdateSheet();

            if (updateOutcome.Failure)
            {
                Console.Write(updateOutcome.ToMultiLine(Environment.NewLine));
                PressAnyKeyToContinue();

                return;
            }

            Console.WriteLine(@"Success!");
                
        }

        static void PressAnyKeyToContinue()
        {
            Console.WriteLine();
            Console.WriteLine(@"Press any key to continue...");
            Console.ReadKey();            
        }

       
    }
}
