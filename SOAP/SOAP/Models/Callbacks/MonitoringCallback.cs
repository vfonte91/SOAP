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
            if (read["a.EquipmentId"].ToString() != "")
                monitor.Equipment.Id = Convert.ToInt32(read["a.EquipmentId"].ToString());

            foreach (Monitoring.LazyComponents a in lazyComponents)
            {
                if (a == Monitoring.LazyComponents.LOAD_EQUIPMENT_WITH_DETAIL && monitor.Equipment.Id != -1)
                {
                    monitor.Equipment.Category.Id = Convert.ToInt32(read["b.CategoryId"].ToString());
                    monitor.Equipment.Label = read["b.Label"].ToString();
                    monitor.Equipment.OtherFlag = Convert.ToChar(read["b.OtherFlag"].ToString());
                    monitor.Equipment.Description = read["b.Description"].ToString();
                    if (read["b.Concentration"].ToString() != "")
                        monitor.Equipment.Concentration = Convert.ToDecimal(read["b.Concentration"].ToString());
                }
            }

            return monitor;
        }
    }
}