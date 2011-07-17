using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace GooglePlus.Touch
{
	public class HomeController : UITableViewController
	{
		UINavigationController accountNav;
		
		public HomeController ()
		{
			Title = "Home";
			
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem (
				"Account",
				UIBarButtonItemStyle.Plain,
				delegate {
					accountNav = new UINavigationController (new AccountController ());
					NavigationController.PresentModalViewController (accountNav, true);
			});
			
			TableView.DataSource = new Data (this);
			TableView.Delegate = new Del (this);
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			
			
		}
	
		class Data : UITableViewDataSource
		{
			HomeController _c;
	
			public Data (HomeController c)
			{
				c = _c;
			}
	
			public override int NumberOfSections (UITableView tableView)
			{
				return 1;
			}
	
			public override int RowsInSection (UITableView tableView, int section)
			{
				return 0;
			}
	
			static readonly NSString CellId = new NSString ("C");
	
			public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				var c = tableView.DequeueReusableCell (CellId);
				if (c == null) {
					c = new UITableViewCell (UITableViewCellStyle.Default, CellId);
					var sec = indexPath.Section;
					var i = indexPath.Row;
				}
				return c;
			}
		}
	
		class Del : UITableViewDelegate
		{
			HomeController _c;
	
			public Del (HomeController c)
			{
				_c = c;
			}
		}
	}
}

