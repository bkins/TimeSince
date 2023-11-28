using System.Xml;
using Google.Apis.Util;
using Plugin.MauiMTAdmob;
using TimeSince.Avails;
using TimeSince.Data;
using TimeSince.MVVM.Views;
using MainPage = TimeSince.MVVM.Views.MainPage;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using TimeSince.Services;
using TimeSince.Services.ServicesIntegration;

namespace TimeSince;

public partial class App : Application
{
	private static readonly string LocalApplicationFolder = Environment.GetFolderPath(Environment.SpecialFolder
	                                                                                             .LocalApplicationData);
	private static readonly string DatabasePath = Path.Combine(LocalApplicationFolder
	                                                         , "TimeSince.db3");

	private static readonly Secrets.FileJsonContentProvider FileJsonContentProvider = new();

	private readonly Secrets _secrets = new(FileJsonContentProvider.GetJsonContent);

	private static SqliteDatabase _database;
	public static  SqliteDatabase Database => _database ??= new SqliteDatabase(DatabasePath);

	private static Logger _logger;
	public static  Logger Logger => _logger ??= new Logger();

	public static readonly AppIntegrationService AppServiceMethods = AppIntegrationService.Instance;

	private static string MainPageBannerId               { get; set; }
	private static string MainPageNewEventInterstitialId { get; set; }

	private const string PathToAndroidManifestXml = "path_to/AndroidManifest.xml";
	private const string AdsApplicationId         = @"//meta-data[@android:name='com.google.android.gms.ads.APPLICATION_ID']";
	private const string AndroidValue             = "android:value";

	public App()
	{
		Syncfusion.Licensing
		          .SyncfusionLicenseProvider
		          .RegisterLicense(_secrets.GetSecretValue(SecretCollections.SyncFusion
		                                                 , SecretKeys.SyncFusionLicense));

		AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainOnUnhandledException;

		InitializeComponent();
		MergeResourcesDictionaries();

		MainPage = new NavigationPage(new MainPage());

		LoadAdMobUnitIds();

		CrossMauiMTAdmob.Current.UserPersonalizedAds = true;
		CrossMauiMTAdmob.Current.ComplyWithFamilyPolicies = true;
		CrossMauiMTAdmob.Current.UseRestrictedDataProcessing = true;
		CrossMauiMTAdmob.Current.AdsId = MainPageBannerId;
	}

	protected override void OnStart()
	{
		AppServiceMethods.InitializeServices();

		base.OnStart();
	}

	private void LoadAdMobUnitIds()
	{
		MainPageBannerId = _secrets.GetSecretValue(SecretCollections.Admob
		                                        , SecretKeys.MainPageBanner);
		MainPageNewEventInterstitialId = _secrets.GetSecretValue(SecretCollections.Admob
		                                                      , SecretKeys.MainPageNewEventInterstitial);
	}

	private static void OnCurrentDomainOnUnhandledException(object                      sender
	                                                      , UnhandledExceptionEventArgs e)
	{
		var exception = (Exception)e.ExceptionObject;
		Logger.LogError(exception);
	}

	private static void MergeResourcesDictionaries()
	{
		var colorsResourceDictionary = new ResourceDictionary();
		colorsResourceDictionary.LoadFromXaml(GetXamlContentFromAssemblyResource());

		Current?.Resources.MergedDictionaries.Add(colorsResourceDictionary);
	}

	private static string GetXamlContentFromAssemblyResource(string resourceName = "TimeSince.Resources.Styles.Colors.xaml")
	{
		var assembly = typeof(App).Assembly;

		using var stream = assembly.GetManifestResourceStream(resourceName);
		if (stream == null) return string.Empty;

		using var reader = new StreamReader(stream);
		return reader.ReadToEnd();
	}
}
