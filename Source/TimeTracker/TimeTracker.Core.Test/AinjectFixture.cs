namespace TimeTracker.Core.Test
{
    using NUnit.Framework;
    using TimeTracker.Core.Ioc;

    [TestFixture]
    public class AinjectFixture
    {
        [Test]
        public void RegisterType_ShouldRegisterTypeToDictionaryAndReturnSomeObject()
        {
            // Arrange
            Ainject.Instance.RegisterType<ITestClass>(() => new TestClass());

            // Act
            var returnObject = Ainject.Instance.ResolveType<ITestClass>();

            // Assert
            Assert.IsNotNull(returnObject);
        }

        [Test]
        public void RegisterType_ShouldRegisterTypeToDictionaryAndReturnCorrectType()
        {
            // Arrange
            Ainject.Instance.RegisterType<ITestClass>(() => new TestClass());

            // Act
            var returnObject = Ainject.Instance.ResolveType<ITestClass>();

            // Assert
            Assert.IsInstanceOf<ITestClass>(returnObject);
        }
    }

    internal interface ITestClass
    {
    }

    internal class TestClass : ITestClass
    {
    }
}
