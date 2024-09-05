using Flight.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flight.Infrastructure.Persistence.Configurations;

public class RouteConfiguration : IEntityTypeConfiguration<Route>
{
    public void Configure(EntityTypeBuilder<Route> builder)
    {
        builder.ToTable("routes")
            .HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedNever()
            .HasColumnName("route_id");

        builder.Property(a => a.OriginCityId)
            .HasColumnName("origin_city_id");

        builder.Property(a => a.DestinationCityId)
            .HasColumnName("destination_city_id");

        builder.Property(a => a.DepartureDate)
            .HasColumnName("departure_date")
            .HasColumnType("INTEGER");

        builder.HasIndex(a => new { a.OriginCityId, a.DestinationCityId });
    }
}