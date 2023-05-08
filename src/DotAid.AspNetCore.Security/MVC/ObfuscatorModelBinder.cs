using DotAid.AspNetCore.Security.Obfuscator;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DotAid.AspNetCore.Security.MVC;

public class ObfuscatorModelBinder : IModelBinder
{
    private IObfuscator Obfuscator { get; }

    public ObfuscatorModelBinder(IObfuscator obfuscator)
    {
        Obfuscator = obfuscator ?? throw new ArgumentNullException(nameof(obfuscator));
    }

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        var modelName = bindingContext.ModelName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);
        var value = valueProviderResult.FirstValue;
        if (string.IsNullOrEmpty(value))
        {
            return Task.CompletedTask;
        }
        
        if (bindingContext.ModelType.IsAssignableTo(typeof(HashedId<long>)))
        {
            SetBindingContextForLong(bindingContext, value);
        }
        else if (bindingContext.ModelType == typeof(HashedId<int>))
        {
            SetBindingContextForInt(bindingContext, value);
        }
        
        return Task.CompletedTask;
    }

    private void SetBindingContextForInt(ModelBindingContext bindingContext, string value)
    {
        if (Obfuscator.TryDecode(value, out int id))
        {
            bindingContext.Result = bindingContext.ModelType == typeof(int)
                ? ModelBindingResult.Success(id)
                : ModelBindingResult.Success((HashedId<int>)id);
        }
        else
        {
            bindingContext.Result = ModelBindingResult.Failed();
        }
    }

    private void SetBindingContextForLong(ModelBindingContext modelBindingContext, string value)
    {
        if (Obfuscator.TryDecode(value, out long id))
        {
            modelBindingContext.Result = modelBindingContext.ModelType == typeof(long)
                ? ModelBindingResult.Success(id)
                : ModelBindingResult.Success((HashedId<long>)id);
        }
        else
        {
            modelBindingContext.Result = ModelBindingResult.Failed();
        }
    }
}