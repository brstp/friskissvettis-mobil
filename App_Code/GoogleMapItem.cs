using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

public class GoogleMapItem
{
    public string Id { get; set; }
    public string City { get; set; }
    public string Owner { get; set; }
    public string Address { get; set; }
    public string ErrorPhone { get; set; }
    public string CustomerSupport { get; set; }
    public string Email { get; set; }
    public string OpeningHours { get; set; }
    public string PaymentOptions { get; set; }
    public string Longitude { get; set; }
    public string Latitude { get; set; }
    public string MapUrl { get; set; }
    public string Icon { get; set; }
    public string MapInfo { get; set; }
    public string MapTextInfo { get; set; }
    public string FullInfo { get; set; }

    public GoogleMapItem() 
    {
        // default
        Latitude = "0.0";
        Longitude = "0.0";
        MapUrl = "";
        City = "";
        Owner = "";
        Address = "";
        ErrorPhone = "";
        CustomerSupport = "";
        Email = "";
        OpeningHours = "";
        PaymentOptions = "";
        Icon = "";
        MapInfo = "";
        FullInfo = "";
    }
}