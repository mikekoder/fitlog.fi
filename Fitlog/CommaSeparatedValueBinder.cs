using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitlog.Web
{
    public class CommaSeparatedValueBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var fieldName = bindingContext.FieldName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(fieldName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(fieldName, valueProviderResult);

            var model = new List<T>();

            foreach (string delimitedString in valueProviderResult.Values)
            {
                var splitValues = delimitedString
                    .Split(',')
                    .Cast<string>();
                IEnumerable<T> convertedValues;

                if(typeof(T) == typeof(Guid))
                {
                    convertedValues = splitValues
                    .Select(s => Guid.Parse(s))
                    .Cast<T>();
                }
                else
                {
                    convertedValues = splitValues
                    .Select(s => Convert.ChangeType(s, typeof(T)))
                    .Cast<T>();
                }

                model.AddRange(convertedValues);
            }

            bindingContext.Result = ModelBindingResult.Success(model.ToArray<T>());

            return Task.CompletedTask;
        }
    }
}
