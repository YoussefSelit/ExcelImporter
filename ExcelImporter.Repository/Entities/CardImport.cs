using System;
using System.Collections.Generic;
using System.Text;

namespace ExcelImporter.Repository.Entities
{
    public class CardImport
    {
        //Id will be auto generated 
        public int Id { get; set; }

        public string? ClientBranch { get; set; }

        public string? CardBranch { get; set; }

        public string? Pan { get; set; }

        public string? Mbr { get; set; }

        public string? CustomerName { get; set; }

        public string? ClientId { get; set; }

        public string? EmbossingName { get; set; }

        public string? CurrentCmsStatus { get; set; }

        public string? CurrentOnlineStatus { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public DateTime? ActivationDate { get; set; }

        public DateTime? ClosingDate { get; set; }

        public string? InternalAcc { get; set; }

        public string? ExternalAcc { get; set; }

        public string? AccountCurrency { get; set; }

        public string? MobileNumber { get; set; }

        public string? PassportNumber { get; set; }

        public decimal? CurrentBalance { get; set; }

        public decimal? CreditLimit { get; set; }

        public string? LimitCurrency { get; set; }

        public decimal? OnHold { get; set; }

        public decimal? ArrestedAmount { get; set; }

        public string? CardType { get; set; }

        public string? LimitGroup { get; set; }

        public string? FinancialProfile { get; set; }

        public string? ClerkCode { get; set; }

        public string? IssuanceReason { get; set; }
        
        public string? CardProductName { get; set; }

        public string? ExternalCode { get; set; }

        public string? IssuancePriority { get; set; }

        public string? PersonalCode { get; set; }

        public string? ContractNumber { get; set; }

        public string? Gender { get; set; }

        public DateTime? Birthday { get; set; }

        public string? ContactAddress { get; set; }

        public string? Contactless { get; set; }

    }
}
