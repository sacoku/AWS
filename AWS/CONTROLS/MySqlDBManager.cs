using AWS.MODEL;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace AWS.CONTROLS
{
    class MySqlDBManager : IAWSDataBase
    {
        public DbConnection Connect(string dataSourceFile)
        {
            return null;
        }

        public void Close()
        {

        }

        public double[] GetSensorMaxData()
        {
            return null;
        }

        public double[] GetSensorMinData()
        {
            return null;
        }

        public void InsertSensorData(DateTime dt, int dev_idx, KMA2 kma, double[] min_values, double[] max_values)
        {

        }

        public double[] GetSensorAvgData()
        {
            return null;
        }

    }
}
