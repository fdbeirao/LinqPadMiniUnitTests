#region LinqPad Unit Tests

///  Author: Fábio Beirão (fdblog -@at- gmail.com)
///  GitHub: https://github.com/fdbeirao/LinqPadMiniUnitTests
/// Version: 0.0.5

public interface IUnitTests {}

[AttributeUsage(System.AttributeTargets.Method, AllowMultiple=false)]
public class TestAttribute : Attribute {
	public string Name { get; set; }
	public Type ExpectedExceptionType { get; set; }
	public bool ShouldTestFail { get; set; }
}

internal class TestResult {
	public string TestName { get; set; }
	internal Action TestCode { get; set; }
	internal Type ExpectedExceptionType { get; set; }
	internal bool ShouldTestFail { get; set; }
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
				ShouldTestFail = testAttribute.ShouldTestFail,
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
			
			if (testResult.ExpectedExceptionType != null) {
				if (failureReason == null) {
					failureReason = new Assert.ExpectedExceptionWasNotThrownAssertException(
						expectedExceptionType: testResult.ExpectedExceptionType);
				} else {
					if (failureReason.GetType() == testResult.ExpectedExceptionType) {
						failureReason = null;
					} else {
						failureReason = new Assert.ExpectedExceptionWasNotThrownAssertException(
							expectedExceptionType: testResult.ExpectedExceptionType, 
							thrownException: failureReason);
					}
				}	
			}
			
			testResult.TestDuration.Content = testStopWatch.Elapsed.ToString();
			if (failureReason != null)
				testResult.TestFailureReason.Content = failureReason;
			
			if (failureReason == null) {
				if (testResult.ShouldTestFail) {
					testResult.TestOutcome.Content = "Success (not expected!)";
					testResult.TestOutcome.Style = "color:red";
					atLeastOneTestFailed = true;
				} else {
					testResult.TestOutcome.Content = "Success";
					testResult.TestOutcome.Style = "color:green";
				}
			} else {
				if (testResult.ShouldTestFail) {
					testResult.TestOutcome.Content = "Failure (expected)";
					testResult.TestOutcome.Style = "color:green";
				} else {
					testResult.TestOutcome.Content = "Failure";
					testResult.TestOutcome.Style = "color:red";
					atLeastOneTestFailed = true;
				}
			}
		}
		
		if (throwIfAnyTestFails && atLeastOneTestFailed)
			throw new AggregateException();
	}
}

public static class Assert {
	public static void AreEqual<T>(T expected, T actual) where T : IComparable  {
		// if they are both null, they are equal
		if (actual == null && expected == null) 
			return;
		if (actual == null || expected == null || actual.CompareTo(expected) != 0) {
			if (typeof(T) == typeof(string))
				throw new AreEqualStringAssertException(expectedValue: expected as string, actualValue: actual as string);
			else
				throw new AreEqualAssertException<T>(expectedValue: expected, actualValue: actual);
		}
	}	
	
	public static void AreNotEqual<T>(T notExpected, T actual) where T : IComparable {
		// if they are both null, they are equal
		if ((actual == null && notExpected == null) || (actual != null && actual.CompareTo(notExpected) == 0)) {
			if (typeof(T) == typeof(string))
				throw new AreNotEqualStringAssertException(notExpectedValue: notExpected as string, actualValue: actual as string);
			else
				throw new AreNotEqualAssertException<T>(notExpectedValue: notExpected, actualValue: actual);
		}
	}
	
	public static void IsTrue(Func<bool> funcToAssert) {
		IsTrue(funcToAssert());
	}
	
	public static void IsTrue(bool value) {
		if (true != value)
			throw new IsTrueAssertException();
	}
	
	public static void IsFalse(Func<bool> funcToAssert) {
		IsFalse(funcToAssert());
	}
	
	public static void IsFalse(bool value) {
		if (false != value)
			throw new IsFalseAssertException();
	}
	
	public static void Fail(string failReason) {
		throw new AssertFailException(failReason: failReason);
	}
	
	internal class AssertException : Exception { 
		public AssertException(string message) : base(message) {}
		
