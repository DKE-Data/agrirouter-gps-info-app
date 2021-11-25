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

namespace Agrirouter.Services.UserInteractions
{
    public interface IUserInteractionService
    {
        void ShowLoading(string message = "");

        void HideLoading();

        Task ShowAlert(string text);

        Task ShowError(string text, string title = "", string okButtonText = "Ok");

        void ShowToast(string text);

    }
}