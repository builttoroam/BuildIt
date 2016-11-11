#if NET45

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace BuildIt.Web.Utitlites
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomControllerFactory : DefaultControllerFactory
    {
        private readonly Dictionary<string, Type> controllerTypesForCustomActivation;
        private readonly Func<Type, object[]> retrieveControllerConstructorParameters;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controllerTypesForCustomActivation"></param>
        /// <param name="retrieveControllerConstructorParameters"></param>
        public CustomControllerFactory(IEnumerable<Type> controllerTypesForCustomActivation, Func<Type, object[]> retrieveControllerConstructorParameters)
        {
            this.controllerTypesForCustomActivation = controllerTypesForCustomActivation?.Where(c => c != null).ToDictionary(c => c.Name);
            this.retrieveControllerConstructorParameters = retrieveControllerConstructorParameters;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="controllerName"></param>
        /// <returns></returns>
        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            var controllerTypeName = $"{controllerName}{nameof(Controller)}";
            if (controllerTypesForCustomActivation.ContainsKey(controllerTypeName))
            {
                try
                {
                    var controllerType = controllerTypesForCustomActivation[controllerTypeName];
                    var constructorParameters = retrieveControllerConstructorParameters?.Invoke(controllerType);
                    return (IController)Activator.CreateInstance(controllerTypesForCustomActivation[controllerTypeName], constructorParameters);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            return base.CreateController(requestContext, controllerName);
        }
    }
}

#endif