using ExcelImporter.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExcelImporter.Repository.Configurations;

public class CardImportConfiguration : IEntityTypeConfiguration<CardImport>
{
    public void Configure(EntityTypeBuilder<CardImport> entity)
    {
        entity.ToTable("CARD_IMPORTS");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
              .HasColumnName("ID")
              .ValueGeneratedOnAdd();

        // String Properties
        entity.Property(e => e.ClientBranch)
              .HasColumnName("CLIENT_BRANCH")
              .HasMaxLength(200);

        entity.Property(e => e.CardBranch)
              .HasColumnName("CARD_BRANCH")
              .HasMaxLength(200);

        entity.Property(e => e.Pan)
              .HasColumnName("PAN")
              .HasMaxLength(200);

        entity.Property(e => e.Mbr)
              .HasColumnName("MBR")
              .HasMaxLength(200);

        entity.Property(e => e.CustomerName)
              .HasColumnName("CUSTOMER_NAME")
              .HasMaxLength(200);

        entity.Property(e => e.ClientId)
              .HasColumnName("CLIENT_ID")
              .HasMaxLength(200);

        entity.Property(e => e.EmbossingName)
              .HasColumnName("EMBOSSING_NAME")
              .HasMaxLength(200);

        entity.Property(e => e.CurrentCmsStatus)
              .HasColumnName("CURRENT_CMS_STATUS")
              .HasMaxLength(200);

        entity.Property(e => e.CurrentOnlineStatus)
              .HasColumnName("CURRENT_ONLINE_STATUS")
              .HasMaxLength(200);

        entity.Property(e => e.InternalAcc)
              .HasColumnName("INTERNAL_ACC")
              .HasMaxLength(200);

        entity.Property(e => e.ExternalAcc)
              .HasColumnName("EXTERNAL_ACC")
              .HasMaxLength(200);

        entity.Property(e => e.AccountCurrency)
              .HasColumnName("ACCOUNT_CURRENCY")
              .HasMaxLength(200);

        entity.Property(e => e.MobileNumber)
              .HasColumnName("MOBILE_NUMBER")
              .HasMaxLength(200);

        entity.Property(e => e.PassportNumber)
              .HasColumnName("PASSPORT_NUMBER")
              .HasMaxLength(200);

        entity.Property(e => e.LimitCurrency)
              .HasColumnName("LIMIT_CURRENCY")
              .HasMaxLength(200);

        entity.Property(e => e.CardType)
              .HasColumnName("CARD_TYPE")
              .HasMaxLength(200);

        entity.Property(e => e.LimitGroup)
              .HasColumnName("LIMIT_GROUP")
              .HasMaxLength(200);

        entity.Property(e => e.FinancialProfile)
              .HasColumnName("FINANCIAL_PROFILE")
              .HasMaxLength(200);

        entity.Property(e => e.ClerkCode)
              .HasColumnName("CLERK_CODE")
              .HasMaxLength(200);

        entity.Property(e => e.IssuanceReason)
              .HasColumnName("ISSUANCE_REASON")
              .HasMaxLength(200);

        entity.Property(e => e.CardProductName)
              .HasColumnName("CARD_PRODUCT_NAME")
              .HasMaxLength(200);

        entity.Property(e => e.ExternalCode)
              .HasColumnName("EXTERNAL_CODE")
              .HasMaxLength(200);

        entity.Property(e => e.IssuancePriority)
              .HasColumnName("ISSUANCE_PRIORITY")
              .HasMaxLength(200);

        entity.Property(e => e.PersonalCode)
              .HasColumnName("PERSONAL_CODE")
              .HasMaxLength(200);

        entity.Property(e => e.ContractNumber)
              .HasColumnName("CONTRACT_NUMBER")
              .HasMaxLength(200);

        entity.Property(e => e.Gender)
              .HasColumnName("GENDER")
              .HasMaxLength(200);

        entity.Property(e => e.ContactAddress)
              .HasColumnName("CONTACT_ADDRESS")
              .HasMaxLength(200);

        entity.Property(e => e.Contactless)
              .HasColumnName("CONTACTLESS")
              .HasMaxLength(200);


        // Decimal Properties
        entity.Property(e => e.CurrentBalance)
              .HasColumnName("CURRENT_BALANCE")
              .HasPrecision(18, 2);

        entity.Property(e => e.CreditLimit)
              .HasColumnName("CREDIT_LIMIT")
              .HasPrecision(18, 2);

        entity.Property(e => e.OnHold)
              .HasColumnName("ON_HOLD")
              .HasPrecision(18, 2);

        entity.Property(e => e.ArrestedAmount)
              .HasColumnName("ARRESTED_AMOUNT")
              .HasPrecision(18, 2);

        // Date Properties
        entity.Property(e => e.CreationDate)
              .HasColumnName("CREATION_DATE");

        entity.Property(e => e.ExpiryDate)
              .HasColumnName("EXPIRY_DATE");

        entity.Property(e => e.ActivationDate)
              .HasColumnName("ACTIVATION_DATE");

        entity.Property(e => e.ClosingDate)
              .HasColumnName("CLOSING_DATE");

        entity.Property(e => e.Birthday)
              .HasColumnName("BIRTHDAY");

    }
}