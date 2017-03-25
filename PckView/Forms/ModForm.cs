using System;
using System.Windows.Forms;


//http://www.vbdotnetheaven.com/Code/May2004/MultiColListViewMCB.asp

namespace PckView
{
	public partial class ModForm
		:
		Form
	{
		private XCom.SharedSpace _share;

		public ModForm()
		{
			InitializeComponent();

			var ri = new DSShared.Windows.RegistryInfo(this);
		}

		public XCom.SharedSpace SharedSpace
		{
			set
			{
				_share = value;

				foreach (XCom.Interfaces.IXCImageFile xcf in _share.GetImageModList())
				{
					if (   xcf.FileExtension == ".bad"
						&& xcf.Author        == "Author"
						&& xcf.Description   == "Description")
					{
						var strings = new string[]
						{
							xcf.FileExtension,
							xcf.Author,
							xcf.GetType().ToString()
						};
						modList.Items.Add(new ListViewItem(strings));
					}
					else
					{
						var strings = new string[]
						{
							xcf.FileExtension,
							xcf.Author,
							xcf.Description
						};
						modList.Items.Add(new ListViewItem(strings));
					}
				}
			}
		}
	}
}
