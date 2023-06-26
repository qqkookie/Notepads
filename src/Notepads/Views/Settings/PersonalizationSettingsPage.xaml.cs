namespace Notepads.Views.Settings
{
    using System.Linq;

    using Notepads.Extensions;
    using Notepads.Services;
    using Notepads.Utilities;
    using System.Collections.Generic;

    using Windows.Globalization;
    using Windows.System.Power;
    using Windows.UI;
    using Windows.UI.ViewManagement;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Media;
    using System;

    public sealed partial class PersonalizationSettingsPage : Page
    {
        private readonly UISettings UISettings = new UISettings();
        private readonly IReadOnlyCollection<LanguageItem> SupportedLanguages = LanguageUtility.GetSupportedLanguageItems();

        public PersonalizationSettingsPage()
        {
            InitializeComponent();

            if (ThemeSettingsService.UseWindowsTheme)
            {
                ThemeModeDefaultButton.IsChecked = true;
            }
            else
            {
                switch (ThemeSettingsService.ThemeMode)
                {
                    case ElementTheme.Light:
                        ThemeModeLightButton.IsChecked = true;
                        break;
                    case ElementTheme.Dark:
                        ThemeModeDarkButton.IsChecked = true;
                        break;
                }
            }

            // AccentColorToggle.IsOn = ThemeSettingsService.UseWindowsAccentColor;
            // AccentColorPicker.IsEnabled = !ThemeSettingsService.UseWindowsAccentColor;
            BackgroundTintOpacitySlider.Value = ThemeSettingsService.AppBackgroundPanelTintOpacity * 100;
            // AccentColorPicker.Color = ThemeSettingsService.AppAccentColor;

            if (App.IsGameBarWidget)
            {
                // Game Bar widgets do not support transparency, disable this setting
                BackgroundTintOpacityTitle.Visibility = Visibility.Collapsed;
                BackgroundTintOpacityControls.Visibility = Visibility.Collapsed;
            }
            else
            {
                BackgroundTintOpacitySlider.IsEnabled = UISettings.AdvancedEffectsEnabled &&
                                                        PowerManager.EnergySaverStatus != EnergySaverStatus.On;
            }

            LanguagePicker.SelectedItem = SupportedLanguages.FirstOrDefault(language => language.ID == ApplicationLanguages.PrimaryLanguageOverride);
            RestartPrompt.Visibility = LanguageUtility.CurrentLanguageID == ApplicationLanguages.PrimaryLanguageOverride ? Visibility.Collapsed : Visibility.Visible;

            InitializeSearchEngineSettings();

            Loaded += PersonalizationSettings_Loaded;
            Unloaded += PersonalizationSettings_Unloaded;
        }

        private void InitializeSearchEngineSettings()
        {
            switch (AppSettingsService.EditorDefaultSearchEngine)
            {
                case SearchEngine.Bing:
                    BingRadioButton.IsChecked = true;
                    CustomSearchUrl.IsEnabled = false;
                    break;
                case SearchEngine.Google:
                    GoogleRadioButton.IsChecked = true;
                    CustomSearchUrl.IsEnabled = false;
                    break;
                case SearchEngine.DuckDuckGo:
                    DuckDuckGoRadioButton.IsChecked = true;
                    CustomSearchUrl.IsEnabled = false;
                    break;
                case SearchEngine.Custom:
                    CustomSearchUrlRadioButton.IsChecked = true;
                    CustomSearchUrl.IsEnabled = true;
                    break;
            }

            if (!string.IsNullOrEmpty(AppSettingsService.EditorCustomMadeSearchUrl))
            {
                CustomSearchUrl.Text = AppSettingsService.EditorCustomMadeSearchUrl;
            }
        }

        private async void ThemeSettingsService_OnAccentColorChanged(object sender, Color color)
        {
            await Dispatcher.CallOnUIThreadAsync(() =>
            {
                BackgroundTintOpacitySlider.Foreground = new SolidColorBrush(color);
                // AccentColorPicker.ColorChanged -= AccentColorPicker_OnColorChanged;
                // AccentColorPicker.Color = color;
                // AccentColorPicker.ColorChanged += AccentColorPicker_OnColorChanged;
            });
        }

        private void PersonalizationSettings_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeModeDefaultButton.Checked += ThemeRadioButton_OnChecked;
            ThemeModeLightButton.Checked += ThemeRadioButton_OnChecked;
            ThemeModeDarkButton.Checked += ThemeRadioButton_OnChecked;
            BackgroundTintOpacitySlider.ValueChanged += BackgroundTintOpacitySlider_OnValueChanged;
            // AccentColorToggle.Toggled += WindowsAccentColorToggle_OnToggled;
            // AccentColorPicker.ColorChanged += AccentColorPicker_OnColorChanged;
            ThemeSettingsService.OnAccentColorChanged += ThemeSettingsService_OnAccentColorChanged;
            if (!App.IsGameBarWidget)
            {
                UISettings.AdvancedEffectsEnabledChanged += UISettings_AdvancedEffectsEnabledChanged;
                PowerManager.EnergySaverStatusChanged += PowerManager_EnergySaverStatusChanged;
            }
            LanguagePicker.SelectionChanged += LanguagePicker_SelectionChanged;

            BingRadioButton.Checked += SearchEngineRadioButton_Checked;
            GoogleRadioButton.Checked += SearchEngineRadioButton_Checked;
            DuckDuckGoRadioButton.Checked += SearchEngineRadioButton_Checked;
            CustomSearchUrlRadioButton.Checked += SearchEngineRadioButton_Checked;
        }

        private void PersonalizationSettings_Unloaded(object sender, RoutedEventArgs e)
        {
            ThemeModeDefaultButton.Checked -= ThemeRadioButton_OnChecked;
            ThemeModeLightButton.Checked -= ThemeRadioButton_OnChecked;
            ThemeModeDarkButton.Checked -= ThemeRadioButton_OnChecked;
            BackgroundTintOpacitySlider.ValueChanged -= BackgroundTintOpacitySlider_OnValueChanged;
            // AccentColorToggle.Toggled -= WindowsAccentColorToggle_OnToggled;
            // AccentColorPicker.ColorChanged -= AccentColorPicker_OnColorChanged;
            ThemeSettingsService.OnAccentColorChanged -= ThemeSettingsService_OnAccentColorChanged;
            if (!App.IsGameBarWidget)
            {
                UISettings.AdvancedEffectsEnabledChanged -= UISettings_AdvancedEffectsEnabledChanged;
                PowerManager.EnergySaverStatusChanged -= PowerManager_EnergySaverStatusChanged;
            }
            LanguagePicker.SelectionChanged -= LanguagePicker_SelectionChanged;
        }

        private async void PowerManager_EnergySaverStatusChanged(object sender, object e)
        {
            await Dispatcher.CallOnUIThreadAsync(() =>
            {
                BackgroundTintOpacitySlider.IsEnabled = UISettings.AdvancedEffectsEnabled &&
                                                        PowerManager.EnergySaverStatus != EnergySaverStatus.On;
            });
        }

        private async void UISettings_AdvancedEffectsEnabledChanged(UISettings sender, object args)
        {
            await Dispatcher.CallOnUIThreadAsync(() =>
            {
                BackgroundTintOpacitySlider.IsEnabled = UISettings.AdvancedEffectsEnabled &&
                                                        PowerManager.EnergySaverStatus != EnergySaverStatus.On;
            });
        }

        private void ThemeRadioButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                switch (radioButton.Tag)
                {
                    case "Light":
                        ThemeSettingsService.UseWindowsTheme = false;
                        ThemeSettingsService.SetTheme(ElementTheme.Light);
                        break;
                    case "Dark":
                        ThemeSettingsService.UseWindowsTheme = false;
                        ThemeSettingsService.SetTheme(ElementTheme.Dark);
                        break;
                    case "Default":
                        ThemeSettingsService.UseWindowsTheme = true;
                        break;
                }
            }
        }
        /*
        private void AccentColorPicker_OnColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            /* if (AccentColorPicker.IsEnabled)
            {
                ThemeSettingsService.AppAccentColor = args.NewColor;
                if (!AccentColorToggle.IsOn) ThemeSettingsService.CustomAccentColor = args.NewColor;
            }
            
        }
        */

        private void BackgroundTintOpacitySlider_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            ThemeSettingsService.AppBackgroundPanelTintOpacity = e.NewValue / 100;
        }
        /*
        private void WindowsAccentColorToggle_OnToggled(object sender, RoutedEventArgs e)
        {
            AccentColorPicker.IsEnabled = !AccentColorToggle.IsOn;
            ThemeSettingsService.UseWindowsAccentColor = AccentColorToggle.IsOn;
            AccentColorPicker.Color = AccentColorToggle.IsOn ? ThemeSettingsService.AppAccentColor : ThemeSettingsService.CustomAccentColor;
        }
        */

        private void LanguagePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var languageId = ((LanguageItem)e.AddedItems.First()).ID;

            RestartPrompt.Visibility = languageId == LanguageUtility.CurrentLanguageID ? Visibility.Collapsed : Visibility.Visible;

            ApplicationLanguages.PrimaryLanguageOverride = languageId;
        }

        private void SearchEngineRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!(sender is RadioButton radioButton)) return;

            switch (radioButton.Name)
            {
                case "BingRadioButton":
                    AppSettingsService.EditorDefaultSearchEngine = SearchEngine.Bing;
                    OnCustomSearchEngineSelectionChanged(false);
                    break;
                case "GoogleRadioButton":
                    AppSettingsService.EditorDefaultSearchEngine = SearchEngine.Google;
                    OnCustomSearchEngineSelectionChanged(false);
                    break;
                case "DuckDuckGoRadioButton":
                    AppSettingsService.EditorDefaultSearchEngine = SearchEngine.DuckDuckGo;
                    OnCustomSearchEngineSelectionChanged(false);
                    break;
                case "CustomSearchUrlRadioButton":
                    OnCustomSearchEngineSelectionChanged(true);
                    break;
            }
        }

        private static bool IsValidUrl(string url)
        {
            try
            {
                if (Uri.TryCreate(url, UriKind.Absolute, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                {
                    if (string.Format(url, "s") == url)
                        return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private void CustomSearchUrl_TextChanged(object sender, TextChangedEventArgs e)
        {
            AppSettingsService.EditorCustomMadeSearchUrl = CustomSearchUrl.Text;
            CustomUrlErrorReport.Visibility = IsValidUrl(CustomSearchUrl.Text) ? Visibility.Collapsed : Visibility.Visible;
        }

        private void CustomSearchUrl_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CustomSearchUrlRadioButton.IsChecked != null &&
                (IsValidUrl(CustomSearchUrl.Text) && (bool)CustomSearchUrlRadioButton.IsChecked))
            {
                AppSettingsService.EditorDefaultSearchEngine = SearchEngine.Custom;
            }
            else if (!IsValidUrl(CustomSearchUrl.Text) && AppSettingsService.EditorDefaultSearchEngine == SearchEngine.Custom)
            {
                AppSettingsService.EditorDefaultSearchEngine = SearchEngine.Bing;
            }

            CustomUrlErrorReport.Visibility = IsValidUrl(CustomSearchUrl.Text) ? Visibility.Collapsed : Visibility.Visible;
            AppSettingsService.EditorCustomMadeSearchUrl = CustomSearchUrl.Text;
        }

        private void OnCustomSearchEngineSelectionChanged(bool selected)
        {
            if (selected)
            {
                CustomSearchUrl.IsEnabled = true;
                CustomSearchUrl.Focus(FocusState.Programmatic);
                CustomSearchUrl.Select(CustomSearchUrl.Text.Length, 0);
                if (IsValidUrl(CustomSearchUrl.Text))
                {
                    AppSettingsService.EditorDefaultSearchEngine = SearchEngine.Custom;
                    AppSettingsService.EditorCustomMadeSearchUrl = CustomSearchUrl.Text;
                }
                CustomSearchUrl_TextChanged(null, null);
            }
            else
            {
                CustomSearchUrl.IsEnabled = false;
                CustomSearchUrl.Text = AppSettingsService.EditorCustomMadeSearchUrl;
                CustomUrlErrorReport.Visibility = Visibility.Collapsed;
            }
        }
    }
}