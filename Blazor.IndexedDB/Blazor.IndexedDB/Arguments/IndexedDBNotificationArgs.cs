namespace Blazor.IndexedDB
{
    public class IndexedDBNotificationArgs : EventArgs
    {
        public IndexDBActionOutCome Outcome { get; set; }

        public string Message { get; set; }
    }
}
