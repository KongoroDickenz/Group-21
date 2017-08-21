using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Device.Location;
using Windows.Devices.Geolocation;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Maps.Controls;

namespace ELBA
{
    public partial class Page2 : PhoneApplicationPage
    {
        List<GeoCoordinate> MyCoordinates = new List<GeoCoordinate>();//instantiate an object of type List
        RouteQuery MyQuery = null;
        GeocodeQuery Mygeocodequery = null;


        // Constructor
        public Page2()
        {
            InitializeComponent();
            this.GetCoordinates();
        }

        private async void GetCoordinates()
        {
            // Get the phone's current location.
            Geolocator MyGeolocator = new Geolocator();
            MyGeolocator.DesiredAccuracyInMeters = 5;
            Geoposition MyGeoPosition = null;
            try
            {
                MyGeoPosition = await MyGeolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10));
                MyCoordinates.Add(new GeoCoordinate(MyGeoPosition.Coordinate.Latitude, MyGeoPosition.Coordinate.Longitude));
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
            Mygeocodequery = new GeocodeQuery();
            Mygeocodequery.SearchTerm = "Mulago Hospital, UG";//here u enter the place to be searched for
            Mygeocodequery.GeoCoordinate = new GeoCoordinate(MyGeoPosition.Coordinate.Latitude, MyGeoPosition.Coordinate.Longitude);
            Mygeocodequery.QueryCompleted += Mygeocodequery_QueryCompleted;
            Mygeocodequery.QueryAsync();
        }
        void Mygeocodequery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            if (e.Error == null)
            {
                MyQuery = new RouteQuery();
                MyCoordinates.Add(e.Result[0].GeoCoordinate);
                MyQuery.Waypoints = MyCoordinates;
                MyQuery.QueryCompleted += MyQuery_QueryCompleted;
                MyQuery.QueryAsync();
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
                MyQuery.Dispose();
            }
        }

    }
}