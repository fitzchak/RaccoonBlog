using Raven.Client;
using Rhino.Mocks;

namespace RaccoonBlog.IntegrationTests.Web.Controllers.RaccoonControllerTests.BlogConfigProperty
{
    public abstract class WhenTestingTheProperty : WhenTestingTheClass
    {
        protected IDocumentSession DocumentSession { get; set; }
        protected ISyncAdvancedSessionOperation SyncAdvancedSessionOperation { get; set; }
        protected IDocumentStore DocumentStore { get; set; }

        public WhenTestingTheProperty()
            : base()
        {
            DocumentSession = MockRepository.GenerateMock<IDocumentSession>();
            SyncAdvancedSessionOperation = MockRepository.GenerateMock<ISyncAdvancedSessionOperation>();
            DocumentStore = MockRepository.GenerateMock<IDocumentStore>();

            DocumentSession.Stub(ds => ds.Advanced).Return(SyncAdvancedSessionOperation);
            SyncAdvancedSessionOperation.Stub(saso => saso.DocumentStore).Return(DocumentStore);

            Controller.SetRavenSession(DocumentSession);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
