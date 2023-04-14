using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CaesarConnection.Platform
{
    public static class WaitHandleExtension
    {
        // https://stackoverflow.com/questions/18756354/wrapping-manualresetevent-as-awaitable-task
        public static Task AsTask(this WaitHandle handle)
        {
            return AsTask(handle, Timeout.InfiniteTimeSpan);
        }

        public static Task AsTask(this WaitHandle handle, TimeSpan timeout)
        {
            var tcs = new TaskCompletionSource<object>();
            var registration = ThreadPool.RegisterWaitForSingleObject(handle, (state, timedOut) =>
            {
                var localTcs = (TaskCompletionSource<object>)state;
                if (timedOut)
                {
                    localTcs.TrySetCanceled();
                }
                else
                {
                    localTcs.TrySetResult(null);
                }
            }, tcs, timeout, executeOnlyOnce: true);
            tcs.Task.ContinueWith((_, state) => ((RegisteredWaitHandle)state).Unregister(null), registration, TaskScheduler.Default);
            return tcs.Task;
        }
        public static Task WaitOneAsync(this WaitHandle handle, CancellationToken cancellationToken, TimeSpan timeout)
        {
            if (handle == null)
            {
                throw new ArgumentNullException(nameof(handle));
            }

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            CancellationTokenRegistration ctr = cancellationToken.Register(() => tcs.TrySetCanceled());

            RegisteredWaitHandle rwh = ThreadPool.RegisterWaitForSingleObject(handle,
                (_, timedOut) =>
                {
                    if (timedOut)
                    {
                        tcs.TrySetCanceled();
                    }
                    else
                    {
                        tcs.TrySetResult(true);
                    }
                },
                null, timeout, true);

            Task<bool> task = tcs.Task;

            _ = task.ContinueWith(_ =>
            {
                rwh.Unregister(null);
                return ctr.Unregister();
            }, CancellationToken.None);

            return task;
        }
    }
}
