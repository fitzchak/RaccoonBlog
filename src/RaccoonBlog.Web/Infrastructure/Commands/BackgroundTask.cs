using System;
using Raven.Abstractions.Exceptions;
using Raven.Client;

namespace RaccoonBlog.Web.Infrastructure.Commands
{
	public abstract class BackgroundTask
	{
		protected IDocumentSession documentSession;

		protected virtual void Initialize(IDocumentSession session)
		{
			documentSession = session;
			documentSession.Advanced.UseOptimisticConcurrency = true;
		}

		protected virtual void OnError(Exception e)
		{
		}

		public bool? Run(IDocumentSession openSession)
		{
			Initialize(openSession);
			try
			{
				Execute();
				documentSession.SaveChanges();
				TaskExecutor.StartExecuting();
				return true;
			}
			catch (ConcurrencyException e)
			{
				OnError(e);
				return null;
			}
			catch (Exception e)
			{
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
