using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace SpotNetCore.Implementation
{
    public class NonBlockingHttpListener
    {
        private readonly HttpListener _listener;
        private readonly Thread _listenerThread;
        private readonly Thread[] _workers;
        private readonly ManualResetEvent _stop, _ready;
        private readonly Queue<HttpListenerContext> _queue;

        public NonBlockingHttpListener(int maxThreads)
        {
            _workers = new Thread[maxThreads];
            _queue = new Queue<HttpListenerContext>();
            _stop = new ManualResetEvent(false);
            _ready = new ManualResetEvent(false);
            _listener = new HttpListener();
            _listenerThread = new Thread(HandleRequests);
        }

        public void Start(int port)
        {
            _listener.Prefixes.Add(String.Format(@"http://+:{0}/", port));
            _listener.Start();
            _listenerThread.Start();

            for (var i = 0; i < _workers.Length; i++)
            {
                _workers[i] = new Thread(Worker);
                _workers[i].Start();
            }
        }

        public void Dispose()
        {
            Stop();
        }

        private void Stop()
        {
            _stop.Set();
            _listenerThread.Join();
            foreach (var worker in _workers)
            {
                worker.Join();
            }
            _listener.Stop();
        }

        private void HandleRequests()
        {
            while (_listener.IsListening)
            {
                var context = _listener.BeginGetContext(ContextReady, null);

                if (0 == WaitHandle.WaitAny(new[] {_stop, context.AsyncWaitHandle}))
                {
                    return;
                }
            }
        }

        private void ContextReady(IAsyncResult ar)
        {
            lock (_queue)
            {
                _queue.Enqueue(_listener.EndGetContext(ar));
                _ready.Set();
            }
        }

        private void Worker()
        {
            var wait = new[] { _ready, _stop };
            while (0 == WaitHandle.WaitAny(wait))
            {
                HttpListenerContext context;
                lock (_queue)
                {
                    if (_queue.Count > 0)
                    {
                        context = _queue.Dequeue();
                    }
                    else
                    {
                        _ready.Reset();
                        continue;
                    }
                }

                try
                {
                    ProcessRequest(context);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                }
            }
        }

        public event Action<HttpListenerContext> ProcessRequest;
    }
}