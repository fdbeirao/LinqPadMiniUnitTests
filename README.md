# LinqPadMiniUnitTests
Run very simple mini-tests for your code on LinqPad

Just the other day I was using [LinqPad](https://www.linqpad.net/) to write what began as a very simple method. The goal was to export some data into a specific format. The more I developed, the more corner cases I was finding about the expected output. This got me thinking: if I were using Visual Studio, I would certainly be doing some red-green TDD. Yet, this seemed like such a simple method, that I didn't think I would require it. Oh boy, was I wrong. You always need to test your code, to make sure that your changes do not break any corner cases, catching you blindsided. This got me thinking: I wish there was an easy way to write small "unit tests" on LinqPad. 

## Compatibility
I tested this project using both [LinqPad 5](https://www.linqpad.net/Download.aspx) and [LinqPad 4](https://www.linqpad.net/Download.aspx), with and without a licence.

## Easy setup: copy-paste
LinqPad has this great functionality: *My Extensions*. You can find it on *My Queries*:
![LinqPadMyExtensions](imgs/LinqPadMyExtensions.png)

All you have to do to be able to use this project is copy the contents of [src/MyExtensions.cs](src/MyExtensions.cs) and *append* them to your *My Extensions* file.

The result will be something like this (keep in mind the code itself may change with future versions):
![LinqPadMyExtensionsWithLinqPadMiniUnitTests](imgs/LinqPadMyExtensionsWithLinqPadMiniUnitTests.png)

Once you *save* it, either LinqPad will tell you *Query compiled successfully* or there will be an error that a file could not be acessed. If this is the case, please close all queries that you have open, and then make any change to your *My Extensions* (press space and then backspace), and *save* again.

You are now ready to use *LinqPad Mini Unit Tests*

## Easy usage
