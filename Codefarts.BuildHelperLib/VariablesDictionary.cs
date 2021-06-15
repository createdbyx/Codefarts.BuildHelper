// <copyright file="VariablesDictionary.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System.Collections.Concurrent;

    /// <summary>
    /// Provides a thread safe dictionary for storing variables.
    /// </summary>
    public class VariablesDictionary : ConcurrentDictionary<string, object>
    {
    }
}