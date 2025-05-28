using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyAPI.Domain.Entities
{
    public class Company : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string Ticker { get; private set; } = string.Empty;
        public string Exchange { get; private set; } = string.Empty;
        public string Isin { get; private set; } = string.Empty;
        public string? Website { get; private set; } = null;

        private Company() { }
        public Company(string name, string ticker, string exchange, string isin, string? website = null)
        {

            Name = name.Trim();
            Ticker = ticker.Trim().ToUpper();
            Exchange = exchange.Trim();
            Isin = isin.Trim().ToUpper();
            Website = website;
        }

        public void Update(string name, string ticker, string exchange, string isin, string? website = null)
        {

            Name = name.Trim();
            Ticker = ticker.Trim().ToUpper();
            Exchange = exchange.Trim();
            Isin = isin.Trim().ToUpper();
            Website = website;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
