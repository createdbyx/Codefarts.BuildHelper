using System;

namespace Codefarts.BuildHelper;

public class RunResult<T>
{
    public RunResult()
    {
        this.Status = RunStatus.Running;
    }

    public RunResult(Exception error)
    {
        this.Error = error ?? throw new ArgumentNullException(nameof(error));
        this.Status = RunStatus.Errored;
    }

    public RunResult(T returnValue)
    {
        this.ReturnValue = returnValue;
        this.Status = RunStatus.Sucessful;
    }

    public Exception Error { get; private set; }

    public RunStatus Status { get; private set; }

    public T ReturnValue { get; }

    public static RunResult Sucessful()
    {
        var value = new RunResult();
        value.Status = RunStatus.Sucessful;
        return value;
    }

    public static RunResult Sucessful(T returnValue)
    {
        return new RunResult(returnValue);
    }

    public static RunResult Errored(Exception error)
    {
        return new RunResult(error);
    }

    public void Done()
    {
        this.Status = RunStatus.Sucessful;
    }

    public void Done(Exception error)
    {
        this.Error = error ?? throw new ArgumentNullException(nameof(error));
        this.Status = RunStatus.Errored;
    }
}