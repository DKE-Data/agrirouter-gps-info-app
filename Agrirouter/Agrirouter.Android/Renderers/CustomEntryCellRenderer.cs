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
using System.ComponentModel;
using Agrirouter.Droid.Renderers;
using Agrirouter.UI.Controls;
using Android.Content;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ExtendedEntryCell), typeof(CustomEntryCellRenderer))]
namespace Agrirouter.Droid.Renderers
{
    public class CustomEntryCellRenderer : EntryCellRenderer, IDisposable
    {
        private ExtendedEntryCell _nativeCell;
        private EntryCellView _cell;
        private bool _disposed;

        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, ViewGroup parent, Context context)
        {
            _nativeCell = (ExtendedEntryCell) item;
            _cell = base.GetCellCore(item, convertView, parent, context) as EntryCellView;

            _nativeCell.PropertyChanged += HandlePropertyChanged;
            
            if (_cell != null)
            {
                var textField = _cell.EditText as TextView;
                textField.SetBackgroundColor(Android.Graphics.Color.Argb(0, 0, 0, 0));
                textField.SetTextSize(Android.Util.ComplexUnitType.Dip, 15);
                textField.SetHintTextColor(Android.Graphics.Color.LightGray);
                textField.SetPadding(30,0,0,0);
                UpdateTextColor();
            }

            return _cell;
        }
        
        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ExtendedEntryCell.TextColorProperty.PropertyName ||
                e.PropertyName == ExtendedEntryCell.IsEnabledProperty.PropertyName)
            {
                UpdateTextColor();
            }
        }
        
        private void UpdateTextColor()
        {
            var textField = _cell.EditText as TextView;
            var textColor = _nativeCell.IsEnabled ? _nativeCell.TextColor : Color.Gray;
            textField.SetTextColor(textColor.ToAndroid());
        }
        
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _nativeCell.PropertyChanged -= HandlePropertyChanged;
            _cell?.Dispose();

            _disposed = true;
        }
    }
}