using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Radius
{
  class Program
  {
    private static string ConnectionString { get; set; }

    public class LatLngFileNameModel
    {
      public string Latitude { get; set; }
      public string Longitude { get; set; }
      public string FileName { get; set; }
      public string CityID { get; set; }
      public string CountryID { get; set; }
    }

    static void Main(string[] args)
    {
      IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("jsconfig.json", true, true).Build();
      ConnectionString = configuration["connectionString"];

      List<LatLngFileNameModel> latLngFileNames = new List<LatLngFileNameModel>();

      string sqlGpslocations = "select * from gpslocations";

      using MySqlConnection mySqlConnection =
        new MySqlConnection(ConnectionString);
      using MySqlCommand mySqlCommand = new MySqlCommand(sqlGpslocations, mySqlConnection);
      mySqlConnection.Open();
      MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();

      while (mySqlDataReader.Read())
      {
        LatLngFileNameModel latLngFileName = new LatLngFileNameModel
        {
          Latitude = mySqlDataReader["Latitude"].ToString().Replace(',', '.')
          , Longitude = mySqlDataReader["Longitude"].ToString().Replace(',', '.')
          , FileName = mySqlDataReader["FileName"].ToString()
          , CityID = mySqlDataReader["CityID"].ToString()
          , CountryID = mySqlDataReader["CountryID"].ToString()
        };

        Console.WriteLine($"Geocoding: {latLngFileName.Latitude}, {latLngFileName.Longitude}, FileName: {latLngFileName.FileName}");

        if (!IsWithinRadius(latLngFileName.Latitude, latLngFileName.Longitude, latLngFileNames, "10"))
        {
          using MySqlCommand gpslocationsgroupedby10kmdistancs = new MySqlCommand();
          using MySqlConnection mySqlConnectionGpslocationsgroupedby10kmdistancs =
            new MySqlConnection(ConnectionString);
          mySqlConnectionGpslocationsgroupedby10kmdistancs.Open();

          gpslocationsgroupedby10kmdistancs.Connection = mySqlConnectionGpslocationsgroupedby10kmdistancs;
          gpslocationsgroupedby10kmdistancs.CommandText =
            $"INSERT INTO reversegeocoding.gpslocationsgroupedby10kmdistancs (Latitude, Longitude, FileName, CityID, CountryID) VALUES " 
            + $"('{latLngFileName.Latitude}'" 
            + $", '{latLngFileName.Longitude}'" 
            + $", '{latLngFileName.FileName}'" 
            + $", '{latLngFileName.CityID}'" 
            + $", '{latLngFileName.CountryID}'" 
            + ");";
          gpslocationsgroupedby10kmdistancs.ExecuteNonQuery();
          latLngFileNames.Add(latLngFileName);
        }
      }
    }

    private static bool IsWithinRadius(string latitude, string longitude, List<LatLngFileNameModel> latLngFileNames, string distanceKm = "0.5")
    {
      if (latLngFileNames.Any())
      {
        string sqlRadius = "SELECT *, "
                           + "(6371 * acos ("
                           + $"cos ( radians({latitude})) "
                           + "* cos( radians( latitude )) "
                           + "* cos( radians( longitude) "
                           + $"- radians({longitude})) "
                           + $"+ sin (radians({latitude})) "
                           + "* sin( radians( latitude )) "
                           + ")) AS distance "
                           + "FROM gpslocations "
                           + $"HAVING distance < {distanceKm} "
                           + "ORDER BY distance ";

        using (MySqlConnection mySqlConnection =
          new MySqlConnection(ConnectionString))
        using (MySqlCommand mySqlCommand = new MySqlCommand(sqlRadius, mySqlConnection))
        {
          mySqlConnection.Open();
          MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();

          while (mySqlDataReader.Read())
          {
            LatLngFileNameModel latLngFileName = new LatLngFileNameModel
            {
              Latitude = mySqlDataReader["Latitude"].ToString().Replace(',', '.'),
              Longitude = mySqlDataReader["Longitude"].ToString().Replace(',', '.'),
              FileName = mySqlDataReader["FileName"].ToString()
            };

            if (latLngFileNames.Any(latLng => latLng.Latitude == latLngFileName.Latitude && latLng.Longitude == latLngFileName.Longitude))
            {
              return true;
            }

          }

        }
      }

      return false;
    }

  }
}