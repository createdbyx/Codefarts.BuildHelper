using Codefarts.BuildHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildHelperTests
{
    [TestClass]
    public class BuildHelperTests
    {
        [TestMethod]
        public void Instanciate()
        {
            var build = new BuildHelper();
        }
    }
}