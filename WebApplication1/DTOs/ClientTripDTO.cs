using System;
namespace WebApplication1.DTOs;

public class ClientTripDTO
{
    public int IdTrip { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int RegisteredAt { get; set; }
    public int? PaymentDate { get; set; }
    public bool IsPaid => PaymentDate.HasValue;
}