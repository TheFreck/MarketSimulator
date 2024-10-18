﻿namespace MarketSimulator.Server.Entities
{
    public class Company
    {
        public Guid GameId;
        public Guid CompanyId;
        public string Name;
        public List<Asset> Portfolio;
        public int Debt;

        public decimal Value { get => Portfolio.Select(p => p.Value).Sum(); }
    }
}
