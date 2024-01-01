using Microsoft.Maui.Controls;
using System;
using Microsoft.Maui.Controls.Compatibility;
using TimeSince.Avails.ColorHelpers;
using TimeSince.MVVM.ViewModels;
using Grid = Microsoft.Maui.Controls.Grid;
using StackLayout = Microsoft.Maui.Controls.StackLayout;

namespace TimeSince.MVVM.Views
{
    public partial class AboutPage : ContentPage
    {
        private Frame          PrivacyPolicyFrame       { get; set; }
        private Frame          AwesomePersonFrame       { get; set; }
        private Frame          SupportMeFrame           { get; set; }
        private Frame          VersionFrame             { get; set; }
        private Frame          DescriptionFrame         { get; set; }
        private Frame          DevBioFrame              { get; set; }
        private Label          EmailLabel               { get; set; }
        private AboutViewModel AboutViewModel           { get; set; }
        private View           AdView                   { get; set; }
        private Button         ShowRewardAdButton       { get; set; }
        private Button         ShowInterstitialAdButton { get; set; }
        private Button         ResetAwesomeScoreButton  { get; set; }
        private Button         PrivacyPolicyButton      { get; set; }
        private Frame          HeaderFrame              { get; set; }
        private Grid           MainGrid                 { get; set; }

        public AboutPage()
        {

            AboutViewModel = new AboutViewModel { CurrentAwesomeScore = "1" };

            try
            {
                InitializeComponents();
            }
            catch (Exception e)
            {
                App.Logger.LogError(e);
            }

            SetButtonsTextColor();
            Title = "About";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            RefreshAdViewOnPage();
        }

        private void SetButtonsTextColor()
        {
            var buttonTextColor = ColorUtility.ChooseReadableTextColor(ColorUtility.GetColorFromResources(ResourceColors.Primary));
            PrivacyPolicyButton.TextColor      = buttonTextColor;
            ResetAwesomeScoreButton.TextColor  = buttonTextColor;
            ShowInterstitialAdButton.TextColor = buttonTextColor;
            ShowRewardAdButton.TextColor       = buttonTextColor;
        }

        private void RefreshAdViewOnPage()
        {
            if (App.AppServiceMethods.IsPhysicalDevice()) return;

            ShowInterstitialAdButton.IsEnabled = false;
            ShowRewardAdButton.IsEnabled       = false;

            App.Logger.ToastMessage("Ads cannot be shown on emulated devices.");
        }

        private void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            // Your implementation for TapGestureRecognizer_OnTapped event
        }

        private void InitializeComponents()
        {
            var frameStyle = GetFrameStyle();

            DefineHeaderFrame(frameStyle);
            DefineMainGrid();
            DefineDescriptionFrame(frameStyle);
            DefineEmailLabel();
            DefineDevBioFrame(frameStyle);
            DefineVersionFrame(frameStyle);
            DefineSupportMeFrame(frameStyle);
            DefineAwesomePersonFrame(frameStyle);
            DefinePrivacyPolicyFrame(frameStyle);

            ArrangeFramesIntoMainGrid();
            FinalizeArrangementForPage();
        }

        private void FinalizeArrangementForPage()
        {
            var innerStackLayout = new StackLayout();
            innerStackLayout.Children.Add(HeaderFrame);
            innerStackLayout.Children.Add(MainGrid);

            var gridLayout = new Grid();
            gridLayout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            gridLayout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            gridLayout.Add(innerStackLayout, 0, 0);

            var adPlaceholder = new ContentView();

            Grid.SetRow(innerStackLayout, 0);
            Grid.SetRow(adPlaceholder, 1);

            var scrollView = new ScrollView
                             {
                                 Content         = gridLayout
                               , BackgroundColor = ColorUtility.GetColorFromResources(ResourceColors.Tertiary)
                               , Padding = new Thickness(0, 0, 0, 50)
                             };

            var relativeLayout = new RelativeLayout();
            relativeLayout.Children.Add(scrollView
                                      , Constraint.Constant(0)
                                      , Constraint.Constant(0)
                                      , Constraint.RelativeToParent(parent => parent.Width)
                                      , Constraint.RelativeToParent(parent => parent.Height));

            AdView = App.AppServiceMethods.GetAdView();

            if (AdView is not null)
            {
                adPlaceholder.Content = AdView;
                relativeLayout.Children.Add(adPlaceholder
                                          , Constraint.Constant(0)
                                          , Constraint.RelativeToParent(parent => parent.Height - 100)
                                          , Constraint.RelativeToParent(parent => parent.Width)
                                          , Constraint.Constant(100));
            }

            Content = relativeLayout;
        }

