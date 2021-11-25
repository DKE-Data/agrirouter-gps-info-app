using System.Threading.Tasks;
using Acr.UserDialogs;
using Xamarin.Forms;

namespace Agrirouter.Services.UserInteractions
{
    public class UserInteractionService : IUserInteractionService
    {
        public void ShowLoading(string loadingMessage = "")
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                UserDialogs.Instance.ShowLoading(loadingMessage);
                await Task.Delay(100);
            });
        }

        public void HideLoading()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(100);
                UserDialogs.Instance.HideLoading();
            });
        }

        public async Task ShowAlert(string text)
        {
            await UserDialogs.Instance.AlertAsync(text, nameof(Agrirouter).ToLower(), "Ok");
        }

        public async Task ShowError(string text, string title = "", string okButtonText = "Ok")
        {
            var titleString = string.IsNullOrEmpty(title) ? "Error" : $"Error: {title}";
            await UserDialogs.Instance.AlertAsync(text, titleString, okButtonText);
        }

        public void ShowToast(string text)
        {
            UserDialogs.Instance.Toast(new ToastConfig(text)
            {
                BackgroundColor = Color.Gray
            });
        }
    }
}