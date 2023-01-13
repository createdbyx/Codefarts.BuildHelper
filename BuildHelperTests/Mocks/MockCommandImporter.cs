using Codefarts.BuildHelper;

namespace BuildHelperTests.Mocks;

public class MockSuccsesfulCommandImporter : ICommandImporter
{
    public RunResult Run()
    {
        return RunResult.Sucessful();
    }
}