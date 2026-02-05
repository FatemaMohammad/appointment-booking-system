using System;

namespace BookingSystem.Domain.Entities;

public class BusinessHours
{
    public int Id { get; set; }
    //0 = Sunday, 1 = Monday, ..., 6 = Saturday
    public int DayOfWeek { get; set; }
    public TimeSpan OpenTime { get; set; }
    public TimeSpan CloseTime { get; set; }
}
//timespan because business hours repeat every week 