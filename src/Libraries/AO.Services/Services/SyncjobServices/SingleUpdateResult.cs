namespace AO.Services.Services.SyncjobServices
{
    public class SingleUpdateResult
    {
        public Result UpdateResult;
        public bool ProductCreated;
        public string WarningMessage;
        public string ErrorMessage;
        public string LogMessage;
        public string LogCrawlErrors;
    }

    public enum Result
    {
        Skipped,
        Updated,
        Created
    }
}
