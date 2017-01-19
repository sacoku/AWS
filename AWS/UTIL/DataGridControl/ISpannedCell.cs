using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AWS.UTIL.DataGridControl
{
  interface ISpannedCell
    {
        int ColumnSpan { get; }
        int RowSpan { get; }
        DataGridViewCell OwnerCell { get; }
    }
}
