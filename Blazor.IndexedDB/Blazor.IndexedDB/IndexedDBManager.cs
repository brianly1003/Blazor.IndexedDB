using Microsoft.JSInterop;

namespace Blazor.IndexedDB
{
    public class IndexedDBManager
    {
        private readonly DbStore _dbStore;
        private readonly IJSRuntime _jsRuntime;
        private const string InteropPrefix = "TimeGhost.IndexedDbManager";
        private bool _isOpen;

        public event EventHandler<IndexedDBNotificationArgs> ActionCompleted;

        public IndexedDBManager(DbStore dbStore, IJSRuntime jsRuntime)
        {
            this._dbStore = dbStore;
            this._jsRuntime = jsRuntime;
        }

        public List<StoreSchema> Stores => this._dbStore.Stores;

        public int CurrentVersion => this._dbStore.Version;

        public string DbName => this._dbStore.DbName;

        public async Task OpenDb()
        {
            IndexedDBManager indexedDbManager = this;
            string result = await indexedDbManager.CallJavascript<string>("openDb", (object)indexedDbManager._dbStore, (object)new
            {
                Instance = DotNetObjectReference.Create<IndexedDBManager>(indexedDbManager),
                MethodName = "Callback"
            });
            indexedDbManager._isOpen = true;
            await indexedDbManager.GetCurrentDbState();
            indexedDbManager.RaiseNotification(IndexDBActionOutCome.Successful, result);
        }

        public async Task DeleteDb(string dbName)
        {
            IndexedDBManager indexedDbManager = this;
            if (string.IsNullOrEmpty(dbName))
                throw new ArgumentException("dbName cannot be null or empty", nameof(dbName));
            string message = await indexedDbManager.CallJavascript<string>("deleteDb", (object)dbName);
            indexedDbManager.RaiseNotification(IndexDBActionOutCome.Successful, message);
        }

        public async Task GetCurrentDbState()
        {
            IndexedDBManager indexedDbManager = this;
            await indexedDbManager.EnsureDbOpen();
            DbInformation dbInformation = await indexedDbManager.CallJavascript<DbInformation>("getDbInfo", (object)indexedDbManager._dbStore.DbName);
            if (dbInformation.Version <= indexedDbManager._dbStore.Version)
                return;
            indexedDbManager._dbStore.Version = dbInformation.Version;
            IEnumerable<string> source = indexedDbManager._dbStore.Stores.Select<StoreSchema, string>((Func<StoreSchema, string>)(s => s.Name));
            foreach (string storeName in dbInformation.StoreNames)
            {
                if (!source.Contains<string>(storeName))
                    indexedDbManager._dbStore.Stores.Add(new StoreSchema()
                    {
                        DbVersion = new int?(dbInformation.Version),
                        Name = storeName
                    });
            }
        }

        public async Task AddNewStore(StoreSchema storeSchema)
        {
            IndexedDBManager indexedDbManager = this;
            if (storeSchema == null || indexedDbManager._dbStore.Stores.Any<StoreSchema>((Func<StoreSchema, bool>)(s => s.Name == storeSchema.Name)))
                return;
            indexedDbManager._dbStore.Stores.Add(storeSchema);
            ++indexedDbManager._dbStore.Version;
            string str = await indexedDbManager.CallJavascript<string>("openDb", (object)indexedDbManager._dbStore, (object)new
            {
                Instance = DotNetObjectReference.Create<IndexedDBManager>(indexedDbManager),
                MethodName = "Callback"
            });
            indexedDbManager._isOpen = true;
            indexedDbManager.RaiseNotification(IndexDBActionOutCome.Successful, "new store " + storeSchema.Name + " added");
        }

        public async Task AddRecord<T>(StoreRecord<T> recordToAdd)
        {
            await this.EnsureDbOpen();
            try
            {
                this.RaiseNotification(IndexDBActionOutCome.Successful, await this.CallJavascript<StoreRecord<T>, string>("addRecord", recordToAdd));
            }
            catch (JSException ex)
            {
                this.RaiseNotification(IndexDBActionOutCome.Failed, ex.Message);
            }
        }

        public async Task UpdateRecord<T>(StoreRecord<T> recordToUpdate)
        {
            await this.EnsureDbOpen();
            try
            {
                this.RaiseNotification(IndexDBActionOutCome.Successful, await this.CallJavascript<StoreRecord<T>, string>("updateRecord", recordToUpdate));
            }
            catch (JSException ex)
            {
                this.RaiseNotification(IndexDBActionOutCome.Failed, ex.Message);
            }
        }

