using System;
using System.Diagnostics.Contracts;
using SQLite.Net;

namespace BuiltToRoam.Data.SQLite.Database.Interfaces
{
    public interface IDatabaseNameProvider
    {
        string DatabseName { get; set; }        
    }
}
