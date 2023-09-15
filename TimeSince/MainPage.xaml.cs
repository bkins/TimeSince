
namespace TimeSince;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();

		BeginningDatePicker.Date     = Settings.BeginDateTime;
		BeginningTimePicker.Time     = Settings.BeginDateTime.TimeOfDay;
		BeginningDateNameEditor.Text = Settings.BeginDateTimeName;

		StartUpdatingElapsedTime();
	}

	private async void StartUpdatingElapsedTime()
	{
		while (true)
		{
			await MainThread.InvokeOnMainThreadAsync(() =>
			{
				var elapsedTime = DateTime.Now - Settings.BeginDateTime;
				var hours   	= elapsedTime.Hours.ToString().PadLeft(2, '0');
				var minutes 	= elapsedTime.Minutes.ToString().PadLeft(2, '0');
				var seconds 	= elapsedTime.Seconds.ToString().PadLeft(2, '0');

				ElapsedTimeLabel.Text = $"Elapsed Time: {elapsedTime.Days} days, {hours}:{minutes}:{seconds}";
			});

			await Task.Delay(TimeSpan.FromSeconds(1));
		}
		// ReSharper disable once FunctionNeverReturns
	}

	private void SaveButton_OnClicked(object    sender
	                                , EventArgs e)
	{
		Settings.BeginDateTime     = GetDateTimeControlsAsDateTime();
		Settings.BeginDateTimeName = BeginningDateNameEditor.Text;

		Application.Current?.Quit();
	}

	private DateTime GetDateTimeControlsAsDateTime()
	{
		var date = BeginningDatePicker.Date;
		var time = BeginningTimePicker.Time;
		return new DateTime(date.Year
		                  , date.Month
		                  , date.Day
		                  , time.Hours
		                  , time.Minutes
		                  , time.Seconds);
	}
}
