using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPulse.Infrastructure.Exceptions;
public class NoAuthKeyException : Exception
{
	// Optionally, add custom properties
	public int ErrorCode { get; set; } = 0;
	public string ErrorDetails { get; set; } = string.Empty; 

	// Default constructor
	public NoAuthKeyException()
	{
	}

	// Constructor with a message
	public NoAuthKeyException(string message)
		: base(message)
	{
	}

	// Constructor with a message and an inner exception
	public NoAuthKeyException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

}
