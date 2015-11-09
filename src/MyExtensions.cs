
#region LinqPad Unit Tests

///  Author: Fábio Beirão (fdblog -@at- gmail.com)
///  GitHub: https://github.com/fdbeirao/LinqPadMiniUnitTests
/// Version: 0.0.2

public interface IUnitTests {}

[AttributeUsage(System.AttributeTargets.Method, AllowMultiple=false)]
public class TestAttribute : Attribute {
	public string Name { get; set; }
	public Type ExpectedExceptionType { get; set; }
}

internal class TestResult {
	public string TestName { get; set; }
	internal Action TestCode { get; set; }
	internal Type ExpectedExceptionType { get; set; }
	public DumpContainer TestOutcome { get; set; }
	public DumpContainer TestDuration { get; set; }
	public DumpContainer TestFailureReason { get; set; }
}

public static class Tests {
	public static void RunTests(this IUnitTests testClass, bool throwIfAnyTestFails = true)  {
		if (testClass == null) 
			throw new ArgumentNullException("testClass");
		
		var classMethods = testClass.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		
		var testResults = new List<TestResult> { };
		foreach (var method in classMethods) {
			if (!method.IsDefined(typeof(TestAttribute))) 
				continue;
			
			var testName = method.Name;
			var testAttribute = (TestAttribute) method.GetCustomAttribute(typeof(TestAttribute));
			if (!string.IsNullOrWhiteSpace(testAttribute.Name))
				testName = string.Format("{0} ({1})", testAttribute.Name, testName);
			
			testResults.Add(new TestResult {
				TestName = testName,
				TestOutcome = new DumpContainer("Not executed yet..."),
				TestDuration = new DumpContainer("N/A"),
				TestFailureReason = new DumpContainer(),
				TestCode = (Action) Delegate.CreateDelegate(typeof(Action), testClass, method.Name),
				ExpectedExceptionType = testAttribute.ExpectedExceptionType,
			});
		}		
				
		testResults.Dump();
		
		var atLeastOneTestFailed = false;
		foreach (var testResult in testResults) {			
			testResult.TestOutcome.Content = "Executing...";
			testResult.TestOutcome.Style = "color:blue";
			
			Exception failureReason = null;
			var testStopWatch = Stopwatch.StartNew();
			try {
				testResult.TestCode();
			} catch (Exception ex) {
				failureReason = ex;
			} finally {
				testStopWatch.Stop();
			}
			
			if (failureReason != null && testResult.ExpectedExceptionType != null && failureReason.GetType() == testResult.ExpectedExceptionType)
				failureReason = null;
			
			testResult.TestDuration.Content = testStopWatch.Elapsed.ToString();
			if (failureReason == null) {
				testResult.TestOutcome.Content = "Success";
				testResult.TestOutcome.Style = "color:green";
			} else {
				testResult.TestOutcome.Content = "Failure";
				testResult.TestOutcome.Style = "color:red";
				testResult.TestFailureReason.Content = failureReason;
			}
		}
		
		if (throwIfAnyTestFails && atLeastOneTestFailed)
			throw new AggregateException();
	}
}

public static class Assert {
	public static void AreEqual(string expected, string actual) {
		// if they are both null, they are equal
		if (actual == null && expected == null) 
			return;		
		if (actual == null || expected == null || !actual.Equals(expected))
			throw new AreEqualAssertException(expectedValue: expected, actualValue: actual);
	}
	
	public static void AreNotEqual(string notExpected, string actual) {
		// if they are both null, they are equal
		if (actual == null && notExpected == null)
			throw new AreNotEqualAssertException(notExpectedValue: notExpected, actualValue: actual);
		if (actual != null && actual.Equals(notExpected))
			throw new AreNotEqualAssertException(notExpectedValue: notExpected, actualValue: actual);
	}
	
	public static void Fail(string failReason) {
		throw new AssertFailException(failReason: failReason);
	}
	
	internal class AssertException : Exception { 
		public AssertException(string message) : base(message) {}
		
		// Hide base exception members, for prettier exception printing
		internal string Message { get; }
		internal IDictionary Data { get; }
		internal MethodBase TargetSite { get; }
		internal string Source { get; }
		internal string StackTrace { get; }
		internal int HResult { get; }
		internal string HelpLink { get; }
		internal Exception InnerException { get; }
	}
	
	internal class AreEqualAssertException : AssertException {
		public AreEqualAssertException(string expectedValue, string actualValue) : base("Assert.AreEqual failed") { 
			ExpectedValue = expectedValue;
			ActualValue = actualValue;
			if (expectedValue != null)
				ExpectedBytes = Util.OnDemand("Get [ExpectedBytes] bytes", () => UTF8Encoding.UTF8.GetBytes(expectedValue));
			else
				ExpectedBytes = new DumpContainer("null");
				
			if (actualValue != null)
				ActualBytes = Util.OnDemand("Get [ActualValue] bytes", () => UTF8Encoding.UTF8.GetBytes(actualValue));
			else
				ActualBytes = new DumpContainer("null");
		}
		
		public string ExpectedValue { get; private set; }
		public DumpContainer ExpectedBytes { get; private set; }
		
		public string ActualValue { get; private set; }
		public DumpContainer ActualBytes { get; private set; }
	}
	
	internal class AreNotEqualAssertException : AssertException {
		public AreNotEqualAssertException(string notExpectedValue, string actualValue) : base("Assert.AreNotEqual failed") { 
			NotExpectedValue = notExpectedValue;
			ActualValue = actualValue;
			if (notExpectedValue != null)
				NotExpectedBytes = Util.OnDemand("Get [NotExpectedValue] bytes", () => UTF8Encoding.UTF8.GetBytes(notExpectedValue));
			else
				NotExpectedBytes = new DumpContainer("null");
				
			if (actualValue != null)
				ActualBytes = Util.OnDemand("Get [ActualValue] bytes", () => UTF8Encoding.UTF8.GetBytes(actualValue));
			else
				ActualBytes = new DumpContainer("null");
		}
		
		public string NotExpectedValue { get; private set; }
		public DumpContainer NotExpectedBytes { get; private set; }
		
		public string ActualValue { get; private set; }
		public DumpContainer ActualBytes { get; private set; }
	}
	
	internal class AssertFailException : AssertException {
		public AssertFailException(string failReason) : base("Assert.Fail: [" + failReason + "]") { }
	}
}

#endregion