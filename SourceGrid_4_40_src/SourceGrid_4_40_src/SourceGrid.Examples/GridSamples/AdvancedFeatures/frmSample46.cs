using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace WindowsFormsSample
{
	/// <summary>
	/// Summary description for frmSample14.
	/// </summary>
    [Sample("SourceGrid - Advanced features", 46, "Real Grid properties test")]
	public class frmSample46 : System.Windows.Forms.Form
	{
		private SourceGrid.Grid grid1;
        private PropertyGrid propertyGrid1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        public frmSample46()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.grid1 = new SourceGrid.Grid();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // grid1
            // 
            this.grid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grid1.EnableSort = true;
            this.grid1.Location = new System.Drawing.Point(324, 13);
            this.grid1.Name = "grid1";
            this.grid1.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.grid1.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.grid1.Size = new System.Drawing.Size(272, 273);
            this.grid1.TabIndex = 0;
            this.grid1.TabStop = true;
            this.grid1.ToolTipText = "";
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.propertyGrid1.Location = new System.Drawing.Point(14, 9);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.SelectedObject = this.grid1;
            this.propertyGrid1.Size = new System.Drawing.Size(303, 277);
            this.propertyGrid1.TabIndex = 1;
            // 
            // frmSample46
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(610, 299);
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.grid1);
            this.Name = "frmSample46";
            this.Text = "Basic Grid";
            this.Load += new System.EventHandler(this.frmSample46_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void frmSample46_Load(object sender, System.EventArgs e)
		{
			grid1.BorderStyle = BorderStyle.FixedSingle;

			grid1.ColumnsCount = 3;
			grid1.FixedRows = 1;
			grid1.Rows.Insert(0);
			grid1[0,0] = new SourceGrid.Cells.ColumnHeader("String");
			grid1[0,1] = new SourceGrid.Cells.ColumnHeader("DateTime");
			grid1[0,2] = new SourceGrid.Cells.ColumnHeader("CheckBox");
			for (int r = 1; r < 10; r++)
			{
				grid1.Rows.Insert(r);
				grid1[r,0] = new SourceGrid.Cells.Cell("Hello " + r.ToString(), typeof(string));
				grid1[r,1] = new SourceGrid.Cells.Cell(DateTime.Today, typeof(DateTime));
				grid1[r,2] = new SourceGrid.Cells.CheckBox(null, true);
			}

            grid1.AutoSizeCells();
		}
	}
}
