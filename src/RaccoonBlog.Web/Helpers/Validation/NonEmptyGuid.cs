using System;
using System.ComponentModel.DataAnnotations;

namespace RaccoonBlog.Web.Helpers.Validation
{
	public class NonEmptyGuidAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value == null)
				return ValidationResult.Success;

			if (value is string == false && value is Guid == false)
				return ValidationResult.Success;

			Guid guid = value is Guid ? (Guid) value : Guid.Empty;
			if (value is string)
			{
				if (Guid.TryParse((string)value, out guid) == false)
					return new ValidationResult("The value is not compatible with Guid type");
			}

			if (guid == Guid.Empty)
				return new ValidationResult("Value cannot be empty");

			return ValidationResult.Success;
		}
	}
}
