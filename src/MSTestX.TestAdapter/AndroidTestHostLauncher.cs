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

using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MSTestX.TestAdapter
{
    public class AndroidTestHostLauncher : ITestHostLauncher
    {
        /// <summary>
        /// Interface defining contract for custom test host implementations
        /// </summary>
        public bool IsDebug => false;

        /// <summary>
        /// Launches custom test host using the default test process start info
        /// </summary>
        /// <param name="defaultTestHostStartInfo">Default TestHost Process Info</param>
        /// <returns>Process id of the launched test host</returns>
        public int LaunchTestHost(TestProcessStartInfo defaultTestHostStartInfo)
        {
            return -1;
        }

        /// <summary>
        /// Launches custom test host using the default test process start info
        /// </summary>
        /// <param name="defaultTestHostStartInfo">Default TestHost Process Info</param>
        /// <param name="cancellationToken">The cancellation Token.</param>
        /// <returns>Process id of the launched test host</returns>
        public int LaunchTestHost(TestProcessStartInfo defaultTestHostStartInfo, CancellationToken cancellationToken)
        {
            return -1;
        }
    }
}
