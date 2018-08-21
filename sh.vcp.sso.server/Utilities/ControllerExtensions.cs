using System;
using Microsoft.AspNetCore.Mvc;

namespace sh.vcp.sso.server.Utilities
{
    public static class ControllerExtensions
    {
        public static IActionResult ServerError(this Controller controller, Exception ex) {
            return new InternalServerErrorResult(ex);
        }

        private class InternalServerErrorResult : ObjectResult
        {
            public InternalServerErrorResult(Exception ex) : base(ex) {
                this.StatusCode = 500;
            }
        }
    }
}