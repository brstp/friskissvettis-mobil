﻿<%@ WebHandler Language="C#" Class="MapPositionHandler" %>

using System;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.Web.SessionState;
using MoMA.Helpers;

public class MapPositionHandler : IHttpHandler, IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/xml";
        context.Response.BufferOutput = true;
        context.Response.Write(Process(context));
        context.Response.End();
    }

    public string Process(HttpContext context)
    {
        string action = ContextHelper.GetValue<string>(context, "action", "");

        switch (action)
        {
            case "GetByBounds":
                return GetByBounds(context);
            default:
                return "";
        }
    }

    public static string GetByBounds(HttpContext context)
    {
        double NorthEastLat = ContextHelper.GetValue<double>(context, "NorthEastLat", 0);
        double NorthEastLng = ContextHelper.GetValue<double>(context, "NorthEastLng", 0);
        double SouthWestLat = ContextHelper.GetValue<double>(context, "SouthWestLat", 0);
        double SouthWestLng = ContextHelper.GetValue<double>(context, "SouthWestLng", 0);

        List<GoogleMapItem> stations = GoogleMapItems.GetByBounds(NorthEastLat, NorthEastLng, SouthWestLat, SouthWestLng);

        StringBuilder xml = new StringBuilder();
        xml.Append("<?xml version='1.0'?><markers>");
        foreach (GoogleMapItem station in stations)
        {
            xml.Append("<marker>");
            xml.AppendFormat("<id><![CDATA[{0}]]></id>", station.Id);
            xml.AppendFormat("<name><![CDATA[{0}]]></name>", station.City);
            xml.AppendFormat("<address><![CDATA[{0}]]></address>", station.Address);
            xml.AppendFormat("<lat><![CDATA[{0}]]></lat>", station.Latitude);
            xml.AppendFormat("<lng><![CDATA[{0}]]></lng>", station.Longitude);
            xml.AppendFormat("<mapUrl><![CDATA[{0}]]></mapUrl>", station.MapUrl);
            xml.AppendFormat("<owner><![CDATA[{0}]]></owner>", station.Owner);
            xml.AppendFormat("<errorPhone><![CDATA[{0}]]></errorPhone>", station.ErrorPhone);
            xml.AppendFormat("<customerSupport><![CDATA[{0}]]></customerSupport>", station.CustomerSupport);
            xml.AppendFormat("<email><![CDATA[{0}]]></email>", station.Email);
            xml.AppendFormat("<openingHours><![CDATA[{0}]]></openingHours>", station.OpeningHours);
            xml.AppendFormat("<paymentOptions><![CDATA[{0}]]></paymentOptions>", station.PaymentOptions);
            xml.AppendFormat("<icon><![CDATA[{0}]]></icon>", station.Icon);
            xml.AppendFormat("<mapInfo><![CDATA[{0}]]></mapInfo>", station.MapInfo);
            xml.AppendFormat("<mapTextInfo><![CDATA[{0}]]></mapTextInfo>", station.MapTextInfo);
            xml.AppendFormat("<fullInfo><![CDATA[{0}]]></fullInfo>", station.FullInfo);

            xml.Append("</marker>");
        }
        xml.Append("</markers>");
        return xml.ToString();
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}