using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace CarParking.Controllers
{
    /// <summary>
    /// Response Class 
    /// </summary>
    public class FeeCalucateResponse
    {
        public float TotalAmount { get; set; }
        public string ResponseMessage { get; set; }
    }
    public class ParkingFeeController : ApiController
    {
        FeeCalucateResponse fc = new FeeCalucateResponse();
        public ParkingFeeController() { }
        [HttpGet]
        public FeeCalucateResponse GetParkingFee(string entryTimeString, string exitTimeString)
        {
            try
            {
                DateTime startDate;
                DateTime endDate;
                if (!DateTime.TryParse(entryTimeString, out startDate))
                {
                    fc.ResponseMessage += string.Format("EntryTime  {0} is not a valid  format {1}", entryTimeString, Environment.NewLine);
                }
                if (!DateTime.TryParse(exitTimeString, out endDate))
                {
                    fc.ResponseMessage += string.Format("ExitTime {0} is not valid format {1}", exitTimeString, Environment.NewLine);
                }
                if (fc.ResponseMessage == null)
                {
                    DayOfWeek entryDay = startDate.DayOfWeek;
                    DayOfWeek exitDay = endDate.DayOfWeek;
                    string entrytime = startDate.ToShortTimeString();
                    string exittime = endDate.ToShortTimeString();

                    TimeSpan duration = endDate - startDate;

                    //Cheapest Rate
                    if (duration.TotalHours < 1)
                    {
                        fc.ResponseMessage = "Hourly Rate  for 1 hour";
                        fc.TotalAmount = 5;
                        return fc;
                    }


                    //Weekend rate
                    if ((entryDay == DayOfWeek.Saturday) || (entryDay == DayOfWeek.Sunday))
                    {
                        if ((exitDay == DayOfWeek.Saturday) || (exitDay == DayOfWeek.Sunday))
                        {// ("This is a weekend");
                            if (duration.TotalDays < 3)
                            {
                                fc.ResponseMessage = " Weekend Rate";
                                fc.TotalAmount = 10;
                                return fc;
                            }
                        }
                    }
                    TimeSpan startOfExit = new TimeSpan(15, 30, 0); //3:30 PM
                    TimeSpan endofExit = new TimeSpan(23, 30, 0); // 11:30 PM
                    // night rate on weekdays
                    if (startDate.Hour >= 18)
                    {
                        if (endDate.TimeOfDay >= startOfExit && endDate.TimeOfDay <= endofExit)
                        {
                            if (duration.TotalDays < 2)
                            {
                                fc.ResponseMessage = "Night Rate";
                                fc.TotalAmount = 6.5F;
                                return fc;
                            }
                        }
                    }

                    //2 hour rate
                    ///This needs to be  done before early  bird rate as this can be $10.00  where as Early bird rate is $13.00
                    if (duration.TotalHours < 2)
                    {
                        fc.ResponseMessage = "Hourly Rate  for 2 hour";
                        fc.TotalAmount = 10;
                        return fc;
                    }

                    // Early bird rate
                    if (startDate.Hour >= 6 && startDate.Hour <= 9)
                    {
                        if (endDate.TimeOfDay >= startOfExit && endDate.TimeOfDay <= endofExit)
                        {
                            if (duration.TotalDays < 1)
                            {
                                fc.ResponseMessage = "Early Bird Rate";
                                fc.TotalAmount = 13;
                                return fc;
                            }
                        }
                    }
                    //flat rate  of 5 per hour upto 4 hours
                    if (duration.TotalHours < 4)
                    {
                        fc.ResponseMessage = "Hourly rate  ";
                        fc.TotalAmount = (float)(Math.Ceiling(duration.TotalHours) * 5.0);
                        return fc;

                    }
                    else // other wise go for Daily rate
                    {
                        fc.ResponseMessage = "Daily rate  ";
                        fc.TotalAmount = (float)(Math.Ceiling(duration.TotalDays) * 20.0);
                        return fc;
                    }
                }

            }
            catch (Exception ex)
            {
                fc.ResponseMessage = ex.Message;
            }

            return fc;
        }
        /// <summary>
        /// asyn method for the ParkingFee
        /// </summary>
        /// <param name="entryTimeString"></param>
        /// <param name="exitTimeString"></param>
        /// <returns></returns>
        public async Task<FeeCalucateResponse> GetParkingFeeAsync(string entryTimeString, string exitTimeString)
        {
            return await Task.FromResult(GetParkingFee(entryTimeString, exitTimeString));
        }

    }
}
