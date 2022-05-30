using Microsoft.Toolkit.Uwp.Notifications;

namespace METCSV.WPF.Helpers
{
    internal class Notification
    {
        public static void ShowNotification(bool success)
        {
            new ToastContentBuilder()
                .AddText("MET CSV - Bzzzzz")
                .AddText(success ? "Ukończono generowanie :)" : "Coś poczło nie tak :(")
                .Show();
        }
    }
}
