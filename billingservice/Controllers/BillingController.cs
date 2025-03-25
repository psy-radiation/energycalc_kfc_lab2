using billingservice;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

using System.Linq;
using System.Threading.Tasks;

namespace billingservice;

[Route("api/billing")]
[ApiController]
public class BillingController : ControllerBase
{
    private readonly AddDbContext _context;
    private const double DayTariff = 1.5;
    private const double NightTariff = 0.8;
    private const double DayCorrection = 100;
    private const double NightCorrection = 80;

    public BillingController(AddDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CalculateBill([FromBody] MeterReading data)
    {
        if (data == null || string.IsNullOrEmpty(data.MeterId))
            return BadRequest("Invalid meter data");

        var previousReading = await _context.MeterReadings
            .Where(r => r.MeterId == data.MeterId)
            .OrderByDescending(r => r.ReadingDate)
            .FirstOrDefaultAsync();

        
        if (previousReading != null)
        {
            if (data.CurrentDay < previousReading.CurrentDay || data.CurrentNight < previousReading.CurrentNight)
            {
                return Ok(new
                {
                    MeterId = data.MeterId,
                    CurrentDay = data.CurrentDay,
                    PreviousDay = previousReading.CurrentDay,
                    CurrentNight = data.CurrentNight,
                    PreviousNight = previousReading.CurrentNight,
                    NeedCorrection = true,
                    Message = "Your current rdngs are lower than pre?"
                });
            }
        }
        else
        {
            data.CurrentDay = data.CurrentDay;
            data.CurrentNight = data.CurrentNight;
        }

        double dayUsage = data.CurrentDay - (previousReading?.CurrentDay ?? 0);
        double nightUsage = data.CurrentNight - (previousReading?.CurrentNight ?? 0);

        double cost = (dayUsage * DayTariff) + (nightUsage * NightTariff);
        data.PreviousDay = previousReading?.CurrentDay ?? 0;
        data.PreviousNight = previousReading?.CurrentNight ?? 0;


        data.ReadingDate = DateTime.UtcNow;

        _context.MeterReadings.Add(data);
        await _context.SaveChangesAsync();

        return Ok(new { MeterId = data.MeterId, Cost = cost});
    }

    [HttpPost("correct")]
    public async Task<IActionResult> CorrectMeterReading([FromBody] MeterReading data)
    {

        var previousReading = await _context.MeterReadings
            .Where(r => r.MeterId == data.MeterId)
            .OrderByDescending(r => r.ReadingDate)
            .FirstOrDefaultAsync();

        double curDay = data.CurrentDay;
        double curNig = data.CurrentNight;

        if (previousReading != null)
        {

            if (curDay < previousReading.CurrentDay)
                curDay = previousReading.CurrentDay + DayCorrection;

            if (curNig < previousReading.CurrentNight)
                curNig = previousReading.CurrentNight + NightCorrection;
        }

        double dayUsage = curDay - (previousReading?.CurrentDay ?? 0);
        double nightUsage = curNig - (previousReading?.CurrentNight ?? 0);
        double cost = (dayUsage * DayTariff) + (nightUsage * NightTariff);


        data.PreviousDay = previousReading?.CurrentDay ?? 0;
        data.PreviousNight = previousReading?.CurrentNight ?? 0;
        data.ReadingDate = DateTime.UtcNow;


        _context.MeterReadings.Add(data);
        await _context.SaveChangesAsync();

        return Ok(new { MeterId = data.MeterId, Corrected = true, Cost = cost });
    }
}
