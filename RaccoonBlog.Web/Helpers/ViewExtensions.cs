namespace RaccoonBlog.Web.Helpers
{
	public static class ViewExtensions
	{
		public static bool IsDebug()
		{
#if DEBUG
			return true;
#else
			return false;
#endif
		}
	}
}