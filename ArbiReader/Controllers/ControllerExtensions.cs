
using ArbiDataLib.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ArbiReader.Controllers
{
    public static class ControllerExtensions
    {
        public static IActionResult OkData(this ControllerBase controller, object data)
        {
            return controller.Ok(new BasicResponse()
            {
                Code = (int)HttpStatusCode.OK,
                Message = "",
                Data = data,
                Success = true
            });
        }
    }
}