        private void ArrangeFramesIntoMainGrid()
        {

            Grid.SetColumnSpan(DescriptionFrame, 2);
            Grid.SetColumnSpan(DevBioFrame, 2);
            Grid.SetColumnSpan(VersionFrame, 2);
            Grid.SetColumnSpan(SupportMeFrame, 2);
            Grid.SetColumnSpan(AwesomePersonFrame, 2);
            Grid.SetColumnSpan(PrivacyPolicyFrame, 2);

            Grid.SetRow(DescriptionFrame, 0);
            Grid.SetRow(DevBioFrame, 1);
            Grid.SetRow(VersionFrame, 2);
            Grid.SetRow(SupportMeFrame, 3);
            Grid.SetRow(AwesomePersonFrame, 4);
            Grid.SetRow(PrivacyPolicyFrame, 5);

            MainGrid.Children.Add(DescriptionFrame);
            MainGrid.Children.Add(DevBioFrame);
            MainGrid.Children.Add(VersionFrame);
            MainGrid.Children.Add(SupportMeFrame);
            MainGrid.Children.Add(AwesomePersonFrame);
            MainGrid.Children.Add(PrivacyPolicyFrame);
        }

        private void DefinePrivacyPolicyFrame(Style frameStyle)
        {

            PrivacyPolicyFrame = new Frame
                                 {
                                     Content = new Grid
                                               {
                                                   RowDefinitions = new RowDefinitionCollection
                                                                    {
                                                                        new(GridLength.Star)
                                                                      , new(GridLength.Auto)
                                                                    }
                                                 , ColumnDefinitions = new ColumnDefinitionCollection
                                                                       {
                                                                           new() { Width = new GridLength(90, GridUnitType.Star) }
                                                                         , new() { Width = new GridLength(10, GridUnitType.Star) }
                                                                       }
                                               }
                                   , Style = frameStyle
                                 };

            var privacyPolicyGrid = (Grid)PrivacyPolicyFrame.Content;

            PrivacyPolicyButton = new Button
                                  {
                                      Text   = "Privacy Policy"
                                    , Margin = new Thickness(5)
                                  };

            privacyPolicyGrid.Add(PrivacyPolicyButton, 0, 0);
            Grid.SetColumnSpan(PrivacyPolicyButton, 2);

            privacyPolicyGrid.Add(new Label
                                  {
                                      Text            = "Hide Privacy Policy Startup Message:"
                                    , TextColor       = ColorUtility.GetColorFromResources(ResourceColors.Tertiary)
                                    , VerticalOptions = LayoutOptions.Center
                                  }, 0, 1);

            privacyPolicyGrid.Add(new CheckBox
                                  {
                                      VerticalOptions   = LayoutOptions.Center
                                    , HorizontalOptions = LayoutOptions.Start
                                  }, 1, 1);


            if (PrivacyPolicyFrame.Content is Grid )
            {
                if (privacyPolicyGrid.Children[2] is CheckBox checkBox)
                {
                    checkBox.BindingContext = AboutViewModel;
                    checkBox.SetBinding(CheckBox.IsCheckedProperty, "IsPrivacyPolicyAccepted", BindingMode.TwoWay);
                }

                if (privacyPolicyGrid.Children[0] is Button)
                {
                    PrivacyPolicyButton.Clicked += PrivacyPolicyButton_OnClicked;
                }
            }
        }

