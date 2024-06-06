using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfBuilderIDE.Models;

public class CommandData : INotifyPropertyChanged
{
    private string description;
    private string name;
    private bool optional;
    private string type;


    /// <summary>
    /// Class CommandData. Implements the <see cref="INotifyPropertyChanged"/>.
    /// This class is used to maintain the information of a command.
    /// </summary>
    public string Name
    {
        /// <summary>
        /// Gets or sets the name of the command.
        /// </summary>
        get
        {
            return this.name;
        }
        set
        {
            if (value == this.name)
            {
                return;
            }

            this.name = value;
            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandData"/> class with the specified properties.
    /// </summary>
    /// <param name="name">The name of the command.</param>
    /// <param name="type">The type of the command.</param>
    /// <param name="optional">A value indicating whether the command is optional or not.</param>
    /// <param name="description">The description of the command.</param>
    public CommandData(string name, string type, bool optional, string description)
    {
        this.name = name;
        this.type = type;
        this.optional = optional;
        this.description = description;
    }

    /// <summary>
    /// Gets or sets the type of the command.
    /// </summary>
    public string Type
    {
        get
        {
            return this.type;
        }
        set
        {
            if (value == this.type)
            {
                return;
            }

            this.type = value;
            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the command is optional or required.
    /// </summary>
    /// <value><c>true</c> if this command is optional; otherwise, <c>false</c>.</value>
    /// <example>This property gets or sets a value that defines if the command is optional or not. </example>
    public bool Optional
    {
        get
        {
            return this.optional;
        }
        set
        {
            if (value == this.optional)
            {
                return;
            }

            this.optional = value;
            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the description of the command.
    /// </summary>
    /// <value>The description of the command.</value>
    /// <example>This property gets or sets the description of the command.  </example>
    public string Description
    {
        get
        {
            return this.description;
        }
        set
        {
            if (value == this.description)
            {
                return;
            }

            this.description = value;
            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Represents the event handler for the PropertyChanged event.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the PropertyChanged event for the specified property name.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}