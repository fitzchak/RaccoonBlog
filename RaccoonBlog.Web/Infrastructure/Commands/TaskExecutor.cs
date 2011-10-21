using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elmah;

namespace RaccoonBlog.Web.Infrastructure.Commands
{
    public static class TaskExecutor
    {
		private static readonly ThreadLocal<List<BackgroundTask>> TasksToExecute =
			new ThreadLocal<List<BackgroundTask>>(() => new List<BackgroundTask>());

		public static void ExcuteLater(BackgroundTask task)
        {
			TasksToExecute.Value.Add(task);
        }

		public static void Discard()
		{
			TasksToExecute.Value.Clear();
		}

		public static void StartExecuting()
		{
			var copy = TasksToExecute.Value.ToArray();
			Discard();

			Task.Factory.StartNew(() =>
			                      	{
			                      		foreach (var backgroundTask in copy)
			                      		{
			                      			ExecuteTask(backgroundTask);
			                      		}
			                      	}, TaskCreationOptions.LongRunning)
				.ContinueWith(task =>
				              	{
				              		ErrorLog.GetDefault(null).Log(new Error(task.Exception));
				              	}, TaskContinuationOptions.OnlyOnFaulted);
		}

    	private static void ExecuteTask(BackgroundTask task)
		{
			for (var i = 0; i < 10; i++)
			{
				using (var session = MvcApplication.DocumentStore.OpenSession())
				{
					switch (task.Run(session))
					{
						case true:
						case false:
							return;
						case null:
							break;
					}
				}
			}
		}
    }
}
