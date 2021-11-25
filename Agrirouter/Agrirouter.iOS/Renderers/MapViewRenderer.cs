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
using System.Reflection;
using Agrirouter.iOS.Extensions;
using Agrirouter.iOS.Renderers;
using Agrirouter.UI.Controls;
using CoreGraphics;
using Foundation;
using MapKit;
using SkiaSharp;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MapView), typeof(MapViewRenderer))]

namespace Agrirouter.iOS.Renderers
{
    public class MapViewRenderer : MapRenderer
    {
        private IList<Pin> _customPins;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                var nativeMap = Control as MKMapView;
                if (nativeMap is null)
                {
                    return;
                }
                
                nativeMap.GetViewForAnnotation = null;
            }

            if (e.NewElement != null)
            {
                var formsMap = (MapView) e.NewElement;
                _customPins = formsMap.Pins;
                
                var nativeMap = Control as MKMapView;
                if (nativeMap is null)
                {
                    return;
                }

                nativeMap.GetViewForAnnotation = GetViewForAnnotation;
            }
        }

        protected override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            MKAnnotationView annotationView = null;

            if (annotation is MKUserLocation)
            {
                return null;
            }

            var pin = GetCustomPin(annotation);
            if (pin == null)
            {
                return null;
            }

            if (pin is EndpointPin customPin)
            {
                annotationView = mapView.DequeueReusableAnnotation(customPin.Label);

                if (annotationView == null)
                {
                    annotationView = new MKAnnotationView(annotation, customPin.Label);
                }
                
                var imageData = NSData.FromArray(customPin.Image);
                
                annotationView.Image = UIImage.LoadFromData(imageData, UIScreen.MainScreen.NativeScale);
                annotationView.CalloutOffset = new CGPoint(0, 0);
                annotationView.Layer.AnchorPoint = new CGPoint(0.5, 1);
                annotationView.CanShowCallout = true;
            }

            return annotationView;
        }

        private Pin GetCustomPin(IMKAnnotation annotation)
        {
            var position = new Position(annotation.Coordinate.Latitude, annotation.Coordinate.Longitude);
            foreach (var pin in _customPins)
            {
                if (pin.Position == position)
                {
                    return pin;
                }
            }

            return null;
        }
    }
}