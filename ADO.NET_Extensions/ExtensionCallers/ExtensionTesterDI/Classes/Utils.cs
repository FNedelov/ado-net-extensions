using System.Data.Common;

namespace ExtensionTesterDI.Classes
{
    internal class Utils
    {
        public static List<T> ConvertToGenericSqlParams<T>(List<DbParameter>? parameters) where T : DbParameter, new()
        {
            List<T> convertedParams = new();
            if (parameters != default)
            {
                parameters.ForEach(x => convertedParams.Add((T)Activator.CreateInstance(typeof(T), new object[] { x.ParameterName, x.Value })));
            }
            return convertedParams;
        }
    }
}