        private void DefineAwesomePersonFrame(Style frameStyle)
        {

            ResetAwesomeScoreButton = new Button
                                      {
                                          Text              = "Reset Awesome Person Score"
                                        , WidthRequest      = 250
                                        , HorizontalOptions = LayoutOptions.Center
                                      };

            AwesomePersonFrame = new Frame
                                 {
                                     Content = new StackLayout
                                               {
                                                   Children =
                                                   {
                                                       new Label
                                                       {
                                                           Text      = "You are already an awesome person! This number just shows how awesomer you are."
                                                         , TextColor = ColorUtility.GetColorFromResources(ResourceColors.Tertiary)
                                                       }
                                                     , new Frame
                                                       {
                                                           Content = new StackLayout
                                                                     {
                                                                         Children =
                                                                         {
                                                                             new Label
                                                                             {
                                                                                 Text           = "Awesome Person Score:"
                                                                               , TextColor      = ColorUtility.GetColorFromResources(ResourceColors.Tertiary)
                                                                               , FontAttributes = FontAttributes.Bold
                                                                             }
                                                                           , new Label
                                                                             {
                                                                                 Text      = AboutViewModel.CurrentAwesomeScore
                                                                               , TextColor = ColorUtility.GetColorFromResources(ResourceColors.Tertiary)
                                                                             }
                                                                         }
                                                                     }
                                                         , Style = frameStyle
                                                       }
                                                     , ResetAwesomeScoreButton
                                                   }
                                               }
                                   , Style = frameStyle
                                 };
            if (((StackLayout)AwesomePersonFrame.Content).Children[1] is Button)
            {
                ResetAwesomeScoreButton.Clicked += ResetAwesomeScoreButton_OnClicked;
            }
        }

        private void DefineSupportMeFrame(Style frameStyle)
        {

            ShowInterstitialAdButton = new Button
                                       {
                                           Text   = "Show Interstitial Ad"
                                         , Margin = new Thickness(5)
                                       };
            ShowRewardAdButton = new Button
                                 {
                                     Text   = "Show Reward Ad"
                                   , Margin = new Thickness(5)
                                 };
            SupportMeFrame = new Frame
                             {
                                 Content = new StackLayout
                                           {
                                               Children =
                                               {
                                                   new Label
                                                   {
                                                       Text                    = "You can support me by tapping on these buttons and watching a few ads"
                                                     , TextColor               = ColorUtility.GetColorFromResources(ResourceColors.Tertiary)
                                                     , HorizontalTextAlignment = TextAlignment.Center
                                                   }
                                                 , ShowInterstitialAdButton
                                                 , ShowRewardAdButton
                                               }
                                           }
                               , Style = frameStyle
                             };

            if (((StackLayout)SupportMeFrame.Content).Children[1] is Button)
            {
                ShowInterstitialAdButton.Clicked += ShowInterstitialAdButton_OnClicked;
            }

            if (((StackLayout)SupportMeFrame.Content).Children[2] is Button)
            {
                ShowRewardAdButton.Clicked += ShowRewardAdButton_OnClicked;
            }
        }

        private void DefineVersionFrame(Style frameStyle)
        {

            VersionFrame = new Frame
                           {
                               Content = new Grid
                                         {
                                             RowDefinitions = new RowDefinitionCollection
                                                              {
                                                                  new() { Height = GridLength.Star }
                                                                , new() { Height = GridLength.Star }
                                                                , new() { Height = GridLength.Star }
                                                              }
                                           , ColumnDefinitions = new ColumnDefinitionCollection
                                                                 {
                                                                     new() { Width = GridLength.Star }
                                                                   , new() { Width = GridLength.Star }
                                                                 }
                                         }
                             , Style = frameStyle
                           };

            ((Grid)VersionFrame.Content).Add(new Label
                                             {
                                                 Text      = "Current Version:"
                                               , FontSize  = 16
                                               , TextColor = ColorUtility.GetColorFromResources(ResourceColors.Tertiary)
                                             }, 0, 0);
            ((Grid)VersionFrame.Content).Add(new Label
                                             {
                                                 Text      = AboutViewModel.CurrentVersion
                                               , FontSize  = 16
                                               , TextColor = ColorUtility.GetColorFromResources(ResourceColors.Tertiary)
                                               , Margin    = new Thickness(5, 0, 0, 0)
                                             }, 1, 0);
            ((Grid)VersionFrame.Content).Add(new Label
                                             {
                                                 Text      = "Current Build:"
                                               , FontSize  = 16
                                               , TextColor = ColorUtility.GetColorFromResources(ResourceColors.Tertiary)
                                               , Margin    = new Thickness(0, 10, 0, 0)
                                             }, 0, 1);
            ((Grid)VersionFrame.Content).Add(new Label
                                             {
                                                 Text      = AboutViewModel.CurrentBuild
                                               , FontSize  = 16
                                               , TextColor = ColorUtility.GetColorFromResources(ResourceColors.Tertiary)
                                               , Margin    = new Thickness(5, 10, 0, 0)
                                             }, 1, 1);

            ((Grid)VersionFrame.Content).Add(new Label
                                             {
                                                 Text      = "Build Mode:"
                                               , FontSize  = 16
                                               , TextColor = ColorUtility.GetColorFromResources(ResourceColors.Tertiary)
                                               , Margin    = new Thickness(0, 10, 0, 0)
                                             }, 0, 2);
            ((Grid)VersionFrame.Content).Add(new Label
                                             {
                                                 Text      = AboutViewModel.CurrentMode
                                               , FontSize  = 16
                                               , TextColor = ColorUtility.GetColorFromResources(ResourceColors.Tertiary)
                                               , Margin    = new Thickness(5, 10, 0, 0)
                                             }, 1, 2);
        }

