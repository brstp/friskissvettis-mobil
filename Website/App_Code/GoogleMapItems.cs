using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Text;

public class GoogleMapItems
{
    /// <summary>
    /// When used in another project, this is where the conversion 
    /// between used itemtype in that project and a list of GoogleMapItem.
    /// </summary>
    /// <param name="NorthEastLat"></param>
    /// <param name="NorthEastLng"></param>
    /// <param name="SouthWestLat"></param>
    /// <param name="SouthWestLng"></param>
    /// <returns></returns>
    public static List<GoogleMapItem> GetByBounds(double NorthEastLat, double NorthEastLng, double SouthWestLat, double SouthWestLng)
    {
        // todo: get all inner facilities with long, lat (if possible) - check pastell and brp

        // google map items
        List<GoogleMapItem> mapItems = new List<GoogleMapItem>();

        // facilities 
        List<Facility> facilities = FacilityHelper.GetAll().Where(f => f.Visible).ToList();
        
        List<Facility> facilitiesInBounds  = facilities; // not possible to filter here when multiple longlat exists in a single field?
          /*= facilities.Where(f =>
        
            // check if center is visible
            !string.IsNullOrEmpty(f.Latitude) && double.Parse(f.Latitude.Replace(".", ",")) <= NorthEastLat &&
            !string.IsNullOrEmpty(f.Longitude) && double.Parse(f.Longitude.Replace(".", ",")) <= NorthEastLng &&

            !string.IsNullOrEmpty(f.Latitude) && double.Parse(f.Latitude.Replace(".", ",")) >= SouthWestLat &&
            !string.IsNullOrEmpty(f.Longitude) && double.Parse(f.Longitude.Replace(".", ",")) >= SouthWestLng

            && !f.City.Contains("Demo")

        ).ToList();*/

        // facility -> google map item
        foreach (Facility facility in facilitiesInBounds)
        {

            // check if multiple cordinates exists
            if (facility.Longitude.Contains(";"))
            {
                List<string> longList = facility.Longitude.Split(';').ToList();
                List<string> latList = facility.Latitude.Split(';').ToList();

                for (int i = 0; i < longList.Count; i++)
                {
                    try
                    {
                        bool hasAddress = longList[i].Split(':').Count() > 2;

                        GoogleMapItem mapItem = new GoogleMapItem();
                        mapItem.Id = facility.LocalId.ToString();
                        mapItem.Latitude = latList[i].Split(':')[1];
                        mapItem.Longitude = longList[i].Split(':')[1];
                        mapItem.Address = (hasAddress ? longList[i].Split(':')[2] : "");
                        mapItem.Icon = "images/map/red.png";
                        mapItem.MapInfo = "<b>F&S " + facility.City + "</b><br />" + mapItem.Address + 
                                        "<br />" + longList[i].Split(':')[0] +
                                        "<br/><br/><a href='" + FriskisService.GetFacilityMainAddress(facility.LocalId.ToString().Replace("-", "")).Replace("&authenticated=true", "") + "' target='_parent'>Gå till förening</a>";
                                        // "<br/><br/><a href='/FacilityMain.aspx?facilityId=" + facility.LocalId.ToString().Replace("-", "") + "' target='_parent'>Gå till förening</a>";

                        // TODO: How should a link be described? 
                        mapItem.MapTextInfo = "F&S " + facility.City; // +Environment.NewLine + mapItem.Address + longList[i].Split(':')[0];

                        mapItems.Add(mapItem);
                    }
                    catch { }
                }
            }
            else
            {
                // Position
                GoogleMapItem mapItem = new GoogleMapItem();

                mapItem.Id = facility.LocalId.ToString();

                mapItem.Latitude = facility.Latitude;
                mapItem.Longitude = facility.Longitude;

                mapItem.Icon = "images/map/red.png";

                // Html description
                mapItem.MapInfo = "<b>" + facility.Name + "</b><br />" + facility.City +
                                    "<br/><br/><a href='/FacilityMain.aspx?facilityId=" + facility.LocalId.ToString().Replace("-", "") + "' target='_parent'>Gå till förening</a>";

                mapItem.MapTextInfo = "F&S " + facility.City;

                mapItems.Add(mapItem);
            }
        }

        return mapItems;
    }
}