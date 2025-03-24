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

        // Получаем предыдущие показания для текущего счетчика
        var previousReading = await _context.MeterReadings
            .Where(r => r.MeterId == data.MeterId)
            .OrderByDescending(r => r.ReadingDate)
            .FirstOrDefaultAsync();

        // Если предыдущие данные есть
        
        if (previousReading != null)
        {
            // Проверка, если текущие показания меньше предыдущих
            if (data.CurrentDay < previousReading.CurrentDay || data.CurrentNight < previousReading.CurrentNight)
            {
                // Отправляем пользователю запрос на подтверждение накрутки
                // Мы возвращаем флаг и показываем текущее и прошлое показание
                return Ok(new
                {
                    MeterId = data.MeterId,
                    CurrentDay = data.CurrentDay,
                    PreviousDay = previousReading.CurrentDay,
                    CurrentNight = data.CurrentNight,
                    PreviousNight = previousReading.CurrentNight,
                    NeedCorrection = true,
                    Message = "Your current readings are lower than previous. Do you want to correct them?"
                });
            }
        }
        else
        {
            // Если предыдущих показаний нет, просто проверяем на минимальные корректные значения
            data.CurrentDay = data.CurrentDay;
            data.CurrentNight = data.CurrentNight;
        }

        // Вычисляем потребление (разницу между текущими и предыдущими показателями)
        double dayUsage = data.CurrentDay - (previousReading?.CurrentDay ?? 0);
        double nightUsage = data.CurrentNight - (previousReading?.CurrentNight ?? 0);

        // Расчет стоимости
        double cost = (dayUsage * DayTariff) + (nightUsage * NightTariff);

        // Обновляем предыдущие значения
        data.PreviousDay = previousReading?.CurrentDay ?? 0;
        data.PreviousNight = previousReading?.CurrentNight ?? 0;

        // Устанавливаем дату показаний
        data.ReadingDate = DateTime.UtcNow;

        // Добавляем новые показания в базу
        _context.MeterReadings.Add(data);
        await _context.SaveChangesAsync();

        // Возвращаем результат с расчетом стоимости
        return Ok(new { MeterId = data.MeterId, Cost = cost , Debik = previousReading?.MeterId ?? "XD"});
    }

    [HttpPost("correct")]
    public async Task<IActionResult> CorrectMeterReading([FromBody] MeterReading data)
    {
        // Метод для обработки подтверждения на накрутку показателей
        var previousReading = await _context.MeterReadings
            .Where(r => r.MeterId == data.MeterId)
            .OrderByDescending(r => r.ReadingDate)
            .FirstOrDefaultAsync();

        double curDay = data.CurrentDay;
        double curNig = data.CurrentNight;

        if (previousReading != null)
        {
            // Если текущее значение меньше предыдущего, корректируем
            if (curDay < previousReading.CurrentDay)
                curDay = previousReading.CurrentDay + DayCorrection;

            if (curNig < previousReading.CurrentNight)
                curNig = previousReading.CurrentNight + NightCorrection;
        }

        // Вычисляем разницу и рассчитываем стоимость
        double dayUsage = curDay - (previousReading?.CurrentDay ?? 0);
        double nightUsage = curNig - (previousReading?.CurrentNight ?? 0);
        double cost = (dayUsage * DayTariff) + (nightUsage * NightTariff);

        // Обновляем предыдущие значения и дату показаний
        data.PreviousDay = previousReading?.CurrentDay ?? 0;
        data.PreviousNight = previousReading?.CurrentNight ?? 0;
        data.ReadingDate = DateTime.UtcNow;

        // Добавляем в базу
        _context.MeterReadings.Add(data);
        await _context.SaveChangesAsync();

        return Ok(new { MeterId = data.MeterId, Corrected = true, Cost = cost });
    }
}