		// Hide base exception members, for prettier exception printing
		internal string Message { get; set; }
		internal IDictionary Data { get; set; }
		internal MethodBase TargetSite { get; set; }
		internal string Source { get; set; }
		internal string StackTrace { get; set; }
		internal int HResult { get; set; }
		internal string HelpLink { get; set; }
		internal Exception InnerException { get; set; }
	}
	
	internal class AreEqualAssertException<T> : AssertException {
		public AreEqualAssertException(T expectedValue, T actualValue) : base("Assert.AreEqual failed") { 
			ExpectedValue = expectedValue;
			ActualValue = actualValue;			
		}
		
		public T ExpectedValue { get; private set; }		
		public T ActualValue { get; private set; }		
	}
	
	internal class AreEqualStringAssertException : AreEqualAssertException<string> {
		public AreEqualStringAssertException(string expectedValue, string actualValue) : base(expectedValue: expectedValue, actualValue: actualValue) {
			if (expectedValue != null)
				ExpectedValueBytes = Util.OnDemand("Get [ExpectedValue] bytes (UTF8)", () => ExpectedValue.GetStringBytes(Encoding.UTF8));
			else
				ExpectedValueBytes = new DumpContainer("null");
			
			if (actualValue != null)
				ActualValueBytes = Util.OnDemand("Get [ActualValue] bytes (UTF8)", () => ActualValue.GetStringBytes(Encoding.UTF8));
			else
				ActualValueBytes = new DumpContainer("null");
		}
		
		public DumpContainer ExpectedValueBytes { get; private set; }
		public DumpContainer ActualValueBytes { get; private set; }
	}
	
	internal class AreNotEqualAssertException<T> : AssertException {
		public AreNotEqualAssertException(T notExpectedValue, T actualValue) : base("Assert.AreNotEqual failed") { 
			NotExpectedValue = notExpectedValue;
			ActualValue = actualValue;			
		}
		
		public T NotExpectedValue { get; private set; }		
		public T ActualValue { get; private set; }		
	}
	
	internal class AreNotEqualStringAssertException : AreNotEqualAssertException<string> {
		public AreNotEqualStringAssertException(string notExpectedValue, string actualValue) : base(notExpectedValue: notExpectedValue, actualValue: actualValue) {
			if (notExpectedValue != null)
				NotExpectedValueBytes = Util.OnDemand("Get [NotExpectedValue] bytes (UTF8)", () => NotExpectedValue.GetStringBytes(Encoding.UTF8));
			else
				NotExpectedValueBytes = new DumpContainer("null");
			
			if (actualValue != null)
				ActualValueBytes = Util.OnDemand("Get [ActualValue] bytes (UTF8)", () => ActualValue.GetStringBytes(Encoding.UTF8));
			else
				ActualValueBytes = new DumpContainer("null");
		}
		
		public DumpContainer NotExpectedValueBytes { get; private set; }
		public DumpContainer ActualValueBytes { get; private set; }
	}
	
	internal static string GetStringBytes(this string inputString, Encoding encoding) {
		return string.Join(" ", encoding.GetBytes(inputString).Select(chr => chr.ToString("000")));
	}
	
	internal class AssertFailException : AssertException {
		public AssertFailException(string failReason) : base("Assert.Fail: [" + failReason + "]") { }
	}

	internal class ExpectedExceptionWasNotThrownAssertException : AssertException {
		public ExpectedExceptionWasNotThrownAssertException(Type expectedExceptionType, Exception thrownException) : 
			base(string.Format("An exception of type [{0}] was expected but an exception of type [{1}] was thrown", expectedExceptionType.Name, thrownException.GetType().Name)) {
			ThrownException = thrownException;
			ExpectedExceptionType = expectedExceptionType;
		}
		
		public ExpectedExceptionWasNotThrownAssertException(Type expectedExceptionType) : 
			base(string.Format("An exception of type [{0}] was expected but none was thrown", expectedExceptionType.Name)) {
			ExpectedExceptionType = expectedExceptionType;
		}
		
		public Exception ThrownException { get; set; }
		public Type ExpectedExceptionType { get; set; }
	}
	
	internal class IsTrueAssertException : AssertException {
		public IsTrueAssertException() : base("Assert.IsTrue failed") {}
	}
	
	internal class IsFalseAssertException : AssertException {
		public IsFalseAssertException() : base("Assert.IsFalse failed") {}
	}
}

#endregion