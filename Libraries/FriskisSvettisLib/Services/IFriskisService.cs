using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for IFriskisService
/// </summary>
public interface IFriskisService
{
    bool ScheduleLoginNeeded { get; set; }
    FriskisServiceType ServiceType { get; }
    string ServiceUrl { get; set; }

    Member Login(string username, string password, Facility facility);
    bool Logout();

    List<ScheduleItem> GetScheduleItems(Facility mainFacility, List<Facility> facilities, Member member, string activity, string activityType, string instructor, DateTime From, DateTime To);

    ScheduleItem GetScheduleItem(string id, Facility facility, DateTime dateTime);
    List<ScheduleItem> GetBookings(Member member);
    ScheduleItem GetBookedItem(string id, string bookId, Facility facility);

    Result Book(string id, Facility facility, DateTime dateTime);
    Result Unbook(string id, string bookingId, Facility facility, DateTime dateTime);

    /// <summary>
    /// Cached 
    /// </summary>
    /// <returns></returns>
    List<Facility> GetAllFacilities();
    Facility CachedFacility { get; set; }
    List<Activity> CachedActivities { get; set; }
    List<Instructor> CachedInstructors { get; set; }
    List<string> CachedRooms { get; set; }

    List<ScheduleItem> GetScheduleStandyItems(Member member);
    Result BookStandby(string activityId, Facility facility, DateTime dateTime);
    Result UnbookStandby(string bookId, Facility facility);
}