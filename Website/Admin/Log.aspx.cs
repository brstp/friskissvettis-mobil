using FriskisSvettisLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_Log : System.Web.UI.Page
{
    private Statistics _searchStatistics = new Statistics();
    protected Statistics SearchStatistics
    {
        get
        {
            return _searchStatistics;
        }
        private set
        {
            _searchStatistics = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        CheckTimeCheckboxes();

        // authenticate
        if (!AdminAuth.Authenticated)
        {
            // failed
            Response.Redirect("Login.aspx");
        }

        if (!Page.IsPostBack)
        {
            // Dont load automaticly
            // ShowDate(DateTime.Now, DateTime.Now);
            List<Facility> facilities = new List<Facility>() {new Facility(){Name = "Alla", Id=""}};
            facilities.AddRange(FacilityHelper.GetAll());
            drpFacilities.DataSource = facilities;
            drpFacilities.DataTextField = "Name";
            drpFacilities.DataValueField = "Name";
            drpFacilities.DataBind();
        }
    }

    protected void btnGetLogs_Click(object sender, EventArgs e)
    {

        DateTime dateFrom = DateTime.Parse(txtTimestampFrom.Text.Trim());
        DateTime dateTo = DateTime.Parse(txtTimestampTo.Text.Trim());

        CheckTimeCheckboxes();

        if (chkStatistics.Checked)
        {
            ShowStatistics(dateFrom, dateTo);
        }
        else
        {
            ShowDate(dateFrom, dateTo);
        }
    }

    private void ShowStatistics(DateTime dateFrom, DateTime dateTo)
    {        
        txtTimestampFrom.Text = DateTimeToString(dateFrom);
        txtTimestampTo.Text = DateTimeToString(dateTo);

        Logger logger = new Logger();
        string facility = drpFacilities.SelectedValue.Trim() == "Alla" ? "" : drpFacilities.SelectedValue.Trim();
        SearchStatistics = logger.GetStatisticsLogFrom(dateFrom, dateTo, txtUsername.Text.Trim(), facility, drpAction.SelectedValue.Trim(), drpType.SelectedValue.Trim(), drpProvider.SelectedValue.Trim());
    }

    private void ShowDate(DateTime dateFrom, DateTime dateTo)
    {
        txtTimestampFrom.Text = DateTimeToString(dateFrom);
        txtTimestampTo.Text = DateTimeToString(dateTo);

        Logger logger = new Logger();
        string facility = drpFacilities.SelectedValue.Trim() == "Alla" ? "" : drpFacilities.SelectedValue.Trim();
        List<LogItemEntity> logs = logger.GetLogFrom(dateFrom, dateTo, txtUsername.Text.Trim(), "Norrköping", drpAction.SelectedValue.Trim(), drpType.SelectedValue.Trim(), drpProvider.SelectedValue.Trim());

        foreach (LogItemEntity entry in logs)
        {
            if (string.IsNullOrEmpty(entry.Message))
            {
                entry.Message = "";
            }
        }

        // statistics
        SearchStatistics = new Statistics(logs);

        // items
        rptrLogs.DataSource = logs;
        rptrLogs.DataBind();

        // messages 
        rptrMessages.DataSource = logs
          .GroupBy(l => l.Message)
          .Select(g => new
          {
              Type = g.First().Type,
              Message = g.Key,
              Count = g.Count()
          }).OrderByDescending(m => m.Count);
        rptrMessages.DataBind();
    }

    private void CheckTimeCheckboxes()
    {
        if (string.IsNullOrEmpty(txtTimestampFrom.Text))
        {
            txtTimestampFrom.Text = DateTimeToString(CleanHour(DateTime.Now));
        }

        if (string.IsNullOrEmpty(txtTimestampTo.Text))
        {
            txtTimestampTo.Text = DateTimeToString(CleanHour(DateTime.Now.AddHours(1)));
        }
    }

    private string DateTimeToString(DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }

    private DateTime CleanHour(DateTime dateTime)
    {
        dateTime = dateTime.AddMinutes(-dateTime.Minute);
        dateTime = dateTime.AddSeconds(-dateTime.Second);
        dateTime = dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerSecond));

        return dateTime;
    }
}