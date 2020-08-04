using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace CarParking.Controllers
{
    public class FeeCalucateResponse
    {
        public float TotalAmount { get; set; }
        public string ResponseMessage { get; set; }
    }
    public class ParkingFeeController : ApiController
    {
        /* string[] formats = {"M/d/yyyy h:mm:ss tt", "M/d/yyyy h:mm tt",
                      "MM/dd/yyyy hh:mm:ss", "M/d/yyyy h:mm:ss",
                      "M/d/yyyy hh:mm tt", "M/d/yyyy hh tt",
                      "M/d/yyyy h:mm", "M/d/yyyy h:mm",
                      "MM/dd/yyyy hh:mm", "M/dd/yyyy hh:mm"};*/
        CultureInfo provider = CultureInfo.InvariantCulture;
        System.Globalization.DateTimeStyles style = DateTimeStyles.None;
        string[] formats = new string[] { "dd/MMM/yyyy h:m:s tt", "dd-MMM-yyyy h:m:s tt" };
        //   [Route(@"CalculateFee/{startDateString}/{endDateString}")]
        [HttpGet]
        public FeeCalucateResponse CalculateFee//([FromUri]string startDateString, [FromUri]string endDateString)
            (string entryTimeString, string exitTimeString)

        {
            FeeCalucateResponse fc = new FeeCalucateResponse();
            try
            {
                DateTime startDate;// = BuildDateTimeFromYAFormat(entryTimeString);
                DateTime endDate;//= BuildDateTimeFromYAFormat(exitTimeString);
                if (!DateTime.TryParse(entryTimeString, out startDate))
                {

                    if (!DateTime.TryParse(entryTimeString, out startDate))
                        fc.ResponseMessage += string.Format("EntryTime  {0} is not a valid  format {1}", entryTimeString, Environment.NewLine);
                }
                if (!DateTime.TryParse(exitTimeString, out endDate))
                {
                    fc.ResponseMessage += string.Format("ExitTime format is not valid . Use one of the following {0} {1}", String.Join(",", formats.Select(o => o.ToString()).ToArray()), Environment.NewLine);
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
                        {
                            if (duration.TotalDays < 3)

                            // ("This is a weekend");
                            {
                                fc.ResponseMessage = " Weekend Rate";
                                fc.TotalAmount = 10;
                                return fc;
                            }
                        }
                    }
                    //2 hour rate
                    if (duration.TotalHours < 2)
                    {
                        fc.ResponseMessage = "Hourly Rate  for 2 hour";
                        fc.TotalAmount = 10;
                        return fc;
                    }

                    TimeSpan startOfExit = new TimeSpan(15, 30, 0); //3:30 PM
                    TimeSpan endofExit = new TimeSpan(23, 30, 0); // 11:30 PM

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
                    // night rate
                    if (startDate.Hour >= 18)// && startDate.Hour <= 9)
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
                    if(duration.TotalHours < 4)
                    {
                        fc.ResponseMessage = "Hourly rate  ";
                        fc.TotalAmount =(float)( Math.Ceiling(duration.TotalHours)*5.0) ;
                        return fc;

                    }
                    else
                    {
                        fc.ResponseMessage = "Daily rate  ";
                        fc.TotalAmount = (float)(Math.Ceiling(duration.TotalDays) * 20.0);
                        return fc;
                    }

                }
                //(DateTime entryTimeStamp, DateTime exitTimeStamp)

                /* DateTime dtEntry = DateTime.Now;
                 DateTime.TryParse(entryTimeString, out dtEntry);
                 DateTime dtExit = DateTime.Now;
                 DateTime.TryParse(exitTimeString, out dtExit);
                 Console.WriteLine(entryTimeString);*/
                /*  DayOfWeek entryDay = entryTimeStamp.DayOfWeek;
                  DayOfWeek exitDay = exitTimeStamp.DayOfWeek;
                  string entrytime = entryTimeStamp.ToShortTimeString();
                  string exittime = exitTimeStamp.ToShortTimeString();
                  */
            }
            catch (Exception ex)
            {
                fc.ResponseMessage = ex.Message;// "The Input date should be in yyyyMMddTHHmmZ format";
            }

            return fc;
            //  return entryTimeStamp + exitTimeStamp;
        }/// <summary>
         /// Convert a UTC Date String of format yyyyMMddThhmmZ into a Local Date
         /// </summary>
         /// <param name="dateString"></param>
         /// <returns></returns>
        private DateTime BuildDateTimeFromYAFormat(string dateString)
        {
            Regex r = new Regex(@"^\d{4}\d{2}\d{2}T\d{2}\d{2}Z$");
            if (!r.IsMatch(dateString))
            {
                throw new FormatException(
                    string.Format("{0} is not the correct format. Should be yyyyMMddTHHmmZ", dateString));
            }

            DateTime dt = DateTime.ParseExact(dateString, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

            return dt;
        }
    }
}
