namespace Codefarts.BuildHelper
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    public interface IBuildCommand
    {
        string Name
        {
            get;
        }

        void Execute(IDictionary<string, string> variables, XElement data);
    }
}