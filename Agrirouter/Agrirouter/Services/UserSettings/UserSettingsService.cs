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
using System.Linq;
using System.Threading.Tasks;
using Agrirouter.Common.AgrirouterApi;
using Agrirouter.Models;
using Agrirouter.Repositories.UserSettings;
using Agrirouter.ViewModels.Items;
using Xamarin.Forms;
using QualityAssuranceEnvironment = Agrirouter.Common.AgrirouterApi.QualityAssuranceEnvironment;

namespace Agrirouter.Services.UserSettings
{
    public class UserSettingsService : IUserSettingsService
    {
        private readonly IUserSettingsRepository _userSettingsRepository;
        private readonly IDeviceInfoService _deviceInfoService;

        public List<EnvironmentViewModel> Environments { get; }

        public List<CycleViewModel> RecordCycleIntervals { get; }

        public List<CycleViewModel> SendCycleIntervals { get; }

        public UserSettingsService(IUserSettingsRepository userSettingsRepository)
        {
            _userSettingsRepository = userSettingsRepository;
            _deviceInfoService = DependencyService.Get<IDeviceInfoService>();

            Environments = new List<EnvironmentViewModel>();
            Environments.Add(new EnvironmentViewModel("QA", new QualityAssuranceEnvironment()));
            Environments.Add(new EnvironmentViewModel("Production", new ProductionEnvironment()));

            RecordCycleIntervals = new List<CycleViewModel>();
            RecordCycleIntervals.Add(new CycleViewModel("1 s", 1000));
            RecordCycleIntervals.Add(new CycleViewModel("5 s", 5000));
            RecordCycleIntervals.Add(new CycleViewModel("10 s", 10000));
            RecordCycleIntervals.Add(new CycleViewModel("20 s", 20000));
            RecordCycleIntervals.Add(new CycleViewModel("1 minute", 60000));

            SendCycleIntervals = new List<CycleViewModel>();
            SendCycleIntervals.Add(new CycleViewModel("10 s", 10000));
            SendCycleIntervals.Add(new CycleViewModel("20 s", 20000));
            SendCycleIntervals.Add(new CycleViewModel("1 minute", 60000));
        }

        public EnvironmentViewModel GetEnvironmentByName(string name)
        {
            return Environments.FirstOrDefault(item => item.Name == name);
        }

        public Task<UserSettingsModel> GetSettings()
        {
            return _userSettingsRepository.GetAsync();
        }

        public string GetUniqId()
        {
            return _deviceInfoService.GetUniqId();
        }

        public async Task<EnvironmentViewModel> GetCurrentEnvironment()
        {
            var settings = await _userSettingsRepository.GetAsync();
            return GetEnvironmentByName(settings.EnvironmentName);
        }

        public Task SaveSettings(UserSettingsModel userSettingsModel)
        {
            return _userSettingsRepository.SetAsync(userSettingsModel);
        }
    }
}