using Syncfusion.Maui.Picker;
using TimeSince.Avails;
using TimeSince.Avails.ColorHelpers;

namespace TimeSince.MVVM.Views;

public partial class SettingsPage : ContentPage
{
    private StackLayout MainStackLayout       { get; set; }
    private SfPicker    PrimaryColorPicker    { get; set; }
    private SfPicker    SecondaryColorPicker  { get; set; }
    private SfPicker    TertiaryColorPicker   { get; set; }
    private Editor      PrimarySearchEditor   { get; set; }
    private Editor      SecondarySearchEditor { get; set; }
    private Editor      TertiarySearchEditor  { get; set; }
    private Button      PrimaryButton         { get; set; }
    private Button      SecondaryButton       { get; set; }
    private Button      TertiaryButton        { get; set; }
    private Color?      ForegroundColor       { get; set; }
    private Label       RemoveAdsLabel        { get; set; }
    private CheckBox    RemoveAdsCheckBox     { get; set; }
    private Switch      RemoveAdsSwitch       { get; set; }
    private View        AdView                { get; set; }
    private Entry       CodeEntry             { get; set; }
    private bool        ViewLogsButtonAdded   { get; set; }

    private void InitializeComponent()
    {
        const int columnWidth = 175;

        Title = "Settings";

        ViewLogsButtonAdded = false;

        CreateToolBar();
        CreateColorPickers(columnWidth);
        CreateSearchEditors(columnWidth);
        CreateSetButtons();
        CreateRemoveAdsControls();

        var mainGrid      = BuildMainGrid();
        var adPlaceholder = new BoxView { HeightRequest = 0 };

        MainStackLayout = new StackLayout
                          {
                              Children =
                                  {
                                      mainGrid
                                    , adPlaceholder
                                  }
                          };

        AddAdView(adPlaceholder);

#if DEBUG
        RemoveAdsViewModel.DoorIsUnlocked = true;
#endif

        if (RemoveAdsViewModel.DoorIsUnlocked)
        {
            BuildViewLogsButton(MainStackLayout /*, forceHide: true*/);
            BuildViewPreferencesButton(MainStackLayout);
        }

        Content = MainStackLayout;
    }

    private void CreateToolBar()
    {
        var enterCodeToolbarItem = new ToolbarItem
                                   {
                                       Text     = "~"
                                     // , Order    = ToolbarItemOrder.Primary
                                     // , Priority = 0 // Set the priority (higher values are displayed first)
                                   };

        enterCodeToolbarItem.Clicked += EnterCodeToolbarItem_Clicked;
        //     (sender, e) =>
        // {
        //     var codeEntry = new Entry
        //                     {
        //                         Placeholder = "code"
        //                         , Margin = new Thickness(5, 0, 5, 0);
        //                     };
        //     codeEntry.TextChanged += (o, args) =>
        //     {
        //         if (codeEntry.Text.Equals("viewLogs", StringComparison.CurrentCultureIgnoreCase))
        //         {
        //             BuildViewLogsButton(stackLayout);
        //         }
        //     };
        //     stackLayout.Children.Add(codeEntry);
        // };
        ToolbarItems.Add(enterCodeToolbarItem);
    }

    private void EnterCodeToolbarItem_Clicked(object     sender
                                             , EventArgs eventArgs)
    {
        CodeEntry = new Entry
                        {
                            Placeholder = "code"
                            , Margin = new Thickness(5, 0, 5, 0)
                        };
        CodeEntry.TextChanged += CodeEntryOnTextChanged;
        //     (o, args) =>
        // {
        //     if (codeEntry.Text.Equals("viewLogs", StringComparison.CurrentCultureIgnoreCase))
        //     {
        //         BuildViewLogsButton(MainStackLayout);
        //     }
        // };

        MainStackLayout.Children.Add(CodeEntry);
    }

    private void CodeEntryOnTextChanged(object               sender
                                      , TextChangedEventArgs e)
    {
        if (CodeEntry.Text.Equals("viewLogs", StringComparison.CurrentCultureIgnoreCase))
        {
            BuildViewLogsButton(MainStackLayout);
        }
        //Preferences can be accessed using the Door Key secret.
    }

    private void AddAdView(IView adPlaceholder)
    {
        if (RemoveAdsViewModel.PaidForAdsToBeRemoved) return;

        AdView = App.AppServiceMethods.GetAdView();
        if (AdView is not null)
        {
            MainStackLayout.Children.Remove(adPlaceholder);
            MainStackLayout.Children.Add(AdView);
        }
    }

