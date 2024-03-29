﻿using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Ata.Investment.Api
{
    /// <summary>
    /// Makes query attribute mandatory
    /// </summary>
    public class RequiredFromQueryAttribute : FromQueryAttribute, IParameterModelConvention
    {
        /// <inheritdoc />
        public void Apply(ParameterModel parameter)
        {
            if (parameter.Action.Selectors != null && parameter.Action.Selectors.Any())
            {
                parameter.Action.Selectors.Last().ActionConstraints.Add(new RequiredFromQueryActionConstraint(parameter.BindingInfo?.BinderModelName ?? parameter.ParameterName));
            }
        }
    }
}
