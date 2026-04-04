using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace JDG.Tests.Editor
{
    /// <summary>
    /// Custom editor menu for running E2E tests with keyboard shortcuts.
    ///
    /// Menu shortcuts:
    /// - Ctrl+Shift+E: Run All E2E Tests
    /// - Ctrl+Shift+G: Run Full Game E2E Tests
    /// - Ctrl+Shift+T: Run Tutorial E2E Tests
    /// - Ctrl+Shift+A: Run Ability E2E Tests
    /// </summary>
    public static class E2ETestMenu
    {
        #region Menu Items

        [MenuItem("Tests/Run All E2E Tests %#e", priority = 100)]
        public static void RunAllE2ETests()
        {
            Debug.Log("[E2E Tests] Running all E2E tests...");
            RunTestsWithCategory("E2E");
        }

        [MenuItem("Tests/Run Full Game E2E Tests %#g", priority = 101)]
        public static void RunFullGameTests()
        {
            Debug.Log("[E2E Tests] Running Full Game E2E tests...");
            RunTestsWithCategories("E2E", "FullGame");
        }

        [MenuItem("Tests/Run Tutorial E2E Tests %#t", priority = 102)]
        public static void RunTutorialTests()
        {
            Debug.Log("[E2E Tests] Running Tutorial E2E tests...");
            RunTestsWithCategories("E2E", "Tutorial");
        }

        [MenuItem("Tests/Run Ability E2E Tests %#a", priority = 103)]
        public static void RunAbilityTests()
        {
            Debug.Log("[E2E Tests] Running Ability E2E tests...");
            RunTestsWithCategories("E2E", "Abilities");
        }

        [MenuItem("Tests/Open Test Runner Window", priority = 200)]
        public static void OpenTestRunner()
        {
            EditorApplication.ExecuteMenuItem("Window/General/Test Runner");
        }

        #endregion

        #region Test Execution

        /// <summary>
        /// Runs tests matching a single category.
        /// </summary>
        private static void RunTestsWithCategory(string category)
        {
            RunTestsWithCategories(category);
        }

        /// <summary>
        /// Runs tests matching all specified categories.
        /// </summary>
        private static void RunTestsWithCategories(params string[] categories)
        {
            var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();

            // Create filter for Play Mode tests with specified categories
            var filter = new Filter
            {
                testMode = TestMode.PlayMode,
                categoryNames = categories
            };

            // Register callbacks to log results
            testRunnerApi.RegisterCallbacks(new TestRunnerCallbacks());

            // Execute tests
            var executionSettings = new ExecutionSettings(filter);
            testRunnerApi.Execute(executionSettings);

            Debug.Log($"[E2E Tests] Started test execution with categories: {string.Join(", ", categories)}");
        }

        #endregion

        #region Test Runner Callbacks

        /// <summary>
        /// Callbacks for test runner events to provide feedback.
        /// </summary>
        private class TestRunnerCallbacks : ICallbacks
        {
            public void RunStarted(ITestAdaptor testsToRun)
            {
                Debug.Log($"[E2E Tests] Test run started. Total tests: {testsToRun.TestCaseCount}");
            }

            public void RunFinished(ITestResultAdaptor result)
            {
                var status = result.TestStatus;
                var passCount = result.PassCount;
                var failCount = result.FailCount;
                var skipCount = result.SkipCount;
                var inconclusiveCount = result.InconclusiveCount;

                string summary = $"[E2E Tests] Test run completed.\n" +
                                 $"  Status: {status}\n" +
                                 $"  Passed: {passCount}\n" +
                                 $"  Failed: {failCount}\n" +
                                 $"  Skipped: {skipCount}\n" +
                                 $"  Inconclusive: {inconclusiveCount}";

                if (failCount > 0)
                {
                    Debug.LogError(summary);
                }
                else
                {
                    Debug.Log(summary);
                }
            }

            public void TestStarted(ITestAdaptor test)
            {
                // Optional: Log each test start
                // Debug.Log($"[E2E Tests] Starting: {test.Name}");
            }

            public void TestFinished(ITestResultAdaptor result)
            {
                if (result.TestStatus == TestStatus.Failed)
                {
                    Debug.LogError($"[E2E Tests] FAILED: {result.Name}\n{result.Message}");
                }
                else if (result.TestStatus == TestStatus.Passed)
                {
                    // Optional: Log each test pass
                    // Debug.Log($"[E2E Tests] PASSED: {result.Name}");
                }
            }
        }

        #endregion
    }

    #region Quick Actions

    /// <summary>
    /// Additional quick action utilities for E2E testing.
    /// </summary>
    public static class E2ETestUtilities
    {
        /// <summary>
        /// Clears the console before running tests.
        /// </summary>
        [MenuItem("Tests/Clear Console Before Tests", priority = 300)]
        public static void ClearConsole()
        {
            var assembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method?.Invoke(null, null);
            Debug.Log("[E2E Tests] Console cleared.");
        }

        /// <summary>
        /// Shows a summary of available E2E test categories.
        /// </summary>
        [MenuItem("Tests/Show E2E Test Categories", priority = 301)]
        public static void ShowTestCategories()
        {
            string info = @"
=== E2E Test Categories ===

[E2E] - All end-to-end tests
  └─ [FullGame] - Complete game flow tests
      • Phase transitions
      • Win/lose conditions
      • Combat scenarios
      • Multi-turn games

  └─ [Tutorial] - Tutorial scenario tests
      • Dialogue triggers
      • Highlight events
      • Scripted actions
      • Tutorial flow

  └─ [Abilities] - Card ability tests
      • Combat abilities (DirectAttack, Aggro, Protection)
      • Stat modifiers (Equipment, Field bonuses)
      • Trigger lifecycle (OnSummon, OnDeath, etc.)
      • Ability combinations

=== Keyboard Shortcuts ===
• Ctrl+Shift+E: Run All E2E Tests
• Ctrl+Shift+G: Run Full Game Tests
• Ctrl+Shift+T: Run Tutorial Tests
• Ctrl+Shift+A: Run Ability Tests

=== Unity Test Runner ===
Window > General > Test Runner
Filter by category in the dropdown.
";
            Debug.Log(info);
        }
    }

    #endregion
}
