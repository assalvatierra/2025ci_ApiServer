using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiServer.Services;
using System.Collections;
using Moq;
using Xunit.Abstractions;

namespace TestProject1
{
    [Collection("Test Collection")]
    public class ClassToTest_Tester: IDisposable
    {
        private readonly ITestOutputHelper output;
        private testClass101 fixture;

        public ClassToTest_Tester(ITestOutputHelper output, testClass101 fixture)
        {
            this.output = output;
            this.fixture = fixture;
        }

        public void Dispose()
        {
            // Cleanup if needed
        }


        [Theory]
        [MemberData(nameof(testClass101.getSimpleData), MemberType = typeof(testClass101))]
        public void addNumbers_simpleAdd(int num1, int num2, int expected)
        {
            int addtionalNumber = this.fixture.additionalNumber;
            var moqclass =  new Mock<IClassToTAdd>();
            moqclass.Setup(m => m.getAdditionalNumber()).Returns(addtionalNumber);

            ClassToTest ctt = new ClassToTest(moqclass.Object);
            int result = ctt.addNumberWithDependency(num1, num2);
            Assert.Equal(expected+addtionalNumber, result);

            this.output.WriteLine($"the result is {result}. Expecting: {expected}");
        }

        [Fact]
        public void getAdditionalNumber()
        {
            Assert.NotEqual(this.fixture.additionalNumber, 0);
        }

    }


    [CollectionDefinition("Test Collection")]
    public class TestCollection : ICollectionFixture<testClass101> { }


    public class testClass101
    {
        public int additionalNumber = 0;

        public testClass101()
        {
            Random rnd = new Random();
            this.additionalNumber = rnd.Next(1, 10);

        }

        public static IEnumerable<object[]> getSimpleData()
        {
            yield return new object[] { 1, 2, 3 };
            yield return new object[] { 4, 5, 9 };
            yield return new object[] { 10, 15, 25 };
        }


    }


}
