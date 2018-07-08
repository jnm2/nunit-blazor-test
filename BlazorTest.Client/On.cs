using System;
using System.Threading;

namespace BlazorTest.Client
{
    internal static class On
    {
        public static IDisposable Dispose(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return new DisposableAction(action);
        }

        private sealed class DisposableAction : IDisposable
        {
            private Action action;

            public DisposableAction(Action action)
            {
                this.action = action;
            }

            public void Dispose()
            {
                Interlocked.Exchange(ref action, null)?.Invoke();
            }
        }
    }
}
