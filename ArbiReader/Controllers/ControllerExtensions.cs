
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

        public static IActionResult BadData(this ControllerBase controller,
            string msg)
        {
            return controller.BadRequest(new BasicResponse()
            {
                Code = (int)HttpStatusCode.BadRequest,
                Message = msg,
                Data = null,
                Success = false
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

        public static IActionResult NotFoundData(this ControllerBase controller, string msg)
        {
            return controller.StatusCode((int)HttpStatusCode.NotFound, new BasicResponse()
            {
                Code = (int)HttpStatusCode.NotFound,
                Message = msg,
                Data = null,
                Success = false
            });
        }

        public static IActionResult NoContentData(this ControllerBase controller, string msg)
        {
            return controller.StatusCode((int)HttpStatusCode.NoContent, new BasicResponse()
            {
                Code = (int)HttpStatusCode.NoContent,
                Message = msg,
                Data = null,
                Success = false
            });
        }
    }
}
