using System.Diagnostics;

namespace Blazor.IndexedDB
{
    internal class IndexedEntity<T> : IndexedEntity
    {
        internal const int DefaultHashCode = -1;
        private readonly IDictionary<string, int> _snapshot;

        internal IndexedEntity(T instance) : base(instance)
        {
            this._snapshot = new Dictionary<string, int>();

            this.TakeSnapshot();
        }

        internal new T Instance => (T)base.Instance;

        internal void TakeSnapshot()
        {
            this._snapshot.Clear();

            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                var code = property.GetValue(this.Instance)?.GetHashCode() ?? DefaultHashCode;
                // TODO: Check if GetHashCode collisions may occur and its severity
                this._snapshot.Add(property.Name, code);
                Debug.WriteLine($"Took snapshot of property {property.Name} with code {code}");
            }
        }

        internal void DetectChanges()
        {
            if (this.State == EntityState.Added ||
               this.State == EntityState.Deleted ||
               this.State == EntityState.Detached)
            {
                return;
            }

            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                this._snapshot.TryGetValue(property.Name, out var originalValue);

                // ToDo: Check if GetHashCode collisions may occur and its severity
                if (originalValue == (property.GetValue(this.Instance)?.GetHashCode() ?? DefaultHashCode))
                {
                    continue;
                }

                this.State = EntityState.Modified;
            }
        }
    }

    internal abstract class IndexedEntity
    {
        internal IndexedEntity(object instance)
        {
            this.Instance = instance;
        }

        internal EntityState State { get; set; }

        internal object Instance { get; }
    }
}
