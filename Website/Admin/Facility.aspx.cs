using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MoMA.Helpers;
using FriskisSvettisLib.Helpers;

public partial class Admin_Facility : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!FacilityHelper.IsLoggedIn)
        {
            Response.Redirect("Login.aspx");
        } 

        if (!Page.IsPostBack)
        {
            // get local id of facility to edit
            Guid LocalId = ContextHelper.GetGuid("id", Guid.Empty);

            // load facility
            List<Facility> facilities = new List<Facility>();
            DatabaseEntities entities = new DatabaseEntities();
            FacilityData facilityData = entities.FacilityDatas.Where(f => f.LocalId.Equals(LocalId)).FirstOrDefault();

            // auth
            if (facilityData != null && FacilityHelper.LoggedInFacility.LocalId == LocalId)
            {
                txtName.Text = facilityData.Name;
                txtHtmlInfo.Text = facilityData.HtmlInfo;
                txtAbout.Text = facilityData.About;
                txtNews.Text = facilityData.News;
                txtOffers.Text = facilityData.Offers;
                txtPhone.Text = facilityData.Phone;
                txtEmail.Text = facilityData.Email;
                txtAddress.Text = facilityData.Address;
                txtZipcode.Text = facilityData.Zipcode;
                txtCity.Text = facilityData.City;

                txtUsernameLabel.Text = facilityData.UsernameLabel;
                txtPasswordLabel.Text = facilityData.PasswordLabel;

                txtScheduleLength.Text = (facilityData.ScheduleLength >= 0 ? facilityData.ScheduleLength.ToString() : "");
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
    }

    protected void btnLogout_OnClick(object sender, EventArgs e)
    {
        FacilityHelper.LoggedInFacility = null;
        Response.Redirect("Login.aspx");
    }

    protected void btnSave_OnClick(object sender, EventArgs e)
    {
        // hide all messages
        HideMessages();

        // get local id of facility to edit
        Guid LocalId = ContextHelper.GetGuid("id", Guid.Empty);

        // load facility
        List<Facility> facilities = new List<Facility>();
        DatabaseEntities entities = new DatabaseEntities();
        FacilityData facilityData = entities.FacilityDatas.Where(f => f.LocalId.Equals(LocalId)).FirstOrDefault();

        // make sure that the logged in account has this facility
        if (facilityData != null && FacilityHelper.LoggedInFacility.LocalId == LocalId)
        {
            facilityData.Name = txtName.Text;
            facilityData.HtmlInfo = txtHtmlInfo.Text;
            facilityData.About = txtAbout.Text;
            facilityData.News = txtNews.Text;
            facilityData.Offers = txtOffers.Text;
            facilityData.Phone = txtPhone.Text;
            facilityData.Email = txtEmail.Text;
            facilityData.Address = txtAddress.Text;
            facilityData.Zipcode = txtZipcode.Text;
            facilityData.City = txtCity.Text;

            facilityData.UsernameLabel = txtUsernameLabel.Text;
            facilityData.PasswordLabel = txtPasswordLabel.Text;

            int scheduleLength = -1;

            if (!int.TryParse(txtScheduleLength.Text, out scheduleLength))
            {
                scheduleLength = -1;
            }

            facilityData.ScheduleLength = scheduleLength;

            // save facility
            try
            {
                entities.SaveChanges();
            }
            catch
            {
                // show to user that the save was successfull
                ShowErrorMessage();
                return;
            }

            // reset cache
            FacilityHelper.Cached = null;

            // show to user that the save was successfull
            ShowSuccessMessage();
        }
    }

    public void ShowErrorMessage()
    {
        saveError.Visible = true;
        saveDone.Visible = false;
    }

    public void ShowSuccessMessage()
    {
        saveError.Visible = false;
        saveDone.Visible = true;
    }

    public void HideMessages()
    {
        saveError.Visible = false;
        saveDone.Visible = false;
    }
}