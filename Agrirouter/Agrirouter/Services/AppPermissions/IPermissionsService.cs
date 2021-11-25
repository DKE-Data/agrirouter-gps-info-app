/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Agrirouter.Services.AppPermissions
{
    public interface IPermissionsService
    {
        Task<bool> AskingForPhotosPermission(bool openSettings = false);

        Task<bool> AskingForCameraPermission(bool openSettings = false);

        Task<bool> AskingForReadStoragePermission();

        Task<bool> AskingForWriteStoragePermission();

        Task<bool> AskingForLocationPermission();

        Task<bool> IsPermissionGranted<T>(T permission) where T : Permissions.BasePermission;
    }
}