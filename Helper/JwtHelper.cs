using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using System;
using System.Linq;

namespace QL_HS.Helper
{
    public class JwtHelper
    {
        public static long GetIdFromToken(IEnumerable<Claim> claims)
        {
            long result = 0L;
            if (claims.Where((Claim e) => e.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault() != null)
            {
                long.TryParse(claims.Where((Claim e) => e.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault().Value, out result);
                Claim claim = claims.Where((Claim e) => e.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault();
            }
            return result;
        }

        public static string GetToken(HttpRequest Request)
        {
            try
            {
                string[] array = Request.Headers["Authorization"].ToString().Split(" ");
                return array[1];
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string GetCurrentInformation(ClaimsPrincipal User, Func<Claim, bool> func)
        {
            Claim claim = User.Claims.Where(func).FirstOrDefault();
            if (claim != null)
            {
                return claim.Value;
            }
            return "";
        }

        public static long GetCurrentInformationLong(ClaimsPrincipal User, Func<Claim, bool> func)
        {
            long result = 0L;
            Claim claim = User.Claims.Where(func).FirstOrDefault();
            if (claim != null)
            {
                long.TryParse(claim.Value, out result);
            }
            return result;
        }
    }
}
