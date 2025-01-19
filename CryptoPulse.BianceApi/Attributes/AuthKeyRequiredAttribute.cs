using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPulse.BianceApi.Attributes;
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class AuthKeyRequiredAttribute: Attribute
{
}
