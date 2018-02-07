using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Documents.Session;

namespace RaccoonBlog
{
    public static class DocumentStoreHolder
    {
        private static IDocumentStore _store;

        private static IDocumentStore Store
        {
            get
            {
                if (_store == null)
                    throw new InvalidOperationException("RavenDB is not reachable!");
                return _store;
            }
            set => _store = value;
        }

        public static void Initialize(IConfiguration configuration)
        {
            var ravendb = configuration.GetSection("RavenDB");
            var urls = ravendb.GetSection("Urls").Value;
            var database = ravendb.GetSection("Database").Value;
            var store = new DocumentStore
            {
                Urls = urls.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries),
                Database = database,
            };

            var certificatePath = ravendb.GetSection("CertificatePath").Value;
            if (certificatePath != null)
            {
                var certificatePassword = ravendb.GetSection("CertificatePassword").Value;
                var certificate = new X509Certificate2(certificatePath, certificatePassword);
                store.Certificate = certificate;
            }

            var requestsTimeout = ravendb.GetSection("RequestsTimeoutInSec").Value;
            if (requestsTimeout != null)
            {
                if (int.TryParse(requestsTimeout, out int seconds) == false)
                    throw new InvalidOperationException("RequestsTimeoutInSec should be a number");
                store.Conventions.RequestTimeout = TimeSpan.FromSeconds(seconds);
            }

            Store = store.Initialize();

            DeployIndexes();
        }

        private static void DeployIndexes()
        {
        }

        public static void Shutdown()
        {
            _store?.Dispose();
        }

        public static IAsyncDocumentSession OpenAsyncSession()
        {
            return Store.OpenAsyncSession();
        }

        public static OperationExecutor Operations()
        {
            return Store.Operations;
        }
    }
}