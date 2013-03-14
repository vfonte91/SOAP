using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SOAP.Models.Callbacks
{
    public class MonitoringCallback
    {
        public Monitoring ProcessRow(SqlDataReader read, params Monitoring.LazyComponents[] lazyComponents)
        {
            Monitoring monitor = new Monitoring();
            monitor.Id = Convert.ToInt32(read["a.Id"]);
            monitor.PatientId = Convert.ToInt32(read["a.PatientId"].ToString());
            monitor.Equipment.Id = Convert.ToInt32(read["a.EquipmentId"].ToString());

            foreach (Monitoring.LazyComponents a in lazyComponents)
            {
                if (a == Monitoring.LazyComponents.LOAD_EQUIPMENT_WITH_DETAIL)
                {
                    monitor.Equipment.Category.Id = Convert.ToInt32(read["b.CategoryId"].ToString());
                    monitor.Equipment.Label = read["b.Label"].ToString();
                    monitor.Equipment.OtherFlag = Convert.ToChar(read["b.OtherFlag"].ToString());
                    monitor.Equipment.Description = read["b.Description"].ToString();
                }
            }

            return monitor;
        }
    }
}