namespace System.Runtime.CompilerServices
{
	class IsExternalInit
	{

	}
}

public record TestRecord(string Name, int Age);

public class Person
{
	public int Age
	{
		get;
		init;
	}
}