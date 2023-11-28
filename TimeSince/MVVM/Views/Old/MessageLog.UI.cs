using TimeSince.Avails.ColorHelpers;
using TimeSince.MVVM.ViewModels;

namespace TimeSince.MVVM.Views.Old;

public partial class MessageLog : ContentPage
{
    public ListView LogListView    { get; set; }
    public Label    CategoryLabel  { get; set; }
    public Label    TimeStampLabel { get; set; }
    public Label    MessageLabel   { get; set; }

    public MessageLog()
    {
        InitializePage();

    }

    private void InitializePage()
    {
        Title = "Message Log";

        LogEntryViewModel = new LogViewModel();
        BindingContext    = LogEntryViewModel;

        var tertiaryColor  = ColorUtility.GetColorFromResources(ResourceColors.Tertiary);
        var secondaryColor = ColorUtility.GetColorFromResources(ResourceColors.Secondary);
        var primaryColor   = ColorUtility.GetColorFromResources(ResourceColors.Primary);

        LogListView = new ListView
                      {
                           ItemTemplate = new DataTemplate(() =>
                           {
                               MessageLabel = new Label
                                              {
                                                  TextColor      = tertiaryColor
                                                , FontAttributes = FontAttributes.Italic
                                                , LineBreakMode  = LineBreakMode.TailTruncation
                                              };
                               MessageLabel.SetBinding(Label.TextProperty
                                                     , "Message");

                               TimeStampLabel = new Label
                                                {
                                                    TextColor      = tertiaryColor
                                                  , FontAttributes = FontAttributes.Italic
                                                };
                               TimeStampLabel.SetBinding(Label.TextProperty
                                                       , "TimeStamp");

                               CategoryLabel = new Label
                                               {
                                                   TextType = TextType.Html
                                               };
                               CategoryLabel.SetBinding(Label.TextProperty
                                                      , "CategoryAsHtml");

                               var grid = new Grid
                                          {
                                              RowDefinitions =
                                              {
                                                  new RowDefinition { Height = GridLength.Auto }
                                                , new RowDefinition { Height = GridLength.Auto }
                                                , new RowDefinition { Height = GridLength.Auto }
                                              }
                                          };

                               grid.Add(MessageLabel, 0, 0);
                               grid.Add(TimeStampLabel, 0, 1);
                               grid.Add(CategoryLabel, 0, 2);

                               var frame = new Frame
                                           {
                                               Content         = grid
                                             , CornerRadius    = 10
                                             , Padding         = new Thickness(10)
                                             , Margin          = new Thickness(5)
                                             , BackgroundColor = secondaryColor
                                             , HasShadow       = true
                                           };

                               var viewCell = new ViewCell { View = frame };

                               return viewCell;
                           })
                         , RowHeight = 85
                       };

        LogListView.ItemTapped += OnLogListViewOnItemTapped;

        LogListView.SetBinding(ListView.ItemsSourceProperty, "LogEntries");

        Content = LogListView;
    }
}
