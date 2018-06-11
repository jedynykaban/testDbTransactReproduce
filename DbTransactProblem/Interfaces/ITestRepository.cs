namespace DbTransactProblem.Interfaces
{
    public interface ITestRepository
    {
        TestViewModel AddOrUpdate(TestViewModel testVmEntity);
        TestViewModel GetFirst();
    }
}
