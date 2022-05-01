using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace DisneyChallenge.Helpers
{
    public class TypeBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var nombrePropiedad = bindingContext.ModelName;
            var proveedoreDeValores = bindingContext.ValueProvider.GetValue(nombrePropiedad);
            if(proveedoreDeValores == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }
            //Si el valor no es nulo debemos deserializarlo.
            try
            {
                var valorDeserializado = JsonConvert.DeserializeObject<T>(proveedoreDeValores.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(valorDeserializado);
            }
            catch
            {
                //Si la deserializacion falla.
                bindingContext.ModelState.TryAddModelError(nombrePropiedad, "Valor inválido para tipoList<int>");
            }
            return Task.CompletedTask;
        }
    }
}
