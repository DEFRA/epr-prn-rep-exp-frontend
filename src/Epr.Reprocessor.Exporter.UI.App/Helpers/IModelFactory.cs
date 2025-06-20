namespace Epr.Reprocessor.Exporter.UI.App.Helpers;

/// <summary>
/// Factory to create model instances in a unit testable way. 
/// </summary>
/// <remarks>Use this in a controller action method specifically a Get method where you need to set up the model but as we new it up directly we can't test it the normal way.
/// By using this factory you can then mock out the creation of the model and return your own instance in a unit test giving greater control.</remarks>
/// <typeparam name="T">The generic type parameter of the type to create.</typeparam>
public interface IModelFactory<out T> where T : class, new()
{
    [ExcludeFromCodeCoverage]
    T Instance => new T();
}

[ExcludeFromCodeCoverage]
public class ModelFactory<T> : IModelFactory<T> where T : class, new()
{
}