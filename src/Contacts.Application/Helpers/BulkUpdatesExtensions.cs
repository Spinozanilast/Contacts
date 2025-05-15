using System.Data.SqlTypes;
using Microsoft.EntityFrameworkCore.Query;

namespace Contacts.Application.Helpers;

public static class BulkUpdatesExtensions
{
    public static SetPropertyCalls<TSource> SetIfNotNull<TSource, TProperty>(
        this SetPropertyCalls<TSource> setters,
        Func<TSource, TProperty> propertyExpression,
        TProperty? value)
    {
        return value is null ? setters : setters.SetProperty(propertyExpression, value);
    }
}