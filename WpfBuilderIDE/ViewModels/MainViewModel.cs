using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Codefarts.AppCore;
using Codefarts.DependencyInjection;
using Codefarts.WPFCommon.Commands;
using WpfBuilderIDE.Models;

namespace WpfBuilderIDE.ViewModels;

public class MainViewModel : PropertyChangedBase
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private ICommand newFileCommand;
    private OpenFileCommand openFileCommand;
    private Application application;
    private readonly IDependencyInjectionProvider ioc;

    public MainViewModel(IDependencyInjectionProvider ioc)
    {
        this.ioc = ioc ?? throw new ArgumentNullException(nameof(ioc));
        this.Application = ioc.Resolve<Application>();
    }

    public string WindowTitle
    {
        get => AppDomain.CurrentDomain.FriendlyName;
    }
    
    public Application Application
    {
        get
        {
            return this.application;
        }

        set
        {
            var currentValue = this.application;
            if (currentValue == value)
            {
                return;
            }

            this.application = value;
            this.NotifyOfPropertyChange(nameof(this.Application));
        }
    }


    public ICommand NewFileCommand
    {
        get
        {
            if (this.newFileCommand == null)
            {
                this.newFileCommand = new DelegateCommand(_ => true, _ => this.Application.BuildScripts.Add(this.ioc.Resolve<BuildScript>()));
            }

            return this.newFileCommand;
        }
    }

    public ICommand OpenFileCommand
    {
        get
        {
            if (this.openFileCommand == null)
            {
                this.openFileCommand = new OpenFileCommand(file =>
                {
                    var script = this.ioc.Resolve<BuildScript>();
                    script.BuildFile = file;
                    this.Application.BuildScripts.Add(script);
                }, "*.xml");

                this.openFileCommand.ExpectsOwnerWindow = true;
            }

            return this.openFileCommand;
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        this.OnPropertyChanged(propertyName);
        return true;
    }
}