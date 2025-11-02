using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiServer.Services;
using System.Collections;
using Moq;

namespace TestProject1
{
    public class ClassToTest_Tester
    {

        [Theory]
        [MemberData(nameof(testClass101.getSimpleData), MemberType = typeof(testClass101))]
        public void addNumbers_simpleAdd(int num1, int num2, int expected)
        {
            int addtionalNumber = 5;
            var moqclass =  new Mock<IClassToTAdd>();
            moqclass.Setup(m => m.getAdditionalNumber()).Returns(addtionalNumber);

            ClassToTest ctt = new ClassToTest(moqclass.Object);
            int result = ctt.addNumberWithDependency(num1, num2);
            Assert.Equal(expected+addtionalNumber, result);
        }   


    }


    public class testClass101
    {
        public static IEnumerable<object[]> getSimpleData()
        {
            yield return new object[] { 1, 2, 3 };
            yield return new object[] { 4, 5, 9 };
            yield return new object[] { 10, 15, 25 };
        }


    }
        

}
