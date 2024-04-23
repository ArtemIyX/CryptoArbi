
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

        public static IActionResult InteralServerErrorData(this ControllerBase controller, Exception ex)
        {
            return controller.StatusCode((int)HttpStatusCode.InternalServerError, new BasicResponse()
            {
                Code = (int)HttpStatusCode.InternalServerError,
                Message = ex.Message,
                Data = null,
                Success = false
            });
        }
    }
}
