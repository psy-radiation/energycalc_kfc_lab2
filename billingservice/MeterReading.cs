using System;

namespace billingservice;

public class MeterReading
{
    public int Id { get; set; }
    public string MeterId { get; set; }
    public double PreviousDay { get; set; }
    public double PreviousNight { get; set; }
    public double CurrentDay { get; set; }
    public double CurrentNight { get; set; }
    public DateTime ReadingDate { get; set; }
}