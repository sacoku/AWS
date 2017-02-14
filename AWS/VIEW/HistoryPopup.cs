//-----------------------------------------------------------------------
// <copyright file="HistoryPopup.cs" company="[Company Name]">
//     Copyright (c) [Company Name] Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using AWS.Config;
using AWS.MODEL;
using AWS.UTIL;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AWS.VIEW
{
	public delegate void OnComplete(String msg);

	public partial class HistoryPopup : Form
	{
		private MainForm frm = null;
		private int nLoggerIdx = 0;
		private Thread th = null;
		public OnComplete CompleteHandler = null;
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
			try
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

				for (int i = 0; i < 3; i++)
				{
					Label lb = (Label)((FieldInfo)this.GetType().GetMember("label" + (i + 1), BindingFlags.Instance | BindingFlags.NonPublic)[0]).GetValue(this);
					ListView v = (ListView)((FieldInfo)this.GetType().GetMember("listView" + (i + 1), BindingFlags.Instance | BindingFlags.NonPublic)[0]).GetValue(this);

					lb.Text = AWSConfig.sValue[i].Name;

					ColumnHeader cHeader = new ColumnHeader();
					cHeader.Width = 100;
					cHeader.Text = "수신시각";
					v.Columns.Add(cHeader);
					ColumnHeader cHeader2 = new ColumnHeader();
					cHeader2.Width = 120;
					cHeader2.Text = "복원 데이터";
					v.Columns.Add(cHeader2);

					v.GridLines = true;

					this.CenterToScreen();

				}
			} 
			catch(Exception ex)
			{
				iLog.Error(ex.ToString());
			}

			//listView1.GridLines = true;
			//listView2.GridLines = true;
			//listView3.GridLines = true;
		}

		public void Stop()
		{
			button2_Click(this, new EventArgs());
		}

		private void button1_Click(object sender, EventArgs e)
		{
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
					   frm.displayStatus(string.Format("{0} 사이트 {1} ~ {2}를 복원합니다.",
											AWS.Config.AWSConfig.sValue[nLoggerIdx].Name,
											monthCalendar1.SelectionStart.ToString("yyyy/MM/dd"),
											monthCalendar1.SelectionEnd.ToString("yyyy/MM/dd")), Color.Blue);

					   DateTime dt = monthCalendar1.SelectionStart;

					   while (DateTime.Compare(dt, monthCalendar1.SelectionEnd) <= 0)
					   {
						   frm.rLogger[nLoggerIdx].bIsReadyToRun2 = false;
						   frm.rLogger[nLoggerIdx].bIsReadyToRun = false;
						   Thread.Sleep(2000);
						   frm.rLogger[nLoggerIdx].bIsReadyToRun2 = true;
						   frm.rLogger[nLoggerIdx].RecoverLostData(new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 0), false);
						   button1.Enabled = true;
						   
						   dt = dt.AddDays(+1);

					   }

					   MessageBox.Show(string.Format("{1} 지점 {2}~{3} 일자의 데이터가 복원 되었습니다.",
										AWS.Config.AWSConfig.sValue[nLoggerIdx].Name,
										monthCalendar1.SelectionStart.ToString("yyyy/MM/dd"),
										monthCalendar1.SelectionEnd.ToString("yyyy/MM/dd")));
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
				iLog.Error(ex.ToString());
			}
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBoxItem item = comboBox1.SelectedItem as ComboBoxItem;
			nLoggerIdx = item.Code;
		}

		public void OnComplete(int i, String msg)
		{
			try
			{
				ListView v = (ListView)((FieldInfo)Program.mf.historyForm.GetType()
									.GetMember("listView" + (i + 1), BindingFlags.Instance | BindingFlags.NonPublic)[0]).GetValue(Program.mf.historyForm);
				ListViewItem lItem = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
				lItem.SubItems.Add(msg);
				v.Items.Insert(0, lItem);
			}
			catch(Exception ex)
			{
				iLog.Error(ex.ToString());
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < 3; i++)
			{
				try
				{
					ListView v = (ListView)((FieldInfo)this.GetType().GetMember("listView" + (i + 1), BindingFlags.Instance | BindingFlags.NonPublic)[0]).GetValue(this);

					v.Clear();

					ColumnHeader cHeader = new ColumnHeader();
					cHeader.Width = 100;
					cHeader.Text = "수신시각";
					v.Columns.Add(cHeader);
					ColumnHeader cHeader2 = new ColumnHeader();
					cHeader2.Width = 120;
					cHeader2.Text = "복원 데이터";
					v.Columns.Add(cHeader2);
				}
				catch(Exception ex)
				{
					iLog.Error(ex.ToString());
				}

			}
		}
	}
}
