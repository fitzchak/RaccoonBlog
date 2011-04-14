using System;
using System.Collections.Concurrent;
using System.Linq;
using Raven.Client;
using Raven.Client.Document;

namespace RavenDbBlog.Infrastructure.Raven
{
    /// <summary>
    /// This class manages the state of objects that desire a document session. We aren't relying on an IoC container here
    /// because this is the sole case where we actually need to do injection.
    /// </summary>
    public class DocumentStoreHolder
    {
        public static readonly IDocumentStore DocumentStore = CreateDocumentStore();

        private static IDocumentStore CreateDocumentStore()
        {
            return new DocumentStore
            {
                Url = "http://localhost:8080"
            }.Initialize();
        }

        private static readonly ConcurrentDictionary<Type, Accessors> AccessorsCache = new ConcurrentDictionary<Type, Accessors>();


        private static Accessors CreateAccessorsForType(Type type)
        {
            var sessionProp =
                type.GetProperties().FirstOrDefault(
                    x => x.PropertyType == typeof(IDocumentSession) && x.CanRead && x.CanWrite);
            if (sessionProp == null)
                return null;

            return new Accessors
                       {
                           Set = (instance, session) => sessionProp.SetValue(instance, session, null),
                           Get = instance => (IDocumentSession)sessionProp.GetValue(instance, null)
                       };
        }


        public static void TryAddSession(object instance)
        {
            var accessors = AccessorsCache.GetOrAdd(instance.GetType(), CreateAccessorsForType);

            if (accessors == null)
                return;

            accessors.Set(instance, DocumentStore.OpenSession());
        }

        public static void TryComplete(object instance, bool succcessfully)
        {
            Accessors accesors;
            if (AccessorsCache.TryGetValue(instance.GetType(), out accesors) == false || accesors == null)
                return;

            using (var documentSession = accesors.Get(instance))
            {
                if (documentSession == null)
                    return;

                if (succcessfully)
                    documentSession.SaveChanges();
            }
        }
     
        private class Accessors
        {
            public Action<object, IDocumentSession> Set;
            public Func<object, IDocumentSession> Get;
        }
    }
}