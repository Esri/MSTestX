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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAppRunner.ViewModels;
#if MAUI
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
#else
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
#endif

namespace TestAppRunner.Views
{
    /// <summary>
    /// Shows all tests in a namespace grouped by class
    /// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public partial class GroupByClassTestsPage : ContentPage
    {
        private TestResultGroup tests;

        internal GroupByClassTestsPage(TestResultGroup tests)
		{
			InitializeComponent ();
            this.tests = tests;
            list.ItemsSource = new List<TestResultGroup>(tests.GroupBy(t => t.ClassName).Select((g, t) => new TestResultGroup(g.Key.StartsWith(tests.Group + ".") ? g.Key.Substring(tests.Group.Length + 1) : g.Key, g)).OrderBy(g=>g.Group));
            currentTestView.BindingContext = TestRunnerVM.Instance;
            loopIterationLabel.BindingContext = TestRunnerVM.Instance;
            startStopButton.BindingContext = TestRunnerVM.Instance;
            runUntilFailureButton.BindingContext = TestRunnerVM.Instance;
            this.BindingContext = tests;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (TestRunnerVM.Instance.IsBusy)
            {
                TestRunnerVM.Instance.Cancel();
            }
            else
            {
                try
                {
                    await TestRunnerVM.Instance.Run(tests.Select(t => t.Test));
                }
                catch (Exception ex)
                {
                    Logger.Log($"Run command failed: {ex}");
                    await DisplayAlert("Test Run Error", ex.Message, "OK");
                }
            }
        }

        private async void RunUntilFailureButton_Clicked(object sender, EventArgs e)
        {
            if (TestRunnerVM.Instance.IsBusy)
            {
                TestRunnerVM.Instance.Cancel();
            }
            else
            {
                try
                {
                    await TestRunnerVM.Instance.RunUntilFailure(tests.Select(t => t.Test));
                }
                catch (Exception ex)
                {
                    Logger.Log($"Run-until-failure command failed: {ex}");
                    await DisplayAlert("Test Run Error", ex.Message, "OK");
                }
            }
        }

        private async void list_ItemSelected(object sender, SelectionChangedEventArgs args)
        {
            var item = args.CurrentSelection?.FirstOrDefault() as TestResultGroup;
            if (item == null)
                return;

            await Navigation.PushAsync(new TestRunPage(item));

            // Manually deselect item.
            (sender as CollectionView).SelectedItem = null;
        }
    }
}
