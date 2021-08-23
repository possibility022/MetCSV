using System;
using System.Linq;
using System.Reflection;

namespace MET.Domain.Logic.GroupsActionExecutors
{
    public class OverrideDefaultValuesDomain : IFinalProductConstructor
    {
        private static PropertyInfo[] _properties = typeof(Product)
            .GetProperties()
            .Where(r => r.CanWrite)
            .Where(r => r.Name != "ID" && r.Name != "StatusProduktu" && r.Name != "StanMagazynowy" && r.Name != "Provider" && r.Name != "Hidden") // Ignore those
            .ToArray();

        public void ExecuteAction(Product source, Product final)
        {
            for (int i = 0; i < _properties.Length; i++)
            {
                var p = _properties[i];

                var from = p.GetValue(source);
                var to = p.GetValue(final);

                if (IsDefault(p.PropertyType, to))
                {
                    p.SetValue(final, from);
                }
            }
        }

        private static readonly Type IntType = typeof(int);
        private static readonly Type DoubleType = typeof(double);
        private static readonly Type BoolType = typeof(bool);
        private static readonly Type ProvidersType = typeof(Providers);
        private static readonly Type StringType = typeof(string);

        private bool IsDefault(Type type, object value)
        {
            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (value == null)
                    return true;

                return HandleNullable(type, value);
            }

            if (type == IntType)
            {
                return (int)value == -1;
            }
            else if (type == StringType)
            {
                var v = (string)value;
                return v is null or "";
            }
            else if (type == BoolType)
            {
                throw new InvalidOperationException(); // We should not work on bool as default value can be correct.
            }
            else if (type == DoubleType)
            {
                return (double)value == default;
            }
            else if (type == ProvidersType)
            {
                return (Providers)value == Providers.None;
            }
            else
            {
                throw new ArgumentOutOfRangeException("type", $"Type is not supported/implemented. {type.FullName}.");
            }
        }

        private bool HandleNullable(Type type, object value)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);

            return IsDefault(underlyingType, value);
        }
    }
}