    private void BuildViewLogsButton(Layout stackLayout, bool forceHide = false)
    {
        if (forceHide || ViewLogsButtonAdded) return;

        var logButton = new Button
                        {
                            Text            = "View Logs"
                          , BackgroundColor = PrimaryButton.BackgroundColor
                          , TextColor       = ForegroundColor
                          , Margin          = new Thickness(5, 5, 5, 0)
                        };

        logButton.Clicked += OnLogButtonOnClicked;

        stackLayout.Children.Add(logButton);

        ViewLogsButtonAdded = true;
    }

    private void BuildViewPreferencesButton(Layout stackLayout
                                          , bool   forceHide = false)
    {
        if (forceHide) return;

        var preferenceButton = new Button
                               {
                                   Text            = "View Preferences"
                                 , BackgroundColor = PrimaryButton.BackgroundColor
                                 , TextColor       = ForegroundColor
                                 , Margin          = new Thickness(5, 5, 5, 0)
                               };
        preferenceButton.Clicked += PreferenceButtonOnClicked;

        stackLayout.Children.Add(preferenceButton);
    }

    private void CreateRemoveAdsControls()
    {
        RemoveAdsLabel = new Label
                         {
                             Text                  = "Remove Ads"
                           , VerticalTextAlignment = TextAlignment.Center
                           , VerticalOptions       = LayoutOptions.Center
                           , TextColor             = TertiaryButton.BackgroundColor
                         };

        // RemoveAdsCheckBox = new CheckBox
        //                     {
        //                         IsChecked         = RemoveAdsViewModel.PaidForAdsToBeRemoved
        //                       , VerticalOptions   = LayoutOptions.Center
        //                       , HorizontalOptions = LayoutOptions.Start
        //                     };
        // RemoveAdsCheckBox.CheckedChanged += RemoveAdsCheckBoxOnCheckedChanged;

        RemoveAdsSwitch = new Switch
                          {
                              IsToggled         = RemoveAdsViewModel.PaidForAdsToBeRemoved
                            , VerticalOptions   = LayoutOptions.Center
                            , HorizontalOptions = LayoutOptions.Start
                            , ThumbColor        = PrimaryButton.BackgroundColor
                            , OnColor           = ForegroundColor
                          };
        RemoveAdsSwitch.Toggled += RemoveAdsSwitchOnToggled;
    }

    private Grid BuildMainGrid()
    {
        var primaryGrid   = BuildPrimaryColorGrid();
        var secondaryGrid = BuildSecondaryColorGrid();
        var tertiaryGrid  = BuildTertiaryColorGrid();

        //Settings sections labels
        // var colorSectionLabel = new Label
        //                         {
        //                             Text            = "App Colors"
        //                           , BackgroundColor = Colors.LightGray
        //                         };
        //
        // var adSectionLabel = new Label
        //                      {
        //                          Text            = "Ads"
        //                        , BackgroundColor = Colors.LightGray
        //                        , Margin = new Thickness(0, 5, 0, 5)
        //                      };
        var colorSectionLabel = new Label
                                {
                                    Text            = "App Colors"
                                   , FontAttributes = FontAttributes.Bold
                                   , FontSize       = 16
                                   , TextColor      = ColorUtility.GetColorFromResources(ResourceColors.Tertiary)
                                   , Margin         = new Thickness(0, 10, 0, 5)
                                };

        var adSectionLabel = new Label
                             {
                                 Text            = "Ads"
                                , FontAttributes = FontAttributes.Bold
                                , FontSize       = 16
                                , TextColor      = ColorUtility.GetColorFromResources(ResourceColors.Tertiary)
                                , Margin         = new Thickness(0, 10, 0, 5)
                             };
        //BENDO: Use the CommonFrameStyle in the CommonStyles.xaml instead
        var frame = new Frame
                    {
                        CornerRadius    = 10
                      , Padding         = 10
                      , Margin          = DefaultThickness()
                      , BorderColor     = ColorUtility.GetColorFromResources(ResourceColors.Primary)
                      , BackgroundColor = ColorUtility.GetColorFromResources(ResourceColors.Secondary)
                    };

        var colorSectionFrame = new Frame
                                {
                                    CornerRadius    = frame.CornerRadius
                                  , Padding         = frame.Padding
                                  , Margin          = frame.Margin
                                  , BorderColor     = frame.BorderColor
                                  , BackgroundColor = frame.BackgroundColor
                                };

        var adSectionFrame    = new Frame
                                {
                                    CornerRadius    = frame.CornerRadius
                                  , Padding         = frame.Padding
                                  , Margin          = frame.Margin
                                  , BorderColor     = frame.BorderColor
                                  , BackgroundColor = frame.BackgroundColor
                                };

        var resetColorsButton = new Button
                                {
                                    Text            = "Reset Colors"
                                  , BackgroundColor = PrimaryButton.BackgroundColor
                                  , TextColor       = ForegroundColor
                                };

        resetColorsButton.Clicked += ResetColorsButtonOnClicked;

        var removeAdsGrid = BuildRemoveAdsGrid();

        colorSectionFrame.Content = new StackLayout
                                    {
                                        Children =
                                        {
                                            colorSectionLabel
                                          , primaryGrid
                                          , secondaryGrid
                                          , tertiaryGrid
                                          , resetColorsButton
                                        }
                                    };

        adSectionFrame.Content = new StackLayout
                                 {
                                     Children =
                                     {
                                         adSectionLabel
                                       , removeAdsGrid
                                     }
                                 };

        var mainGrid = new Grid { Margin = DefaultThickness() };
        UiUtilities.AddRowDefinitions(mainGrid, GridLength.Auto, 2);

        mainGrid.Add(colorSectionFrame, 0, 0);
        mainGrid.Add(adSectionFrame, 0, 1);

        // UiUtilities.AddRowDefinitions(mainGrid, GridLength.Auto, 8);
        // mainGrid.Add(colorSectionLabel, 0, 0);
        // mainGrid.Add(primaryGrid, 0, 1);
        // mainGrid.Add(secondaryGrid, 0, 2);
        // mainGrid.Add(tertiaryGrid, 0, 3);
        // mainGrid.Add(resetColorsButton, 0, 4);
        // mainGrid.Add(adSectionLabel, 0, 5);
        // mainGrid.Add(removeAdsGrid, 0, 6);

        return mainGrid;
    }

