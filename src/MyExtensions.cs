#region LinqPad Mini Unit Tests

///  Author: Fábio Beirão (fdblog -@at- gmail.com)
///  GitHub: https://github.com/fdbeirao/LinqPadMiniUnitTests
/// Version: 0.0.1

public interface IUnitTests { 
	List<Test> Tests { get; } 
}

public class Test {
	public string Name { get; set; }
	public Func<string> Code { get; set; }
}

internal class TestResult {
	public string TestName { get; set; }	
	public DumpContainer TestOutcome { get; set; }
	public DumpContainer TestDuration { get; set; }
	public DumpContainer TestFailureReason { get; set; }
	internal Test Test { get; set; }
}

public static class Tests {
	public static void RunTests(this IUnitTests testClass)  {
		if (testClass == null) throw new ArgumentNullException("testClass");
		if (testClass.Tests == null) throw new ArgumentNullException("testClass.Tests");
		
		var unnamedTestCounter = 1;
		var testResults = testClass.Tests
			.Where(test => test != null)
			.Select(test =>
			new TestResult { 
				Test = test,
				TestName = (string.IsNullOrWhiteSpace(test.Name) ? string.Format("Unknown Test [{0}]", unnamedTestCounter++) : test.Name), 
				TestOutcome = new DumpContainer("not executed yet..."),
				TestDuration = new DumpContainer("N/A"),
				TestFailureReason = new DumpContainer(),
			}).ToList();
		testResults.Dump();
		
		foreach (var test in testResults) {			
			if (test.Test.Code == null) {
				test.TestOutcome.Content = "No code to execute!"; 
				test.TestOutcome.Style = "color:olive";
				continue;
			}
			
			test.TestOutcome.Content = "executing...";
			test.TestOutcome.Style = "color:blue";
			
			var testStopWatch = Stopwatch.StartNew();
			var testOutcome = test.Test.Code();
			testStopWatch.Stop();
			
			test.TestDuration.Content = testStopWatch.Elapsed.ToString();
			if (string.IsNullOrWhiteSpace(testOutcome)) {
				test.TestOutcome.Content = "Success";
				test.TestOutcome.Style = "color:green";
			} else {
				test.TestOutcome.Content = "Failure";
				test.TestOutcome.Style = "color:red";
				test.TestFailureReason.Content = testOutcome;
			}
		}
	}
}

#endregion