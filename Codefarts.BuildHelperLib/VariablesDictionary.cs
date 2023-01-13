// <copyright file="VariablesDictionary.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper;

using System.Collections.Concurrent;
using System.Collections.Generic;

/// <summary>
/// Provides a thread safe dictionary for storing variables.
/// </summary>
public class VariablesDictionary : ConcurrentDictionary<string, object>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VariablesDictionary"/> class.
    /// </summary>
    public VariablesDictionary()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VariablesDictionary"/> class.
    /// </summary>
    /// <param name="collection">The collection to copy entries from.</param>
    public VariablesDictionary(IEnumerable<KeyValuePair<string, object>> collection) 
        : base(collection)
    {
    }
}