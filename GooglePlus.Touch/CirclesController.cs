using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace GooglePlus.Touch
{
	public class CirclesController : UITableViewController
	{
		public CirclesController ()
		{
			Title = "Circles";
			
			TableView.DataSource = new Data (this);
			TableView.Delegate = new Del (this);
		}

		class Data : UITableViewDataSource
		{
			CirclesController _c;

			public Data (CirclesController c)
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
			CirclesController _c;

			public Del (CirclesController c)
			{
				_c = c;
			}
		}
	}
}

