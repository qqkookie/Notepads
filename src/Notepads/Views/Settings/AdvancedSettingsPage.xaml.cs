namespace Notepads.Views.Settings
{
    using Notepads.Services;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class AdvancedSettingsPage : Page
    {
        public AdvancedSettingsPage()
        {
            InitializeComponent();

            ShowStatusBarToggleSwitch.IsOn = AppSettingsService.ShowStatusBar;
            EnableSmartCopyToggleSwitch.IsOn = AppSettingsService.IsSmartCopyEnabled;

            // Disable session snapshot toggle for shadow windows
            if (!App.IsPrimaryInstance)
            {
                EnableSessionSnapshotToggleSwitch.IsOn = false;
                EnableSessionSnapshotToggleSwitch.IsEnabled = false;
            }
            else
            {
                EnableSessionSnapshotToggleSwitch.IsOn = AppSettingsService.IsSessionSnapshotEnabled;
            }

            ExitWhenLastTabClosedToggleSwitch.IsOn = AppSettingsService.ExitWhenLastTabClosed;
            AlwaysOpenNewWindowToggleSwitch.IsOn = AppSettingsService.AlwaysOpenNewWindow;
            UntitledTimeStampToggleSwitch.IsOn = AppSettingsService.UntitledTimeStamp;

            if (App.IsGameBarWidget)
            {
                // these settings don't make sense for Game Bar, there can be only one
                SessionSnapshotSettingsTitle.Visibility = Visibility.Collapsed;
                SessionSnapshotSettingsControls.Visibility = Visibility.Collapsed;
                LaunchPreferenceSettingsTitle.Visibility = Visibility.Collapsed;
                LaunchPreferenceSettingsControls.Visibility = Visibility.Collapsed;
            }

            Loaded += AdvancedSettings_Loaded;
            Unloaded += AdvancedSettings_Unloaded;
        }

        private void AdvancedSettings_Loaded(object sender, RoutedEventArgs e)
        {
            ShowStatusBarToggleSwitch.Toggled += ShowStatusBarToggleSwitch_Toggled;
            EnableSmartCopyToggleSwitch.Toggled += EnableSmartCopyToggleSwitch_Toggled;
            EnableSessionSnapshotToggleSwitch.Toggled += EnableSessionBackupAndRestoreToggleSwitch_Toggled;
            ExitWhenLastTabClosedToggleSwitch.Toggled += ExitWhenLastTabClosedToggleSwitch_Toggled;
            AlwaysOpenNewWindowToggleSwitch.Toggled += AlwaysOpenNewWindowToggleSwitch_Toggled;
        }

        private void AdvancedSettings_Unloaded(object sender, RoutedEventArgs e)
        {
            ShowStatusBarToggleSwitch.Toggled -= ShowStatusBarToggleSwitch_Toggled;
            EnableSmartCopyToggleSwitch.Toggled -= EnableSmartCopyToggleSwitch_Toggled;
            EnableSessionSnapshotToggleSwitch.Toggled -= EnableSessionBackupAndRestoreToggleSwitch_Toggled;
            ExitWhenLastTabClosedToggleSwitch.Toggled -= ExitWhenLastTabClosedToggleSwitch_Toggled;
            AlwaysOpenNewWindowToggleSwitch.Toggled -= AlwaysOpenNewWindowToggleSwitch_Toggled;
        }

        private void EnableSmartCopyToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            AppSettingsService.IsSmartCopyEnabled = EnableSmartCopyToggleSwitch.IsOn;
        }

        private void EnableSessionBackupAndRestoreToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            AppSettingsService.IsSessionSnapshotEnabled = EnableSessionSnapshotToggleSwitch.IsOn;
        }

        private void ShowStatusBarToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            AppSettingsService.ShowStatusBar = ShowStatusBarToggleSwitch.IsOn;
        }

        private void ExitWhenLastTabClosedToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            AppSettingsService.ExitWhenLastTabClosed = ExitWhenLastTabClosedToggleSwitch.IsOn;
        }

        private void AlwaysOpenNewWindowToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            AppSettingsService.AlwaysOpenNewWindow = AlwaysOpenNewWindowToggleSwitch.IsOn;
        }

        private void UntitledTimeStampToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            AppSettingsService.UntitledTimeStamp = UntitledTimeStampToggleSwitch.IsOn;
        }
    }
}
