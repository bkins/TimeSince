using TimeSince.Avails.ColorHelpers;
using TimeSince.MVVM.ViewModels;
using TimeSince.Data;

namespace TimeSince.MVVM.Views;

public partial class MainPage : ContentPage
{
	private   TimeElapsedViewModel TimeElapsedViewModel { get; set; }

	protected View AdView { get; set; }

	public MainPage()
	{
		ColorUtility.PopulateColorNames();

		InitializeComponent();

		ShowPrivacyPolicyMessage();

		TimeElapsedViewModel = new TimeElapsedViewModel();

		BindingContext = TimeElapsedViewModel;

		Appearing += OnPageAppearing;
		TimeElapsedViewModel.Events.CollectionChanged += Events_OnCollectionChanged;

		AdView = App.AppServiceMethods.GetAdView();
		if (AdView is not null)
		{
			MainStackLayout.Add(AdView);
		}

		PreferencesDataStore.AwesomePersonScore += 1;

		TimeElapsedViewModel.SortEvents((SortOptions)PreferencesDataStore.LastSortOption);

		ScrollToFirstItemInListView();

	}


	private void ShowPrivacyPolicyMessage()
	{
		if (! PreferencesDataStore.HideStartupMessage)
		{
			PrivacyPolicyPopup.Show();
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
		var hasPurchased         = TimeElapsedViewModel.HasPurchasedToHideAds();
		var adsAreEnabled        = App.AppServiceMethods.AreAdsEnabled();
		var adViewHasValue       = AdView is not null;
		var adViewIsInMainLayout = MainStackLayout.Children.Contains(AdView);

		if (hasPurchased)
		{
			UpdateAdViewVisibility(false
			                     , false);
		}
		else switch (adViewHasValue)
		{
			case false when adsAreEnabled:
				AdView = App.AppServiceMethods.GetAdView();
				UpdateAdViewVisibility(true
				                     , true);

				break;

			case true when ! adViewIsInMainLayout
			            && adsAreEnabled:
				UpdateAdViewVisibility(true
				                     , true);

				break;

			default:
			{
				if (AdView is not null && ! adsAreEnabled && adViewIsInMainLayout)
				{
					UpdateAdViewVisibility(false
					                     , true);
				}

				break;
			}
		}
	}

	private void UpdateAdViewVisibility(bool isVisible
	                                  , bool addToLayout)
	{
		if (AdView is null) return;

		AdView.IsVisible = isVisible;

		switch (addToLayout)
		{
			case true when ! MainStackLayout.Children.Contains(AdView):
				MainStackLayout.Add(AdView);

				break;

			case false when MainStackLayout.Children.Contains(AdView):
				MainStackLayout.Remove(AdView);

				break;
		}
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();

		EventsListView.ItemTapped -= EventsListView_OnItemTapped;
	}


	private async void OkButton_OnClicked(object    sender
	                                    , EventArgs e)
	{
		if (PrivacyPolicyPopup.BindingContext is not TimeElapsedViewModel viewModel) return;

		if (viewModel.IsPrivacyPolicyAccepted)
		{
			PrivacyPolicyPopup.IsOpen = false;

			PreferencesDataStore.HideStartupMessage = true;
		}
		else
		{
			await DisplayAlert("Error", "Please check the privacy policy acceptance checkbox to continue."
			                 , "OK");
		}
	}

	private void SetButtonsTextColor()
	{
		var buttonTextColor = ColorUtility.ChooseReadableTextColor(ColorUtility.GetColorFromResources(ResourceColors.Primary));
		foreach (var beginningEvent in TimeElapsedViewModel.Events)
		{
			beginningEvent.ButtonTextColor = buttonTextColor;
		}

		AddEventButton.TextColor = buttonTextColor;
	}
}
