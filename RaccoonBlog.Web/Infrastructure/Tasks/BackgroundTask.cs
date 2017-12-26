/*
using System;
using Raven.Client.Documents.Session;
using Raven.Client.Exceptions;

namespace HibernatingRhinos.Loci.Common.Tasks
{
	public abstract class BackgroundTask
	{
		protected IAsyncDocumentSession DocumentSession;

		private readonly Logger logger = LogManager.GetCurrentClassLogger();

		protected void Initialize(IAsyncDocumentSession session)
		{
			DocumentSession = session;
			DocumentSession.Advanced.UseOptimisticConcurrency = true;
		}

		protected virtual void OnError(Exception e)
		{
		}

		public bool? Run(IAsyncDocumentSession openSession)
		{
			Initialize(openSession);
			try
			{
				Execute();
				DocumentSession.SaveChanges();
				TaskExecutor.StartExecuting();
				return true;
			}
			catch (ConcurrencyException e)
			{
				logger.ErrorException("Could not execute task " + GetType().Name, e);
				OnError(e);
				return null;
			}
			catch (Exception e)
			{
				logger.ErrorException("Could not execute task " + GetType().Name, e);
				OnError(e);
				return false;
			}
			finally
			{
				TaskExecutor.Discard();
			}
		}

		public abstract void Execute();
	}
}
*/
