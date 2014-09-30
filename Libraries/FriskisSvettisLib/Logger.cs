using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;

namespace FriskisSvettisLib
{
    public enum LogItemProvider
    {
        Pastell,
        BRP,
        Defendo,
        DLSoftware,
    }

    public enum LogItemAction
    {
        Book,
        Unbook,
        BookStandby,
        UnbookStandby
    }

    public enum LogItemType
    {
        Debug,
        Information,
        Warning,
        Error,
    }

    public class Logger
    {
        //private const string LOGGER_TABLE_NAME = "fslogger";
        //private string DataConnectionString = "DefaultEndpointsProtocol=https;AccountName=momadev;AccountKey=w1J+43leThaVd+TSgQHVXXBPozRWEI4d4oDTVFdpJPjNYhNcChD+YIgsWAyHgtlDzhqqrPJeZj0rBL5B/Wg2OA==";
        //private Mutex mutex = new Mutex();
        //private TableServiceContext context = null;

        /// <summary>
        /// Checks the web.config if logging should be used
        /// </summary>
        private static bool UseLogging
        {
            get
            {

                return false;
                //bool useLogging = false;

                //if (WebConfigurationManager.AppSettings["UseLogging"] != null)
                //{
                //    useLogging = WebConfigurationManager.AppSettings["UseLogging"].Equals("true");
                //}

                //return useLogging;
            }
        }

        public Logger()
        {
            //var account = CloudStorageAccount.Parse(DataConnectionString);
            //var client = account.CreateCloudTableClient();
            //var table = client.GetTableReference(LOGGER_TABLE_NAME);
            //table.CreateIfNotExists();
            //context = client.GetTableServiceContext();
        }

        /// <summary>
        /// Adds a log item to the logger table.
        /// </summary>
        /// <param name="source">Source of the log item.</param>
        /// <param name="message">Description of the log entry.</param>
        /// <param name="type">Type (severity) of the log entry.</param>
        public void Log(string source, LogItemProvider provider, string facility, LogItemAction action, string username, string message, LogItemType type)
        {
            if (UseLogging)
            {
                //try
                //{
                //    mutex.WaitOne();
                //    LogItemEntity item = new LogItemEntity(source, provider, facility, action, username, message, type);
                //    context.AddObject(LOGGER_TABLE_NAME, item);
                //    context.SaveChangesWithRetries();
                //    // context.BeginSaveChangesWithRetries(new AsyncCallback((IAsyncResult) => { }), null);
                //}
                //catch (Exception ex)
                //{
                //    string msg = "Failed to log entry (source = " + source + ", message = " + message + ", type = " + type + ", error = " + ex.Message + ").";
                //    Debug.WriteLine(msg);
                //}
                //finally
                //{
                //    mutex.ReleaseMutex();
                //}
            }
        }

        /// <summary>
        /// Returns all log items starting at the date that is sent to the function. 
        /// With timezone fix
        /// </summary>
        private IQueryable<LogItemEntity> GetIqueryableLogFrom(DateTime dateFrom, DateTime dateTo, string username, string facilityName, string action, string type, string provider)
        {
            //// fix timezone to utc
            //dateFrom = dateFrom.ToUniversalTime();
            //dateTo = dateTo.ToUniversalTime();

            IQueryable<LogItemEntity> entities = null;

            //try
            //{
            //    mutex.WaitOne();
            //    // entities = context.CreateQuery<LogItemEntity>(LOGGER_TABLE_NAME).Where(i => i.Timestamp.CompareTo(from) > 0).ToList();
            //    entities = context.CreateQuery<LogItemEntity>(LOGGER_TABLE_NAME).Where(i => i.Timestamp.CompareTo(dateFrom) > 0 && i.Timestamp.CompareTo(dateTo) < 0 && (i.Username.Equals(username) || string.IsNullOrWhiteSpace(username)) && (i.Facility.Equals(facilityName) || string.IsNullOrWhiteSpace(facilityName)) && (i.Action.Equals(action) || string.IsNullOrWhiteSpace(action)) && (i.Type.Equals(type) || string.IsNullOrWhiteSpace(type)) && (i.Provider.Equals(provider) || string.IsNullOrWhiteSpace(provider)));
            //}
            //catch (Exception ex)
            //{
            //}
            //finally
            //{
            //    mutex.ReleaseMutex();
            //}

            return entities;
        }

