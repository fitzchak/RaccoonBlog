using System;
using System.Collections.Concurrent;
using System.Web.Mvc;
using Raven.Client;
using Raven.Client.Document;
using System.Linq;

namespace RavenDbBlog.Infrastructure
{
    /// <summary>
    /// This filter will manage the session for all of the controllers that needs a Raven Document Session.
    /// It does so by automatically injecting a session to the first public property of type IDocumentSession available
    /// on the controller.
    /// </summary>
    /// <remarks>
    /// We intentionally do not use an IoC container here, because that would add additional complexity to the solution, which 
    /// isn't warranted at this time.
    /// </remarks>
    public class RavenActionFilterAttribute : ActionFilterAttribute
    {
        private static readonly IDocumentStore DocumentStore = CreateDocumentStore();

        private readonly ConcurrentDictionary<Type, Tuple<Action<ControllerBase, IDocumentSession>, Func<ControllerBase, IDocumentSession>>> _accessorsCache = new ConcurrentDictionary<Type, Tuple<Action<ControllerBase, IDocumentSession>, Func<ControllerBase, IDocumentSession>>>();

        private static IDocumentStore CreateDocumentStore()
        {
            return new DocumentStore
                       {
                           Url = "http://localhost:8080"
                       }.Initialize();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var accessors = _accessorsCache.GetOrAdd(filterContext.Controller.GetType(), CreateAccessorsForType);
            
            if(accessors == null)
                return;

            accessors.Item1(filterContext.Controller, DocumentStore.OpenSession());
        }

        private static Tuple<Action<ControllerBase, IDocumentSession>, Func<ControllerBase, IDocumentSession>> CreateAccessorsForType(Type type)
        {
            var sessionProp =
                type.GetProperties().FirstOrDefault(
                    x => x.PropertyType == typeof (IDocumentSession) && x.CanRead && x.CanWrite);
            if (sessionProp == null)
                return null;

            return new Tuple<Action<ControllerBase, IDocumentSession>, Func<ControllerBase, IDocumentSession>>(
                (controller, session) => sessionProp.SetValue(controller, session, null),
                controller => (IDocumentSession)sessionProp.GetValue(controller, null)
                );
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            Tuple<Action<ControllerBase, IDocumentSession>, Func<ControllerBase, IDocumentSession>> accesors;
            if (_accessorsCache.TryGetValue(filterContext.Controller.GetType(), out accesors) == false || accesors == null)
                return;

            using (var documentSession = accesors.Item2(filterContext.Controller))
            {
                if (documentSession == null)
                    return;

                if (filterContext.Exception != null)
                    documentSession.SaveChanges();
            }
        }
    }
}