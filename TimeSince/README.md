## Purpose

- To track and display events and the Time Since they occurred

## Features

- Displays the amount of time since an event has occurred.
- Change the way the time since is displayed by tapping on the label.
- Change the Primary, Secondary, and Tertiary colors of the app.
- Sort by event Title or Start Date

## Future Features & Bugs

- High Priority

  - In Setting page
    - ~~Try to guess at when the font color is too light or too dark and switch it~~
    - ~~The Set color buttons and color pickers should be in the same column~~
    - ~~The section labels ("App Colors" and "Ads") don't look that good.~~
    - Option for user to remove ads
      - The option is there, really need a way for user to pay to turn off ads.
  
  - Need to test the app in Airplane Mode
  
- Medium Priority 
  
  - Add End Date (Goal Date?) to events
    - When the End Date is set, do not calculate time since in loop.  Just set it at initialization.
    - If I decide to use "Goal" instead of an End date, then come up with a way to highlight completed events
  - Get Widget working
    - User should be able to select which event(s) will display in widget.
  
- Low Priority

  - Remove hard coded RowHeight in the EventsListView in the MainPage.  This should grow based on the size of the controls within it.
  - Change Splash Screen
    - Is this needed?
    - What would a better one look like?

- Done
  - ~~App crashes on my device when scrolling on the MainPage~~
    - Fixed by following: https://stackoverflow.com/questions/75080266/during-scrolling-listview-error-cannot-access-a-disposed-object
  - ~~Implement ads~~
  - ~~Save the sort order selected by user~~
  - ~~Log entries are not being saved.~~
    - ~~When app is closed and restarted the previous log entries are not present~~
    - ~~Need way to see log; and/or send the log to developer.~~
- ~~Display a message at start up that gives the user the option to:~~
  - ~~read the Privacy Policy~~
  - ~~turn the message off (so it does not display at startup)~~
  - ~~And an option in the settings page to control whether or not the massage is displayed at startup.~~
  - ~~Add About Page~~
    - ~~App description~~
    - ~~Version~~
    - ~~Contact info~~
    - ~~Separate information on this page in Frames~~
