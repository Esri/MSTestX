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

using Microsoft.VisualStudio.TestPlatform.MSTestAdapter.PlatformServices.Interface;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.TestPlatform.MSTest.TestAdapter
{
    internal class TestDeployment : ITestDeployment
    {
        public void Cleanup()
        {
        }

        public bool Deploy(IEnumerable<TestCase> testCases, IRunContext? runContext, IFrameworkHandle frameworkHandle)
        {
            return false;
        }

        public string GetDeploymentDirectory()
        {
            return string.Empty;
        }

        public KeyValuePair<string, string>[] GetDeploymentItems(MethodInfo method, Type type, ICollection<string> warnings)
        {
            return new KeyValuePair<string, string>[] { };
        }
    }
}
