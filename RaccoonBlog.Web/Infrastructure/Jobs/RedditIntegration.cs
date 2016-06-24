using System;
using System.Web;
using System.Web.Hosting;
using FluentScheduler;
using HibernatingRhinos.Loci.Common.Controllers;
using RaccoonBlog.Web.Helpers;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.Services;
using Raven.Client;

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
            _documentStore = RavenController.DocumentStore;
            HostingEnvironment.RegisterObject(this);
        }

        public void Execute()
        {
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