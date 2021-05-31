// <copyright file="IBuildCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    public interface IBuildCommand
    {
        string Name { get; }

        void Run(RunCommandArgs args);
    }
}