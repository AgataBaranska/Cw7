using Cw7.Models;
using Cw7.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Cw7.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogService _log;
        public LoggingMiddleware(RequestDelegate next, ILogService log)
        {
            _next = next;
            _log = log;

        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();
            if (context.Request != null)
            {
                string path = context.Request.Path;//api/students
                string method = context.Request.Method;//GET, POST
                string queryString = context.Request?.QueryString.ToString();
                string bodyStr = "";


                using (var reader = new StreamReader(context.Request.Body, System.Text.Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = await reader.ReadToEndAsync();
                    //zapisać do pliku
                    _log.Log(new LoggingData { Date = DateTime.Now, Method = method, Path = path, Body = bodyStr, QueryString = queryString });
                }



            }


            if (_next != null)
            {
                await _next(context);
            }
        }


    }
}
