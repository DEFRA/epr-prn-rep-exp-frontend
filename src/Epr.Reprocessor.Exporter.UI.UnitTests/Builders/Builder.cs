namespace Epr.Reprocessor.Exporter.UI.UnitTests.Builders;

/// <summary>
/// Defines a generic builder that provides the ability to set properties on an object in a generic manner.
/// </summary>
/// <typeparam name="T">The type that is being acted on.</typeparam>
/// <param name="instance">The object instance to be acted on.</param>
public class Builder<T>(T instance)
    where T : class, new()
{
    #region Fields

    private readonly T _instance = instance;

    #endregion

    #region Main Api

    /// <summary>
    /// Sets the specified property's value.
    /// </summary>
    /// <typeparam name="TProperty">Type parameter for the property.</typeparam>
    /// <param name="propertyAccessor">An expression that defines the property being accessed.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>The builder instance.</returns>
    public Builder<T> Set<TProperty>(Expression<Func<T, TProperty>> propertyAccessor, TProperty value)
    {
        if (propertyAccessor.Body is MemberExpression { Member: PropertyInfo propertyInfo })
        {
            propertyInfo.SetValue(_instance, value);
        }
        else
        {
            throw new ArgumentException("Selector must be a property expression.");
        }

        return this;
    }

    /// <summary>
    /// Build the instance.
    /// </summary>
    /// <returns>The built instance.</returns>
    public T Build() => _instance;

    #endregion
}