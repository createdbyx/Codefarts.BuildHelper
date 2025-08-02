using Codefarts.DependencyInjection.CodefartsIoc;
using Codefarts.IoC;
using Codefarts.WpfAppBootstrapper;

namespace WpfBuilderIDE;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var container = new Container();
        var shim = new DependencyInjectorShim(container);
        var app = new BootstrappedApp(shim, "Main");
        app.Run();
    }
}