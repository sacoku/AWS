using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AWS.VIEW
{
	public partial class HistoryForm : Form
	{
		private MainForm main = null;

		public HistoryForm()
		{
			InitializeComponent();
		}

		public HistoryForm(MainForm frm)
		{
			this.main = frm;
		}
	}
}
