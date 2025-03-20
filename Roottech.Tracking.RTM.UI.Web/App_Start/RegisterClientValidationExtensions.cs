using DataAnnotationsExtensions.ClientValidation;
using Roottech.Tracking.RTM.UI.Web.App_Start;
using WebActivator;

[assembly: PreApplicationStartMethod(typeof (RegisterClientValidationExtensions), "Start")]

namespace Roottech.Tracking.RTM.UI.Web.App_Start
{
    public static class RegisterClientValidationExtensions
    {
        public static void Start()
        {
            DataAnnotationsModelValidatorProviderExtensions.RegisterValidationExtensions();
        }
    }
}