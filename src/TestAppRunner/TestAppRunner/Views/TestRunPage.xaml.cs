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

using TestAppRunner.ViewModels;

namespace TestAppRunner.Views
{
    /// <summary>
    /// Shows the test results for a single test
    /// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public partial class TestRunPage : ContentPage
    {
        internal TestRunPage (TestResultGroup testCases)
		{
			InitializeComponent();
            currentTestView.BindingContext = TestRunnerVM.Instance;
            loopIterationLabel.BindingContext = TestRunnerVM.Instance;
            startStopButton.BindingContext = TestRunnerVM.Instance;
            runUntilFailureButton.BindingContext = TestRunnerVM.Instance;
            this.BindingContext = testCases;
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
                    await TestRunnerVM.Instance.Run(((TestResultGroup)BindingContext).Select(t => t.Test));
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
                    await TestRunnerVM.Instance.RunUntilFailure(((TestResultGroup)BindingContext).Select(t => t.Test));
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
            var item = args.CurrentSelection?.FirstOrDefault() as TestResultVM;
            if (item == null)
                return;

            await Navigation.PushAsync(new ItemDetailPage(item));

            // Manually deselect item.
            (sender as CollectionView).SelectedItem = null;
        }
    }
}
