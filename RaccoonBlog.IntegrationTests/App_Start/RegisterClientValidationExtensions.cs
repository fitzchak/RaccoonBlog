using DataAnnotationsExtensions.ClientValidation;

[assembly: WebActivator.PreApplicationStartMethod(typeof(RaccoonBlog.IntegrationTests.App_Start.RegisterClientValidationExtensions), "Start")]
 
namespace RaccoonBlog.IntegrationTests.App_Start {
    public static class RegisterClientValidationExtensions {
        public static void Start() {
            DataAnnotationsModelValidatorProviderExtensions.RegisterValidationExtensions();            
        }
    }
}