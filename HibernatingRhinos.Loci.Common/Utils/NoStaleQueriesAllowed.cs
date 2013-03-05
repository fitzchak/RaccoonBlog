using Raven.Client;
using Raven.Client.Listeners;

namespace HibernatingRhinos.Loci.Common.Utils
{
    public class NoStaleQueriesAllowed : IDocumentQueryListener
    {
        public void BeforeQueryExecuted(IDocumentQueryCustomization queryCustomization)
        {
            queryCustomization.WaitForNonStaleResults();
        }
    }
}