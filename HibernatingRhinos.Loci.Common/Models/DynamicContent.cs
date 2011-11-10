namespace HibernatingRhinos.Loci.Common.Models
{
	public enum DynamicContentType
	{
		Markdown,
		Html,
		Video,
		HttpRedirection,
	}

	public interface IDynamicContent
	{
		string Content { get; set; }
		DynamicContentType ContentType { get; set; }
	}

	public interface ISearchable
	{
		string Slug { get; set; }
		string Title { get; set; }
		string Content { get; set; }
	}
}
