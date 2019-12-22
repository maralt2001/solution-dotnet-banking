using System;

namespace ApiDataService
{
    public interface IBankingAccount
    {
        string _id { get; set; }
        AccountChanged changed { get; set; }
        DateTime? createdAt { get; set; }
        string firstname { get; set; }
        bool? isActive { get; set; }
        string lastname { get; set; }
        
    }
}