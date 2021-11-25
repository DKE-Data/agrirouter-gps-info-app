/*
 * Agrirouter GPS Info App
 *  Copyright 2021 by dev4Agriculture
 *  
 *  Funded by the Bundesministerium für Ernährung und Landwirtschaft (BMEL)
 *  as part of the Experimentierfelder-Project
 *
 * Licensed under Apache2
 */

using System;
using Agrirouter.Services.Shiny;
using Foundation;
using Plugin.SimpleLogger;
using Plugin.SimpleLogger.Abstractions;
using Prism;
using Prism.Ioc;
using Shiny;
using UIKit;

namespace Agrirouter.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Xamarin.Forms.Forms.Init();
            this.ShinyFinishedLaunching(new Startup());
            Xamarin.FormsMaps.Init();
            LoadApplication(new App(new iOSInitializer()));
            return base.FinishedLaunching(app, options);
        }
        
        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
            => this.ShinyDidReceiveRemoteNotification(userInfo, null);

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
            => this.ShinyDidReceiveRemoteNotification(userInfo, completionHandler);

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
            => this.ShinyRegisteredForRemoteNotifications(deviceToken);

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
            => this.ShinyFailedToRegisterForRemoteNotifications(error);

        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
            => this.ShinyPerformFetch(completionHandler);

        public override void HandleEventsForBackgroundUrl(UIApplication application, string sessionIdentifier, Action completionHandler)
            => this.ShinyHandleEventsForBackgroundUrl(sessionIdentifier, completionHandler);
        
        public override void OnResignActivation (UIApplication application)
        {
            Console.WriteLine ("App moving to inactive state.");
        }

        public override void DidEnterBackground (UIApplication application)
        {
            Console.WriteLine ("App entering background state.");
            Console.WriteLine ("Now receiving location updates in the background");
        }

        public override void WillEnterForeground (UIApplication application)
        {
            Console.WriteLine ("App will enter foreground");
        }

        public override void OnActivated (UIApplication application)
        {
            Console.WriteLine ("App is becoming active");
        }
        
        public class iOSInitializer : IPlatformInitializer
        {
            public void RegisterTypes(IContainerRegistry containerRegistry)
            {
                // Register any platform specific implementations
            }
        }
    }
}
