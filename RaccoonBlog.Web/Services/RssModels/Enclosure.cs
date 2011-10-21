using CookComputing.XmlRpc;

namespace RaccoonBlog.Web.Services.RssModels
{
	[XmlRpcMissingMapping(MappingAction.Ignore)]
	public struct Enclosure
	{
		public int length;
		public string type;
		public string url;
	}
}
