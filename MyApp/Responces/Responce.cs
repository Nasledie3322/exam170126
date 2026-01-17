using System;
using System.Collections.Generic;
using System.Net;

namespace WebApi.Responses;

public class Response<T>
{
    public int StatusCode { get; set; } = 0;
    public List<string> Description { get; set; } = new List<string>();
    public T? Data { get; set; }
    public List<T>? Datas { get; set; }

    public Response(HttpStatusCode httpStatusCode, string message, T data)
    {
        StatusCode = (int)httpStatusCode;
        Description.Add(message);
        Data = data;
    }

    public Response(HttpStatusCode httpStatusCode, string message, List<T> data)
    {
        StatusCode = (int)httpStatusCode;
        Description.Add(message);
        Datas = data;
    }

    public Response(HttpStatusCode httpStatusCode, string message)
    {
        StatusCode = (int)httpStatusCode;
        Description.Add(message);
    }

    public Response(HttpStatusCode httpStatusCode, List<string> message)
    {
        StatusCode = (int)httpStatusCode;
        Description = message;
    }

    public Response(HttpStatusCode httpStatusCode, List<string> message, T data)
    {
        StatusCode = (int)httpStatusCode;
        Description = message;
        Data = data;
    }
}
