// <copyright file="CommandDataTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace BuildHelperTests
{
    using Codefarts.BuildHelper;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [TestCategory(nameof(CommandDataTests))]
    public class CommandDataTests
    {
        [TestMethod]
        public void DefaultValues()
        {
            var data = new CommandData();
            Assert.IsNull(data.Name);
            Assert.IsNull(data.Parent);
            Assert.IsNotNull(data.Children);
            Assert.AreEqual(0, data.Children.Count);
            Assert.IsNotNull(data.Parameters);
            Assert.AreEqual(0, data.Parameters.Count);
        }

        [TestMethod]
        public void AddChildViaCollection()
        {
            var parentA = new CommandData();
            var parentB = new CommandData();
            var child = new CommandData();
                                                                          
            Assert.IsNull(child.Parent);
            parentA.Children.Add(child);

            Assert.IsNotNull(child.Parent);
            Assert.AreSame(parentA, child.Parent);
            Assert.AreNotSame(parentB, child.Parent);
        }
        
        [TestMethod]
        public void AddChildViaCollectionThenAddChildToDifferentCollection()
        {
            var parentA = new CommandData();
            var parentB = new CommandData();
            var child = new CommandData();
                                                                          
            // validate the child hos no parent
            Assert.IsNull(child.Parent);
            
            // add child to parent a via children collection
            parentA.Children.Add(child);

            // validate that child has a parent property set and that is parent a not parent b
            Assert.IsNotNull(child.Parent);
            Assert.AreSame(parentA, child.Parent);
            Assert.AreNotSame(parentB, child.Parent);
            
            // now add child to parent b's children collection
            parentB.Children.Add(child);

            // validate that child has a parent property set and that is parent b not parent a
            Assert.IsNotNull(child.Parent);
            Assert.AreNotSame(parentA, child.Parent);
            Assert.AreSame(parentB, child.Parent);
        }

        [TestMethod]
        public void AddChildTwiceToCollection()
        {
            var parentA = new CommandData();
            var child = new CommandData();

            var count = 0;
            parentA.Children.CollectionChanged += (s, e) => { count++; };

            Assert.IsNull(child.Parent);
            parentA.Children.Add(child);
            Assert.IsNotNull(child.Parent);

            // add again
            parentA.Children.Add(child);

            // check count
            Assert.AreEqual(1, parentA.Children.Count);
        }

        [TestMethod]
        public void SetParent()
        {
            var parentA = new CommandData();
            var parentB = new CommandData();
            var child = new CommandData();

            // validate patent children collections
            Assert.AreEqual(0, parentA.Children.Count);
            Assert.AreEqual(0, parentB.Children.Count);

            Assert.IsNull(child.Parent);
            child.Parent = parentA;

            Assert.IsNotNull(child.Parent);
            Assert.AreSame(parentA, child.Parent);
            Assert.AreNotSame(parentB, child.Parent);

            // validate patent children collections
            Assert.AreEqual(1, parentA.Children.Count);
            Assert.AreEqual(0, parentB.Children.Count);
        }

        [TestMethod]
        public void SetParentThenSetParentNull()
        {
            var parentA = new CommandData();
            var parentB = new CommandData();
            var child = new CommandData();

            // validate patent children collections
            Assert.AreEqual(0, parentA.Children.Count);
            Assert.AreEqual(0, parentB.Children.Count);

            Assert.IsNull(child.Parent);
            child.Parent = parentA;

            Assert.IsNotNull(child.Parent);
            Assert.AreSame(parentA, child.Parent);
            Assert.AreNotSame(parentB, child.Parent);

            // validate patent children collections
            Assert.AreEqual(1, parentA.Children.Count);
            Assert.AreEqual(0, parentB.Children.Count);

            child.Parent = null;

            Assert.IsNull(child.Parent);
            Assert.AreNotSame(parentA, child.Parent);
            Assert.AreNotSame(parentB, child.Parent);

            // validate patent children collections
            Assert.AreEqual(0, parentA.Children.Count);
            Assert.AreEqual(0, parentB.Children.Count);
        }

        [TestMethod]
        public void ChangeParent()
        {
            var parentA = new CommandData();
            var parentB = new CommandData();
            var child = new CommandData();

            // should be no parent
            Assert.IsNull(child.Parent);

            // add to parent via the parents Children collection
            parentA.Children.Add(child);

            // validate that child parent prop was set
            Assert.IsNotNull(child.Parent);

            // validate that is correct parent
            Assert.AreSame(parentA, child.Parent);
            Assert.AreNotSame(parentB, child.Parent);

            // validate parent children collections
            Assert.AreEqual(1, parentA.Children.Count);
            Assert.AreEqual(0, parentB.Children.Count);

            // set new parent
            child.Parent = parentB;

            // validate that new parent was set
            Assert.AreNotSame(parentA, child.Parent);
            Assert.AreSame(parentB, child.Parent);

            // validate parent children collections
            Assert.AreEqual(0, parentA.Children.Count);
            Assert.AreEqual(1, parentB.Children.Count);
        }

        [TestMethod]
        public void NameAndParentPropertyChanged()
        {
            var parentA = new CommandData();
            var child = new CommandData();

            var nameChanged = false;
            var parentChanged = false;
            child.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(child.Name):
                        nameChanged = true;
                        break;

                    case nameof(child.Parent):
                        parentChanged = true;
                        break;
                }
            };

            child.Parent = parentA;
            child.Name = "child";

            // validate that child name and parent property change events occurred
            Assert.IsTrue(nameChanged);
            Assert.IsTrue(parentChanged);
        }

        [TestMethod]
        public void CollectionOwner()
        {
            var child = new CommandData();

            Assert.AreSame(child, child.Children.Owner);
        }

        [TestMethod]
        public void AddNullChild()
        {
            var child = new CommandData();

            child.Children.Add(null);
            Assert.AreEqual(0, child.Children.Count);
        }

        [TestMethod]
        public void ChangeParentTwice()
        {
            var parentA = new CommandData();
            var parentB = new CommandData();
            var child = new CommandData();

            // should be no parent
            Assert.IsNull(child.Parent);

            // add to parent via the parents Children collection
            child.Parent = parentA;

            // validate that child parent prop was set
            Assert.IsNotNull(child.Parent);

            // validate that is correct parent
            Assert.AreSame(parentA, child.Parent);
            Assert.AreNotSame(parentB, child.Parent);

            // validate patent children collections
            Assert.AreEqual(1, parentA.Children.Count);
            Assert.AreEqual(0, parentB.Children.Count);

            // set new parent
            child.Parent = parentB;

            // validate that new parent was set
            Assert.AreNotSame(parentA, child.Parent);
            Assert.AreSame(parentB, child.Parent);

            // validate parent children collections
            Assert.AreEqual(0, parentA.Children.Count);
            Assert.AreEqual(1, parentB.Children.Count);
        }

        [TestMethod]
        public void ClearParentCollectionAfterSettingParent()
        {
            var parentA = new CommandData();
            var parentB = new CommandData();
            var child = new CommandData();

            // should be no parent
            Assert.IsNull(child.Parent);

            // add to parent via the parents Children collection
            parentA.Children.Add(child);

            // validate that child parent prop was set
            Assert.IsNotNull(child.Parent);

            // validate that is correct parent
            Assert.AreSame(parentA, child.Parent);
            Assert.AreNotSame(parentB, child.Parent);

            // validate parent children collections
            Assert.AreEqual(1, parentA.Children.Count);
            Assert.AreEqual(0, parentB.Children.Count);

            // clear parent collections
            parentA.Children.Clear();
            parentB.Children.Clear();

            // ensure child parent is null
            Assert.IsNull(child.Parent);
        }
    }
}