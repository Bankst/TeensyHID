using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace TeensyHIDWindows.Comm
{
	public delegate void HIDMessageHandlerMethod(HIDMessage message, HIDConnection connection);

    public class HIDMessageHandler
	{
		public const int MAX_TASK_RUNTIME = 1500; // milliseconds

        public static int Count => Handlers.Count;

        private static readonly Dictionary<HIDOpcode, HIDMessageHandlerMethod> Handlers = new Dictionary<HIDOpcode, HIDMessageHandlerMethod>();

        private static readonly TaskFactory HandlerFactory = new TaskFactory(TaskCreationOptions.PreferFairness, TaskContinuationOptions.None);

		private static readonly ConcurrentDictionary<int, Task> RunningHandlers = new ConcurrentDictionary<int, Task>();
        public static void Invoke(HIDMessageHandlerMethod method, HIDMessage message, HIDConnection connection)
		{
			if (method == null || !connection.IsConnected)
			{
				return;
			}

            // TODO: Log handler invoke

			var stopwatch = new Stopwatch();
            stopwatch.Start();

			var cancelSource = new CancellationTokenSource();
			var token = cancelSource.Token;

			var task = HandlerFactory.StartNew(() =>
			{
				method.Invoke(message, connection);
				token.ThrowIfCancellationRequested();
			}, token);

			RunningHandlers.TryAdd(task.Id, task);

			try
			{
				if (!task.Wait(MAX_TASK_RUNTIME, token))
				{
					cancelSource.Cancel();
				}
			}
			catch (AggregateException ex)
			{
				foreach (var unused in ex.Flatten().InnerExceptions)
				{
					// TODO: log exceptions
				}

				// connection.Dispose();
			}
			catch (OperationCanceledException)
			{
				// TODO: log timeout
			}
			finally
			{
                stopwatch.Stop();
				RunningHandlers.TryRemove(task.Id, out var unused);
                cancelSource.Dispose();
				Object.Destroy(message);

				// TODO: log handler time
			}
		}

		public static void Store(HIDOpcode opcode, HIDMessageHandlerMethod method)
		{
			if (!Handlers.ContainsKey(opcode))
			{
				Handlers.Add(opcode, method);
			}
		}

		public static bool TryFetch(HIDOpcode opcode, out HIDMessageHandlerMethod method)
		{
			return Handlers.TryGetValue(opcode, out method);
		}
	}
}
