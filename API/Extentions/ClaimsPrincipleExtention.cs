using System;
using System.Security.Claims;

namespace API.Extentions;

public static class ClaimsPrincipleExtention
{
    public static string GetUserName(this ClaimsPrincipal user)
    {
        var userName = user.FindFirstValue(ClaimTypes.Name) ?? throw new Exception("User name claim not found");

        return userName;
    }

    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userId = Guid.Parse(user.FindFirstValue("nameId") ?? throw new Exception("User ID claim not found"));

        return userId;
    }
}
