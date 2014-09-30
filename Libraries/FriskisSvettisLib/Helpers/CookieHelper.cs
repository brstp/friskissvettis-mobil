using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

public class CookieHelper
{
    #region Helpers

        public static HttpCookie GetCookie(string name)
        {
            // Get Cookie
            HttpCookie cookie = null;
            if (HttpContext.Current.Request.Cookies[name] != null)
            {
                cookie = HttpContext.Current.Request.Cookies[name];
            }
            return cookie;
        }

        public static void SetCookie(string name, string value)
        {
            // Set Cookie
            HttpCookie cookie = new HttpCookie(name, value);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static void SetCookie(string name, Dictionary<string, string> values)
        {
            // Set Cookie
            HttpCookie cookie = new HttpCookie(name);
            cookie.Expires = DateTime.MaxValue;

            foreach (KeyValuePair<string, string> keyValuePair in values)
            {
                cookie.Values.Add(keyValuePair.Key, keyValuePair.Value);
            }

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static void RemoveCookie(string name)
        {
            // Remove Cookie
            if (HttpContext.Current.Request.Cookies[name] != null)
            {
                HttpCookie cookie = new HttpCookie(name);
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

    #endregion

    #region Login

        public static void SetLoginCookie(string username, string password, string facilityId)
        {
            Dictionary<string, string> values = new Dictionary<string, string>()
            {
                {"FACILITY", facilityId},
                {"USERNAME", username},
                {"PASSWORD", password}
            };

            CookieHelper.SetCookie("AUTH", values);
        }

        public static HttpCookie CheckLoginCookie()
        {
            string AUTH = "AUTH";

            HttpCookie cookie = CookieHelper.GetCookie(AUTH);

            if (CookieHelper.GetCookie(AUTH) != null)
            {
                // check data
                string facilityId = CookieHelper.GetCookie(AUTH).Values["FACILITY"];
                string username = CookieHelper.GetCookie(AUTH).Values["USERNAME"];
                string password = CookieHelper.GetCookie(AUTH).Values["PASSWORD"];

                if (!string.IsNullOrEmpty(facilityId) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    return cookie;
                }
            }

            return null;
        }

    #endregion
}
