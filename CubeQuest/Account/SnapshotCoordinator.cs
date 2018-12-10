using System;
using Android.App;
using Android.Gms.Games.Snapshot;
using Java.Lang;
using Java.Util.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Void = Java.Lang.Void;

namespace CubeQuest.Account
{
    // TODO: This is probably overkill, remove
    public class SnapshotCoordinator
    {
        /*
         * (Java)           =>  (C#)
         * SnapShotsClient  =>  ISnapshot
         * Snapshot         =>  SnapshotEntity
         */

        /// <summary>
        /// Opened files
        /// </summary>
        private Dictionary<string, CountDownLatch> opened;

        /// <summary>
        /// Files in the process of closing
        /// </summary>
        private HashSet<string> closing;

        /// <summary>
        /// Singleton for coordinating the Snapshots API
        /// </summary>
        public static SnapshotCoordinator Instance { get; } = new SnapshotCoordinator();

        public SnapshotCoordinator()
        {
            opened  = new Dictionary<string, CountDownLatch>();
            closing = new HashSet<string>();
        }

        /// <summary>
        /// If named file is already opened
        /// </summary>
        public bool IsOpen(string filename) => 
            opened.ContainsKey(filename);

        /// <summary>
        /// If the named file is in the process of closing
        /// </summary>
        public bool IsClosing(string filename) => 
            closing.Contains(filename);

        /// <summary>
        /// Records that the named file is closing
        /// </summary>
        private void SetClosing(string filename) => 
            closing.Add(filename);

        /// <summary>
        /// Record that the named file is closed
        /// </summary>
        /// <param name="filename"></param>
        private void SetClosed(string filename)
        {
            closing.Remove(filename);
            var pair = opened.FirstOrDefault(p => p.Key == filename);
            opened.Remove(pair.Key);
            pair.Value?.CountDown();
        }

        /// <summary>
        /// Record that the named file is opening
        /// </summary>
        /// <param name="filename"></param>
        private void SetOpening(string filename) => 
            opened.Add(filename, new CountDownLatch(1));

        /// <summary>
        /// Completed once the given file is closed
        /// </summary>
        public Task<Result> WaitForClosed(string filename)
        {
            var source = new TaskCompletionSource<Result>();
            var latch  = opened.FirstOrDefault(o => o.Key == filename).Value;

            if (latch != null)
            {
                source.SetResult(default(Result));
                return source.Task;
            }

            var result = new CountDownTask(latch).Await();
            source.SetResult(result);

            return source.Task;
        }

        public Task<Void> DiscardAndClose(ISnapshot snapshot, SnapshotEntity entity)
        {
            var filename = entity.Metadata.UniqueName;

            // TODO
            return null;
        }

        private Task<Void> SetOpeningTask(string filename)
        {
            var source = new TaskCompletionSource<Void>();

            if (IsOpen(filename))
                source.SetException(new IllegalStateException($"{filename} is already open"));
            else if (IsClosing(filename))
                source.SetException(new IllegalStateException($"{filename} is closing"));
            else
            {
                SetOpening(filename);
                source.SetResult(null);
            }

            return source.Task;
        }

        private Task<Void> SetClosingTask(string filename)
        {
            var source = new TaskCompletionSource<Void>();

            if (!IsOpen(filename))
                source.SetException(new IllegalStateException($"{filename} is already closed"));
            else if (!IsClosing(filename))
                source.SetException(new IllegalStateException($"{filename} is closing"));
            else
            {
                SetClosing(filename);
                source.SetResult(null);
            }

            return source.Task;
        }
    }

    internal class CountDownTask
    {
        private readonly CountDownLatch latch;

        private readonly bool canceled;

        public CountDownTask(CountDownLatch latch)
        {
            this.latch = latch;
            canceled = false;
        }

        public Result Await()
        {
            if (!canceled && latch != null)
            {
                try
                {
                    latch.Await();
                }
                catch (InterruptedException e)
                {
                    return Result.Canceled;
                }
            }

            return canceled ? Result.Canceled : Result.Ok;
        }
    }
}