public interface ITestService
{
    Task AddTestValueAsync(string value);
}

public class TestService : ITestService
{
    private readonly ITestRepository _repository;

    public TestService(ITestRepository repository)
    {
        _repository = repository;
    }

    public async Task AddTestValueAsync(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be empty");

        await _repository.InsertTestAsync(value);
    }
}
