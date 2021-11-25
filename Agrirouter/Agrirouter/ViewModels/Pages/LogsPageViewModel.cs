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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Agrirouter.ViewModels.Pages.Abstract;
using Plugin.SimpleLogger;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Essentials;

namespace Agrirouter.ViewModels.Pages
{
    public class LogsPageViewModel : BasePageViewModel
    {
        private readonly string _allLogs;
        public List<string> Logs { get; set; }

        public ICommand DeleteLogsCommand { get; set; }
        
        public ICommand ExportLogsCommand { get; set; }

        public LogsPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            _allLogs = CrossSimpleLogger.Current.GetAllLogContent().Trim();
            DeleteLogsCommand = new DelegateCommand(async () => await DeleteLogsCommandExecute());
            ExportLogsCommand = new DelegateCommand(async () => await ExportLogsCommandExecute());
        }
        
        private async Task ExportLogsCommandExecute()
        {
            try {
                string fileName = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData), "logs.txt");
                File.WriteAllText(fileName, _allLogs);
                
                ShareFile shareFile = new ShareFile(fileName);
                var request = new ShareFileRequest()
                {
                    File = shareFile,
                    Title = "Share Logs"
                };
                await Share.RequestAsync(request);
                File.Delete(fileName);
            }catch(Exception e)
            {
                Logs.Add("Logs could not be exported; Error message: " + e.Message);
            }
        }

        private Task DeleteLogsCommandExecute()
        {
            CrossSimpleLogger.Current.PurgeLog();
            Logs = new List<string>();
            return Task.CompletedTask;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            Logs = new List<string>(_allLogs.Split("\r\n").ToList());
        }
    }
}