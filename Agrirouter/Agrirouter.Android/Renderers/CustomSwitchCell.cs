/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */

using Agrirouter.Droid.Renderers;
using Android.Support.V4.Content.Res;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Android.Graphics.Color;

[assembly: ExportRenderer(typeof(SwitchCell), typeof(CustomSwitchCell))]

namespace Agrirouter.Droid.Renderers
{
    public class CustomSwitchCell : SwitchCellRenderer
    {
        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, Android.Views.ViewGroup parent, Android.Content.Context context)
        {
            var cell = base.GetCellCore(item, convertView, parent, context);
            var child = ((LinearLayout) cell).GetChildAt(1);
            if (child is null)
            {
                return cell;
            }

            var label = (TextView) ((LinearLayout) child).GetChildAt(0);
            if (label is null)
            {
                return cell;
            }

            label.SetTextColor(Color.Black);

            return cell;
        }
    }
}