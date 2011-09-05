using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Elmah;
using Raven.Client;

namespace RaccoonBlog.Web.Infrastructure.Commands
{
    public static class CommandExecutor
    {
        public static void ExcuteLater(ICommand command)
        {
            Task.Factory.StartNew(() =>
            {
                var succcessfully = false;
                try
                {
                    TryAddSession(command);
                    command.Execute();
                    succcessfully = true;
                }
                finally
                {
                    TryComplete(command, succcessfully);
                }
            }, TaskCreationOptions.LongRunning)
                .ContinueWith(task =>
                {
                    ErrorLog.GetDefault(null).Log(new Error(task.Exception));
                }, TaskContinuationOptions.OnlyOnFaulted);
        }

    	private class Accessors
    	{
    		public Action<object, IDocumentSession> Set;
    		public Func<object, IDocumentSession> Get;
    	}

    	private static readonly ConcurrentDictionary<Type, Accessors> AccessorsCache = new ConcurrentDictionary<Type, Accessors>();
        private static Accessors CreateAccessorsForType(Type type)
        {
            var sessionProp =
                type.GetProperties().FirstOrDefault(
                    x => x.PropertyType == typeof(IDocumentSession) && x.CanRead && x.CanWrite);
            if (sessionProp == null)
                return null;			return new Accessors                       {                           Set = (instance, session) => sessionProp.SetValue(instance, session, null),                           Get = instance => (IDocumentSession)sessionProp.GetValue(instance, null)                       };
        }
		public static IDocumentSession TryAddSession(object instance)
        {
            var accessors = AccessorsCache.GetOrAdd(instance.GetType(), CreateAccessorsForType);
			if (accessors == null)				return null;
			var documentSession = MvcApplication.DocumentStore.OpenSession();
			accessors.Set(instance, documentSession);
			return documentSession;
        }
        public static void TryComplete(object instance, bool succcessfully)
        {
            Accessors accesors;
            if (AccessorsCache.TryGetValue(instance.GetType(), out accesors) == false || accesors == null)
                return;
            using (var documentSession = accesors.Get(instance))
            {
                if (documentSession == null)                    return;
                if (succcessfully)                    documentSession.SaveChanges();
            }
        }
    }
}
