using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MoMA.Helpers;
using FriskisSvettisLib.Helpers;
using FriskisSvettisLib;

public partial class Admin_Edit : System.Web.UI.Page
{
    #region Properties 

        /// <summary>
        /// Facility to edit
        /// </summary>
        Guid LocalId
    {
        get
        {
            string sLocalId = ContextHelper.GetValue<string>("localid", Guid.Empty.ToString());
            return new Guid(sLocalId);
        }
    }

        /// <summary>
        /// If facility should be added to updated
        /// </summary>
        bool Add
    {
        get
        {
            return LocalId.Equals(Guid.Empty);
        }
    }

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        // authenticate
        if (!AdminAuth.Authenticated)
        {
            // failed
            Response.Redirect("/Default.aspx");
        }

        if (!Page.IsPostBack)
        {
            // List<Facility> facilities = FacilityHelper.GetAll();
            Facility facility = null;

            if (Add)
            {
                facility = new Facility();
                facility.LocalId = Guid.NewGuid();
            }
            else
            {
                facility = FacilityHelper.GetByLocalId(LocalId);
            }

            // fill properties
            Load(facility);

            divBrp.Visible = false;
            divPastellData.Visible = false;
            divDefendo.Visible = false;
            divDLSoftware.Visible = false;

            if (Add)
            {
                divBrp.Visible = true;

                // BRP
                txtBRPServiceUrl.Text = DictionaryHelper.GetValue<string>("ServiceUrl", BRPService.DefaultValues, "");
                txtBRPServiceLicense.Text = DictionaryHelper.GetValue<string>("ServiceLicense", BRPService.DefaultValues, "");
                chkBRPScheduleLoginNeeded.Checked = DictionaryHelper.GetValue<bool>("ScheduleLoginNeeded", BRPService.DefaultValues, false);

                // PastellData
                txtPastellDataServiceUrl.Text = DictionaryHelper.GetValue<string>("ServiceUrl", PastellService.DefaultValues, "");
                txtPastellDataServiceLicense.Text = DictionaryHelper.GetValue<string>("ServiceLicense", PastellService.DefaultValues, "");
                txtPastellDataServiceRevision.Text = DictionaryHelper.GetValue<string>("ServiceRevision", PastellService.DefaultValues, "");
                txtPastellDataLoginId.Text = DictionaryHelper.GetValue<string>("LoginId", PastellService.DefaultValues, "");
                chkPastellDataScheduleLoginNeeded.Checked = DictionaryHelper.GetValue<bool>("ScheduleLoginNeeded", PastellService.DefaultValues, true);

                // Defendo 
                txtDefendoServiceUrl.Text = DictionaryHelper.GetValue<string>("ServiceUrl", DefendoService.DefaultValues, "");
                txtDefendoServiceContextId.Text = DictionaryHelper.GetValue<string>("ServiceContextId", DefendoService.DefaultValues, "");
                txtDefendoServiceLicense.Text = DictionaryHelper.GetValue<string>("ServiceLicense", DefendoService.DefaultValues, "");
                chkDefendoScheduleLoginNeeded.Checked = DictionaryHelper.GetValue<bool>("ScheduleLoginNeeded", DefendoService.DefaultValues, false);

                // DLSoftware 
                txtDLServiceUrl.Text = DictionaryHelper.GetValue<string>("ServiceUrl", DLSoftwareService.DefaultValues, "");
                // txtDLServiceContextId.Text = DictionaryHelper.GetValue<string>("ServiceContextId", DLSoftwareService.DefaultValues, "");
                // txtDLServiceLicense.Text = DictionaryHelper.GetValue<string>("ServiceLicense", DLSoftwareService.DefaultValues, "");
                // chkDLScheduleLoginNeeded.Checked = DictionaryHelper.GetValue<bool>("ScheduleLoginNeeded", DLSoftwareService.DefaultValues, false);
            }
            else
            {
                switch (facility.Service.GetType().Name)
                {
                    case "BRPService":
                        ddlService.SelectedValue = "BRPService";
                        divBrp.Visible = true;
                        break;
                    case "PastellService":
                        ddlService.SelectedValue = "PastellService";
                        divPastellData.Visible = true;
                        break;
                    case "DefendoService":
                        ddlService.SelectedValue = "DefendoService";
                        divDefendo.Visible = true;
                        break;
                    case "DLSoftwareService":
                        ddlService.SelectedValue = "DLSoftwareService";
                        divDLSoftware.Visible = true;
                        break;
                }
            }
        }
    }

    private void Load(Facility facility)
    {
        // facility
        txtLocalId.Text = facility.LocalId.ToString();
        txtId.Text = facility.Id;
        txtId2.Text = facility.Id2;

        chkVisible.Checked = facility.Visible;
        txtVisibleGuid.Text = facility.VisibleGuid.Equals(Guid.Empty) ? "" : facility.VisibleGuid.ToString();

        txtName.Text = facility.Name;

        txtHtmlInfo.Text = facility.HtmlInfo;
        txtNews.Text = facility.News;
        txtOffers.Text = facility.Offers;
        txtAbout.Text = facility.About;

        txtPhone.Text = facility.Phone;
        txtEmail.Text = facility.Email;
        txtDescription.Text = facility.Description;
        txtAddress.Text = facility.Address;
        txtZipcode.Text = facility.Zipcode;
        txtCity.Text = facility.City;

        txtHomepage.Text = facility.Homepage;
        txtFacebook.Text = facility.Facebook;
        txtTwitter.Text = facility.Twitter;

        txtLongitude.Text = facility.Longitude;
        txtLatitude.Text = facility.Latitude;

        txtLanguages.Text = facility.Languages;

        txtUsername.Text = facility.Username;
        txtPassword.Text = facility.Password;

        txtUsernameLabel.Text = facility.UsernameLabel;
        txtPasswordLabel.Text = facility.PasswordLabel;

        txtScheduleLength.Text = (facility.ScheduleLength >= 0 ? facility.ScheduleLength.ToString() : "");

        // service
        if (facility.Service != null)
        {
            switch (facility.Service.GetType().Name)
            {
                case "BRPService":
                    BRPService brpService = (BRPService)facility.Service;

                    txtBRPServiceUrl.Text = brpService.ServiceUrl;
                    txtBRPServiceLicense.Text = brpService.ServiceLicense;
                    chkBRPScheduleLoginNeeded.Checked = brpService.ScheduleLoginNeeded;

                    facility.Service = brpService;

                    divBrp.Visible = true;
                    ddlService.SelectedValue = "BRP";

                    break;
                case "PastellService":
                    PastellService pastellService = (PastellService)facility.Service;

                    txtPastellDataServiceUrl.Text = pastellService.ServiceUrl;
                    txtPastellDataServiceLicense.Text = pastellService.ServiceLicense;
                    txtPastellDataServiceRevision.Text = pastellService.ServiceRevision;
                    chkPastellDataScheduleLoginNeeded.Checked = pastellService.ScheduleLoginNeeded;
                    txtPastellDataLoginId.Text = pastellService.LoginId;

                    facility.Service = pastellService;

                    divPastellData.Visible = true;
                    ddlService.SelectedValue = "PastellData";   

                    break;
                case "DefendoService":
                    DefendoService defendoService = (DefendoService)facility.Service;

                    txtDefendoServiceUrl.Text = defendoService.ServiceUrl;
                    txtDefendoServiceContextId.Text = defendoService.ServiceContextId;
                    txtDefendoServiceLicense.Text = defendoService.ServiceLicense;
                    chkDefendoScheduleLoginNeeded.Checked = defendoService.ScheduleLoginNeeded;

                    facility.Service = defendoService;

                    divDefendo.Visible = true;
                    ddlService.SelectedValue = "Defendo";   

                    break;
                case "DLSoftwareService":
                    DLSoftwareService dlService = (DLSoftwareService)facility.Service;

                    txtDLServiceUrl.Text = dlService.ServiceUrl;

                    facility.Service = dlService;
                    ddlService.SelectedValue = "DLSoftwareService";   

                    break;
            }
        }
    }

    protected void btnSave_OnClick(object sender, EventArgs e)
    {
        // get facility to edit
        Facility facility = FacilityHelper.GetByLocalId(new Guid(txtLocalId.Text));

        if (facility == null)
        {
            facility = new Facility();
        }

        // facility
        facility.LocalId = new Guid(txtLocalId.Text);
        facility.Id = txtId.Text;
        facility.Id2 = txtId2.Text;
            
        facility.Visible = chkVisible.Checked;
        try
        {
            facility.VisibleGuid = new Guid(txtVisibleGuid.Text);
        }
        catch { }

        facility.Name = txtName.Text;

        facility.HtmlInfo = txtHtmlInfo.Text;
        facility.News = txtNews.Text;
        facility.Offers = txtOffers.Text;
        facility.About = txtAbout.Text;

        facility.Phone = txtPhone.Text;
        facility.Email = txtEmail.Text;
        facility.Description = txtDescription.Text;
        facility.Address = txtAddress.Text;
        facility.Zipcode = txtZipcode.Text;
        facility.City = txtCity.Text;

        facility.Homepage = txtHomepage.Text;
        facility.Facebook = txtFacebook.Text;
        facility.Twitter = txtTwitter.Text;

        facility.Longitude = txtLongitude.Text;
        facility.Latitude = txtLatitude.Text;

        facility.Languages = txtLanguages.Text;

        facility.Username = txtUsername.Text;
        facility.Password = txtPassword.Text;

        facility.UsernameLabel = txtUsernameLabel.Text;
        facility.PasswordLabel = txtPasswordLabel.Text;

        int scheduleLength = -1;

        if (!int.TryParse(txtScheduleLength.Text, out scheduleLength))
        {
            scheduleLength = -1;
        }

        facility.ScheduleLength = scheduleLength;

        string serviceName = ddlService.SelectedValue;

        // service
        switch (serviceName)
        {
            case "BRPService":
                BRPService brpService = new BRPService();

                brpService.ServiceUrl = txtBRPServiceUrl.Text;
                brpService.ServiceLicense = txtBRPServiceLicense.Text;
                brpService.ScheduleLoginNeeded = chkBRPScheduleLoginNeeded.Checked;

                facility.Service = brpService;

                break;
            case "PastellService":
                PastellService pastellService = new PastellService();

                pastellService.ServiceUrl = txtPastellDataServiceUrl.Text;
                pastellService.ServiceLicense = txtPastellDataServiceLicense.Text;
                pastellService.ServiceRevision = txtPastellDataServiceRevision.Text;
                pastellService.ScheduleLoginNeeded = chkPastellDataScheduleLoginNeeded.Checked;
                pastellService.LoginId = txtPastellDataLoginId.Text;

                facility.Service = pastellService;

                break;
            case "DefendoService":
                DefendoService defendoService = new DefendoService();

                defendoService.ServiceUrl = txtDefendoServiceUrl.Text;
                defendoService.ServiceContextId = txtDefendoServiceContextId.Text;
                defendoService.ServiceLicense = txtDefendoServiceLicense.Text;
                defendoService.ScheduleLoginNeeded = chkDefendoScheduleLoginNeeded.Checked;

                facility.Service = defendoService;

                break;
            case "DLSoftwareService":
                DLSoftwareService dlService = new DLSoftwareService();

                dlService.ServiceUrl = txtDLServiceUrl.Text;

                facility.Service = dlService;
                break;
        }

        // save to xml
        FacilityHelper.Save(facility);
        Response.Redirect("/Admin/Default.aspx?auth=" + AdminAuth.AuthGuid);
    }

    protected void ddlService_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        divBrp.Visible = false;
        divPastellData.Visible = false;
        divDefendo.Visible = false;
        divDLSoftware.Visible = false;

        switch (ddlService.SelectedValue)
        {
            case "BRPService":
                divBrp.Visible = true;
                break;
            case "PastellService":
                divPastellData.Visible = true;
                break;
            case "DefendoService":
                divDefendo.Visible = true;
                break;
            case "DLSoftwareService":
                divDLSoftware.Visible = true;
                break;
        }
    }
}