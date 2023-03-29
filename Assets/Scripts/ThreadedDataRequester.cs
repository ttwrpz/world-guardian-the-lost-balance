using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ThreadedDataRequester : MonoBehaviour
{
    static ThreadedDataRequester instance;
    private ConcurrentQueue<ThreadInfo> dataQueue = new();
    private int maxThreads = 4;
    CustomThreadPool threadPool;

    private void Awake()
    {
        instance = FindFirstObjectByType<ThreadedDataRequester>();
        threadPool = new CustomThreadPool(maxThreads);
    }

    public static void RequestData(Func<object> generateData, Action<object> callback)
    {
        if (instance == null)
        {
            Debug.LogWarning("ThreadedDataRequester instance is null");
            return;
        }
        instance.DataThread(generateData, callback);
    }

    void DataThread(Func<object> generateData, Action<object> callback)
    {
        threadPool.Enqueue(() =>
        {
            object data = generateData();
            dataQueue.Enqueue(new ThreadInfo(callback, data));
        });
    }

    private void Update()
    {
        ThreadInfo threadInfo;
        while (dataQueue.TryDequeue(out threadInfo))
        {
            threadInfo.callback(threadInfo.parameter);
        }
    }

    public class CustomThreadPool
    {
        private readonly object queueLock = new object();
        private readonly Queue<Action> queue = new Queue<Action>();
        private readonly List<Thread> threads = new List<Thread>();
        private int maxThreads;

        public CustomThreadPool(int maxThreads)
        {
            this.maxThreads = maxThreads;
            for (int i = 0; i < maxThreads; i++)
            {
                Thread thread = new Thread(Worker);
                thread.Start();
                threads.Add(thread);
            }
        }

        public void Enqueue(Action action)
        {
            lock (queueLock)
            {
                queue.Enqueue(action);
                Monitor.Pulse(queueLock);
            }
        }

        private void Worker()
        {
            while (true)
            {
                Action action = null;
                lock (queueLock)
                {
                    while (queue.Count == 0)
                    {
                        Monitor.Wait(queueLock);
                    }
                    action = queue.Dequeue();
                }
                if (action == null)
                {
                    return;
                }
                action();
            }
        }

        public void Shutdown()
        {
            lock (queueLock)
            {
                for (int i = 0; i < maxThreads; i++)
                {
                    queue.Enqueue(null);
                }
                Monitor.PulseAll(queueLock);
            }
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
            threads.Clear();
        }
    }


    struct ThreadInfo
    {
        public readonly Action<object> callback;
        public readonly object parameter;
        public ThreadInfo(Action<object> callback, object parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}