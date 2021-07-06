// <copyright file="MockCommandPlugin.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace BuildHelperTests.Mocks
{
    using System;
    using Codefarts.BuildHelper;

    public class MockCommandPlugin : ICommandPlugin
    {
        private string name;

        public MockCommandPlugin()
        {
        }

        public MockCommandPlugin(string name)
        {
            this.name = name;
        }

        public MockCommandPlugin(Action<RunCommandArgs> callback)
        {
            this.Callback = callback;
        }

        public MockCommandPlugin(string name, Action<RunCommandArgs> callback)
        {
            this.name = name;
            this.Callback = callback;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public Action<RunCommandArgs> Callback { get; set; }

        public void Run(RunCommandArgs args)
        {
            var callback = this.Callback;
            if (callback != null)
            {
                callback(args);
            }
        }
    }
}