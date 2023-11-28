using Syncfusion.Maui.Popup;
using TimeSince.Avails;
using TimeSince.Avails.Attributes;
using TimeSince.Avails.ColorHelpers;
using TimeSince.MVVM.ViewModels;
using TimeSince.Data;
using TimeSince.Services.ServicesIntegration;

namespace TimeSince.MVVM.Views;

public partial class MainPage : ContentPage
{
	private   TimeElapsedViewModel TimeElapsedViewModel { get; set; }

	protected View AdView { get; set; }

	public MainPage()
	{
		ColorUtility.PopulateColorNames();

		ShowPrivacyPolicyMessage();

		InitializeComponent();

		TimeElapsedViewModel = new TimeElapsedViewModel();

		BindingContext = TimeElapsedViewModel;

		Appearing += OnPageAppearing;
		TimeElapsedViewModel.Events.CollectionChanged += Events_OnCollectionChanged;

		AdView = AppIntegrationService.GetAdView();
		if (AdView is not null)
		{
			MainStackLayout.Add(AdView);
		}

		PreferencesDataStore.AwesomePersonScore += 1;

		TimeElapsedViewModel.SortEvents((SortOptions)PreferencesDataStore.LastSortOption);

		ScrollToFirstItemInListView();

	}

	[UnderConstruction("Need to implement a message a the start up of the app that gives the user to read the Privacy Policy.")]
	private void ShowPrivacyPolicyMessage()
	{
		// Check if the message should be displayed

		if (! PreferencesDataStore.HideStartupMessage)
		{
			// Create a popup or dialog
			// var popup = new SfPopup
			//             {
			// 	            HeaderTitle = "Welcome!"
			// 	           , ContentTemplate = new StackLayout
			// 	                      {
			// 		                      Children =
			// 		                      {
			// 			                      new Label { Text = "Your short message here" },
			// 			                      new Button
			// 			                      {
			// 				                      Text = "Visit our website",
			// 				                      Command = new Command(() =>
			// 				                      {
			// 					                      // Open the web page on click
			// 					                      Device.OpenUri(new Uri("https://yourwebsite.com"));
			// 				                      })
			// 			                      },
			// 			                      new Button
			// 			                      {
			// 				                      Text = "Don't show again",
			// 				                      Command = new Command(() =>
			// 				                      {
			// 					                      // Store the preference not to show the message again
			// 					                      Preferences.Set("HideStartupMessage", true);
			// 					                      popup.Dismiss();
			// 				                      })
			// 			                      }
			// 		                      }
			// 	                      }
			//             };
			//
			// // Show the popup
			// App.Current.MainPage.Navigation.PushModalAsync(popup);
		}
	}

	private void ScrollToFirstItemInListView()
	{

		var firstItem = EventsListView.ItemsSource?.Cast<object>().FirstOrDefault();
		ScrollToItemInListView(firstItem, ScrollToPosition.Start);
	}

	private void ScrollToLastItemInListView()
	{
		var lastItem = EventsListView.ItemsSource?.Cast<object>().LastOrDefault();
		ScrollToItemInListView(lastItem, ScrollToPosition.End);
	}

	private void ScrollToItemInListView(object item, ScrollToPosition position)
	{
		if (item is not null)
		{
			EventsListView.ScrollTo(item, position, true);
		}
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

		EventsListView.ItemTapped += EventsListView_OnItemTapped;

		RefreshAdViewOnPage();

		PreferencesDataStore.AwesomePersonScore += 1;
	}

	private void RefreshAdViewOnPage()
	{
		var adsAreEnabled        = App.AppServiceMethods.AreAdsEnabled();
		var adViewHasValue       = AdView is not null;
		var adViewIsInMainLayout = MainStackLayout.Children.Contains(AdView);

		if ( ! adViewHasValue
		  && adsAreEnabled)
		{
			AdView           = AppIntegrationService.GetAdView();
			AdView.IsVisible = true;

			MainStackLayout.Add(AdView);

			return;
		}

		if (adViewHasValue
		 && adsAreEnabled
		 && ! adViewIsInMainLayout)
		{
			AdView.IsVisible = true;
			MainStackLayout.Add(AdView);

			return;
		}

		if (AdView is not null
		 && ! adsAreEnabled
		 && adViewIsInMainLayout)
		{
			AdView.IsVisible = false;
			MainStackLayout.Remove(AdView);
		}
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();

		EventsListView.ItemTapped -= EventsListView_OnItemTapped;
	}

}
