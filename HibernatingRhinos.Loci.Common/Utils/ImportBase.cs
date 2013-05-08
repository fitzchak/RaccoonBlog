using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;

namespace HibernatingRhinos.Loci.Common.Utils
{
    public class ImportBase
    {
        protected IDocumentStore GetDocumentStore(bool isTest)
        {
            return isTest ? GetInMemoryDocumentStore() : GetDocumentStore();
        }

        protected DocumentStore GetInMemoryDocumentStore()
        {
            var documentStore = new EmbeddableDocumentStore
                {
                    RunInMemory = true
                };

            documentStore.RegisterListener(new NoStaleQueriesAllowed());
            return documentStore;
        }

        protected DocumentStore GetDocumentStore()
        {
            return new DocumentStore
                {
                    Url = "http://localhost:8080",
                    DefaultDatabase = "RaccoonBlog"
                };
        }
    }
}