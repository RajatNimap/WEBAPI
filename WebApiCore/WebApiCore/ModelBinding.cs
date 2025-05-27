using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApiCore
{
    public class ModelBinding : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var Data = bindingContext.HttpContext.Request.Query;
            var result =Data.TryGetValue("name", out var data);
            if (result) { 
                 var array=data.ToString().Split('|');    
                 bindingContext.Result=ModelBindingResult.Success(array);
            }

            return Task.CompletedTask;
        }
    }
}
