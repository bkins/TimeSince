//
// using Android.App;
// using Android.Appwidget;
// using Android.Content;
// using Android.Widget;
//
// namespace TimeSince.Platforms.Android
// {
//     [BroadcastReceiver(Label = "TimeSinceWidget"
//                      , Enabled = true
//                      , Exported = true
//                      , Name = "TimeSince.Platforms.Android.WidgetProvider")]
//     [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
//     [MetaData("android.appwidget.provider", Resource = "@xml/WidgetConfig")]
//     [Service(Exported = true)]
//     public class WidgetProvider : AppWidgetProvider
//     {
//         public override void OnUpdate(Context          context
//                                     , AppWidgetManager appWidgetManager
//                                     , int[]            appWidgetIds)
//         {
//             if (context is null) return;
//             var ids = appWidgetIds ?? Array.Empty<int>();
//
//             foreach (var appWidgetId in ids)
//             {
//                 var widgetView = new RemoteViews(context.PackageName, Resource.Layout.Widget);
//
//                 SetTextViewText(widgetView);
//
//                 // Update the widget
//                 appWidgetManager?.UpdateAppWidget(appWidgetId, widgetView);
//             }
//             // var componentName = new ComponentName(context
//             //                                     , Java.Lang
//             //                                           .Class
//             //                                           .FromType(typeof(AppWidget)).Name);
//             // appWidgetManager?.UpdateAppWidget(componentName
//             //                                 , BuildRemoteViews(context
//             //                                                  , appWidgetIds));
//         }
//
//         private RemoteViews BuildRemoteViews(Context context, int[] appWidgetIds)
//         {
//             var widgetView = new RemoteViews(context.PackageName, Resource.Layout.Widget);
//
//             SetTextViewText(widgetView);
//
//             return widgetView;
//         }
//
//         private static void SetTextViewText(RemoteViews widgetView)
//         {
//             widgetView.SetTextViewText(Resource.Id.widgetTitle, "The Widget");
//             widgetView.SetTextViewText(Resource.Id.widgetDays, "TimeSinceWidget");
//             widgetView.SetTextViewText(Resource.Id.widgetTime, $"Last update: {DateTime.Now:H:mm:ss}");
//         }
//         public override void OnReceive(Context context, Intent intent) { }
//     }
// }
