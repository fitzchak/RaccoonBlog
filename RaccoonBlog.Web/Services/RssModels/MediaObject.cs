using CookComputing.XmlRpc;

namespace RaccoonBlog.Web.Services.RssModels
{
	[XmlRpcMissingMapping(MappingAction.Ignore)]
	public struct MediaObject
	{
		public string name;
		public string type;
		public byte[] bits;
	}
}
