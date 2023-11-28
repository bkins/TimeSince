using Syncfusion.Maui.ListView.Helpers;
using TimeSince.Avails;
using TimeSince.MVVM.Models;

namespace TimeSince.Behaviors;

public class SfListViewAccordionBehavior : Behavior<ContentPage>
{
    #region Fields

        private LogLine tappedItem;
        private Syncfusion.Maui.ListView.SfListView listview;

    #endregion

    #region Override Methods

        protected override void OnAttachedTo(ContentPage bindable)
        {
            listview = bindable.FindByName<Syncfusion.Maui.ListView.SfListView>("listView");
            listview.ItemTapped += ListView_ItemTapped;
        }

        protected override void OnDetachingFrom(BindableObject bindable)
        {
            listview.ItemTapped -= ListView_ItemTapped;
            listview = null;
        }

    #endregion

    #region Private Methods

        private void ListView_ItemTapped(object sender, Syncfusion.Maui.ListView.ItemTappedEventArgs e)
        {
            if (tappedItem is { IsVisible: true })
            {
                var previousIndex = listview.DataSource?.DisplayItems.IndexOf(tappedItem) ?? 0;

                tappedItem.IsVisible = false;

                if (DeviceInfo.Platform != DevicePlatform.MacCatalyst)
                {
                    listview.RefreshItem(previousIndex, previousIndex, false);
                }
            }

            if (tappedItem == e.DataItem as LogLine)
            {
                if (DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                {
                    if (tappedItem != null)
                    {
                        var previousIndex = listview.DataSource?.DisplayItems.IndexOf(tappedItem) ?? 0;
                        listview.RefreshItem(previousIndex, previousIndex, false);
                    }
                }

                tappedItem = null;
                return;
            }

            tappedItem           = e.DataItem as LogLine;
            if (tappedItem != null) tappedItem.IsVisible = true;

            if (DeviceInfo.Platform == DevicePlatform.MacCatalyst)
            {
                var visibleLines = listview?.GetVisualContainer().ScrollRows?.GetVisibleLines();
                if (visibleLines != null)
                {
                    var firstIndex = visibleLines[visibleLines.FirstBodyVisibleIndex].LineIndex;
                    var lastIndex  = visibleLines[visibleLines.LastBodyVisibleIndex].LineIndex;
                    listview.RefreshItem(firstIndex, lastIndex, false);
                }

            }
            else
            {
                var currentIndex = listview.DataSource?.DisplayItems.IndexOf(e.DataItem) ?? 0;
                 listview.RefreshItem(currentIndex, currentIndex, false);
            }
        }
        #endregion
}
