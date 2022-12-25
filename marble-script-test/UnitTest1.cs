using NuGet.Frameworks;

namespace Marble.Test;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Assert.That(Calculator.Add(1, 3), Is.EqualTo(4));
        Assert.That(Calculator.Subtract(5, 2), Is.EqualTo(3));
    }
}