using Dapper;

namespace CleanArchitecture.Application.Common.Mappings;
public class DynamicParameterMap
{
    public DynamicParameters Map<T>(T Data) where T : class
    {
        DynamicParameters parameters = new DynamicParameters();
        var typeData = Data.GetType();
        foreach (var info in typeData.GetProperties())
        {
            parameters.Add($"@{info.Name}", info.GetValue(Data, null));
        }
        return parameters;
    }

    public DynamicParameters MapUnderScore<T>(T Data) where T : class
    {
        DynamicParameters parameters = new DynamicParameters();
        var typeData = Data.GetType();
        foreach (var info in typeData.GetProperties())
        {
            string varName = CamelcaseToUnderscore(info.Name);
            parameters.Add($"@{varName}", info.GetValue(Data, null));
        }
        return parameters;
    }

    private string CamelcaseToUnderscore(string value)
    {
        string result = string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()));

        return result.ToLower();
    }

    public DynamicParameters Map(Dictionary<string, object> data)
    {
        DynamicParameters parameters = new DynamicParameters();
        foreach (var pair in data)
        {
            parameters.Add($"@{pair.Key}", pair.Value);
        }
        return parameters;
    }
}
