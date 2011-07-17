using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace GooglePlus.Touch
{
	public class UserController : UITableViewController
	{
		public UserController ()
		{
			Title = "User";
			
			TableView.DataSource = new Data (this);
			TableView.Delegate = new Del (this);
		}

		class Data : UITableViewDataSource
		{
			UserController _c;

			public Data (UserController c)
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
			UserController _c;

			public Del (UserController c)
			{
				_c = c;
			}
		}
	}
}

