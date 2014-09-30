using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Result
{
    public bool Success { get; set; }
    public string Message { get; set; }

    public Result()
    {

    }

    public Result(string message, bool success)
    {
        Message = message;
        Success = success;
    }
}
