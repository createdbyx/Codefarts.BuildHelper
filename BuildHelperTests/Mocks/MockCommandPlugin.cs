using Codefarts.BuildHelper;

namespace BuildHelperTests.Mocks
{
    public class MockCommandPlugin : ICommandPlugin
    {
        private string name;

        public MockCommandPlugin()
        {
        }

        public MockCommandPlugin(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public void Run(RunCommandArgs args)
        {
        }
    }
}