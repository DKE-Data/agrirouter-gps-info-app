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
using Agrirouter.iOS.Renderers;
using Agrirouter.UI.Controls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedEntryCell), typeof(CustomEntryCellRenderer))]
namespace Agrirouter.iOS.Renderers
{
    public class CustomEntryCellRenderer : EntryCellRenderer, IDisposable
    {
        private UITableViewCell _cell;
        private ExtendedEntryCell _nativeCell;
        private bool _disposed;

        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            _nativeCell = (ExtendedEntryCell) item;
            _cell = base.GetCell(_nativeCell, reusableCell, tv);

            _nativeCell.PropertyChanged += HandlePropertyChanged;

            UpdateTextColor();

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
            var textColor = _nativeCell.IsEnabled ? _nativeCell.TextColor : Color.Gray;
            ((UITextField) _cell.Subviews[0].Subviews[0]).TextColor = textColor.ToUIColor();
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