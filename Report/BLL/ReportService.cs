using System;
using System.Data;
using Npgsql;
using ReportUI.DAL;

namespace ReportUI.BLL
{
    public class ReportService
    {
        private ParkingRepository repo = new ParkingRepository();

        public (int Total, int Moto, int Car) LoadVehicleCount()
        {
            return (repo.CountAll(), repo.CountMoto(), repo.CountCar());
        }

        public DataTable GetColumnChart(int filterType, DateTime date, int month, int quarter)
        {
            string sql = "";
            NpgsqlParameter[] parameters = null;

            if (filterType == 0) // Day
            {
                sql = @"SELECT EXTRACT(HOUR FROM TIME_CHECK_IN) AS Hour, COUNT(*) AS Count
                        FROM PARKING_INFO
                        WHERE TIME_CHECK_IN::date = @Day
                        GROUP BY EXTRACT(HOUR FROM TIME_CHECK_IN)
                        ORDER BY Hour";

                parameters = new[] { new NpgsqlParameter("@Day", date.Date) };
            }
            else if (filterType == 1) // Month
            {
                sql = @"SELECT EXTRACT(DAY FROM TIME_CHECK_IN) AS Day, COUNT(*) AS Count
                        FROM PARKING_INFO
                        WHERE EXTRACT(MONTH FROM TIME_CHECK_IN) = @Month
                        AND EXTRACT(YEAR FROM TIME_CHECK_IN) = @Year
                        GROUP BY EXTRACT(DAY FROM TIME_CHECK_IN)
                        ORDER BY Day";

                parameters = new[]
                {
                    new NpgsqlParameter("@Month", month),
                    new NpgsqlParameter("@Year", DateTime.Now.Year)
                };
            }
            else // Quarter
            {
                int start = (quarter - 1) * 3 + 1;
                int end = start + 2;

                sql = @"SELECT EXTRACT(MONTH FROM TIME_CHECK_IN) AS Month, COUNT(*) AS Count
                        FROM PARKING_INFO
                        WHERE EXTRACT(MONTH FROM TIME_CHECK_IN) BETWEEN @Start AND @End
                        AND EXTRACT(YEAR FROM TIME_CHECK_IN) = @Year
                        GROUP BY EXTRACT(MONTH FROM TIME_CHECK_IN)
                        ORDER BY Month";

                parameters = new[]
                {
                    new NpgsqlParameter("@Start", start),
                    new NpgsqlParameter("@End", end),
                    new NpgsqlParameter("@Year", DateTime.Now.Year)
                };
            }

            return repo.LoadColumnChart(sql, parameters);
        }
    }
}
