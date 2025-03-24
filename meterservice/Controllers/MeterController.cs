using Microsoft.AspNetCore.Mvc;

[Route("api/meters")]
[ApiController]
public class MeterController : ControllerBase
{
    private static Dictionary<string, (double day, double night)> meters = new();

    [HttpPost]
    public IActionResult UpdateMeter([FromBody] MeterData data)
    {
        if (!meters.ContainsKey(data.MeterId))
            meters[data.MeterId] = (data.Day, data.Night);
        else
            meters[data.MeterId] = (meters[data.MeterId].day + data.Day, meters[data.MeterId].night + data.Night);

        return Ok(new { Message = "Data Updated", meters });
    }
}
public class MeterData
{
    public string MeterId { get; set; }
    public double Day { get; set; }
    public double Night { get; set; }
}