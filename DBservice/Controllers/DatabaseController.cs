using Microsoft.AspNetCore.Mvc;
using System.Linq;

[Route("api/database")]
[ApiController]
public class DatabaseController : ControllerBase
{
    [HttpGet]
    public IActionResult GetHistory()
    {
        using var db = new DatabaseContext();
        return Ok(db.MeterRecords.ToList());
    }

    [HttpPost]
    public IActionResult SaveRecord([FromBody] MeterRecord record)
    {
        using var db = new DatabaseContext();
        db.MeterRecords.Add(record);
        db.SaveChanges();
        return Ok(new { Message = "Saved to DB" });
    }
}