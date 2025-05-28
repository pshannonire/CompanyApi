using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyAPI.Domain.Exceptions.Company
{
    public class CompanyDomainException : DomainException
    {
        public CompanyDomainException(string message) : base(message)
        {
        }

        public CompanyDomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
