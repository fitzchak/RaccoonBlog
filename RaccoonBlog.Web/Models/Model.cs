using System.Web.Mvc;
using Newtonsoft.Json;
using RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers;

namespace RaccoonBlog.Web.Models
{
	public class Model
	{
		[HiddenInput]
		public string Id { get; set; }

		[JsonIgnore]
		public int DenormalizedId
		{
			get { return RavenIdResolver.Resolve(Id); }
		}
	}
}