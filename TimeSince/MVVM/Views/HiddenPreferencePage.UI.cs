using System.Globalization;
using Microsoft.AppCenter.Crashes;
using Microsoft.Maui.Controls;
using TimeSince.Data;
using TimeSince.MVVM.ViewModels;

namespace TimeSince.MVVM.Views;

public class HiddenPreferencePage : ContentPage
    {
        private Label PrimaryColorLabel   { get; set; }
        private Entry PrimaryColorEntry   { get; set; }
        private Label SecondaryColorLabel { get; set; }
        private Entry SecondaryColorEntry { get; set; }
        private Label TertiaryColorLabel { get; set; }
        private Entry TertiaryColorEntry { get; set; }

        private Label DefaultPrimaryColorLabel   { get; set; }
        private Entry DefaultPrimaryColorEntry   { get; set; }
        private Label DefaultSecondaryColorLabel { get; set; }
        private Entry DefaultSecondaryColorEntry { get; set; }
        private Label DefaultTertiaryColorLabel  { get; set; }
        private Entry DefaultTertiaryColorEntry  { get; set; }

        private Label  PaidToTurnOffAdsLabel  { get; set; }
        private Switch PaidToTurnOffAdsSwitch { get; set; }

        private Label AwesomePersonScoreLabel { get; set; }
        private Entry AwesomePersonScoreEntry { get; set; }

        private Label LastSortOptionLabel { get; set; }
        private Entry LastSortOptionEntry { get; set; }

        private Label  HideStartupMessageLabel  { get; set; }
        private Switch HideStartupMessageSwitch { get; set; }

        private Label       ErrorReportingIdLabel { get; set; }
        private Entry       ErrorReportingIdEntry { get; set; }

        private StackLayout StackLayout           { get; set; }

        private RemoveAdsViewModel RemoveAdsViewModel { get; set; }

        public HiddenPreferencePage()
        {
            RemoveAdsViewModel = new RemoveAdsViewModel();
            BindingContext     = RemoveAdsViewModel;

            InitializeUi();
        }

        private void InitializeUi()
        {
            BuildColorControls();

            PaidToTurnOffAdsLabel  = BuildPaidToTurnOffAdsLabel();
            PaidToTurnOffAdsSwitch = BuildPaidToTurnOffAdsSwitch();

            AwesomePersonScoreLabel = BuildAwesomePersonScoreLabel();
            AwesomePersonScoreEntry = BuildAwesomePersonScoreEntry();

            LastSortOptionLabel = BuildLastSortOptionLabel();
            LastSortOptionEntry = BuildLastSortOptionEntry();

            HideStartupMessageLabel  = BuildHideStartupMessageLabel();
            HideStartupMessageSwitch = BuildHideStartupMessageSwitch();

            ErrorReportingIdLabel = BuildErrorReportingIdLabel();
            ErrorReportingIdEntry = BuildErrorReportingIdEntry();

            StackLayout = BuildStackLayout();
            var scrollView = new ScrollView { Content = StackLayout };

            Content = scrollView;
        }

        private StackLayout BuildStackLayout()
        {
            var stackLayout = new StackLayout
                          {
                              Margin = new Thickness(20)
                            , Children =
                              {
                                  PrimaryColorLabel
                                , PrimaryColorEntry
                                , SecondaryColorLabel
                                , SecondaryColorEntry
                                , TertiaryColorLabel
                                , TertiaryColorEntry
                                , DefaultPrimaryColorLabel
                                , DefaultPrimaryColorEntry
                                , DefaultSecondaryColorLabel
                                , DefaultSecondaryColorEntry
                                , DefaultTertiaryColorLabel
                                , DefaultTertiaryColorEntry
                                , PaidToTurnOffAdsLabel
                                , PaidToTurnOffAdsSwitch
                                , AwesomePersonScoreLabel
                                , AwesomePersonScoreEntry
                                , LastSortOptionLabel
                                , LastSortOptionEntry
                                , HideStartupMessageLabel
                                , HideStartupMessageSwitch
                                , ErrorReportingIdLabel
                                , ErrorReportingIdEntry
                              }
                          };

            return stackLayout;
        }

        private void BuildColorControls()
        {
            PrimaryColorLabel = BuildPrimaryColorLabel();
            PrimaryColorEntry = BuildPrimaryColorEntry();

            SecondaryColorLabel = BuildSecondaryColorLabel();
            SecondaryColorEntry = BuildSecondaryColorEntry();

            TertiaryColorLabel = BuildTertiaryColorLabel();
            TertiaryColorEntry = BuildTertiaryColorEntry();

            DefaultPrimaryColorLabel = BuildDefaultPrimaryColorLabel();
            DefaultPrimaryColorEntry = BuildDefaultPrimaryColorEntry();

            DefaultSecondaryColorLabel = BuildDefaultSecondaryColorLabel();
            DefaultSecondaryColorEntry = BuildDefaultSecondaryColorEntry();

            DefaultTertiaryColorLabel = BuildDefaultTertiaryColorLabel();
            DefaultTertiaryColorEntry = BuildDefaultTertiaryColorEntry();
        }

        private static Switch BuildHideStartupMessageSwitch()
        {
            var hideStartupMessageSwitch = new Switch
                                           {
                                               IsToggled = PreferencesDataStore.HideStartupMessage
                                           };
            hideStartupMessageSwitch.SetBinding(IsEnabledProperty
                                              , new Binding("DoorReadOnlyIsUnlocked"
                                                          , converter: new InverseBooleanConverter()));

            hideStartupMessageSwitch.Toggled += (sender, e) =>
            {
                PreferencesDataStore.HideStartupMessage = e.Value;
            };

            return hideStartupMessageSwitch;
        }

        private static Label BuildHideStartupMessageLabel()
        {
            var hideStartupMessageLabel = new Label
                                          {
                                              Text           = "Hide Startup Message"
                                            , FontAttributes = FontAttributes.Bold
                                          };

            return hideStartupMessageLabel;
        }

        private Label BuildErrorReportingIdLabel()
        {
            var errorReportingIdLabel = new Label
                                        {
                                            Text           = "Error Reporting Id"
                                          , FontAttributes = FontAttributes.Bold
                                        };

            return errorReportingIdLabel;
        }

        private Entry BuildErrorReportingIdEntry()
        {
            var errorReportingIdEntry = new Entry
                                        {
                                            Placeholder = "Error Reporting Id"
                                          , Text        = PreferencesDataStore.ErrorReportingId
                                        };
            errorReportingIdEntry.SetBinding(IsEnabledProperty
                                           , new Binding("DoorReadOnlyIsUnlocked"
                                                       , converter: new InverseBooleanConverter()));

            errorReportingIdEntry.TextChanged += (sender
                                                , e) =>
            {
                PreferencesDataStore.ErrorReportingId = e.NewTextValue;
            };

            return errorReportingIdEntry;
        }

        private static Entry BuildLastSortOptionEntry()
        {
            var lastSortOptionEntry = new Entry
                                      {
                                          Placeholder = "Enter Last Sort Option"
                                        , Text = PreferencesDataStore.LastSortOption
                                                                     .ToString(CultureInfo.CurrentCulture)
                                      };
            lastSortOptionEntry.SetBinding(IsEnabledProperty
                                         , new Binding("DoorReadOnlyIsUnlocked"
                                                     , converter: new InverseBooleanConverter()));

            lastSortOptionEntry.TextChanged += (sender, e) =>
            {
                PreferencesDataStore.LastSortOption = int.Parse(e.NewTextValue) ;
            };

            return lastSortOptionEntry;
        }

        private static Label BuildLastSortOptionLabel()
        {
            var lastSortOptionLabel = new Label
                                      {
                                          Text           = "Last Sort Option"
                                        , FontAttributes = FontAttributes.Bold
                                      };

            return lastSortOptionLabel;
        }

        private static Entry BuildAwesomePersonScoreEntry()
        {
            var awesomePersonScoreEntry = new Entry
                                          {
                                              Placeholder = "Enter Awesome Person Score"
                                            , Text = PreferencesDataStore.AwesomePersonScore
                                                                         .ToString(CultureInfo.CurrentCulture)
                                          };
            awesomePersonScoreEntry.SetBinding(IsEnabledProperty
                                             , new Binding("DoorReadOnlyIsUnlocked"
                                                         , converter: new InverseBooleanConverter()));

            awesomePersonScoreEntry.TextChanged += (sender, e) =>
            {
                PreferencesDataStore.AwesomePersonScore = double.Parse(e.NewTextValue) ;
            };

            return awesomePersonScoreEntry;
        }

        private static Label BuildAwesomePersonScoreLabel()
        {
            var awesomePersonScoreLabel = new Label
                                          {
                                              Text           = "Awesome Person Score"
                                            , FontAttributes = FontAttributes.Bold
                                          };

            return awesomePersonScoreLabel;
        }

        private static Switch BuildPaidToTurnOffAdsSwitch()
        {
            var paidToTurnOffAdsSwitch = new Switch
                                         {
                                             IsToggled = PreferencesDataStore.PaidToTurnOffAds
                                         };
            paidToTurnOffAdsSwitch.SetBinding(IsEnabledProperty
                                            , new Binding("DoorReadOnlyIsUnlocked"
                                                        , converter: new InverseBooleanConverter()));

            paidToTurnOffAdsSwitch.Toggled += (sender, e) =>
            {
                PreferencesDataStore.PaidToTurnOffAds = e.Value;
            };

            return paidToTurnOffAdsSwitch;
        }

        private static Label BuildPaidToTurnOffAdsLabel()
        {
            var paidToTurnOffAdsLabel = new Label
                                        {
                                            Text           = "Paid to Turn Off Ads"
                                          , FontAttributes = FontAttributes.Bold
                                        };

            return paidToTurnOffAdsLabel;
        }

        private static Entry BuildDefaultTertiaryColorEntry()
        {
            var defaultTertiaryColorEntry = new Entry
                                            {
                                                Placeholder = "Enter Default Tertiary Color Name"
                                              , Text        = PreferencesDataStore.DefaultTertiaryColorName
                                            };
            defaultTertiaryColorEntry.SetBinding(IsEnabledProperty
                                                , new Binding("DoorReadOnlyIsUnlocked"
                                                            , converter: new InverseBooleanConverter()));

            defaultTertiaryColorEntry.TextChanged += (sender, e) =>
            {
                PreferencesDataStore.DefaultTertiaryColorName = e.NewTextValue;
            };

            return defaultTertiaryColorEntry;
        }

        private static Label BuildDefaultTertiaryColorLabel()
        {
            var defaultTertiaryColorLabel = new Label
                                            {
                                                Text           = "Default Tertiary Color Name"
                                              , FontAttributes = FontAttributes.Bold
                                            };

            return defaultTertiaryColorLabel;
        }

        private static Entry BuildDefaultSecondaryColorEntry()
        {
            var defaultSecondaryColorEntry = new Entry
                                             {
                                                 Placeholder = "Enter Default Secondary Color Name"
                                               , Text        = PreferencesDataStore.DefaultSecondaryColorName
                                             };
            defaultSecondaryColorEntry.SetBinding(IsEnabledProperty
                                              , new Binding("DoorReadOnlyIsUnlocked"
                                                          , converter: new InverseBooleanConverter()));

            defaultSecondaryColorEntry.TextChanged += (sender, e) =>
            {
                PreferencesDataStore.DefaultSecondaryColorName = e.NewTextValue;
            };

            return defaultSecondaryColorEntry;
        }

        private static Label BuildDefaultSecondaryColorLabel()
        {
            var defaultSecondaryColorLabel = new Label
                                             {
                                                 Text           = "Default Secondary Color Name"
                                               , FontAttributes = FontAttributes.Bold
                                             };

            return defaultSecondaryColorLabel;
        }

        private static Entry BuildDefaultPrimaryColorEntry()
        {
            var defaultPrimaryColorEntry = new Entry
                                           {
                                               Placeholder = "Enter Default Primary Color Name"
                                             , Text        = PreferencesDataStore.DefaultPrimaryColorName
                                           };
            defaultPrimaryColorEntry.SetBinding(IsEnabledProperty
                                              , new Binding("DoorReadOnlyIsUnlocked"
                                                          , converter: new InverseBooleanConverter()));

            defaultPrimaryColorEntry.TextChanged += (sender
                                                   , e) =>
            {
                PreferencesDataStore.DefaultPrimaryColorName = e.NewTextValue;
            };

            return defaultPrimaryColorEntry;
        }

        private static Label BuildDefaultPrimaryColorLabel()
        {
            var defaultPrimaryColorLabel = new Label
                                           {
                                               Text           = "Default Primary Color Name"
                                             , FontAttributes = FontAttributes.Bold
                                           };

            return defaultPrimaryColorLabel;
        }

        private static Entry BuildTertiaryColorEntry()
        {
            var tertiaryColorEntry = new Entry
                                     {
                                         Placeholder = "Enter Tertiary Color"
                                       , Text        = PreferencesDataStore.TertiaryColorName
                                     };
            tertiaryColorEntry.SetBinding(IsEnabledProperty
                                        , new Binding("DoorReadOnlyIsUnlocked"
                                                    , converter: new InverseBooleanConverter()));

            tertiaryColorEntry.TextChanged += (sender, e) =>
            {
                PreferencesDataStore.TertiaryColorName = e.NewTextValue;
            };

            return tertiaryColorEntry;
        }

        private static Label BuildTertiaryColorLabel()
        {
            var tertiaryColorLabel = new Label
                                     {
                                         Text           = "Tertiary Color"
                                       , FontAttributes = FontAttributes.Bold
                                     };

            return tertiaryColorLabel;
        }

        private static Entry BuildSecondaryColorEntry()
        {
            var secondaryColorEntry = new Entry
                                      {
                                          Placeholder = "Enter Secondary Color"
                                        , Text        = PreferencesDataStore.SecondaryColorName
                                      };
            secondaryColorEntry.SetBinding(IsEnabledProperty
                                         , new Binding("DoorReadOnlyIsUnlocked"
                                                     , converter: new InverseBooleanConverter()));
            secondaryColorEntry.TextChanged += (sender, e) =>
            {
                PreferencesDataStore.SecondaryColorName = e.NewTextValue;
            };

            return secondaryColorEntry;
        }

        private static Label BuildSecondaryColorLabel()
        {
            var secondaryColorLabel = new Label
                                      {
                                          Text           = "Secondary Color"
                                        , FontAttributes = FontAttributes.Bold
                                      };

            return secondaryColorLabel;
        }

        private static Entry BuildPrimaryColorEntry()
        {
            var primaryColorEntry = new Entry
                                    {
                                        Placeholder = "Enter Primary Color"
                                      , Text        = PreferencesDataStore.PrimaryColorName
                                    };
            primaryColorEntry.SetBinding(IsEnabledProperty
                                       , new Binding("DoorReadOnlyIsUnlocked"
                                                   , converter: new InverseBooleanConverter()));
            primaryColorEntry.TextChanged += (sender, e) =>
            {
                PreferencesDataStore.PrimaryColorName = e.NewTextValue;
            };

            return primaryColorEntry;
        }

        private static Label BuildPrimaryColorLabel()
        {
            var primaryColorLabel = new Label
                                    {
                                        Text           = "Primary Color"
                                      , FontAttributes = FontAttributes.Bold
                                    };

            return primaryColorLabel;
        }
    }
