using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPulse.Infrastructure.Exceptions;
public class NoAuthKeyExeption : Exception
{
	// Optionally, add custom properties
	public int ErrorCode { get; set; } = 0;
	public string ErrorDetails { get; set; } = string.Empty; 

	// Default constructor
	public NoAuthKeyExeption()
	{
	}

	// Constructor with a message
	public NoAuthKeyExeption(string message)
		: base(message)
	{
	}

	// Constructor with a message and an inner exception
	public NoAuthKeyExeption(string message, Exception innerException)
		: base(message, innerException)
	{
	}

}