    private Grid BuildRemoveAdsGrid()
    {
        var removeAdsGrid = new Grid { Margin = DefaultThickness() };

        removeAdsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        removeAdsGrid.ColumnDefinitions.Add(UiUtilities.NewColumnDefinition(GridLength.Auto));
        removeAdsGrid.ColumnDefinitions.Add(UiUtilities.NewColumnDefinition(GridLength.Star));

        removeAdsGrid.Add(RemoveAdsLabel, 0, 0);
        removeAdsGrid.Add(RemoveAdsSwitch, 1, 0);

        return removeAdsGrid;
    }

    private Grid BuildTertiaryColorGrid()
    {
        var tertiaryGrid = new Grid { Margin = DefaultThickness() };
        tertiaryGrid.RowDefinitions.Add(UiUtilities.NewRowDefinition(GridLength.Auto));
        tertiaryGrid.RowDefinitions.Add(UiUtilities.NewRowDefinition(GridLength.Auto));

        tertiaryGrid.ColumnDefinitions.Add(UiUtilities.NewColumnDefinition(GridLength.Star));
        tertiaryGrid.ColumnDefinitions.Add(UiUtilities.NewColumnDefinition(GridLength.Star));

        tertiaryGrid.Add(TertiaryButton, 0, 0);
        tertiaryGrid.Add(TertiarySearchEditor, 0, 0);
        tertiaryGrid.Add(TertiaryColorPicker, 0, 1);

        return tertiaryGrid;
    }

    private Grid BuildSecondaryColorGrid()
    {
        var secondaryGrid = new Grid { Margin = DefaultThickness() };
        secondaryGrid.RowDefinitions.Add(UiUtilities.NewRowDefinition(GridLength.Auto));
        secondaryGrid.RowDefinitions.Add(UiUtilities.NewRowDefinition(GridLength.Auto));

        secondaryGrid.ColumnDefinitions.Add(UiUtilities.NewColumnDefinition(GridLength.Star));
        secondaryGrid.ColumnDefinitions.Add(UiUtilities.NewColumnDefinition(GridLength.Star));

        secondaryGrid.Add(SecondaryButton, 0, 0);
        secondaryGrid.Add(SecondarySearchEditor, 0, 0);
        secondaryGrid.Add(SecondaryColorPicker, 0, 1);

        return secondaryGrid;
    }

    private Grid BuildPrimaryColorGrid()
    {
        var primaryGrid = new Grid { Margin = DefaultThickness() };

        UiUtilities.AddRowDefinitions(primaryGrid
                                    , GridLength.Auto
                                    , numberOfRows: 2);
        UiUtilities.AddColumnDefinitions(primaryGrid
                                       , GridLength.Star
                                       , numberOfColumns: 2);

        primaryGrid.Add(PrimaryButton, 0, 0);
        primaryGrid.Add(PrimarySearchEditor, 0, 0);
        primaryGrid.Add(PrimaryColorPicker, 0, 1);

        return primaryGrid;
    }

