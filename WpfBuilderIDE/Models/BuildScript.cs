using System.Collections.ObjectModel;
using Codefarts.AppCore;
using Codefarts.BuildHelper;

namespace WpfBuilderIDE.Models;

public class BuildScript : PropertyChangedBase
{
    private PluginCollection commands = new PluginCollection();
    private string buildFile;
    private bool isDirty;

    public bool IsDirty
    {
        get
        {
            return this.isDirty;
        }

        private set
        {
            var currentValue = this.isDirty;
            if (currentValue == value)
            {
                return;
            }

            this.isDirty = value;
            this.NotifyOfPropertyChange(nameof(this.IsDirty));
        }
    }

    public BuildScript()
    {
        this.Commands.CollectionChanged += (s, e) => this.IsDirty = true;
    }

    public override string ToString()
    {
        return string.IsNullOrWhiteSpace(this.buildFile) ? "Untitled" : this.buildFile;
    }

    public string BuildFile
    {
        get
        {
            return this.buildFile;
        }

        set
        {
            var currentValue = this.buildFile;
            if (currentValue == value)
            {
                return;
            }

            this.buildFile = value;
            this.NotifyOfPropertyChange(nameof(this.BuildFile));
        }
    }

    public PluginCollection Commands
    {
        get
        {
            return this.commands;
        }

        private set
        {
            var currentValue = this.commands;
            if (currentValue == value)
            {
                return;
            }

            this.commands = value;
            this.NotifyOfPropertyChange(nameof(this.Commands));
        }
    }
}