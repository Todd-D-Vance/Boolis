using System;
using System.Runtime.Serialization;

[Serializable]
internal class AssertionError : Exception
{
    private string v1;
    private string v2;

    public AssertionError()
    {
    }

    public AssertionError(string message) : base(message)
    {
    }

    public AssertionError(string message, Exception innerException) : base(message, innerException)
    {
    }

    public AssertionError(string v1, string v2)
    {
        this.v1 = v1;
        this.v2 = v2;
    }

    protected AssertionError(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}