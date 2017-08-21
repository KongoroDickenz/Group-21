using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Device.Location;
using Windows.Devices.Geolocation;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Tasks;


namespace ELBA
{
    public partial class Page7 : PhoneApplicationPage
    {
        List<GeoCoordinate> MyCoordinates = new List<GeoCoordinate>();//list to display the routes
        RouteQuery MyQuery = null;
        GeocodeQuery Mygeocodequery = null;


        // Constructor
        public Page7()
        {
            InitializeComponent();
            this.GetCoordinates();
        }
        void Mygeocodequery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            if (e.Error == null)
            {
                try 
                { 
                    MyQuery = new RouteQuery();
                    MyCoordinates.Add(e.Result[0].GeoCoordinate);
                    MyQuery.Waypoints = MyCoordinates;
                    MyQuery.QueryCompleted += MyQuery_QueryCompleted;
                    MyQuery.QueryAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                Mygeocodequery.Dispose();
            }
        }
        void MyQuery_QueryCompleted(object sender, QueryCompletedEventArgs<Route> e)
        {
            if (e.Error == null)
            {
                Route MyRoute = e.Result;
                MapRoute MyMapRoute = new MapRoute(MyRoute);
                MyMap.AddRoute(MyMapRoute);
                List<string> RouteList = new List<string>();
                foreach (RouteLeg leg in MyRoute.Legs)
                {
                    foreach (RouteManeuver maneuver in leg.Maneuvers)
                    {
                        RouteList.Add(maneuver.InstructionText);
                    }
                }

                RouteLLS.ItemsSource = RouteList;

                MyQuery.Dispose();
            }
        }


        private async void GetCoordinates()
        {
            // Get the phone's current location.
            Geolocator MyGeolocator = new Geolocator();//instantiate an object MyGeolocator of type Geolocator
            MyGeolocator.DesiredAccuracyInMeters =5;
            Geoposition MyGeoPosition = null;
            try
            {
                MyGeoPosition = await MyGeolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10));
                MyCoordinates.Add(new GeoCoordinate(MyGeoPosition.Coordinate.Latitude, MyGeoPosition.Coordinate.Longitude));
                Mygeocodequery = new GeocodeQuery();
               BingMapsTask bingMapsTask = new BingMapsTask();
                //Omit the Center property to use the user's current location.
                bingMapsTask.Center = new GeoCoordinate(0.3323,32.5698);
                bingMapsTask.SearchTerm = "hospitals";
                bingMapsTask.ZoomLevel =2;
                bingMapsTask.Show();
               // Mygeocodequery.SearchTerm = "Mulago, UG";//Here enter your destination
                Mygeocodequery.GeoCoordinate = new GeoCoordinate(MyGeoPosition.Coordinate.Latitude, MyGeoPosition.Coordinate.Longitude);
                Mygeocodequery.QueryCompleted += Mygeocodequery_QueryCompleted;
                Mygeocodequery.QueryAsync();
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Location is disabled in phone settings or capabilities are not checked.");
            }
            catch (Exception ex)
            {
                // Something else happened while acquiring the location.
                MessageBox.Show(ex.Message);
            }
        }
        public class Location
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        public static class TrackingHelper
        {
            private static double DegreesToRadians(double degrees)
            {
                return degrees * Math.PI / 180.0;
            }

            public static double CalculateDistance(Location location1, Location location2)
            {
                double circumference = 40000.0; // Earth's circumference at the equator in km
                double distance = 0.0;

                //Calculate radians
                double latitude1Rad = DegreesToRadians(location1.Latitude);
                double longitude1Rad = DegreesToRadians(location1.Longitude);
                double latititude2Rad = DegreesToRadians(location2.Latitude);
                double longitude2Rad = DegreesToRadians(location2.Longitude);

                double logitudeDiff = Math.Abs(longitude1Rad - longitude2Rad);

                if (logitudeDiff > Math.PI)
                {
                    logitudeDiff = 2.0 * Math.PI - logitudeDiff;
                }

                double angleCalculation =
                    Math.Acos(
                      Math.Sin(latititude2Rad) * Math.Sin(latitude1Rad) +
                      Math.Cos(latititude2Rad) * Math.Cos(latitude1Rad) * Math.Cos(logitudeDiff));

                distance = circumference * angleCalculation / (2.0 * Math.PI);

                return distance;
            }

            public static double CalculateDistance(params Location[] locations)
            {
                double totalDistance = 0.0;

                for (int i = 0; i < locations.Length - 1; i++)
                {
                    Location current = locations[i];
                    Location next = locations[i + 1];

                    totalDistance += CalculateDistance(current, next);
                }

                return totalDistance;
            }
        }
        /*double distance =
TrackingHelper.CalculateDistance(
new Location() { Latitude = 49.4833270, Longitude = 8.4748935 },
new Location() { Latitude = 49.4822117, Longitude = 8.4378147 },
new Location() { Latitude = 48.7794059, Longitude = 9.1755294 },
new Location() { Latitude = 52.5230417, Longitude = 13.4108447 });*/
    }
  /* private double CalculateDistance(double prevLat, double prevLong, double currLat, double currLong)
       {
           const double degreesToRadians = (Math.PI / 180.0);
           const double earthRadius = 6371; // kilometers
 
           // convert latitude and longitude values to radians
           var prevRadLat = prevLat * degreesToRadians;
           var prevRadLong = prevLong * degreesToRadians;
           var currRadLat = currLat * degreesToRadians;
           var currRadLong = currLong * degreesToRadians;
 
           // calculate radian delta between each position.
           var radDeltaLat = currRadLat - prevRadLat;
           var radDeltaLong = currRadLong - prevRadLong;
 
           // calculate distance
           var expr1 = (Math.Sin(radDeltaLat / 2.0) *
                        Math.Sin(radDeltaLat / 2.0)) +
 
                       (Math.Cos(prevRadLat) *
                        Math.Cos(currRadLat) *
                        Math.Sin(radDeltaLong / 2.0) *
                        Math.Sin(radDeltaLong / 2.0));
 
           var expr2 = 2.0 * Math.Atan2(Math.Sqrt(expr1),
                                        Math.Sqrt(1 - expr1));
 
           var distance = (earthRadius * expr2);
           return distance * 1000;  // return results as meters
       }*/
}