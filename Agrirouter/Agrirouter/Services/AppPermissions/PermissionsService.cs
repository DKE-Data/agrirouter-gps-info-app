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
using System.Threading.Tasks;
using Acr.UserDialogs;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Agrirouter.Services.AppPermissions
{
    public class PermissionsService : IPermissionsService
    {
        public async Task<bool> AskingForPhotosPermission(bool openSettings = false)
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Photos>();

                if (status != PermissionStatus.Granted)
                {
                    var results = await Permissions.RequestAsync<Permissions.Photos>();

                    status = await Permissions.CheckStatusAsync<Permissions.Photos>();

                    if (Device.RuntimePlatform == Device.iOS && status == PermissionStatus.Denied && openSettings)
                    {
                        //var result = await UserDialogs.Instance.ConfirmAsync(Strings.ProvideAccessToPhotos, Strings.NeedPermission, Strings.GoToSettings, Strings.Cancel);

                        //if (result)
                        //    AppInfo.ShowSettingsUI();

                        return false;
                    }
                }

                if (status == PermissionStatus.Granted)
                {
                    return true;
                }
                else if (status != PermissionStatus.Unknown)
                {
                    //await _userDialogs.AlertAsync(Texts.CanNotContinue, Texts.PhotosDenied, Texts.Ok);
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> AskingForCameraPermission(bool openSettings = false)
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

                if (status != PermissionStatus.Granted)
                {
                    var results = await Permissions.RequestAsync<Permissions.Camera>();

                    status = await Permissions.CheckStatusAsync<Permissions.Camera>();

                    if (Device.RuntimePlatform == Device.iOS && status == PermissionStatus.Denied && openSettings)
                    {
                        //var result = await UserDialogs.Instance.ConfirmAsync(Strings.ProvideAccessToCamera, Strings.NeedPermission, Strings.GoToSettings, Strings.Cancel);

                        //if (result)
                        //    AppInfo.ShowSettingsUI();

                        return false;
                    }
                }

                if (status == PermissionStatus.Granted)
                {
                    return true;
                }
                else if (status != PermissionStatus.Unknown)
                {
                    //await _userDialogs.AlertAsync(Texts.CanNotContinue, Texts.PhotosDenied, Texts.Ok);
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> AskingForLocationPermission()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();

                if (status != PermissionStatus.Granted)
                {
                    _ = await Permissions.RequestAsync<Permissions.LocationAlways>();

                    status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();

                    if (status == PermissionStatus.Denied)
                    {
                        var result = await UserDialogs.Instance.ConfirmAsync("This app needs access to your location. Please, provide \"Always / All the time\" location permission first", "Location Permissions", "Open Settings", "Cancel");
                        if (result)
                        {
                            AppInfo.ShowSettingsUI();
                        }

                        return false;
                    }
                }

                if (status == PermissionStatus.Granted)
                {
                    return true;
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await UserDialogs.Instance.AlertAsync("Can not continue", "Location Permissions", "Ok");
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> AskingForReadStoragePermission()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();

                if (status != PermissionStatus.Granted)
                {
                    var results = await Permissions.RequestAsync<Permissions.StorageRead>();

                    status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();

                    if (Device.RuntimePlatform == Device.iOS && status == PermissionStatus.Denied)
                    {
                        //var result = await _userDialogs.ConfirmAsync(Texts.ProvideAccessToPhotos, Texts.NeedPermission,
                        //    Texts.GoToSettings, Texts.Cancel);

                        //if (result)
                        //    AppInfo.ShowSettingsUI();

                        return false;
                    }
                }

                if (status == PermissionStatus.Granted)
                {
                    return true;
                }
                else if (status != PermissionStatus.Unknown)
                {
                    //await _userDialogs.AlertAsync(Texts.CanNotContinue, Texts.PhotosDenied, Texts.Ok);
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> AskingForWriteStoragePermission()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

                if (status != PermissionStatus.Granted)
                {
                    _ = await Permissions.RequestAsync<Permissions.StorageWrite>();

                    status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

                    if (Device.RuntimePlatform == Device.iOS && status == PermissionStatus.Denied)
                        return false;
                }

                return status == PermissionStatus.Granted;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> IsPermissionGranted<T>(T permission) where T : Permissions.BasePermission
        {
            var status = await permission.CheckStatusAsync();
            return status == PermissionStatus.Granted;
        }
    }
}