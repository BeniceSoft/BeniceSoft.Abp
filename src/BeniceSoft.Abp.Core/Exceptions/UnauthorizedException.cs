using System;

namespace BeniceSoft.Abp.Core.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException() : base("Unauthorized")
    {
    }

    public UnauthorizedException(string message)
    {
    }
}