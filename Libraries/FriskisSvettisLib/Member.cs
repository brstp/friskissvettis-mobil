using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using FriskisSvettisLib.DefendoWebService;

/// <summary>
/// Summary description for Member
/// </summary>
public class Member
{
    public string Id { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public string Mobile { get; set; }
    public Address Address { get; set; }
    public string Cardnumber { get; set; }
    public string BussinessUnit { get; set; }
    public Gender Gender { get; set; }
    public bool Staffmember { get; set; }
    public string ImageUrl { get; set; }
    public DateTime Birthdate { get; set; }

    public Facility Facility { get; set; } 
    public string Username { get; set; }
    public string Password { get; set; }

    public string SessionId { get; set; } // Used when logged in at Pastell

    public LoginToken Token { get; set; } // Used when logged in at Defendo
    public Session DefendoSession { get; set; } // Used when logged in at Defendo
    public string DLSession { get; set; }

    public Member()
    {
        this.Address = new Address()
        {
            Street = "",
            City = "",
            Zip = "",
            Country = ""
        };
        this.Gender = Gender.Unknown;
    }

	public Member(string data, IFriskisService service) : this()
	{
        switch (service.ServiceType)
        {
            case FriskisServiceType.Demo:
                break;
            case FriskisServiceType.BRP:
                FillMemberFromBRP(data);
                break;
            case FriskisServiceType.PastellData:
                FillMemberFromPastellData(data);
                break;
        }
	}

    #region Pastell

        private void FillMemberFromPastellData(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            SessionId = XmlHelper.GetValue<string>(xmlDoc, "//ProfitAndroid/GUID", "");
            Firstname = XmlHelper.GetValue<string>(xmlDoc, "//ProfitAndroid/user/firstname", "");
            Lastname = XmlHelper.GetValue<string>(xmlDoc, "//ProfitAndroid/user/lastname", "");

            // no more info here to get.. :(
        }

    #endregion

    #region BRP

        private void FillMemberFromBRP(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            Id = XmlHelper.GetValue<string>(xmlDoc, "//authresponse/user/customerid", "");
            Firstname = XmlHelper.GetValue<string>(xmlDoc, "//authresponse/user/firstname", "");
            Lastname = XmlHelper.GetValue<string>(xmlDoc, "//authresponse/user/surname", "");
            Email = XmlHelper.GetValue<string>(xmlDoc, "//authresponse/user/email", "");
            Mobile = XmlHelper.GetValue<string>(xmlDoc, "//authresponse/user/mobile", "");

            Address.Street = XmlHelper.GetValue<string>(xmlDoc, "//authresponse/user/address/street", "");
            Address.Zip = XmlHelper.GetValue<string>(xmlDoc, "//authresponse/user/address/zip", "");
            Address.City = XmlHelper.GetValue<string>(xmlDoc, "//authresponse/user/address/city", "");

            Cardnumber = XmlHelper.GetValue<string>(xmlDoc, "//authresponse/user/cardnumber", "");
            BussinessUnit = XmlHelper.GetValue<string>(xmlDoc, "//authresponse/user/businessunit", "");

            switch (XmlHelper.GetValue<string>(xmlDoc, "//authresponse/user/gender", ""))
            {
                case "Man":
                    this.Gender = Gender.Male;
                    break;
                case "Kvinna":
                    this.Gender = Gender.Female;
                    break;
                case "Odefinierad":
                    this.Gender = Gender.Unknown;
                    break;
            }

            Staffmember = XmlHelper.GetValue<string>(xmlDoc, "//authresponse/user/staffmember", "").Equals("true");
            ImageUrl = XmlHelper.GetValue<string>(xmlDoc, "//authresponse/user/image", "");

            DateTime birthDay = DateTime.MinValue;
            DateTime.TryParse(XmlHelper.GetValue<string>(xmlDoc, "//authresponse/user/birthDate", ""), out birthDay);
            Birthdate = birthDay;
        }

    #endregion
}