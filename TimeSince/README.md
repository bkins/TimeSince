## Purpose

- To track and display events and the Time Since they occurred

## Features

- Displays the amount of time since an event has occurred.
- Change the way the time since is displayed by tapping on the label.
- Change the Primary, Secondary, and Tertiary colors of the app.
- Sort by event Title or Start Date

## Future Features & Bugs

- High Priority
  - ~~App crashes on my device when scrolling on the MainPage~~
    - Fixed by following: https://stackoverflow.com/questions/75080266/during-scrolling-listview-error-cannot-access-a-disposed-object
- Medium Priority 
  
  - Add End Date (Goal Date?) to events
    - When the End Date is set, do not calculate time since in loop.  Just set it at initialization.
    - If I decide to use "Goal" instead of an End date, then come up with a way to highlight completed events
  - Get Widget working
  - Add About Page
    - App description
    - Version
    - Contact info
- Low Priority
  - Remove hard coded RowHeight in the EventsListView in the MainPage.  This should grow based on the size of the controls within it.
  - Change Splash Screen
  - Implement ads?
