using System;
using System.Collections.Generic;
using AlphaMiner;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class AlphaUnitTest
    {
        PrivateObject alpha = new PrivateObject(new Alpha());
        List<string> firstTasks = new List<string>() { "a" };
        List<string> lastTasks = new List<string>() { "d" };
        List<string> allTasks = new List<string>() { "a", "b", "c", "d", "e" };
        List<string[]> traces = new List<string[]>()
        {
            new string[] { "a", "b", "c", "d"},
            new string[] { "a", "c", "b", "d"},
            new string[] { "a", "e", "d"}
        };

        [TestMethod]
        public void MainTestMethod()
        {
            PrivateObject matrix = new PrivateObject(alpha.GetProperty("Matrix").GetType(), allTasks);
            alpha.SetFieldOrProperty("AllTasks", allTasks);
            alpha.SetFieldOrProperty("FirstTasks", firstTasks);
            alpha.SetFieldOrProperty("LastTasks", lastTasks);

        }

        [TestMethod]
        public void TestMethod()
        {
        }
    }
}
