void Main() {
	new Test_UnitTests().RunTests();
}

#region Testing the LinqPadMiniUnitTesting using LinqPad - Such meta, much tests...

///  Author: Fábio Beirão (fdblog -@at- gmail.com)
///  GitHub: https://github.com/fdbeirao/LinqPadMiniUnitTests
/// Version: 0.0.2

public class Test_UnitTests : IUnitTests {
	[Test]
	public void Test_AreEqual() {
		// Setup
		var someString = "foo";
		// Act
		someString += "bar";
		// Assert
		Assert.AreEqual(expected: "foobar", actual: someString);
	}
	
	[Test]
	public void Test_AreNotEqual() {
		// Setup
		var someString = "foo";
		// Act
		someString += "bar";
		// Assert
		Assert.AreNotEqual(notExpected: "xpto", actual: someString);
	}
	
	[Test]
	public void Test_AssertFail() {
		// Assert
		Assert.Fail("This test should fail!");
	}
	
	[Test(ExpectedExceptionType=typeof(Exception))]
	public void Test_ExpectedException() {
		// Act
		throw new Exception("This exception is expected! This test should pass.");
		// Assert
		Assert.Fail("This test should not have failed, the exception was expected");
	}
}

#endregion