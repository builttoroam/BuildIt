using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// <copyright file="UtilitiesTest.ConvertToDateTimeFromUnixTimestamp.g.cs" company="Built to Roam Pty LTd">Copyright ©  2015</copyright>
// <auto-generated>
// This file contains automatically generated tests.
// Do not modify this file manually.
// 
// If the contents of this file becomes outdated, you can delete it.
// For example, if it no longer compiles.
// </auto-generated>
using System;

namespace BuildIt.Tests
{
    public partial class UtilitiesTest
    {

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void ConvertToDateTimeFromUnixTimestamp6()
{
    DateTime dt;
    dt = this.ConvertToDateTimeFromUnixTimestamp((string)null);
    Assert.AreEqual<int>(1, dt.Day);
    Assert.AreEqual<DayOfWeek>(DayOfWeek.Thursday, dt.DayOfWeek);
    Assert.AreEqual<int>(1, dt.DayOfYear);
    Assert.AreEqual<int>(0, dt.Hour);
    Assert.AreEqual<DateTimeKind>(DateTimeKind.Utc, dt.Kind);
    Assert.AreEqual<int>(0, dt.Millisecond);
    Assert.AreEqual<int>(0, dt.Minute);
    Assert.AreEqual<int>(1, dt.Month);
    Assert.AreEqual<int>(0, dt.Second);
    Assert.AreEqual<int>(1970, dt.Year);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void ConvertToDateTimeFromUnixTimestamp20()
{
    DateTime dt;
    dt = this.ConvertToDateTimeFromUnixTimestamp("\0");
    Assert.AreEqual<int>(1, dt.Day);
    Assert.AreEqual<DayOfWeek>(DayOfWeek.Thursday, dt.DayOfWeek);
    Assert.AreEqual<int>(1, dt.DayOfYear);
    Assert.AreEqual<int>(0, dt.Hour);
    Assert.AreEqual<DateTimeKind>(DateTimeKind.Utc, dt.Kind);
    Assert.AreEqual<int>(0, dt.Millisecond);
    Assert.AreEqual<int>(0, dt.Minute);
    Assert.AreEqual<int>(1, dt.Month);
    Assert.AreEqual<int>(0, dt.Second);
    Assert.AreEqual<int>(1970, dt.Year);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void ConvertToDateTimeFromUnixTimestamp66()
{
    DateTime dt;
    dt = this.ConvertToDateTimeFromUnixTimestamp("-耰e:");
    Assert.AreEqual<int>(1, dt.Day);
    Assert.AreEqual<DayOfWeek>(DayOfWeek.Thursday, dt.DayOfWeek);
    Assert.AreEqual<int>(1, dt.DayOfYear);
    Assert.AreEqual<int>(0, dt.Hour);
    Assert.AreEqual<DateTimeKind>(DateTimeKind.Utc, dt.Kind);
    Assert.AreEqual<int>(0, dt.Millisecond);
    Assert.AreEqual<int>(0, dt.Minute);
    Assert.AreEqual<int>(1, dt.Month);
    Assert.AreEqual<int>(0, dt.Second);
    Assert.AreEqual<int>(1970, dt.Year);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void ConvertToDateTimeFromUnixTimestamp79()
{
    DateTime dt;
    dt = this.ConvertToDateTimeFromUnixTimestamp("-耰e00");
    Assert.AreEqual<int>(1, dt.Day);
    Assert.AreEqual<DayOfWeek>(DayOfWeek.Thursday, dt.DayOfWeek);
    Assert.AreEqual<int>(1, dt.DayOfYear);
    Assert.AreEqual<int>(0, dt.Hour);
    Assert.AreEqual<DateTimeKind>(DateTimeKind.Utc, dt.Kind);
    Assert.AreEqual<int>(0, dt.Millisecond);
    Assert.AreEqual<int>(0, dt.Minute);
    Assert.AreEqual<int>(1, dt.Month);
    Assert.AreEqual<int>(0, dt.Second);
    Assert.AreEqual<int>(1970, dt.Year);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void ConvertToDateTimeFromUnixTimestamp138()
{
    DateTime dt;
    dt = this.ConvertToDateTimeFromUnixTimestamp("-耰e0\0");
    Assert.AreEqual<int>(1, dt.Day);
    Assert.AreEqual<DayOfWeek>(DayOfWeek.Thursday, dt.DayOfWeek);
    Assert.AreEqual<int>(1, dt.DayOfYear);
    Assert.AreEqual<int>(0, dt.Hour);
    Assert.AreEqual<DateTimeKind>(DateTimeKind.Utc, dt.Kind);
    Assert.AreEqual<int>(0, dt.Millisecond);
    Assert.AreEqual<int>(0, dt.Minute);
    Assert.AreEqual<int>(1, dt.Month);
    Assert.AreEqual<int>(0, dt.Second);
    Assert.AreEqual<int>(1970, dt.Year);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void ConvertToDateTimeFromUnixTimestamp161()
{
    DateTime dt;
    dt = this.ConvertToDateTimeFromUnixTimestamp("耰.0\0");
    Assert.AreEqual<int>(1, dt.Day);
    Assert.AreEqual<DayOfWeek>(DayOfWeek.Thursday, dt.DayOfWeek);
    Assert.AreEqual<int>(1, dt.DayOfYear);
    Assert.AreEqual<int>(0, dt.Hour);
    Assert.AreEqual<DateTimeKind>(DateTimeKind.Utc, dt.Kind);
    Assert.AreEqual<int>(0, dt.Millisecond);
    Assert.AreEqual<int>(0, dt.Minute);
    Assert.AreEqual<int>(1, dt.Month);
    Assert.AreEqual<int>(0, dt.Second);
    Assert.AreEqual<int>(1970, dt.Year);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void ConvertToDateTimeFromUnixTimestamp190()
{
    DateTime dt;
    dt = this.ConvertToDateTimeFromUnixTimestamp("-");
    Assert.AreEqual<int>(1, dt.Day);
    Assert.AreEqual<DayOfWeek>(DayOfWeek.Thursday, dt.DayOfWeek);
    Assert.AreEqual<int>(1, dt.DayOfYear);
    Assert.AreEqual<int>(0, dt.Hour);
    Assert.AreEqual<DateTimeKind>(DateTimeKind.Utc, dt.Kind);
    Assert.AreEqual<int>(0, dt.Millisecond);
    Assert.AreEqual<int>(0, dt.Minute);
    Assert.AreEqual<int>(1, dt.Month);
    Assert.AreEqual<int>(0, dt.Second);
    Assert.AreEqual<int>(1970, dt.Year);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void ConvertToDateTimeFromUnixTimestamp369()
{
    DateTime dt;
    dt = this.ConvertToDateTimeFromUnixTimestamp("0");
    Assert.AreEqual<int>(1, dt.Day);
    Assert.AreEqual<DayOfWeek>(DayOfWeek.Thursday, dt.DayOfWeek);
    Assert.AreEqual<int>(1, dt.DayOfYear);
    Assert.AreEqual<int>(0, dt.Hour);
    Assert.AreEqual<DateTimeKind>(DateTimeKind.Utc, dt.Kind);
    Assert.AreEqual<int>(0, dt.Millisecond);
    Assert.AreEqual<int>(0, dt.Minute);
    Assert.AreEqual<int>(1, dt.Month);
    Assert.AreEqual<int>(0, dt.Second);
    Assert.AreEqual<int>(1970, dt.Year);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void ConvertToDateTimeFromUnixTimestamp445()
{
    DateTime dt;
    dt = this.ConvertToDateTimeFromUnixTimestamp("耰\0");
    Assert.AreEqual<int>(1, dt.Day);
    Assert.AreEqual<DayOfWeek>(DayOfWeek.Thursday, dt.DayOfWeek);
    Assert.AreEqual<int>(1, dt.DayOfYear);
    Assert.AreEqual<int>(0, dt.Hour);
    Assert.AreEqual<DateTimeKind>(DateTimeKind.Utc, dt.Kind);
    Assert.AreEqual<int>(0, dt.Millisecond);
    Assert.AreEqual<int>(0, dt.Minute);
    Assert.AreEqual<int>(1, dt.Month);
    Assert.AreEqual<int>(0, dt.Second);
    Assert.AreEqual<int>(1970, dt.Year);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void ConvertToDateTimeFromUnixTimestamp483()
{
    DateTime dt;
    dt = this.ConvertToDateTimeFromUnixTimestamp("-耰.00");
    Assert.AreEqual<int>(1, dt.Day);
    Assert.AreEqual<DayOfWeek>(DayOfWeek.Thursday, dt.DayOfWeek);
    Assert.AreEqual<int>(1, dt.DayOfYear);
    Assert.AreEqual<int>(0, dt.Hour);
    Assert.AreEqual<DateTimeKind>(DateTimeKind.Utc, dt.Kind);
    Assert.AreEqual<int>(0, dt.Millisecond);
    Assert.AreEqual<int>(0, dt.Minute);
    Assert.AreEqual<int>(1, dt.Month);
    Assert.AreEqual<int>(0, dt.Second);
    Assert.AreEqual<int>(1970, dt.Year);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void ConvertToDateTimeFromUnixTimestamp585()
{
    DateTime dt;
    dt = this.ConvertToDateTimeFromUnixTimestamp("-耰.00\0");
    Assert.AreEqual<int>(1, dt.Day);
    Assert.AreEqual<DayOfWeek>(DayOfWeek.Thursday, dt.DayOfWeek);
    Assert.AreEqual<int>(1, dt.DayOfYear);
    Assert.AreEqual<int>(0, dt.Hour);
    Assert.AreEqual<DateTimeKind>(DateTimeKind.Utc, dt.Kind);
    Assert.AreEqual<int>(0, dt.Millisecond);
    Assert.AreEqual<int>(0, dt.Minute);
    Assert.AreEqual<int>(1, dt.Month);
    Assert.AreEqual<int>(0, dt.Second);
    Assert.AreEqual<int>(1970, dt.Year);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void ConvertToDateTimeFromUnixTimestamp652()
{
    DateTime dt;
    dt = this.ConvertToDateTimeFromUnixTimestamp("");
    Assert.AreEqual<int>(1, dt.Day);
    Assert.AreEqual<DayOfWeek>(DayOfWeek.Thursday, dt.DayOfWeek);
    Assert.AreEqual<int>(1, dt.DayOfYear);
    Assert.AreEqual<int>(0, dt.Hour);
    Assert.AreEqual<DateTimeKind>(DateTimeKind.Utc, dt.Kind);
    Assert.AreEqual<int>(0, dt.Millisecond);
    Assert.AreEqual<int>(0, dt.Minute);
    Assert.AreEqual<int>(1, dt.Month);
    Assert.AreEqual<int>(0, dt.Second);
    Assert.AreEqual<int>(1970, dt.Year);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void ConvertToDateTimeFromUnixTimestamp753()
{
    DateTime dt;
    dt = this.ConvertToDateTimeFromUnixTimestamp("-00");
    Assert.AreEqual<int>(1, dt.Day);
    Assert.AreEqual<DayOfWeek>(DayOfWeek.Thursday, dt.DayOfWeek);
    Assert.AreEqual<int>(1, dt.DayOfYear);
    Assert.AreEqual<int>(0, dt.Hour);
    Assert.AreEqual<DateTimeKind>(DateTimeKind.Utc, dt.Kind);
    Assert.AreEqual<int>(0, dt.Millisecond);
    Assert.AreEqual<int>(0, dt.Minute);
    Assert.AreEqual<int>(1, dt.Month);
    Assert.AreEqual<int>(0, dt.Second);
    Assert.AreEqual<int>(1970, dt.Year);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void ConvertToDateTimeFromUnixTimestamp785()
{
    DateTime dt;
    dt = this.ConvertToDateTimeFromUnixTimestamp("00\0");
    Assert.AreEqual<int>(1, dt.Day);
    Assert.AreEqual<DayOfWeek>(DayOfWeek.Thursday, dt.DayOfWeek);
    Assert.AreEqual<int>(1, dt.DayOfYear);
    Assert.AreEqual<int>(0, dt.Hour);
    Assert.AreEqual<DateTimeKind>(DateTimeKind.Utc, dt.Kind);
    Assert.AreEqual<int>(0, dt.Millisecond);
    Assert.AreEqual<int>(0, dt.Minute);
    Assert.AreEqual<int>(1, dt.Month);
    Assert.AreEqual<int>(0, dt.Second);
    Assert.AreEqual<int>(1970, dt.Year);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void ConvertToDateTimeFromUnixTimestamp880()
{
    DateTime dt;
    dt = this.ConvertToDateTimeFromUnixTimestamp("-耰e00\0");
    Assert.AreEqual<int>(1, dt.Day);
    Assert.AreEqual<DayOfWeek>(DayOfWeek.Thursday, dt.DayOfWeek);
    Assert.AreEqual<int>(1, dt.DayOfYear);
    Assert.AreEqual<int>(0, dt.Hour);
    Assert.AreEqual<DateTimeKind>(DateTimeKind.Utc, dt.Kind);
    Assert.AreEqual<int>(0, dt.Millisecond);
    Assert.AreEqual<int>(0, dt.Minute);
    Assert.AreEqual<int>(1, dt.Month);
    Assert.AreEqual<int>(0, dt.Second);
    Assert.AreEqual<int>(1970, dt.Year);
}

[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void ConvertToDateTimeFromUnixTimestamp961()
{
    DateTime dt;
    dt = this.ConvertToDateTimeFromUnixTimestamp("耰");
    Assert.AreEqual<int>(1, dt.Day);
    Assert.AreEqual<DayOfWeek>(DayOfWeek.Thursday, dt.DayOfWeek);
    Assert.AreEqual<int>(1, dt.DayOfYear);
    Assert.AreEqual<int>(0, dt.Hour);
    Assert.AreEqual<DateTimeKind>(DateTimeKind.Utc, dt.Kind);
    Assert.AreEqual<int>(0, dt.Millisecond);
    Assert.AreEqual<int>(0, dt.Minute);
    Assert.AreEqual<int>(1, dt.Month);
    Assert.AreEqual<int>(0, dt.Second);
    Assert.AreEqual<int>(1970, dt.Year);
}
    }
}
