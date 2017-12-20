using System;
using System.Web.Hosting;
using FluentScheduler;
using RaccoonBlog.Web.Controllers;
using RaccoonBlog.Web.Services;
using Raven.Client.Documents;

namespace RaccoonBlog.Web.Infrastructure.Jobs
{
    public class RedditIntegration : IJob, IRegisteredObject
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        private readonly object _lock = new object();

        private bool _shuttingDown;

        private readonly IDocumentStore _documentStore;

        public RedditIntegration()
        {
            _documentStore = RaccoonController.DocumentStore;
            HostingEnvironment.RegisterObject(this);
        }

        public void Execute()
        {
            _log.Info("Started execution Reddit integration job.");

            lock (_lock)
            {
                if (_shuttingDown)
                    return;

                using (var session = _documentStore.OpenSession())
                {
                    var submitToReddit = new SubmitToRedditStrategy(session);
                    submitToReddit.SubmitPostsToReddit(DateTimeOffset.UtcNow);

                    session.SaveChanges();
                }
            }

            _log.Info("Finished execution Reddit integration job.");
        }


        public void Stop(bool immediate)
        {
            lock (_lock)
            {
                _shuttingDown = true;
            }

            HostingEnvironment.UnregisterObject(this);
        }
    }
}