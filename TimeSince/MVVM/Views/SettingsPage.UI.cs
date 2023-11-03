using Syncfusion.Maui.Picker;
using TimeSince.Avails;

namespace TimeSince.MVVM.Views;

public partial class SettingsPage : ContentPage
{
    private SfPicker PrimaryColorPicker   { get; set; }
    private SfPicker SecondaryColorPicker { get; set; }
    private SfPicker TertiaryColorPicker  { get; set; }

    private Editor PrimarySearchEditor   { get; set; }
    private Editor SecondarySearchEditor { get; set; }
    private Editor TertiarySearchEditor  { get; set; }

    private Button PrimaryButton   { get; set; }
    private Button SecondaryButton { get; set; }
    private Button TertiaryButton  { get; set; }

    private   Label    RemoveAdsLabel    { get; set; }
    private   CheckBox RemoveAdsCheckBox { get; set; }
    protected View     AdView            { get; set; }

    private void InitializeComponent()
    {
        const int columnWidth = 175;

        Title = "Settings";

        CreateColorPickers(columnWidth);
        CreateSearchEditors(columnWidth);
        CreateSetButtons();
        CreateRemoveAdsControls();

        // Create and configure your grid as before
        var mainGrid      = BuildMainGrid();
        var adPlaceholder = new BoxView { HeightRequest = 0 };

        // Create a StackLayout to hold the mainGrid and AdView
        var stackLayout = new StackLayout
                          {
                              Children = { mainGrid, adPlaceholder }
                          };

        AdView = AdManager.Instance.GetAdView();
        if (AdView is not null)
        {
            // Replace the empty BoxView with the AdView in the StackLayout
            stackLayout.Children.Remove(adPlaceholder);
            stackLayout.Children.Add(AdView);
        }

        // Set the StackLayout as the content of the page
        Content = stackLayout;
    }

    private void CreateRemoveAdsControls()
    {
        RemoveAdsLabel = new Label
                         {
                             Text                  = "Show Ads"
                           , VerticalTextAlignment = TextAlignment.Center
                           , VerticalOptions       = LayoutOptions.Center
                         };

        RemoveAdsCheckBox = new CheckBox
                            {
                                IsChecked         = RemoveAdsViewModel.PaidForAdsToBeRemoved
                              , VerticalOptions   = LayoutOptions.Center
                              , HorizontalOptions = LayoutOptions.Start
                            };
        RemoveAdsCheckBox.CheckedChanged += RemoveAdsCheckBoxOnCheckedChanged;
    }

    private Grid BuildMainGrid()
    {
        var mainGrid = new Grid { Margin = DefaultThickness() };

        mainGrid.RowDefinitions.Add(UiUtilities.NewRowDefinition(GridLength.Auto));
        mainGrid.RowDefinitions.Add(UiUtilities.NewRowDefinition(GridLength.Auto));
        mainGrid.RowDefinitions.Add(UiUtilities.NewRowDefinition(GridLength.Auto));
        mainGrid.RowDefinitions.Add(UiUtilities.NewRowDefinition(GridLength.Auto));
        mainGrid.RowDefinitions.Add(UiUtilities.NewRowDefinition(GridLength.Auto));
        mainGrid.RowDefinitions.Add(UiUtilities.NewRowDefinition(GridLength.Auto));
        mainGrid.RowDefinitions.Add(UiUtilities.NewRowDefinition(GridLength.Auto));
        mainGrid.RowDefinitions.Add(UiUtilities.NewRowDefinition(GridLength.Auto));

        var primaryGrid   = new Grid { Margin = DefaultThickness() };
        var secondaryGrid = new Grid { Margin = DefaultThickness() };
        var tertiaryGrid  = new Grid { Margin = DefaultThickness() };

        // Define the rows and columns for the grid
        primaryGrid.RowDefinitions.Add(UiUtilities.NewRowDefinition(GridLength.Auto));
        primaryGrid.RowDefinitions.Add(UiUtilities.NewRowDefinition(GridLength.Auto));
        secondaryGrid.RowDefinitions.Add(UiUtilities.NewRowDefinition(GridLength.Auto));
        secondaryGrid.RowDefinitions.Add(UiUtilities.NewRowDefinition(GridLength.Auto));
        tertiaryGrid.RowDefinitions.Add(UiUtilities.NewRowDefinition(GridLength.Auto));
        tertiaryGrid.RowDefinitions.Add(UiUtilities.NewRowDefinition(GridLength.Auto));

        primaryGrid.ColumnDefinitions.Add(UiUtilities.NewColumnDefinition(GridLength.Star));
        primaryGrid.ColumnDefinitions.Add(UiUtilities.NewColumnDefinition(GridLength.Star));

        secondaryGrid.ColumnDefinitions.Add(UiUtilities.NewColumnDefinition(GridLength.Star));
        secondaryGrid.ColumnDefinitions.Add(UiUtilities.NewColumnDefinition(GridLength.Star));

        tertiaryGrid.ColumnDefinitions.Add(UiUtilities.NewColumnDefinition(GridLength.Star));
        tertiaryGrid.ColumnDefinitions.Add(UiUtilities.NewColumnDefinition(GridLength.Star));

        primaryGrid.Add(PrimaryButton, 0, 0);
        primaryGrid.Add(PrimarySearchEditor, 1, 0);
        primaryGrid.Add(PrimaryColorPicker, 1, 1);

        secondaryGrid.Add(SecondaryButton, 0, 0);
        secondaryGrid.Add(SecondarySearchEditor, 1, 0);
        secondaryGrid.Add(SecondaryColorPicker, 1, 1);

        tertiaryGrid.Add(TertiaryButton, 0, 0);
        tertiaryGrid.Add(TertiarySearchEditor, 1, 0);
        tertiaryGrid.Add(TertiaryColorPicker, 1, 1);

        var colorSectionLabel = new Label
                                {
                                    Text            = "App Colors"
                                  , BackgroundColor = Colors.LightGray
                                };

        var adSectionLabel = new Label
                             {
                                 Text            = "Ads"
                               , BackgroundColor = Colors.LightGray
                               , Margin = new Thickness(0, 5, 0, 5)
                             };
        mainGrid.Add(colorSectionLabel, 0, 0);
        mainGrid.Add(primaryGrid, 0, 1);
        mainGrid.Add(secondaryGrid, 0, 2);
        mainGrid.Add(tertiaryGrid, 0, 3);

        var resetColorsButton = new Button { Text = "Reset Colors" };

        resetColorsButton.Clicked += ResetColorsButtonOnClicked;

        mainGrid.Add(resetColorsButton, 0, 4);

        var removeAdsGrid = new Grid { Margin = DefaultThickness() };

        removeAdsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        removeAdsGrid.ColumnDefinitions.Add(UiUtilities.NewColumnDefinition(GridLength.Auto));
        removeAdsGrid.ColumnDefinitions.Add(UiUtilities.NewColumnDefinition(GridLength.Star));

        removeAdsGrid.Add(RemoveAdsLabel, 0, 0);
        removeAdsGrid.Add(RemoveAdsCheckBox, 1, 0);

        mainGrid.Add(adSectionLabel, 0, 5);

        mainGrid.Add(removeAdsGrid, 0, 6);

        // AdView = AdManager.Instance.GetAdView();
        //
        // if (AdView is not null)
        // {
        //     mainGrid.Add(AdView, 0, 6);
        // }

        return mainGrid;
    }


