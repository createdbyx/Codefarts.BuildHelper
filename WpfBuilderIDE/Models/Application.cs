using Codefarts.AppCore;
using WpfBuilderIDE.Models.Collections;

namespace WpfBuilderIDE.Models;

public class Application : PropertyChangedBase
{
    private BuildScriptColleciton buildScripts = new BuildScriptColleciton();
    private int selectedBuildScriptIndex;
    private BuildScript selectedBuildScript;

    public BuildScript SelectedBuildScript
    {
        get
        {
            return this.selectedBuildScript;
        }

        private set
        {
            var currentValue = this.selectedBuildScript;
            if (currentValue == value)
            {
                return;
            }

            this.selectedBuildScript = value;
            this.NotifyOfPropertyChange(nameof(this.SelectedBuildScript));
        }
    }

    public int SelectedBuildScriptIndex
    {
        get
        {
            return this.selectedBuildScriptIndex;
        }

        set
        {
            var currentValue = this.selectedBuildScriptIndex;
            if (currentValue == value)
            {
                return;
            }

            this.selectedBuildScriptIndex = value;
            if (value >= 0 && value < this.buildScripts.Count)
            {
                this.selectedBuildScript = this.buildScripts[value];
            }

            this.NotifyOfPropertyChange(nameof(this.SelectedBuildScript));
            this.NotifyOfPropertyChange(nameof(this.SelectedBuildScriptIndex));
        }
    }

    public BuildScriptColleciton BuildScripts
    {
        get
        {
            return this.buildScripts;
        }

        private set
        {
            var currentValue = this.buildScripts;
            if (currentValue == value)
            {
                return;
            }

            this.buildScripts = value;
            this.NotifyOfPropertyChange(nameof(this.BuildScripts));
        }
    }
}