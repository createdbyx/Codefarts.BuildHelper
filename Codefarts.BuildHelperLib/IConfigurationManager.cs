namespace Codefarts.BuildHelper;

public interface IConfigurationManager
{
    object GetValue(string name);
}