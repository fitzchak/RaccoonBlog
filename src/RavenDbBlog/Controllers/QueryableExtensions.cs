using System.Linq;

namespace RavenDbBlog.Controllers
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Paging<T>(this IQueryable<T> query, int currentPage, int defaultPage, int pageSize)
        {
            return query
                .Skip((currentPage - defaultPage)*pageSize)
                .Take(pageSize);
        }
    }
}