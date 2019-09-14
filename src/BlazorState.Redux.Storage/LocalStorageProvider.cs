using System.Text.Json;
using System.Threading.Tasks;
using Blazor.Extensions.Storage;
using BlazorState.Redux.Interfaces;
using Newtonsoft.Json;

namespace BlazorState.Redux.Storage
{
    public class LocalStorageProvider : IStateStorage
    {
        private readonly LocalStorage _storage;

        public LocalStorageProvider(LocalStorage storage)
        {
            _storage = storage;
        }

        public async ValueTask<T> Get<T>(string key)
        {
            var stateJsonMemory = await _storage.GetItem<JsonElement>(key);
            return JsonConvert.DeserializeObject<T>(stateJsonMemory.ToString());
        }

        public async ValueTask Save<T>(string key, T state)
        {
            await _storage.SetItem(key, state);
        }
    }
}