    private void CreateSetButtons()
    {

        PrimaryButton           =  CreateButton("Primary", "Button");
        ForegroundColor         =  ColorUtility.ChooseReadableTextColor(PrimaryButton.BackgroundColor);
        PrimaryButton.TextColor =  ForegroundColor;
        PrimaryButton.Clicked   += PrimaryButtonOnClicked;

        SecondaryButton         =  CreateButton("Secondary", "Events Background");
        SecondaryButton.TextColor = ColorUtility.ChooseReadableTextColor(SecondaryButton.BackgroundColor);
        SecondaryButton.Clicked += SecondaryButtonOnClicked;

        TertiaryButton         =  CreateButton("Tertiary", "Events Text");
        TertiaryButton.TextColor  = ColorUtility.ChooseReadableTextColor(TertiaryButton.BackgroundColor);
        TertiaryButton.Clicked += TertiaryButtonOnClicked;
    }

    private void CreateSearchEditors(int columnWidth)
    {
        PrimarySearchEditor             =  CreateSearchEditor("Primary", columnWidth);
        PrimarySearchEditor.TextChanged += PrimarySearchEditorOnTextChanged;

        SecondarySearchEditor             =  CreateSearchEditor("Secondary", columnWidth);
        SecondarySearchEditor.TextChanged += SecondarySearchEditorOnTextChanged;

        TertiarySearchEditor             =  CreateSearchEditor("Tertiary", columnWidth);
        TertiarySearchEditor.TextChanged += TertiarySearchEditorOnTextChanged;
    }

    private void CreateColorPickers(int columnWidth)
    {
        PrimaryColorPicker                     =  CreatPicker("Primary", columnWidth);
        PrimaryColorPicker.SelectionChanged    += PrimaryColorPickerOnSelectionChanged;
        PrimaryColorPicker.OkButtonClicked     += PrimaryColorPickerOkButtonClicked;
        PrimaryColorPicker.CancelButtonClicked += PrimaryColorPickerCancelButtonClicked;
        PrimaryColorPicker.FooterView
                          .TextStyle = new PickerTextStyle
                                       {
                                           TextColor = ColorUtility.ChooseReadableTextColor(PrimaryColorPicker.BackgroundColor)
                                       };

        SecondaryColorPicker                     =  CreatPicker("Secondary", columnWidth);
        SecondaryColorPicker.SelectionChanged    += SecondaryColorPickerOnSelectionChanged;
        SecondaryColorPicker.OkButtonClicked     += SecondaryColorPickerOkButtonClicked;
        SecondaryColorPicker.CancelButtonClicked += SecondaryColorPickerCancelButtonClicked;
        SecondaryColorPicker.FooterView
                            .TextStyle = new PickerTextStyle
                                         {
                                             TextColor = ColorUtility.ChooseReadableTextColor(SecondaryColorPicker.BackgroundColor)
                                         };

        TertiaryColorPicker                     =  CreatPicker("Tertiary", columnWidth);
        TertiaryColorPicker.SelectionChanged    += TertiaryColorPickerOnSelectionChanged;
        TertiaryColorPicker.OkButtonClicked     += TertiaryColorPickerOkButtonClicked;
        TertiaryColorPicker.CancelButtonClicked += TertiaryColorPickerCancelButtonClicked;
        TertiaryColorPicker.FooterView
                           .TextStyle = new PickerTextStyle
                                        {
                                            TextColor = ColorUtility.ChooseReadableTextColor(TertiaryColorPicker.BackgroundColor)
                                        };
    }

    private Button CreateButton(string name, string nameOnButton)
    {
        var button = new Button
                     {
                         Text = $"Set {nameOnButton} color"
                     };
        button.SetDynamicResource(BackgroundColorProperty, name);

        return button;
    }

    private static Editor CreateSearchEditor(string editorName, int requestedWidth)
    {
        return new Editor
               {
                   Placeholder  = $"Search {editorName} colors"
                 , WidthRequest = requestedWidth
                 , IsVisible    = false
               };
    }

    private SfPicker CreatPicker(string pickerName, int requestedWidth)
    {
        var picker = new SfPicker
                     {
                         HeaderView = new PickerHeaderView
                                      {
                                          Text   = $"{pickerName} color"
                                        , Height = 40
                                      }
                       , FooterView = new PickerFooterView
                                      {
                                          ShowOkButton = true
                                        , Height       = 40
                                      }
                       , HeightRequest = 200
                       , WidthRequest  = requestedWidth
                       , IsVisible     = false
                     };
        picker.SetDynamicResource(BackgroundColorProperty, pickerName);

        var colorPickerColumn = new PickerColumn
                                {
                                    DisplayMemberPath = "Name"
                                  , ItemsSource       = ColorUtility.ColorNames
                                  , SelectedIndex     = ColorUtility.GetIndexFromColor(picker.BackgroundColor)
                                };

        picker.Columns.Add(colorPickerColumn);

        return picker;
    }

    private Thickness DefaultThickness()
    {
        return new Thickness(5, 5, 5, 5);
    }
}
