using Xunit;
using task05;

namespace task05tests;

public class TestClass
{
    public int PublicField;
    private string _privateField;
    public int Property { get; set; }
    public void Method() { }
    public int ComplexMethod(string name, int age) => 0;
}

[Serializable]
public class AttributedClass { }

public class NonAttributedClass { }

public class ClassAnalyzerTests
{
    [Fact]
    public void GetPublicMethods_ReturnsCorrectMethods()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var methods = analyzer.GetPublicMethods();

        Assert.Contains("Method", methods);
        Assert.Contains("ComplexMethod", methods);
        Assert.Contains("get_Property", methods); 
    }

    [Fact]
    public void GetAllFields_IncludesPrivateFieldsAndBackingFields()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var fields = analyzer.GetAllFields().ToList();

        Assert.Contains("PublicField", fields);
        Assert.Contains("_privateField", fields);
        Assert.Contains(fields, f => f.Contains("BackingField"));
    }

    // --- НОВЫЕ ТЕСТЫ ---

    [Fact]
    public void GetProperties_ReturnsDeclaredProperties()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));

        var properties = analyzer.GetProperties();

        Assert.Contains("Property", properties);
    }

    [Fact]
    public void GetMethodParams_ReturnsCorrectSignature_WithReturnAndArgs()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));

        var result = analyzer.GetMethodParams("ComplexMethod").ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal("Int32", result[0]);
        Assert.Equal("String name", result[1]);
        Assert.Equal("Int32 age", result[2]);
    }

    [Fact]
    public void HasAttribute_ReturnsTrue_WhenAttributeIsPresent()
    {
        var analyzer = new ClassAnalyzer(typeof(AttributedClass));

        var hasAttribute = analyzer.HasAttribute<SerializableAttribute>();

        Assert.True(hasAttribute);
    }

    [Fact]
    public void HasAttribute_ReturnsFalse_WhenAttributeIsMissing()
    {
        var analyzer = new ClassAnalyzer(typeof(NonAttributedClass));

        var hasAttribute = analyzer.HasAttribute<SerializableAttribute>();

        Assert.False(hasAttribute);
    }
}
