namespace ApiServer.Services
{
    public class ClassToTest
    {
        public IClassToTAdd _classtoadd;
        public ClassToTest(IClassToTAdd classToTAdd)
        {
            this._classtoadd = classToTAdd;
        }
        public int addNumbers(int num1, int num2)
        {
            return num1 + num2;
        }

        public int addNumberWithDependency(int num1, int num2)
        {
            return num1 + num2 + this._classtoadd.getAdditionalNumber();
        }
    }

    public interface IClassToTAdd
    {
        int getAdditionalNumber();
    }

    public class classToAdd: IClassToTAdd
    {
        public int getAdditionalNumber()
        {
            return 10;
        }
    }

}
