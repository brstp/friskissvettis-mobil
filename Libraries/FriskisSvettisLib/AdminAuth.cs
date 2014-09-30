using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoMA.Helpers;

namespace FriskisSvettisLib
{
    public class AdminAuth
    {
        private static Guid valid = new Guid("BE651F81-7C25-4F87-B6D5-C965DE1E1067");

        public static Guid AuthGuid
        {
            get
            {
                return ContextHelper.GetGuid("auth", Guid.Empty);
            }
        }

        public static bool Authenticated
        {
            get
            {
                return valid.Equals(AuthGuid);
            }
        }
    }
}
