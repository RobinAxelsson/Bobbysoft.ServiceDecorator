using System;
using System.Diagnostics.CodeAnalysis;

namespace Bobbysoft.Extensions.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public class ServiceDecorationException : Exception
    {
        public ServiceDecorationException()
        {
        }

        public ServiceDecorationException(string message) : base(message)
        {
        }

        public ServiceDecorationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
