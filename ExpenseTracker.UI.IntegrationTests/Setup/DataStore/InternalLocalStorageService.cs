using Blazored.LocalStorage;
using System.Text.Json;

namespace ExpenseTracker.IntegrationTests.Setup.DataStore
{
    internal class InternalLocalStorageService : ILocalStorageService
    {
        private readonly IDictionary<string, string> _storage;

        public event EventHandler<ChangingEventArgs>? Changing;
        public event EventHandler<ChangedEventArgs>? Changed;

        public InternalLocalStorageService(IDictionary<string, string> storage)
        {
            _storage = storage;
        }

        private void RaiseChanging(string key, object? oldValue, object? newValue)
        {
            Changing?.Invoke(this, new ChangingEventArgs
            {
                Key = key,
                OldValue = oldValue,
                NewValue = newValue
            });
        }

        private void RaiseChanged(string key, object? oldValue, object? newValue)
        {
            Changed?.Invoke(this, new ChangedEventArgs
            {
                Key = key,
                OldValue = oldValue,
                NewValue = newValue
            });
        }

        public ValueTask ClearAsync(CancellationToken cancellationToken = default)
        {
            var keys = _storage.Keys.ToList();
            foreach (var key in keys)
            {
                RaiseChanging(key, _storage[key], null);
            }

            _storage.Clear();

            foreach (var key in keys)
            {
                RaiseChanged(key, null, null);
            }

            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> ContainKeyAsync(string key, CancellationToken cancellationToken = default)
            => new(_storage.ContainsKey(key));

        public ValueTask<string?> GetItemAsStringAsync(string key, CancellationToken cancellationToken = default)
            => new(_storage.TryGetValue(key, out var value) ? value : null);

        public ValueTask<T?> GetItemAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            if (!_storage.TryGetValue(key, out var json))
            {
                return new(default(T));
            }

            if (typeof(T).IsAssignableFrom(typeof(string)))
            {
                return new((T?)(object?)json);
            }

            return new(JsonSerializer.Deserialize<T>(json));
        }

        public ValueTask<string?> KeyAsync(int index, CancellationToken cancellationToken = default)
        {
            if (index < 0 || index >= _storage.Count)
                return new((string?)null);

            return new(_storage.Keys.ElementAt(index));
        }

        public ValueTask<IEnumerable<string>> KeysAsync(CancellationToken cancellationToken = default)
            => new(_storage.Keys.AsEnumerable());

        public ValueTask<int> LengthAsync(CancellationToken cancellationToken = default)
            => new(_storage.Count);

        public ValueTask RemoveItemAsync(string key, CancellationToken cancellationToken = default)
        {
            if (_storage.TryGetValue(key, out var oldValue))
            {
                RaiseChanging(key, oldValue, null);
                _storage.Remove(key);
                RaiseChanged(key, oldValue, null);
            }
            return ValueTask.CompletedTask;
        }

        public ValueTask RemoveItemsAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
        {
            foreach (var key in keys.ToList())
            {
                _ = RemoveItemAsync(key, cancellationToken);
            }
            return ValueTask.CompletedTask;
        }

        public ValueTask SetItemAsStringAsync(string key, string data, CancellationToken cancellationToken = default)
        {
            _storage.TryGetValue(key, out var oldValue);
            RaiseChanging(key, oldValue, data);
            _storage[key] = data;
            RaiseChanged(key, oldValue, data);
            return ValueTask.CompletedTask;
        }

        public ValueTask SetItemAsync<T>(string key, T data, CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(data);
            return SetItemAsStringAsync(key, json, cancellationToken);
        }
    }
}
