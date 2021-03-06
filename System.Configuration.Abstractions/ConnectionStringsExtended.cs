using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.Configuration.Abstractions
{
    public class ConnectionStringsExtended : IConnectionStrings
    {
        public ConnectionStringSettingsCollection Raw { get; private set; }

        private readonly IEnumerable<IConnectionStringInterceptor> _interceptors;
        private readonly IAppSettings _appSettings;

        public ConnectionStringsExtended(ConnectionStringSettingsCollection raw, IAppSettings appSettings, IEnumerable<IConnectionStringInterceptor> interceptors = null)
        {
            _appSettings = appSettings;
            _interceptors = interceptors ?? new List<IConnectionStringInterceptor>();
            Raw = raw;
        }

        public ConnectionStringSettings this[string name]
        {
            get { return Intercept(Raw[name]); }
        }

        public ConnectionStringSettings this[int index]
        {
            get { return Intercept(Raw[index]); }
        }

        public int IndexOf(ConnectionStringSettings settings)
        {
            return Raw.IndexOf(settings);
        }

        public void Add(ConnectionStringSettings settings)
        {
            Raw.Add(settings);
        }

        public void Remove(ConnectionStringSettings settings)
        {
            Raw.Remove(settings);
        }

        public void RemoveAt(int index)
        {
            Raw.RemoveAt(index);
        }

        public void Remove(string name)
        {
            Raw.Remove(name);
        }

        public void Clear()
        {
            Raw.Clear();
        }

        private ConnectionStringSettings Intercept(ConnectionStringSettings rawSetting)
        {
            rawSetting = _interceptors.Aggregate(rawSetting,
                (current, interceptor) => interceptor.OnConnectionStringRetrieve(_appSettings, this, current));
            return rawSetting;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Raw.GetEnumerator();
        }

        public IEnumerator<ConnectionStringSettings> GetEnumerator()
        {
            for (int pos = 0; pos < Raw.Count; pos++)
            {
                yield return Intercept(Raw[pos]);
            }
        }
    }
}