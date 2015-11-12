void Main() {
	new Test_UnitTests().RunTests();
}

#region Testing the LinqPadMiniUnitTesting using LinqPad - Such meta, much tests...

///  Author: Fábio Beirão (fdblog -@at- gmail.com)
///  GitHub: https://github.com/fdbeirao/LinqPadMiniUnitTests
/// Version: 0.0.5

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
	public void Test_IsTrue() {
		Assert.IsTrue(true);
	}
	
	[Test]
	public void Test_IsTrue_Func() {
		Assert.IsTrue(() => { return true; });
	}
	
	[Test(ShouldTestFail=true)]
	public void Test_IsTrue_Fail() {
		Assert.IsTrue(false);
	}
	
	[Test]
	public void Test_IsFalse() {
		Assert.IsFalse(false);
	}
	
	[Test]
	public void Test_IsFalse_Func() {
		Assert.IsFalse(() => { return false; });
	}
	
	[Test(ShouldTestFail=true)]
	public void Test_IsFalse_Fail() {
		Assert.IsFalse(true);
	}
	
	[Test(ShouldTestFail=true)]
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
	
	[Test(ExpectedExceptionType=typeof(Exception), ShouldTestFail=true)]
	public void Test_FailIf_ExpectedExceptionIsNotThrown() {
		// Nothing to do, this test should fail because no exception was thrown
	}
}

#endregion