// Copyright 2026 Esri
// 
// Licensed under the Apache License Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#nullable enable
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TestAppRunner
{
    internal class TestCaseDiscoverySink : ITestCaseDiscoverySink
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseDiscoverySink"/> class.
        /// </summary>
        public TestCaseDiscoverySink()
        {
        }

        /// <summary>
        /// Gets the tests.
        /// </summary>
        public ICollection<TestCase> Tests { get; private set; } = new Collection<TestCase>();
        private object TestsSync = new object();

        /// <summary>
        /// Sends the test case to the discoverer.
        /// </summary>
        /// <param name="discoveredTest">The discovered test.</param>
        void ITestCaseDiscoverySink.SendTestCase(TestCase discoveredTest)
        {
            if (discoveredTest != null)
            {
                lock (TestsSync)
                    Tests.Add(discoveredTest);
            }
        }
    }
}
