/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */
using Agrirouter.Resources;
using Agrirouter.ViewModels.Pages.Abstract;
using Prism.Navigation;
using Prism.Services;

namespace Agrirouter.ViewModels.Pages
{
    public class AboutPageViewModel : BasePageViewModel
    {
        public string AboutText { get; }
        
        public AboutPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            AboutText = AppResources.About;
        }
    }
}