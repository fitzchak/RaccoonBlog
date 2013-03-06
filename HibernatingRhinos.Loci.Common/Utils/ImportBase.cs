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

        protected void CreateSections(IDocumentStore store)
        {
            using (var s = store.OpenSession())
            {
                s.Store(new Section { Title = "Archive", IsActive = true, Position = 1, ControllerName = "Section", ActionName = "ArchivesList" });
                s.Store(new Section { Title = "Tags", IsActive = true, Position = 2, ControllerName = "Section", ActionName = "TagsList" });
                s.Store(new Section { Title = "Statistics", IsActive = true, Position = 3, ControllerName = "Section", ActionName = "PostsStatistics" });
                s.Store(new Section { Title = "Future Posts", IsActive = true, Position = 4, ControllerName = "Section", ActionName = "FuturePosts" });
                s.SaveChanges();
            }
        }
    }
}