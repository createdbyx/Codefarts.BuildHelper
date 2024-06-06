using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Codefarts.AppCore;
using Codefarts.WPFCommon.Commands;
using WpfBuilderIDE.Models;

namespace WpfBuilderIDE.ViewModels;

public class MainVM : PropertyChangedBase
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private ICommand newFileCommand;
    private Application application = new Application();

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
            return new DelegateCommand(_ => true, _ => this.Application.BuildScripts.Add(new BuildScript()));
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
        OnPropertyChanged(propertyName);
        return true;
    }
}