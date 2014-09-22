using FriskisSvettisLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_Log : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // authenticate
        if (!AdminAuth.Authenticated)
        {
            // failed
            Response.Redirect("Login.aspx");
        }

        if (!Page.IsPostBack)
        {
            ShowDate(DateTime.Now, DateTime.Now);
        }
    }

    protected void btnGetLogs_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtTimestampFrom.Text))
        {
            txtTimestampFrom.Text = DateTime.Now.ToShortDateString();
        }

        if (string.IsNullOrEmpty(txtTimestampTo.Text))
        {
            txtTimestampTo.Text = DateTime.Now.ToShortDateString();
        }

        ShowDate(DateTime.Parse(txtTimestampFrom.Text.Trim()), DateTime.Parse(txtTimestampTo.Text.Trim()));
    }

    private void ShowDate(DateTime dateFrom, DateTime dateTo)
    {
        txtTimestampFrom.Text = dateFrom.ToShortDateString();
        txtTimestampTo.Text = dateTo.ToShortDateString();

        Logger logger = new Logger();
        List<LogItemEntity> logs = logger.GetLogFrom(dateFrom.Date, dateTo.AddDays(1).Date, txtUsername.Text.Trim(), drpType.SelectedValue.Trim(), drpProvider.SelectedValue.Trim());

        foreach (LogItemEntity entry in logs)
        {
            if (string.IsNullOrEmpty(entry.Message))
            {
                entry.Message = "";
            }
        }

        rptrLogs.DataSource = logs;
        rptrLogs.DataBind();
    }
}