        private void DefineDevBioFrame(Style frameStyle)
        {

            DevBioFrame = new Frame
                          {
                              Content = new StackLayout
                                        {
                                            Children =
                                            {
                                                new Label
                                                {
                                                    Text      = "Hi! My name is Ben. I create apps like this in my spare time. I do this mostly for fun and to keep my skills up. I also do it to bring in a bit of supplemental income."
                                                  , Margin    = new Thickness(0, 0, 0, 10)
                                                  , TextColor = ColorUtility.GetColorFromResources(ResourceColors.Tertiary)
                                                }
                                              , new Label
                                                {
                                                    Text      = "Send me questions or comments:"
                                                  , TextColor = ColorUtility.GetColorFromResources(ResourceColors.Tertiary)
                                                }
                                              , EmailLabel
                                            }
                                        }
                            , Style = frameStyle
                          };
        }

        private void DefineEmailLabel()
        {

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_OnTapped;

            EmailLabel = new Label
                         {
                             Text               = "BenHopApps@Gmail.com (tap to send email)"
                           , TextColor          = ColorUtility.GetColorFromResources(ResourceColors.Tertiary)
                           , GestureRecognizers = { tapGestureRecognizer }
                         };
        }

        private void DefineDescriptionFrame(Style frameStyle)
        {

            DescriptionFrame = new Frame
                               {
                                   Content = new Label
                                             {
                                                 Text      = "Time Since is an app to help track how long it has been since an event."
                                               , Margin    = new Thickness(0, 0, 0, 10)
                                               , TextColor = ColorUtility.GetColorFromResources(ResourceColors.Tertiary)
                                             }
                                 , Style = frameStyle
                               };
        }

        private void DefineMainGrid()
        {

            MainGrid = new Grid
                       {
                           RowDefinitions = new RowDefinitionCollection
                                            {
                                                new() { Height = GridLength.Star }
                                              , new() { Height = GridLength.Star }
                                              , new() { Height = GridLength.Star }
                                              , new() { Height = GridLength.Star }
                                              , new() { Height = GridLength.Star }
                                              , new() { Height = GridLength.Star }
                                            }
                         , ColumnDefinitions = new ColumnDefinitionCollection
                                               {
                                                   new() { Width = GridLength.Star }
                                                 , new() { Width = GridLength.Star }
                                               }
                       };
        }


        private void DefineHeaderFrame(Style frameStyle)
        {

            HeaderFrame = new Frame
                          {
                              Content = new Label
                                        {
                                            Text                    = "Time Since"
                                          , TextColor               = ColorUtility.GetColorFromResources(ResourceColors.Tertiary)
                                          , HorizontalTextAlignment = TextAlignment.Center
                                          , FontSize                = 24
                                        }
                            , HorizontalOptions = LayoutOptions.Center
                            , Style             = frameStyle
                          };
        }

        private static Style GetFrameStyle()
        {

            if (Application.Current?.Resources.TryGetValue("CommonFrameStyle", out var style) ?? false)
            {
                return (Style)style;
            }

            return null;
        }
    }
}
