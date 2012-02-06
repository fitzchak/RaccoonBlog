using System.Linq;
using System.Web.Mvc;

namespace RaccoonBlog.Web.Helpers
{
	public static class ModelStateExtensions
	{
		public static string FirstErrorMessage(this ModelStateDictionary modelState)
		{
			var state = modelState.Values.FirstOrDefault(v => v.Errors.Count > 0);

			if (state == null) return null;

			var message = state.Errors
				.Where(error => string.IsNullOrEmpty(error.ErrorMessage) == false)
				.Select(error => error.ErrorMessage)
				.FirstOrDefault();
			return message;
		}
	}
}