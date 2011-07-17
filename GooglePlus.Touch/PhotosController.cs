using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace GooglePlus.Touch
{
	public class PhotosController : UITableViewController
	{
		public PhotosController ()
		{
			Title = "Photos";
			
			TableView.DataSource = new Data (this);
			TableView.Delegate = new Del (this);
		}

		class Data : UITableViewDataSource
		{
			PhotosController _c;

			public Data (PhotosController c)
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
			PhotosController _c;

			public Del (PhotosController c)
			{
				_c = c;
			}
		}
	}
}

