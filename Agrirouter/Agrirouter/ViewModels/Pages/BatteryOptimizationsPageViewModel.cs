/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */
using System.Windows.Input;
using Agrirouter.UI.Pages;
using Agrirouter.ViewModels.Pages.Abstract;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace Agrirouter.ViewModels.Pages
{
    public class BatteryOptimizationsPageViewModel : BasePageViewModel
    {
        public ICommand ContinueCommand { get; }

        public BatteryOptimizationsPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            ContinueCommand = new Command(Continue);
        }

        private async void Continue()
        {
            await NavigationService.NavigateAsync($"/{nameof(NavigationPage)}/{nameof(MapPage)}");
        }
    }
}