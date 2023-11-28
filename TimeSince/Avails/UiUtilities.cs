namespace TimeSince.Avails;

public static class UiUtilities
{
    public static void AddCommandToGestureToImage(object sender, Action command)
    {
        if (sender is not Image image) return;

        var parent = image.Parent as View;

        parent?.GestureRecognizers
              .Clear();

        parent?.GestureRecognizers
              .Add(new TapGestureRecognizer { Command = new Command(command) });
    }

    #region Grid Definitions

    public static ColumnDefinition NewColumnDefinition(GridLength length)
    {
        return new ColumnDefinition { Width = length };
    }

    public static ColumnDefinition NewColumnDefinition(int          value
                                                     , GridUnitType type)
    {
        return new ColumnDefinition { Width = new GridLength(value, type) };
    }

    public static RowDefinition NewRowDefinition(int          value
                                               , GridUnitType type)
    {
        return new RowDefinition { Height = new GridLength(value, type) };
    }

    public static RowDefinition NewRowDefinition(GridLength height)
    {
        return new RowDefinition { Height = height };
    }

    public static void AddRowDefinitions(Grid grid, GridLength gridLength, int numberOfRows)
    {
        for (var i = 0; i < numberOfRows; i++)
        {
            grid.RowDefinitions.Add(NewRowDefinition(gridLength));
        }
    }

    public static void AddColumnDefinitions(Grid       grid
                                          , GridLength gridLength
                                          , int        numberOfColumns)
    {
        for (var i = 0; i < numberOfColumns; i++)
        {
            grid.ColumnDefinitions.Add(NewColumnDefinition(gridLength));
        }
    }

    #endregion

    #region Clipboard Access

    public static async Task<string> GetClipboardValueAsync()
    {
        if (Clipboard.HasText)
        {
            return await Clipboard.GetTextAsync();
        }

        return null;
    }

    public static async Task SetClipboardValueAsync(string text)
    {
        await Clipboard.SetTextAsync(text);
    }

    #endregion

    [Obsolete("Obsolete in MAUI. Ok to use in Xamarin")]
    public static class FontSizes<T>
    {
        public static double Default  => Device.GetNamedSize(NamedSize.Default, typeof(T));
        public static double Micro    => Device.GetNamedSize(NamedSize.Micro, typeof(T));
        public static double Small    => Device.GetNamedSize(NamedSize.Small, typeof(T));
        public static double Medium   => Device.GetNamedSize(NamedSize.Medium, typeof(T));
        public static double Large    => Device.GetNamedSize(NamedSize.Large, typeof(T));
        public static double Body     => Device.GetNamedSize(NamedSize.Body, typeof(T));
        public static double Caption  => Device.GetNamedSize(NamedSize.Caption, typeof(T));
        public static double Header   => Device.GetNamedSize(NamedSize.Header, typeof(T));
        public static double Subtitle => Device.GetNamedSize(NamedSize.Subtitle, typeof(T));
        public static double Title    => Device.GetNamedSize(NamedSize.Title, typeof(T));
    }

    /// <summary>
    /// Temporarily changes the text of the button passed in for the number of seconds passed in.
    /// </summary>
    /// <param name="button">The button to change the text of.</param>
    /// <param name="text">The text to change temporarily.</param>
    /// <param name="seconds">The number of seconds to change the text for.</param>
    /// <param name="timer">The timer to use to control how long the text will be displayed.</param>
    public static void TemporarilyChangeButtonText(Button           button
                                                 , string           text
                                                 , int              seconds
                                                 , IDispatcherTimer timer)
    {
        var originalButtonText = button.Text;

        MainThread.BeginInvokeOnMainThread(() => { button.Text = text; });

        timer.Interval = TimeSpan.FromMilliseconds(seconds * 1000);
        timer.Tick += (_, _) =>
        {
            button.Text = originalButtonText;
        };
        timer.Start();
    }

}