        /// <summary>
        /// Returns all log items starting at the date that is sent to the function. 
        /// </summary>
        public List<LogItemEntity> GetLogFrom(DateTime dateFrom, DateTime dateTo, string username, string facilityName, string action, string type, string provider)
        {
            List<LogItemEntity> entities = GetIqueryableLogFrom(dateFrom, dateTo, username, facilityName, action, type, provider).ToList();

            //// fix timezones from utc
            //foreach (LogItemEntity entity in entities)
            //{
            //    entity.Timestamp = entity.Timestamp.ToLocalTime();
            //}

            return entities;
        }

        /// <summary>
        /// Returns all log items starting at the date that is sent to the function. 
        /// </summary>
        public Statistics GetStatisticsLogFrom(DateTime dateFrom, DateTime dateTo, string username, string facilityName, string action, string type, string provider)
        {
            // NO COUNT() FUNCTION IN AZURE! =(
            return new Statistics();

            //IQueryable<LogItemEntity> entities = GetIqueryableLogFrom(dateFrom, dateTo, username,facilityName, action, type, provider);
            //Statistics statistics = new Statistics();

            //// Booking
            //statistics.Bookings = entities.Where(i => i.Action.Equals(FriskisSvettisLib.LogItemAction.Book.ToString())).Count();
            //statistics.FailedBookings = entities.Where(i => i.Action.Equals(FriskisSvettisLib.LogItemAction.Book.ToString()) && i.Type.Equals(FriskisSvettisLib.LogItemType.Error.ToString())).Count();

            //// UnBooking
            //statistics.UnBookings = entities.Where(i => i.Action.Equals(FriskisSvettisLib.LogItemAction.Unbook.ToString())).Count();
            //statistics.FailedUnBookings = entities.Where(i => i.Action.Equals(FriskisSvettisLib.LogItemAction.Unbook.ToString()) && i.Type.Equals(FriskisSvettisLib.LogItemType.Error.ToString())).Count();

            //// Standby booking
            //statistics.StandbyBookings = entities.Where(i => i.Action.Equals(FriskisSvettisLib.LogItemAction.BookStandby.ToString())).Count();
            //statistics.FailedStandbyBookings = entities.Where(i => i.Action.Equals(FriskisSvettisLib.LogItemAction.BookStandby.ToString()) && i.Type.Equals(FriskisSvettisLib.LogItemType.Error.ToString())).Count();

            //// Standby unbooking
            //statistics.StandbyUnBookings = entities.Where(i => i.Action.Equals(FriskisSvettisLib.LogItemAction.UnbookStandby.ToString())).Count();
            //statistics.FailedStandbyUnBookings = entities.Where(i => i.Action.Equals(FriskisSvettisLib.LogItemAction.UnbookStandby.ToString()) && i.Type.Equals(FriskisSvettisLib.LogItemType.Error.ToString())).Count();

            //// Total count
            //statistics.Total = entities.Count();

            // return statistics;
        }

        /// <summary>
        /// Returns all log items at a specific date
        /// </summary>
        /// <param name="date">Date to search for log at</param>
        /// <param name="Username">Username for the log, leave empty to ignore</param>
        /// <param name="Type">Type of log, [Debug | Information | Warning | Error], Empty == All</param>
        /// <returns></returns>
        public List<LogItemEntity> GetLogAt(DateTime date, string Username, string Type)
        {
            List<LogItemEntity> entities = new List<LogItemEntity>();

            //try
            //{
            //    mutex.WaitOne();
            //    entities = context.CreateQuery<LogItemEntity>(LOGGER_TABLE_NAME).Where(i => i.Timestamp.CompareTo(date.Date) > 0 && i.Timestamp.CompareTo(date.Date.AddDays(1)) < 0 && (i.Username.Equals(Username) || string.IsNullOrWhiteSpace(Username)) && (i.Type.Equals(Type) || string.IsNullOrWhiteSpace(Type))).ToList();
            //}
            //catch (Exception ex)
            //{
            //}
            //finally
            //{
            //    mutex.ReleaseMutex();
            //}

            return entities;
        }
    }

    public class LogItemEntity : TableServiceEntity
    {
        public LogItemEntity()
        {
        }

        public LogItemEntity(string source, LogItemProvider provider, string facility, LogItemAction action, string username, string message, LogItemType type)
        {
            //this.PartitionKey = source;
            //this.RowKey = DateTime.UtcNow.Ticks.ToString();
            //this.Type = type.ToString();
            //this.Provider = provider.ToString();
            //this.Facility = facility;
            //this.Action = action.ToString();
            //this.Username = username;
            //this.Message = message;
        }

        public string Type { get; set; }

        public string Provider { get; set; }

        public string Facility { get; set; }

        public string Action { get; set; }

        public string Username { get; set; }



        public string Message { get; set; }

        public string Time
        {
            get
            {
                return Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
            }
            set
            {
            }
        }
    }
}
