using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class LoginResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public Member Member { get; set; }

    public LoginResult()
    {

    }

    public LoginResult(Member member, string message, bool success)
    {
        Message = message;
        Success = success;
        Member = Member;
    }
}
