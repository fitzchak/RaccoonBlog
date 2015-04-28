using Microsoft.Owin;

using Owin;

using RaccoonBlog.Web;

[assembly: OwinStartup(typeof(Startup))]
namespace RaccoonBlog.Web
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			ConfigureAuth(app);
		}
	}
}