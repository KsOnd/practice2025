using System;
using System.Reflection;
using System.Collections.Generic;

namespace task05;

public class ClassAnalyzer
{
    private const BindingFlags AllMethods = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    private Type _type;
    public IEnumerable<string> GetPublicMethods() => _type.GetMethods(AllMethods)
                                                          .Where(m => m.IsPublic)
                                                          .Select(m => m.Name);

    public IEnumerable<string> GetMethodParams(string methodName) 
    => _type.GetMethods(AllMethods)
            .Where(method => method.Name == methodName)
            .SelectMany(method => 
                method.GetParameters()
                      .Select(parameter => $"{parameter.ParameterType.Name} {parameter.Name}")
                      .Prepend($"{method.ReturnType.Name}")
            );

    public IEnumerable<string> GetAllFields() => _type.GetFields(AllMethods).Select(field => field.Name);

    public IEnumerable<string> GetProperties() => _type.GetProperties(AllMethods).Select(property => property.Name);

    public bool HasAttribute<T>() where T : Attribute => _type.GetCustomAttribute<T>() != null;

    public ClassAnalyzer(Type type)
    {
        _type = type;
    }
}
