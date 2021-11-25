# agrirouter-gps-info-app

## What is this repository about?
This repository includes the source code to an app for android and ios; based on xamarin that exchanges GPS Information with the agrirouter

![GPSApp Logo](assets/images/gpsapp_Logo.png "GPS App Logo")

A project funded by the Bundesministerium für Landwirtschaft und Ernährung

![BMEL Logo](assets/images/BMEL_Logo.svg "BMEL Logo")

as part of the Project

![Experimentierfeld Logo](assets/images/logo-agro-nordwest.svg "Experimentierfeld Logo")

## The author
The GPS Info App was developed by [dev4Agriculture](https://www.dev4Agriculture.de) on behalf of [DKE Data](https://www.my-agrirouter.com)
![dev4Agriculture Logo](assets/images/dev4Agriculture_Logo.svg "dev4Agriculture Logo")


## Specification
The specification of the functionalities can be found [here](docs/specification).


## Setting up the app for building in your environment

The source code misses the following sensitive information to be buildable:

1.) To build the agrirouter DotNet SDK, 

2.) The API Code for the Google Maps integration. Get your code [in your google developer account](https://cloud.google.com/maps-platform/) and enter it in the file [AndroidManifest.XML](/Agrirouter/Agrirouter.Android/Properties/AndroidManifest.XML) at __android:name="com.google.android.geo.API_KEY"__

3.) The ApplicationId and the CertificationVersionId have to be entered from an application generated in the agrirouter UI. See [the agrirouter documentation](https://docs.my-agrirouter.com/agrirouter-interface-documentation/latest/applications.html). These codes need to be entered [Constants.cs](/Agrirouter/Agrirouter/Common/Constants.cs)