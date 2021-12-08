# Specification

## Stories

The application shall fullfill the following stories

* As a farmer/contractor/driver, I want to send my GPS position to a software via agrirouter
* As a farmer/contractor/driver, I want to see the GPS position of others
* As a farmer/contractor/driver, I want to be aware that the app sends GPS positions
* As a farmer/contractor/driver, I want to be able to disconnect the app from agrirouter

## Technical Requirements
The app shall:
* run on Android 10+ 
* run on iOS 13+ (which covers > 80% of all iPhones)
* collect GPS positions every 1, 5, 10 or 20 seconds
* send GPS positions in a package every 10 or 20 seconds
* display my own GPS position on a map
* display the GPS position of others on a map
* have a possibility to onboard, disconnect and reonboard to agrirouter
* store the login credentials and load them on start



## UI Behaviour
* Splash Screen
    * While loading, a splash screen shall show the App logo
* Start Screen 
    * The start screen is the map
    * On top of the map, there is a round button that leads to the settings
    * On the map, there is a button to pause sending positions to agrirouter
    * On start of the app, the sending is active (if already onboarded)
* Settings Screen
    * The settings have a field to enter a TAN and a button to perform onboarding
    * The settings - after onboarding succeeded - provide a TAN field, a ReOnboard-Button and a Disconnect-Button
    * The settings screen requires a selection field for QA/Production Environment
    * The settings screen 
* Notifications and others:
    * Errors are displayed as popup
    * The map view changes with orientation of the smartphone (Horizontal/Vertical)
    * If GPS access is disallowed, this is an error that produces a Popup
    * Provided Language is English


## On Maps

* The Android App will use Google Maps. The map costs are up to the customer, see [the pricing model](https://cloud.google.com/maps-platform/pricing#:~:text=Google%20Maps%20Platform%20offers%20a,exceeds%20%24200%20in%20a%20month.)

* The iOS App uses MapKit, which is free of charge, see [mapKit](https://developer.apple.com/maps/) 

## Agrirouter Requirements

* **Capabilities**: The app shall have the following Capabilities:
    * Send/Receive GPS:Info

* **Subscriptions**:  
    * The app shall subscribe for GPS:Info messages
    
* **Sending Cycle**: The app shall send GPS packages every 5,10 or 20 Seconds depending on the settings

* **Connection Interface**: The app shall use the HTTP REST interface of agrirouter

* **Push Notifications**: Push notifications shall be activated

* **Requesting the feed**: The app shall request the messages from the feed for the last 15 minutes on connection
* **Clear the feed**: The app shall clear its feed after receiving the messages for the last 15 minutes

* **Requesting the ecosystem**: To apply proper naming for the received GPS positions, the app will request the ecosystem every 5 minutes.

* **Environment**: 
    * The app shall be able to connect to the QA environment
    * The app shall be able to connect to the Prod environment
    * The environment can be selected in the Settings Screen (QA is default)
    * The selection is disabled, when the app is onboarded.

## Edge cases

### User disallows GPS usage
A popup informs him, that this is required, afterwards the app ends. On restart of the app, GPS is requested again.

### Connection to agrirouter is lost
GPS positions are buffered and will be sent once the connection could be reestablished. 
The data needs to be stored in the local storage so that it does not get lost even though the app might be deactivated.

### No GPS position is available
The information that no GPS is available is sent to agrirouter

### A member that's displayed on the map does not send new positions
The marker on the map shall turn grey to indicate, that the position is outdated (after 5 minutes without data)

### The user leaves the app
The app shall keep sending GPS positions even though it's in the background. An indicator in the Notification bar shall indicate that.


## Deliverables

As a result, we will deliver:
* The source code in a publicly available Repository
* An app documentation within the repository
* App-Delivery for testing; e.g. in TestFlight
* A short video showing the functionalities (no marketing quality)
* An app store release is not required

# Priorities and Timeline

A first version shall be available by End of April; not providing all functionalities. The second version can be available for Mid of June. A potential AppStore release can be focused afterwards for Mid of July

