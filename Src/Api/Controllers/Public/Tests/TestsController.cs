using Api.Common.ControllerWrappers;
using Api.Tests.Entities;
using Application.Common.ApiWrapper;
using Application.Common.Definitions.Messages;
using Application.Modules.Users.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Public.Tests;

public class TestsController : PublicBaseController
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    [HttpGet("WeatherForecast")]
    public ActionResult<SuccessResultResponse<IEnumerable<WeatherForecast>>> Get()
    {
        var response = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        });

        return ResultResponse(response, Message<User>.View());
    }

    [HttpGet("Health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            success = true,
            message = "PeopleHub API is running",
            timestamp = DateTimeOffset.UtcNow
        });
    }

}
