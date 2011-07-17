using System;
using MonoTouch.UIKit;
using System.Threading;
using System.Drawing;

namespace GooglePlus.Touch
{
	public class AccountController : UIViewController
	{
		UITextField emailField;
		UITextField passwordField;
		
		public AccountController ()
		{
			Title = "Account";
			
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem (
				UIBarButtonSystemItem.Cancel,
				delegate {
				
				DismissModalViewControllerAnimated (true);
				
			});
			
			NavigationItem.RightBarButtonItem = new UIBarButtonItem (
				"Sign in",
				UIBarButtonItemStyle.Done,
				delegate {
				
					TrySignin ();		
			});
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			emailField = new UITextField {
				AutocapitalizationType = UITextAutocapitalizationType.None,
				AutocorrectionType = UITextAutocorrectionType.No,
				KeyboardType = UIKeyboardType.EmailAddress,
				Placeholder = "Email",
				Frame = new RectangleF (10, 60, 300, 33),
				ReturnKeyType = UIReturnKeyType.Next,
				ShouldReturn = delegate {
					passwordField.BecomeFirstResponder ();
					return true;
				},
			};
			View.AddSubview (emailField);
			
			passwordField = new UITextField {
				AutocapitalizationType = UITextAutocapitalizationType.None,
				AutocorrectionType = UITextAutocorrectionType.No,
				Placeholder = "Password",
				SecureTextEntry = true,
				Frame = new RectangleF (10, 110, 300, 33),
				ReturnKeyType = UIReturnKeyType.Done,
				ShouldReturn = delegate {
					ResignFirstResponder ();
					TrySignin ();
					return true;
				},
			};
			View.AddSubview (passwordField);
		}
		
		void TrySignin ()
		{
			var email = (emailField.Text ?? "").Trim ();
			var password = (passwordField.Text ?? "");
			
			if (string.IsNullOrEmpty (email) || string.IsNullOrEmpty (password)) {
				failureAlert = new UIAlertView (
					"Incomplete Information",
					"Please provide both an email address and password.",
					null,
					"OK");
				failureAlert.Show ();
				return;
			}

			var cookeisPath = System.IO.Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments),
				Uri.EscapeDataString ("cookies-" + email));
			var api = new Api (cookeisPath);
			
			
			NavigationItem.LeftBarButtonItem.Enabled = false;
			NavigationItem.RightBarButtonItem.Enabled = false;
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			
			ThreadPool.QueueUserWorkItem (delegate {
				try {
					api.SignIn (email, password);
					BeginInvokeOnMainThread (delegate {
						OnSigninSuccess (api);
					});
				}
				catch (Exception error) {
					BeginInvokeOnMainThread (delegate {
						OnSigninFailure (error);
					});
				}
				finally {
					BeginInvokeOnMainThread (delegate {
						NavigationItem.LeftBarButtonItem.Enabled = true;
						NavigationItem.RightBarButtonItem.Enabled = true;
						UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
					});
				}
			});
		}
		
		UIAlertView failureAlert;
		
		void OnSigninFailure (Exception error)
		{
			failureAlert = new UIAlertView (
				"Sigin in Failure",
				error.ToString (),
				null,
				"OK");
			failureAlert.Show ();
		}
		
		void OnSigninSuccess (Api api)
		{
			DismissModalViewControllerAnimated (true);
				
			var c = AccountChanged;
			if (c != null) {
				c (this, new ApiEventArgs {
					Api = api,
				});
			}
		}
		
		public static event EventHandler<ApiEventArgs> AccountChanged;		
	}
	
	public class ApiEventArgs : EventArgs
	{
		public Api Api { get; set; }
	}
}

