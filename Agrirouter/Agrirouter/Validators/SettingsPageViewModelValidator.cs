/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */
using Agrirouter.ViewModels.Pages;
using FluentValidation;

namespace Agrirouter.Validators
{
    public class SettingsPageViewModelValidator : AbstractValidator<SettingsPageViewModel>
    {
        public SettingsPageViewModelValidator()
        {
            ValidatorOptions.Global.LanguageManager.Enabled = false;

            RuleFor(x => x.RegistrationCode)
                .NotNull().NotEmpty().WithMessage("Please enter a Registration Code").DependentRules(() =>
                {
                    RuleFor(x => x.RegistrationCode.Trim())
                        .Length(10).WithMessage("Please enter correct Registration Code. Length should be 10 chars");
                });
        }
    }
}