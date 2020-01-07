using System.Reflection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace ApplicationBuilderWithBranches {
    /// <summary>
    ///     <inheritdoc cref="ControllerFeatureProvider" />
    /// </summary>
    /// <typeparam name="TController"></typeparam>
    public class TypedControllerFeatureProvider<TController> : ControllerFeatureProvider
        where TController : ControllerBase {
        /// <summary>
        ///     <inheritdoc cref="ControllerFeatureProvider.IsController" />
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        protected override bool IsController(TypeInfo typeInfo) {
            if (!typeof(TController).GetTypeInfo().IsAssignableFrom(typeInfo)) return false;
            return base.IsController(typeInfo);
        }
    }
}