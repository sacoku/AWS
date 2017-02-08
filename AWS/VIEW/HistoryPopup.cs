using AWS.MODEL;
using AWS.UTIL;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AWS.VIEW
{
	public partial class HistoryPopup : Form
	{
		private MainForm frm = null;
		private int nLoggerIdx = 0;
		private Boolean isPause = false;
		private Thread th = null;
		ILog iLog = log4net.LogManager.GetLogger("Logger");

		public class ComboBoxItem
		{
			#region Fields
			private int code;
			private string name;
			#endregion

			#region Properties
			public int Code
			{
				get { return code; }
				set { code = value; }
			}
			public string Name
			{
				get { return name; }
				set { name = value; }
			}
			#endregion

			public override string ToString()
			{
				return this.name;
			}
		}

		public HistoryPopup(MainForm frm)
		{
			this.frm = frm;

			InitializeComponent();

			for (int i = 0; i < AWS.Config.AWSConfig.sCount; i++)
			{
				ComboBoxItem item = new ComboBoxItem();
				item.Code = i;
				item.Name = AWS.Config.AWSConfig.sValue[i].Name;
				comboBox1.Items.Add(item);
			}

			ComboBoxItem item2 = new ComboBoxItem();
			item2.Code = 3;
			item2.Name = "전체";
			comboBox1.Items.Add(item2);

			comboBox1.SelectedIndex = 0;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			isPause = false;

			if( monthCalendar1.SelectionStart.Year == DateTime.Now.Year &&
				monthCalendar1.SelectionStart.Month == DateTime.Now.Month &&
				monthCalendar1.SelectionStart.Day == DateTime.Now.Day)
			{
				MessageBox.Show("오늘 이전 날짜부터 가능합니다.");
				return;
			}

			if (nLoggerIdx == 3)
			{
				/*
				for(int i=0;i<3;i++)
				{
					new Thread(() =>
					{
						button1.Enabled = false;
						DateTime dt = monthCalendar1.SelectionStart;
						frm.rLogger[i].bIsReadyToRun = false;
						Thread.Sleep(1000);
						frm.rLogger[i].bIsReadyToRun2 = true;
						frm.rLogger[i].RecoverLostData(new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 0), false);
						button1.Enabled = true;
					}).Start();

					Thread.Sleep(1000);
				}
				*/
				MessageBox.Show("전체 장비 복원은 준비중 입니다.");
			}
			else
			{
				th = new Thread(() =>
			   {
				   try
				   {
					   button1.Enabled = false;
					   DateTime dt = monthCalendar1.SelectionStart;
					   frm.rLogger[nLoggerIdx].bIsReadyToRun = false;
					   frm.rLogger[nLoggerIdx].bIsReadyToRun2 = false;
					   Thread.Sleep(2000);
					   frm.rLogger[nLoggerIdx].bIsReadyToRun2 = true;
					   frm.rLogger[nLoggerIdx].RecoverLostData(new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 0), false);
					   button1.Enabled = true;

					   MessageBox.Show(string.Format("{1} 지점 {2}/{3}/{4} 일자의 데이터가 복원 되었습니다.",
								   AWS.Config.AWSConfig.sValue[nLoggerIdx].Name,
								   dt.Year,
								   dt.Month,
								   dt.Day));
				   }
				   catch (Exception ex)
				   {
					   iLog.Error(ex.ToString());
				   }
			   }); th.Start();
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			try
			{
				isPause = true;
				frm.rLogger[nLoggerIdx].bIsReadyToRun2 = false;
				Thread.Sleep(1000);
				if (th != null)
				{
					th.Abort();
					th = null;
				}

				frm.rLogger[nLoggerIdx].bIsReadyToRun = true;
				frm.rLogger[nLoggerIdx].bIsReadyToRun2 = true;
				button1.Enabled = true;
			} 
			catch(Exception ex)
			{
				frm.rLogger[nLoggerIdx].bIsReadyToRun = true;
				frm.rLogger[nLoggerIdx].bIsReadyToRun2 = true;
				//iLog.Error(ex.ToString());
			}
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBoxItem item = comboBox1.SelectedItem as ComboBoxItem;
			nLoggerIdx = item.Code;
		}

		private void HistoryPopup_FormClosed(object sender, FormClosedEventArgs e)
		{
			button2_Click(sender, e);
		}
	}
}