    private void CreateSetButtons()
    {

        PrimaryButton         =  CreateButton("Primary");
        PrimaryButton.Clicked += PrimaryButtonOnClicked;

        SecondaryButton         =  CreateButton("Secondary");
        SecondaryButton.Clicked += SecondaryButtonOnClicked;

        TertiaryButton         =  CreateButton("Tertiary");
        TertiaryButton.Clicked += TertiaryButtonOnClicked;
    }

    private void CreateSearchEditors(int columnWidth)
    {

        PrimarySearchEditor = CreateSearchEditor("Primary"
                                               , columnWidth);
        PrimarySearchEditor.TextChanged += PrimarySearchEditorOnTextChanged;

        SecondarySearchEditor = CreateSearchEditor("Secondary"
                                                 , columnWidth);
        SecondarySearchEditor.TextChanged += SecondarySearchEditorOnTextChanged;

        TertiarySearchEditor = CreateSearchEditor("Tertiary"
                                                , columnWidth);
        TertiarySearchEditor.TextChanged += TertiarySearchEditorOnTextChanged;
    }

    private void CreateColorPickers(int columnWidth)
    {

        PrimaryColorPicker = CreatPicker("Primary"
                                       , columnWidth);
        PrimaryColorPicker.SelectionChanged    += PrimaryColorPickerOnSelectionChanged;
        PrimaryColorPicker.OkButtonClicked     += PrimaryColorPickerOkButtonClicked;
        PrimaryColorPicker.CancelButtonClicked += PrimaryColorPickerCancelButtonClicked;

        SecondaryColorPicker = CreatPicker("Secondary"
                                         , columnWidth);
        SecondaryColorPicker.SelectionChanged    += SecondaryColorPickerOnSelectionChanged;
        SecondaryColorPicker.OkButtonClicked     += SecondaryColorPickerOkButtonClicked;
        SecondaryColorPicker.CancelButtonClicked += SecondaryColorPickerCancelButtonClicked;

        TertiaryColorPicker = CreatPicker("Tertiary"
                                        , columnWidth);
        TertiaryColorPicker.SelectionChanged    += TertiaryColorPickerOnSelectionChanged;
        TertiaryColorPicker.OkButtonClicked     += TertiaryColorPickerOkButtonClicked;
        TertiaryColorPicker.CancelButtonClicked += TertiaryColorPickerCancelButtonClicked;
    }


    private Button CreateButton(string name)
    {
        var button = new Button
               {
                   Text = $"Set {name} color"
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
                         HeaderView    = new PickerHeaderView { Text         = $"{pickerName} color", Height = 40 }
                       , FooterView    = new PickerFooterView { ShowOkButton = true, Height                  = 40 }
                       , HeightRequest = 200
                       , WidthRequest  = requestedWidth
                       , IsVisible     = false
                     };
        picker.SetDynamicResource(BackgroundColorProperty, pickerName);

        var colorPickerColumn = new PickerColumn
                                {
                                    DisplayMemberPath = "Name"
                                  , ItemsSource       = ColorUtility.ColorNames
                                };
        colorPickerColumn.SelectedIndex = ColorUtility.GetIndexFromColor(picker.BackgroundColor);

        // Add the PickerColumn to the SfPicker
        picker.Columns.Add(colorPickerColumn);

        return picker;
    }

    private Thickness DefaultThickness()
    {
        return new Thickness(5, 5, 5, 5);
    }
}
