using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using RaccoonBlog.Web.Controllers;
using Raven.Client.Documents;

namespace RaccoonBlog.Web
{
    public static class DocumentStoreHolder
    {
        public static IDocumentStore DocumentStore { get; private set; }

        public static void InitializeDocumentStore(IConfiguration configuration)
        {
            if (DocumentStore != null) 
                return; // prevent misuse

            var connection = configuration.GetSection("RavenConnection");
            var urls = connection.GetSection("Urls").Get<string[]>();
            var database = connection.GetSection("Database").Value;
            var documentStore = new DocumentStore
            {
                Urls = urls,
                Database = database,
            };

            var certificatePath = connection.GetSection("CertificatePath").Value;
            if (certificatePath != null)
            {
                var certificatePassword = connection.GetSection("CertificatePassword").Value;
                var certificate = new X509Certificate2(certificatePath, certificatePassword);
                documentStore.Certificate = certificate;
            }

            RaccoonController.DocumentStore = DocumentStore = documentStore.Initialize();

            // TryCreatingIndexesOrRedirectToErrorPage();
        }
    }
}