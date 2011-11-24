using DataAnnotationsExtensions.ClientValidation;

[assembly: WebActivator.PreApplicationStartMethod(typeof(HibernatingRhinos.Loci.Common.App_Start.RegisterClientValidationExtensions), "Start")]
 
namespace HibernatingRhinos.Loci.Common.App_Start {
    public static class RegisterClientValidationExtensions {
        public static void Start() {
            DataAnnotationsModelValidatorProviderExtensions.RegisterValidationExtensions();            
        }
    }
}