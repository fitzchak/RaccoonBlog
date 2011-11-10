using System;
using System.Web.Mvc;

namespace RaccoonBlog.Web.Helpers.Binders
{
	public class GuidBinder : DefaultModelBinder
	{
		public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			if (value == null)
				return Guid.Empty;

			Guid guid;
			if (Guid.TryParse(value.AttemptedValue, out guid) == false)
				return Guid.Empty;

			return guid;
		}
	}
}
