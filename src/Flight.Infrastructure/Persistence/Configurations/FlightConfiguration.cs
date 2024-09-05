using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flight.Infrastructure.Persistence.Configurations;

public class FlightConfiguration : IEntityTypeConfiguration<Domain.Entities.Flight>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Flight> builder)
    {
        builder.ToTable("flights")
            .HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedNever()
            .HasColumnName("flight_id");

        builder.Property(a => a.RouteId)
            .HasColumnName("route_id");

        builder.Property(a => a.DepartureTime)
            .HasColumnName("departure_time");

        builder.Property(a => a.ArrivalTime)
            .HasColumnName("arrival_time");

        builder.Property(a => a.AirlineId)
            .HasColumnName("airline_id");

        builder.HasOne(a => a.Route)
            .WithMany()
            .HasForeignKey(a => a.RouteId);

        builder.HasIndex(a => new { a.RouteId, a.DepartureTime });
    }
}