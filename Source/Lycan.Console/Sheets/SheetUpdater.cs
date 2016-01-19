using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Ether.Outcomes;
using Lycan.Core;

namespace Lycan.UI.Sheets
{
    public class SheetUpdater
    {
        private readonly string CsvPath;
        private readonly FeedModel Feed;

        public SheetUpdater(string csvPath, FeedModel feed)
        {
            CsvPath = csvPath;
            Feed = feed;
        }

        /// <summary>
        /// Updates the CSV file, creating it if necessary, appending if there is already data.
        /// </summary>
        /// <returns></returns>
        public IOutcome UpdateSheet()
        {
            var recordsOutcome = GetRelevantRecords();

            if (recordsOutcome.Failure)
                return recordsOutcome;

            UpdateCsv(recordsOutcome.Value);

            return Outcomes.Success();
        }

        /// <summary>
        /// Reads the entire CSV from disk, then filters down to only the feed records that need to be written.
        /// </summary>
        private IOutcome<List<SheetModel>> GetRelevantRecords()
        {
            var readOutcome = ReadRecordsFromDisk();

            if (readOutcome.Failure)
                return readOutcome;

            List<SheetModel> rows = readOutcome.Value;

            int lastID = (rows.Count == 0) ? 0 : rows.Last().ArticleID;

            var relevantSet = Feed.Articles.Where(a => a.ArticleID > lastID).ToList();
            var models = GetSheetModels(relevantSet);

            return Outcomes.Success<List<SheetModel>>()
                           .WithValue(models);
        }

        /// <summary>
        /// Wraps DoReadRecordsFromDisk in exception handlers, so that if there's a problem, we can provide row, line, etc.
        /// </summary>
        private IOutcome<List<SheetModel>> ReadRecordsFromDisk()
        {
            if (!File.Exists(CsvPath))
                return Outcomes.Success<List<SheetModel>>()
                               .WithValue(new List<SheetModel>());

            try
            {                              
                var records = DoReadRecordsFromDisk();

                return Outcomes.Success<List<SheetModel>>()
                               .WithValue(records);
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Data["CsvHelper"].ToString();

                //var messages = (from object key in ex.Data.Keys
                //                select string.Format("{0}: {1}", key, ex.Data[key])).ToList();

                return Outcomes.Failure<List<SheetModel>>()
                               .WithMessage("Exception loading CSV")
                               .FromException(ex)
                               .WithMessage(errorMessage);
            }
        }


        /// <summary>
        /// Loads existing records from disk.
        /// </summary>
        private List<SheetModel> DoReadRecordsFromDisk()
        {
            using (var csv = new CsvReader(new StreamReader(CsvPath), new CsvConfiguration() { HasHeaderRecord = true}))
            {
               
                var rows = csv.GetRecords<SheetModel>().ToList();

                return rows;
            }           
        }

        /// <summary>
        /// Updates (or creates) the CSV file.
        /// </summary>
        private void UpdateCsv(List<SheetModel> models)
        {
            var newFile = !File.Exists(CsvPath);

            using (var csv = new CsvWriter(new StreamWriter(CsvPath, append: true)))
            {
                csv.Configuration.HasHeaderRecord = newFile; //Controls whether or not a header record is added.

                csv.WriteRecords(models);
            }
        }

        /// <summary>
        /// Converts Articles, which come from the feed, into the shape required for the spreadsheet.
        /// </summary>
        public List<SheetModel> GetSheetModels(List<ArticleModel> articles)
        {
            return articles.Select(a => new SheetModel() {
                Author = a.User,
                ReplyTo = a.ReplyTo,
                GameDay = "",
                Summary = "",
                Alignment = "",
                Link = a.Link,
                ArticleID = a.ArticleID }).ToList();
        }
    }
}
