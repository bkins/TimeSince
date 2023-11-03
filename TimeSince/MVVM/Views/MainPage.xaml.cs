using TimeSince.Avails;
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

		TimeElapsedViewModel = new TimeElapsedViewModel();

		BindingContext = TimeElapsedViewModel;

		Appearing += OnPageAppearing;
		TimeElapsedViewModel.Events.CollectionChanged += Events_OnCollectionChanged;

		//AddBanner.AdsId = App.MainPageBannerId;
		AdView = AdManager.Instance.GetAdView();
		if (AdView is not null)
		{
			MainStackLayout.Add(AdView);
		}

		PreferencesDataStore.AwesomePersonScore += 1;
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
		var adsAreEnabled        = AdManager.Instance.AreAdsEnabled;
		var adViewHasValue       = AdView is not null;
		var adViewIsInMainLayout = MainStackLayout.Children.Contains(AdView);

		if ( ! adViewHasValue
		  && adsAreEnabled)
		{
			AdView           = AdManager.Instance.GetAdView();
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
