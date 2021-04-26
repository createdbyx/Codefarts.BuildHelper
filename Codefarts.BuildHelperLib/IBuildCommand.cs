// <copyright file="IBuildCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System;

namespace Codefarts.BuildHelper
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ParameterTypeHint : Attribute
    {
        public Type Type
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public int Index
        {
            get; set;
        }
    }

    public interface IBuildCommand
    {
        string Name
        {
            get;
        }

        void Execute(ExecuteCommandArgs args);
    }
}