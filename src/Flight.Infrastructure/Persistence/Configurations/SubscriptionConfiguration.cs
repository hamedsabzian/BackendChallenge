using Flight.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flight.Infrastructure.Persistence.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("subscriptions")
            .HasKey(a => new { a.AgencyId, a.OriginCityId, a.DestinationCityId });

        builder.Property(a => a.AgencyId)
            .HasColumnName("agency_id");
        
        builder.Property(a => a.OriginCityId)
            .HasColumnName("origin_city_id");
        
        builder.Property(a => a.DestinationCityId)
            .HasColumnName("destination_city_id");
    }
}