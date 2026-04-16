using System.Net;

namespace Talage.SDK.Internal.ApiClient;

public sealed class TalageApiRequestException : Exception
{
    public TalageApiRequestException(
        string message,
        string operation,
        string method,
        string uri,
        long elapsedMilliseconds,
        HttpStatusCode mappedStatusCode,
        Exception? innerException = null) : base(message, innerException)
    {
        Operation = operation;
        Method = method;
        Uri = uri;
        ElapsedMilliseconds = elapsedMilliseconds;
        MappedStatusCode = mappedStatusCode;
    }

    public string Operation { get; }

    public string Method { get; }

    public string Uri { get; }

    public long ElapsedMilliseconds { get; }

    public HttpStatusCode MappedStatusCode { get; }
}


