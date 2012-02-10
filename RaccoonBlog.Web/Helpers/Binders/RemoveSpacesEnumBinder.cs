using System.Web.Mvc;

namespace RaccoonBlog.Web.Helpers.Binders
{
	public class RemoveSpacesEnumBinder : DefaultModelBinder
	{
		public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			if (value == null)
				return base.BindModel(controllerContext, bindingContext);

			var values = value.RawValue as string[];
			if (values == null)
				return base.BindModel(controllerContext, bindingContext);

			for (int i = 0; i < values.Length; i++)
			{
				values[i] = values[i].Replace(" ", string.Empty);
			}
			return base.BindModel(controllerContext, bindingContext);
		}
	}
}