using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Activity
{
    public string Id { get; set; }
    public string Name { get; set; }
    public object originalObject { get; set; }
    public List<Activity> ChildActivites { get; set; }
}