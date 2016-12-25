using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WsOwinDemo.Logging;

namespace WsOwinDemo.Monitoring
{
    public class ServiceMonitor : IDisposable
    {
        private readonly Uri _serviceUri;
        private readonly TimeSpan _interval;
        private readonly HttpClient _client;
        private Timer _timer;
        private bool _disposed;
        private readonly CancellationTokenSource _cancelationSource;
        private readonly LogWriter _logWriter;

        public ServiceMonitor(Uri serviceUri, TimeSpan interval, LogWriter logWriter)
        {
            if(serviceUri == null)
                throw new ArgumentNullException(nameof(serviceUri));

            _serviceUri = serviceUri;
            _interval = interval;
            _client = new HttpClient();
            _cancelationSource = new CancellationTokenSource();
            _logWriter = logWriter;
        }

        public void Start()
        {
            ThrowIfDisposed();

            if(_timer != null)
                throw new InvalidOperationException("Monitoring has beed already started");

            _timer = new Timer(async state =>
            {
                await ChechServiceAvailabilityAsync(_cancelationSource.Token);
            }, null, TimeSpan.Zero, _interval);
        }

        public void Stop()
        {
            ThrowIfDisposed();

            _cancelationSource.Cancel();

            if (_timer != null)
            {
                _timer?.Dispose();
                _timer = null;
            }

        }

        #region IDisposable
        public void Dispose()
        {
            if (!_disposed)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            _disposed = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _client?.Dispose();
                _timer?.Dispose();
                _cancelationSource?.Dispose();
            }
        }
        #endregion IDisposable

        private async Task ChechServiceAvailabilityAsync(CancellationToken token)
        {
            try
            {
                var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Head, _serviceUri), token);
                response.EnsureSuccessStatusCode();
                _logWriter.WriteToLog($"Service {_serviceUri} OK");

            }
            catch (TimeoutException ex)
            {
                // Handle timeout...
                _logWriter.WriteToLog($"Service {_serviceUri} unreachable\n{ex.Message}");
            }
            catch (HttpRequestException ex)
            {
                // Handle failed...
                _logWriter.WriteToLog($"Service {_serviceUri} unreachable\n{ex.Message}");
            }
        }

        private void ThrowIfDisposed()
        {
            if(_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
    }
}
    