namespace System.Runtime.CompilerServices
{
	class IsExternalInit
	{

	}
}

public record TestRecord(string Name, int Age);

public class Persion
{
	public int Age
	{
		get;
		init;
	}
}