        public async Task<List<TResult>> GetRecords<TResult>(string storeName)
        {
            IndexedDBManager indexedDbManager = this;
            await indexedDbManager.EnsureDbOpen();
            try
            {
                List<TResult> records = await indexedDbManager.CallJavascript<List<TResult>>("getRecords", (object)storeName);
                indexedDbManager.RaiseNotification(IndexDBActionOutCome.Successful, string.Format("Retrieved {0} records from {1}", (object)records.Count, (object)storeName));
                return records;
            }
            catch (JSException ex)
            {
                indexedDbManager.RaiseNotification(IndexDBActionOutCome.Failed, ex.Message);
                return (List<TResult>)null;
            }
        }

        public async Task<TResult> GetRecordById<TInput, TResult>(string storeName, TInput id)
        {
            IndexedDBManager indexedDbManager = this;
            await indexedDbManager.EnsureDbOpen();
            var data = new { Storename = storeName, Id = id };
            try
            {
                return await indexedDbManager.CallJavascript<TResult>("getRecordById", (object)storeName, (object)(TInput)id);
            }
            catch (JSException ex)
            {
                indexedDbManager.RaiseNotification(IndexDBActionOutCome.Failed, ex.Message);
                return default(TResult);
            }
        }

        public async Task DeleteRecord<TInput>(string storeName, TInput id)
        {
            IndexedDBManager indexedDbManager = this;
            try
            {
                string str = await indexedDbManager.CallJavascript<string>("deleteRecord", (object)storeName, (object)(TInput)id);
                indexedDbManager.RaiseNotification(IndexDBActionOutCome.Deleted, string.Format("Deleted from {0} record: {1}", (object)storeName, (object)(TInput)id));
            }
            catch (JSException ex)
            {
                indexedDbManager.RaiseNotification(IndexDBActionOutCome.Failed, ex.Message);
            }
        }

        public async Task ClearStore(string storeName)
        {
            int num = 0;
            if (num != 0 && string.IsNullOrEmpty(storeName))
                throw new ArgumentException("Parameter cannot be null or empty", nameof(storeName));
            try
            {
                this.RaiseNotification(IndexDBActionOutCome.Successful, await this.CallJavascript<string, string>("clearStore", storeName));
            }
            catch (JSException ex)
            {
                this.RaiseNotification(IndexDBActionOutCome.Failed, ex.Message);
            }
        }

        public async Task<TResult> GetRecordByIndex<TInput, TResult>(StoreIndexQuery<TInput> searchQuery)
        {
            await this.EnsureDbOpen();
            try
            {
                return await this.CallJavascript<StoreIndexQuery<TInput>, TResult>("getRecordByIndex", searchQuery);
            }
            catch (JSException ex)
            {
                this.RaiseNotification(IndexDBActionOutCome.Failed, ex.Message);
                return default(TResult);
            }
        }

        public async Task<IList<TResult>> GetAllRecordsByIndex<TInput, TResult>(
          StoreIndexQuery<TInput> searchQuery)
        {
            await this.EnsureDbOpen();
            try
            {
                IList<TResult> allRecordsByIndex = await this.CallJavascript<StoreIndexQuery<TInput>, IList<TResult>>("getAllRecordsByIndex", searchQuery);
                this.RaiseNotification(IndexDBActionOutCome.Successful, string.Format("Retrieved {0} records, for {1} on index {2}", (object)allRecordsByIndex.Count, (object)searchQuery.QueryValue, (object)searchQuery.IndexName));
                return allRecordsByIndex;
            }
            catch (JSException ex)
            {
                this.RaiseNotification(IndexDBActionOutCome.Failed, ex.Message);
                return (IList<TResult>)null;
            }
        }

        [JSInvokable("Callback")]
        public void CalledFromJS(string message) => Console.WriteLine("called from JS: " + message);

        private async Task<TResult> CallJavascript<TData, TResult>(string functionName, TData data) => await this._jsRuntime.InvokeAsync<TResult>("TimeGhost.IndexedDbManager." + functionName, (object)(TData)data);

        private async Task<TResult> CallJavascript<TResult>(string functionName, params object[] args) => await this._jsRuntime.InvokeAsync<TResult>("TimeGhost.IndexedDbManager." + functionName, args);

        private async Task EnsureDbOpen()
        {
            if (this._isOpen)
                return;
            await this.OpenDb();
        }

        private void RaiseNotification(IndexDBActionOutCome outcome, string message)
        {
            EventHandler<IndexedDBNotificationArgs> actionCompleted = this.ActionCompleted;
            if (actionCompleted == null)
                return;
            actionCompleted((object)this, new IndexedDBNotificationArgs()
            {
                Outcome = outcome,
                Message = message
            });
        }
    }
}
