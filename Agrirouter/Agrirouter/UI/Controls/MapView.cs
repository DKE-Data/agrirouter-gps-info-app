/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Agrirouter.UI.Controls
{
    public class MapView : Map
    {
        public MapView()
        {
            MoveToLastRegionOnLayoutChange = false;
        }

        public static readonly BindableProperty PositionProperty =
            BindableProperty.Create(nameof(Position), typeof(Position?), typeof(MapView), default, BindingMode.TwoWay, propertyChanged: PositionChanged);

        public static readonly BindableProperty PinsProperty =
            BindableProperty.Create(nameof(Pins), typeof(IList<EndpointPin>), typeof(MapView), propertyChanged: PinsChanged);

        public static readonly BindableProperty MapViewTypeProperty =
            BindableProperty.Create(nameof(MapViewType), typeof(MapViewTypeEnum), typeof(MapView), MapViewTypeEnum.Default, BindingMode.TwoWay, propertyChanged: MapViewTypeChanged);

        public Position? Position
        {
            get => (Position?) GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public MapViewTypeEnum MapViewType
        {
            get => (MapViewTypeEnum) GetValue(MapViewTypeProperty);
            set => SetValue(MapViewTypeProperty, value);
        }

        private static void PinsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var mapView = bindable as MapView;

            if (mapView is null)
            {
                return;
            }

            if (newValue is null)
            {
                return;
            }

            mapView.Pins.Clear();

            var pins = (IList<EndpointPin>) newValue;

            foreach (var pin in pins)
            {
                mapView.Pins.Add(pin);
            }

            mapView.HandleMapViewType(true);
        }

        private static void PositionChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var mapView = bindable as MapView;
            if (mapView is null)
            {
                return;
            }

            if (newValue is null)
            {
                return;
            }

            mapView.HandleMapViewType();
        }

        private static void MapViewTypeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var mapView = bindable as MapView;
            if (mapView is null)
            {
                return;
            }

            if (oldValue is null)
            {
                return;
            }

            if (newValue is null)
            {
                return;
            }

            if (oldValue is MapViewTypeEnum oldMapViewType && newValue is MapViewTypeEnum newMapViewType)
            {
                if ((oldMapViewType == MapViewTypeEnum.AlwaysShowAllPinsAndMyPosition || oldMapViewType == MapViewTypeEnum.AlwaysShowMyPosition) && newMapViewType == MapViewTypeEnum.Default)
                {
                    return;
                }
            }

            mapView.HandleMapViewType();
        }

        private void HandleMapViewType(bool isDefaultIgnored = false)
        {
            switch (MapViewType)
            {
                case MapViewTypeEnum.AlwaysShowMyPosition:
                    ShowMyPosition();
                    break;
                case MapViewTypeEnum.AlwaysShowAllPinsAndMyPosition:
                    ShowAllPinsAndMyPosition();
                    break;
                case MapViewTypeEnum.Default when !isDefaultIgnored:
                    ShowMyPosition();
                    break;
            }
        }

        public void ShowMyPosition()
        {
            if (Position.HasValue)
            {
                var mapSpan = VisibleRegion is null ? new MapSpan(Position.Value, 0.01, 0.01) : MapSpan.FromCenterAndRadius(Position.Value, VisibleRegion.Radius);

                MoveToRegion(mapSpan);
            }
        }

        public void ShowAllPinsAndMyPosition()
        {
            if (!Pins.Any())
            {
                ShowMyPosition();
                return;
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                var positions = Pins.Select(pin => pin.Position).ToList();

                if (Position.HasValue)
                {
                    positions.Add(Position.Value);
                }

                var centerPosition = GetCentralGeoCoordinate(positions);

                double maxDistance = 0;

                foreach (var position in positions)
                {
                    var distance = DistanceTo(centerPosition, position);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                    }
                }

                var mapSpan = MapSpan.FromCenterAndRadius(centerPosition, Distance.FromMiles(maxDistance * 1.1));
                MoveToRegion(mapSpan);
            });
        }

        public static double DistanceTo(Position baseCoordinates, Position targetCoordinates)
        {
            var baseRad = Math.PI * baseCoordinates.Latitude / 180;
            var targetRad = Math.PI * targetCoordinates.Latitude / 180;
            var theta = baseCoordinates.Longitude - targetCoordinates.Longitude;
            var thetaRad = Math.PI * theta / 180;

            var distance = Math.Sin(baseRad) * Math.Sin(targetRad) + Math.Cos(baseRad) * Math.Cos(targetRad) * Math.Cos(thetaRad);
            distance = Math.Acos(distance);
            distance = distance * 180 / Math.PI;
            distance = distance * 60 * 1.1515;

            return distance;
        }

        public Position GetCentralGeoCoordinate(IEnumerable<Position> items)
        {
            var positions = items.ToArray();

            if (positions.Length == 1)
            {
                return positions.Single();
            }

            double x = 0;
            double y = 0;
            double z = 0;

            foreach (var position in positions)
            {
                var latitude = position.Latitude * Math.PI / 180;
                var longitude = position.Longitude * Math.PI / 180;

                x += Math.Cos(latitude) * Math.Cos(longitude);
                y += Math.Cos(latitude) * Math.Sin(longitude);
                z += Math.Sin(latitude);
            }

            var total = positions.Length;

            x = x / total;
            y = y / total;
            z = z / total;

            var centralLongitude = Math.Atan2(y, x);
            var centralSquareRoot = Math.Sqrt(x * x + y * y);
            var centralLatitude = Math.Atan2(z, centralSquareRoot);

            return new Position(centralLatitude * 180 / Math.PI, centralLongitude * 180 / Math.PI);
        }
    }

    public enum MapViewTypeEnum
    {
        Default = 0,
        AlwaysShowMyPosition = 1,
        AlwaysShowAllPinsAndMyPosition = 2
    }

    public class EndpointPin : Pin, IDisposable
    {
        private bool _isDisposed;

        public EndpointPin(string label, bool isActive)
        {
            Label = label ?? "No name";
            IsActive = isActive;
            Type = PinType.Place;
            MarkerClicked += OnMarkerClicked;
        }

        public static readonly BindableProperty TappedCommandProperty = BindableProperty.Create(nameof(TappedCommand), typeof(ICommand), typeof(EndpointPin));

        public ICommand TappedCommand
        {
            get => (ICommand) GetValue(TappedCommandProperty);
            set => SetValue(TappedCommandProperty, value);
        }

        public bool IsActive { get; }

        public byte[] Image { get; set; }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            MarkerClicked -= OnMarkerClicked;

            _isDisposed = true;
        }

        private void OnMarkerClicked(object sender, PinClickedEventArgs e)
        {
            e.HideInfoWindow = true;
            TappedCommand?.Execute(sender as EndpointPin);
        }
    }
}