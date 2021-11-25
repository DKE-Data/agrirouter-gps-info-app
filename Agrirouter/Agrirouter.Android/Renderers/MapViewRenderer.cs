/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */

using System.ComponentModel;
using Agrirouter.Droid.Renderers;
using Agrirouter.UI.Controls;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using MapView = Agrirouter.UI.Controls.MapView;

[assembly: ExportRenderer(typeof(MapView), typeof(MapViewRenderer))]

namespace Agrirouter.Droid.Renderers
{
    public class MapViewRenderer : MapRenderer
    {
        private GoogleMap _map;

        public MapViewRenderer(Context context) : base(context)
        {
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }

        protected override void OnMapReady(GoogleMap map)
        {
            _map = map;
            
            base.OnMapReady(_map);

            _map.UiSettings.ZoomControlsEnabled = false;
            _map.UiSettings.MyLocationButtonEnabled = false;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var locationButton = ((Android.Views.View) FindViewById(int.Parse("1"))?.Parent)?.FindViewById(int.Parse("2"));
            if (locationButton is null)
            {
                return;
            }

            locationButton.Visibility = ViewStates.Gone;
        }

        protected override MarkerOptions CreateMarker(Pin pin)
        {
            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
            marker.SetTitle(pin.Label);
            marker.SetSnippet(pin.Address);

            if (pin is EndpointPin customPin)
            {
                marker.SetIcon(BitmapDescriptorFactory.FromBitmap(BitmapFactory.DecodeByteArray(customPin.Image, 0, customPin.Image.Length)));
            }

            return marker;
        }
    }
}