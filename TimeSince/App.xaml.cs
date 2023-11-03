using System.Xml;
using Google.Apis.Util;
using Plugin.MauiMTAdmob;
using TimeSince.Avails;
using TimeSince.Data;
using TimeSince.MVVM.Views;
using MainPage = TimeSince.MVVM.Views.MainPage;

namespace TimeSince;

public partial class App : Application
{
	private static readonly string LocalApplicationFolder = Environment.GetFolderPath(Environment.SpecialFolder
	                                                                                             .LocalApplicationData);
	private static readonly string DatabasePath = Path.Combine(LocalApplicationFolder
	                                                         , "TimeSince.db3");

	private const string PathToAndroidManifestXml = "path_to/AndroidManifest.xml";
	private const string AdsApplicationId         = @"//meta-data[@android:name='com.google.android.gms.ads.APPLICATION_ID']";
	private const string AndroidValue             = "android:value";

	private static SqliteDatabase _database;
	public static  SqliteDatabase Database => _database ??= new SqliteDatabase(DatabasePath);

	public        string AdMobAppId                   { get; set; }
	public static string MainPageBannerId             { get; set; }
	public static string MainPageNewEventInterstitialId { get; set; }

	public App()
	{
		LogToConsole("Loading SyncfusionLicenseProvider...");
		Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjcwNDI0MUAzMjMzMmUzMDJlMzBGWXBmSGtQRnI0eVBacUk0OWZoT2Y0OS92bDg3MVJRYnV3eEtneXB6UCtRPQ==");

		AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainOnUnhandledException;

		LogToConsole(("Initializing Components..."));
		InitializeComponent();

		LogToConsole(("Merging resource dictionaries..."));
		MergeResourcesDictionaries();

		LogToConsole("Setting MainPage...");
		MainPage = new NavigationPage(new MainPage());

		LogToConsole("Loading AdMob unit ids...");
		LoadAdMobUnitIds();

		LogToConsole("Initializing CrossMauiMTAdmob settings...");
		CrossMauiMTAdmob.Current.UserPersonalizedAds = true;
		CrossMauiMTAdmob.Current.ComplyWithFamilyPolicies = true;
		CrossMauiMTAdmob.Current.UseRestrictedDataProcessing = true;
		CrossMauiMTAdmob.Current.AdsId = MainPageBannerId;
	}

	private void LogToConsole(string message)
	{
		var traceLoggingOn = false;
#if DEBUG
		traceLoggingOn = true;
#endif

		const string preLogTag = "TraceLogging: ";

		Console.WriteLine($"{preLogTag}{message}");
	}
	private void LoadAdMobUnitIds()
	{
		MainPageBannerId = Avails.Utilities.GetSecretValue(SecretCollections.Admob
		                                                 , SecretKeys.MainPageBanner);
		MainPageNewEventInterstitialId = Avails.Utilities.GetSecretValue(SecretCollections.Admob
		                                                               , SecretKeys.MainPageNewEventInterstitial);
	}

	private void OnCurrentDomainOnUnhandledException(object                      sender
	                                               , UnhandledExceptionEventArgs e)
	{
		// Log the exception details
		var exception = (Exception)e.ExceptionObject;
		// Handle the exception or log it as needed
	}

	private void MergeResourcesDictionaries()
	{

		var colorsResourceDictionary = new ResourceDictionary();
		colorsResourceDictionary.LoadFromXaml(GetColorsXamlContent()); // Assuming your XAML file is named "Colors.xaml"

		// Merge the resource dictionary into the current application resources
		Current?.Resources.MergedDictionaries.Add(colorsResourceDictionary);
	}

	private string GetColorsXamlContent()
	{
		const string resourceName = "TimeSince.Resources.Styles.Colors.xaml";

		var assembly = typeof(App).Assembly;

		using var stream = assembly.GetManifestResourceStream(resourceName);
		if (stream == null) return string.Empty;

		using var reader = new StreamReader(stream);
		return reader.ReadToEnd();
	}
}
