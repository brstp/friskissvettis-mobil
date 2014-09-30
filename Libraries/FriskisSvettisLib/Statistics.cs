using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FriskisSvettisLib
{
    public class Statistics
    {
        public int Bookings { get; set; }
        public int FailedBookings { get; set; }

        public int UnBookings { get; set; }
        public int FailedUnBookings { get; set; }

        public int StandbyBookings { get; set; }
        public int FailedStandbyBookings { get; set; }

        public int StandbyUnBookings { get; set; }
        public int FailedStandbyUnBookings { get; set; }

        public int Total { get; set; }
        public int TotalErrors { get; set; }

        public Statistics()
        {

        }

        public Statistics(List<LogItemEntity> entities)
        {
            // Booking
            Bookings = entities.Where(i => i.Action.Equals(FriskisSvettisLib.LogItemAction.Book.ToString()) && i.Type.Equals(FriskisSvettisLib.LogItemType.Error.ToString())).Count();
            FailedBookings = entities.Where(i => i.Action.Equals(FriskisSvettisLib.LogItemAction.Book.ToString())).Count();

            // UnBooking
            UnBookings = entities.Where(i => i.Action.Equals(FriskisSvettisLib.LogItemAction.Unbook.ToString()) && i.Type.Equals(FriskisSvettisLib.LogItemType.Error.ToString())).Count();
            FailedUnBookings = entities.Where(i => i.Action.Equals(FriskisSvettisLib.LogItemAction.Unbook.ToString())).Count();

            // Standby booking
            StandbyBookings = entities.Where(i => i.Action.Equals(FriskisSvettisLib.LogItemAction.BookStandby.ToString()) && i.Type.Equals(FriskisSvettisLib.LogItemType.Error.ToString())).Count();
            FailedStandbyBookings = entities.Where(i => i.Action.Equals(FriskisSvettisLib.LogItemAction.BookStandby.ToString())).Count();

            // Standby unbooking
            StandbyUnBookings = entities.Where(i => i.Action.Equals(FriskisSvettisLib.LogItemAction.UnbookStandby.ToString()) && i.Type.Equals(FriskisSvettisLib.LogItemType.Error.ToString())).Count();
            FailedStandbyUnBookings = entities.Where(i => i.Action.Equals(FriskisSvettisLib.LogItemAction.UnbookStandby.ToString())).Count();

            Total = entities.Count;
            TotalErrors = entities.Where(i => i.Type.Equals(FriskisSvettisLib.LogItemType.Error.ToString())).Count();
        }
    }
}
