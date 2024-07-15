using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class RadiansUtility
{
    #region STELLAR
    public static string ConvertToRightAscensionText(double radians)
    {
        // Ensure the degree are within 0 - 360
        //degree = degree % 360;
        var degree = radians * Mathf.Rad2Deg;

        // Convert degree to hours
        double totalHours = degree / 15.0;

        // Get the integer part of the hours
        int hours = (int)totalHours;

        // Get the minutes and seconds from the fractional part of the hours
        double fractionalHours = totalHours - hours;
        double totalMinutes = fractionalHours * 60.0;
        int minutes = (int)totalMinutes;

        double fractionalMinutes = totalMinutes - minutes;
        double totalSeconds = fractionalMinutes * 60.0;
        double seconds = totalSeconds;

        var supH = $"<sup>h</sup>";
        var supM = $"<sup>m</sup>";
        var supS = $"<sup>s</sup>";

        // Return the formatted string
        return string.Format($"{hours}{supH} {minutes}{supM} {seconds:00.00}{supS}");
    }

    public static string ConvertToDeclinationText(double radians)
    {
        // Convert radians to degrees
        double degrees = radians * Mathf.Rad2Deg;

        // Determine the sign for declination
        string sign = degrees < 0 ? "-" : "+";
        degrees = Math.Abs(degrees);

        // Get the integer part of the degrees
        int d = (int)degrees;

        // Get the minutes and seconds from the fractional part of the degrees
        double fractionalDegrees = degrees - d;
        double totalMinutes = fractionalDegrees * 60.0;
        int m = (int)totalMinutes;

        double fractionalMinutes = totalMinutes - m;
        double totalSeconds = fractionalMinutes * 60.0;
        double s = totalSeconds;

        // Return the formatted string
        return $"{sign}{d:00}° {m:00}' {s:00.00}\"";
    }
    #endregion
}