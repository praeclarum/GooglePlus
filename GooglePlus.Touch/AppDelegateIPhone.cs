using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace GooglePlus.Touch
{
	public partial class AppDelegateIPhone : UIApplicationDelegate
	{
		UITabBarController tabs;
		
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			tabs = new UITabBarController ();
			
			tabs.SetViewControllers (new UIViewController[] {
				new UINavigationController (new HomeController ()),
				new UINavigationController (new PhotosController ()),
				new UINavigationController (new ProfileController ()),
				new UINavigationController (new CirclesController ()),
				
			}, false);
			window.AddSubview (tabs.View);

			window.MakeKeyAndVisible ();
	
			return true;
		}
	}
}

