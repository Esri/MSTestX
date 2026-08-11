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

using Microsoft.VisualStudio.TestPlatform.Common.Filtering;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace MSTestX.Console
{
    internal static class TestCaseSelector
    {
        private const string Category = "Category";
        private const string TestCategory = "TestCategory";
        private const string Traits = "Traits";

        internal static List<TestCase> SelectTests(
            IEnumerable<TestCase> discoveredTests,
            string settingsXml)
        {
            if (discoveredTests == null)
                throw new ArgumentNullException(nameof(discoveredTests));

            var tests = discoveredTests.ToList();
            var filter = GetTestCaseFilter(settingsXml);
            if (string.IsNullOrWhiteSpace(filter))
                return tests;

            var filterWrapper = new FilterExpressionWrapper(filter);
            if (!string.IsNullOrEmpty(filterWrapper.ParseError))
            {
                throw new FormatException(
                    $"Invalid test case filter '{filter}': {filterWrapper.ParseError}");
            }

            var filterExpression = new TestCaseFilterExpression(filterWrapper);
            return tests
                .Where(testCase => Matches(filterExpression, testCase))
                .ToList();
        }

        private static string GetTestCaseFilter(string settingsXml)
        {
            if (string.IsNullOrWhiteSpace(settingsXml))
                return null;

            var document = new XmlDocument();
            try
            {
                document.LoadXml(settingsXml);
            }
            catch (XmlException ex)
            {
                throw new FormatException(
                    "Unable to read TestCaseFilter because the runsettings XML is invalid: " + ex.Message,
                    ex);
            }

            return document.DocumentElement?
                .SelectSingleNode("RunConfiguration/TestCaseFilter")?
                .InnerText;
        }

        private static bool Matches(
            TestCaseFilterExpression filterExpression,
            TestCase testCase)
        {
            var propertyValues = GetPropertyValues(testCase);
            return filterExpression.MatchTestCase(
                testCase,
                propertyName => propertyValues.TryGetValue(propertyName, out var values)
                    ? values.ToArray()
                    : null);
        }

        private static Dictionary<string, List<string>> GetPropertyValues(TestCase testCase)
        {
            var propertyValues = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            foreach (var property in testCase.Properties)
            {
                if (string.Equals(property.Label, Traits, StringComparison.OrdinalIgnoreCase))
                    continue;

                AddValue(propertyValues, property.Label, testCase.GetPropertyValue(property));
            }

            foreach (var trait in testCase.Traits)
            {
                AddValue(propertyValues, trait.Name, trait.Value);
            }

            if (!propertyValues.ContainsKey(TestCategory)
                && propertyValues.TryGetValue(Category, out var categoryValues))
            {
                propertyValues.Add(TestCategory, categoryValues);
            }

            return propertyValues;
        }

        private static void AddValue(
            Dictionary<string, List<string>> propertyValues,
            string propertyName,
            object value)
        {
            if (string.IsNullOrEmpty(propertyName) || value == null)
                return;

            if (!propertyValues.TryGetValue(propertyName, out var values))
            {
                values = new List<string>();
                propertyValues.Add(propertyName, values);
            }

            if (value is string[] multipleValues)
            {
                values.AddRange(multipleValues);
            }
            else
            {
                values.Add(value.ToString());
            }
        }
    }
}
