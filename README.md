# agrirouter-gps-info-app

## What is this repository about?
This repository includes the source code to an app for android and ios; based on xamarin that exchanges GPS Information with the agrirouter

<img src="assets/images/gpsapp_Logo.png" width="400px"/>


## Funded Project
This app was developed by DKE Data in cooperation with Hochschule Osnabrück as part of and funded by the project "Experimentierfeld Agro-Nordwest". The project is funded by the Federal ministry of food and agriculture. Project sponsor is the federal agency of food and agriculture (BLE).

German original:
> "Die App wurde von DKE Data in Zusammenarbeit mit der Hochschule Osnabrück im Rahmen und aus Mitteln des Förderprojektes Experimentierfeld Agro-NordWest entwickelt. Das Projekt wird gefördert durch das Bundesministerium für Ernährung und Landwirtschaft. Projektträger ist die Bundesanstalt für Landwirtschaft und Ernährung (BLE)."


<img src="assets/images/BMEL_Logo.svg " width="200px"/>
as part of the Project
<img src="assets/images/logo-agro-nordwest.svg" width="200px"/>


## The author
The GPS Info App was implemented by [dev4Agriculture](https://www.dev4Agriculture.de) on behalf of [DKE Data](https://www.my-agrirouter.com)
<img src="assets/images/dev4Agriculture_Logo.svg" width="400px"/>


## Specification
The specification of the functionalities can be found [here](docs/Specification.md).


## UseCase

There is a blog article describing potential [use cases for the app](https://www.dev4agriculture.de/en/2022/12/04/agrirouter-gps-info-app-2/).

## Setting up the app for building in your environment

The source code misses the following sensitive information to be buildable:

1.) The API Code for the Google Maps integration. Get your code [in your google developer account](https://cloud.google.com/maps-platform/) and enter it in the file [AndroidManifest.XML](/Agrirouter/Agrirouter.Android/Properties/AndroidManifest.XML) at __android:name="com.google.android.geo.API_KEY"__

2.) The ApplicationId and the CertificationVersionId have to be entered from an application generated in the agrirouter UI. See [the agrirouter documentation](https://docs.my-agrirouter.com/agrirouter-interface-documentation/latest/applications.html). These codes need to be entered [Constants.cs](/Agrirouter/Agrirouter/Common/Constants.cs)

--- 

## App Handling

### Setup

 On start of the app, the user needs to allow GPS positions. 

 When the user clicks the "Play" button, he will be redirected to the Settings with the info, that he's not yet onboarded. 

 He will need to onboard the app by entering a registration code (see also [The agrirouter docs](https://docs.my-agrirouter.com/agrirouter-interface-documentation/latest/integration/onboarding.html#creating-a-registration-code))


### Map view
In the map view, the user has the following possibilities:
* <img src="assets/images/ic_map_layers.png" width="20">**Change map layer**: Shall Satellite, Street or Hybrid be shown?
* <img src="assets/images/ic_help.png" width="20">**Display/hide legend**: Shall the legend be shown
* <img src="assets/images/ic_start.png" width="20">**Start/<img src="assets/images/ic_pause.png" width="20">Stop record?**: Shall GPS positions be recorded and sent?
* <img src="assets/images/ic_my_position_black.png" width="20">**Zoom to position**: Shall the map consistantly center my own position?
* <img src="assets/images/ic_auto_center_green.png" width="20">**Zoom to all**: Shall the map zoom out so that all members are displayed?
* <img src="assets/images/ic_settings.png" width="20">**Settings**: Call the settings

### On the map

The application shows markers of all other endpoints that send GPS positions. The color indicates how recent those data is:

* <img src="assets/images/ic_pin_online.png" width="20">: The position is not older than 10 minutes
* <img src="assets/images/ic_pin_offline.png" width="20">: The position is not older than 15 minutes
* No Pin: If there was a position, it is now older than 15 minutes

When clicking on a marker, the user can gain more information about this specific endpoint


### Settings
In the settings, the user has the following possibilities:

* **Onboarding/Reonboarding/Disconnect**: The user can handle the agrirouter connection status here
* **Change the record cycle**: How often shall GPS positions be recorded?
* **Change the send cycle**: How often shall GPS positions be sent?
* **Change the environment**: Shall the app be onboarded to QA or Production? Only possible before onboarding
* **Keep display on**: This prohibits the screen from going black while working
* **View the logs**: Logs can be exported e.g. in case of errors
* **View status information**: When was the last message sent, are there buffered messages due to internet connection issues?