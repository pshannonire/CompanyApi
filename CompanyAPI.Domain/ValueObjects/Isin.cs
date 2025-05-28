using CompanyAPI.Domain.Exceptions.Company;
using System.Text.RegularExpressions;

namespace CompanyAPI.Domain.ValueObjects
{
    public class Isin : IEquatable<Isin>
    {
        private static readonly Regex IsinRegex = new(@"^[A-Za-z]{2}[A-Za-z0-9]{10}$", RegexOptions.Compiled);

        public string Value { get; }

        public Isin(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new CompanyDomainException("ISIN cannot be empty");

            if (value.Length != 12)
                throw new CompanyDomainException("ISIN must be exactly 12 characters long");

            if (!IsinRegex.IsMatch(value))
                throw new CompanyDomainException("ISIN must start with 2 letters followed by 10 alphanumeric characters");

            Value = value.ToUpperInvariant();
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Isin);
        }

        public bool Equals(Isin? other)
        {
            return other is not null && Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }

        public static implicit operator string(Isin isin)
        {
            return isin.Value;
        }

        public static bool operator ==(Isin? left, Isin? right)
        {
            return EqualityComparer<Isin>.Default.Equals(left, right);
        }

        public static bool operator !=(Isin? left, Isin? right)
        {
            return !(left == right);
        }
    }
}
