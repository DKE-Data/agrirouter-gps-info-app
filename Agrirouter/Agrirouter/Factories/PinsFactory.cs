/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Agrirouter.Common.Extensions;
using Agrirouter.Models;
using Agrirouter.UI.Controls;
using SkiaSharp;
using Xamarin.Essentials;
using Xamarin.Forms.Maps;

namespace Agrirouter.Factories
{
    public class PinsFactory : IPinsFactory
    {
        private const string PinOnlineResource = "Agrirouter.Resources.ic_pin_online.png";
        private const string PinOfflineResource = "Agrirouter.Resources.ic_pin_offline.png";
        
        private SKBitmap pinOnlineBitmap;
        private SKBitmap pinOfflineBitmap;

        public PinsFactory()
        {
            var assembly = GetType().GetTypeInfo().Assembly;

            using (var stream = assembly.GetManifestResourceStream(PinOnlineResource))
            {
                pinOnlineBitmap = SKBitmap.Decode(stream);
            }

            using (var stream = assembly.GetManifestResourceStream(PinOfflineResource))
            {
                pinOfflineBitmap = SKBitmap.Decode(stream);
            }
        }

        public IEnumerable<EndpointPin> ProducePins(IEnumerable<EndpointModel> endpointModels, ICommand tappedCommand)
        {
            return endpointModels.Where(endpointModel => endpointModel.HasPosition && endpointModel.IsVisible)
                                 .Select(endpointModel => ProducePin(endpointModel, tappedCommand))
                                 .ToList();
        }

        private EndpointPin ProducePin(EndpointModel model, ICommand tappedCommand)
        {
            if (model == null)
            {
                return null;
            }

            return new EndpointPin(model.Name, model.IsActive)
            {
                TappedCommand = tappedCommand,
                Position = new Position(model.Latitude, model.Longitude),
                Address = $"Last updated: {model.LastUpdateDateTime.ToLocalTime().ToString("HH:mm")}",
                Image = GeneratePinImage(model.Name, model.IsActive)
            };
        }

        private static float? _density;

        private static float Density
        {
            get
            {
                if (_density is null)
                {
                    _density = (float) DeviceDisplay.MainDisplayInfo.Density;
                }

                return _density.Value;
            }
        }
        
        private float ToPixels(float dp) =>
            dp * Density;
        
        private byte[] GeneratePinImage(string endpointName, bool isActive)
        {
            var pinBitmap = isActive ? pinOnlineBitmap : pinOfflineBitmap;

            var textBounds = new SKRect();
            
            var textPaint = new SKPaint();
            textPaint.TextSize = ToPixels(12);
            textPaint.IsAntialias = true;
            textPaint.MeasureText(endpointName, ref textBounds);

            var pinResizeInfo = new SKImageInfo((int) ToPixels(60), (int) ToPixels(60));
            pinBitmap = pinBitmap.Resize(pinResizeInfo, SKFilterQuality.High);

            var imageMarginTop = ToPixels(10);
            var textMarginLeftRight = ToPixels(10);
            
            var canvasHeight = textBounds.Height + imageMarginTop + imageMarginTop / 2 + pinBitmap.Height;
            var canvasWidth = textMarginLeftRight * 2 + textBounds.Width;

            var resultBitmap = new SKBitmap((int) canvasWidth, (int) canvasHeight);

            var whiteFramePaint = new SKPaint();
            whiteFramePaint.Style = SKPaintStyle.StrokeAndFill;
            whiteFramePaint.Color = SKColors.White;

            using (var canvas = new SKCanvas(resultBitmap))
            {
                canvas.DrawRoundRect(new SKRoundRect(new SKRect(0, 0, canvasWidth, textBounds.Height + imageMarginTop), ToPixels(15)), whiteFramePaint);
                canvas.DrawText(endpointName, canvasWidth / 2 - textBounds.Width / 2, textBounds.Height + imageMarginTop / 2 - ToPixels(2), textPaint);
                canvas.DrawBitmap(pinBitmap, new SKPoint(canvasWidth / 2 - pinBitmap.Width / 2, imageMarginTop + imageMarginTop / 2 + textBounds.Height));
            }

            return SKImage.FromPixels(resultBitmap.PeekPixels()).Encode().AsStream().ToByteArray();
        }
    }
}