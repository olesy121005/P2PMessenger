namespace P2PMessenger.Core.Exceptions;

public class PeerConnectionException : Exception
{
    public PeerConnectionException(string message) : base(message) { }
    public PeerConnectionException(string message, Exception inner) : base(message, inner) { }
}