namespace Blazor.IndexedDB
{
    public class StoreRecord<T>
    {
        public string Storename { get; set; }

        public T Data { get; set; }
    }
}
