﻿namespace CompanyAPI.Application.Features.Companies.DTOs
{
    public class CompanyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Ticker { get; set; } = string.Empty;
        public string Exchange { get; set; } = string.Empty;
        public string Isin { get; set; } = string.Empty;
        public string? Website { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public CompanyDto()
        {
            
        }

        public CompanyDto(int id, string name, string ticker, string exchange, string isin, string? website, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            Name = name;
            Ticker = ticker;
            Exchange = exchange;
            Isin = isin;
            Website = website;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }
}
