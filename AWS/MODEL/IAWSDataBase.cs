using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWS.MODEL
{
    interface IAWSDataBase
    {
        System.Data.Common.DbConnection Connect(string connStr);
        void Close();
        double[] GetSensorMaxData();
        double[] GetSensorMinData();
        void InsertSensorData(DateTime dt, int dev_idx, KMA2 kma, double[] min_values, double[] max_values);
        double[] GetSensorAvgData();
    }
}
