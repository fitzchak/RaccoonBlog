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

        protected IDocumentStore GetInMemoryDocumentStore()
        {
            var documentStore = new EmbeddableDocumentStore
                {
                    RunInMemory = true
                };

            documentStore.RegisterListener(new NoStaleQueriesAllowed());
            return documentStore;
        }

        protected IDocumentStore GetDocumentStore()
        {
            return new DocumentStore
                {
                    Url = "http://localhost:8080",
                    DefaultDatabase = "RaccoonBlog"
                };
        }
    }
}