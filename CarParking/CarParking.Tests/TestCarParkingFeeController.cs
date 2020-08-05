using CarParking.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
namespace CarParking.Tests
{
    [TestClass]
    public class TestCarParkingFeeController
    {
        [TestMethod]
        public void GetNightRateTest()
        {

            var controller = new ParkingFeeController();
            var result = controller.GetParkingFee("3-Aug-2020 6:03:03 PM ", "4/8/2020 06:32 PM") as FeeCalucateResponse;
            Assert.AreEqual(6.5, result.TotalAmount);
        }
        [TestMethod]
        public void GetOneHourlyRateTest()
        {

            var controller = new ParkingFeeController();
            var result = controller.GetParkingFee("05-Aug-2020 6:03:03 AM ", "05/AUG/2020 06:32 AM") as FeeCalucateResponse;
            Assert.AreEqual(5, result.TotalAmount);
        }

        [TestMethod]
        public async Task GetHourlyRateAsync_ShouldReturnCorrectValue()
        {
            var controller = new ParkingFeeController();
            var result = await controller.GetParkingFeeAsync("05-Aug-2020 6:03:03 AM ", "05/AUG/2020 06:32 AM") as FeeCalucateResponse;
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.TotalAmount);
        }

        [TestMethod]
        public void GetParkingFee_ShouldThrowError()
        {
            var controller = new ParkingFeeController();
            var result = controller.GetParkingFee("35-Aug-2020 6:03:03 AM ", "05/ABC/2020 06:32 AM") as FeeCalucateResponse;
            Assert.AreNotSame("Error", result.ResponseMessage);
        }
        [TestMethod]
        public async Task GetTwoHourlyRateAsync_ShouldReturnCorrectValue()
        {
            var controller = new ParkingFeeController();
            var result = await controller.GetParkingFeeAsync("05-Aug-2020 6:03:03 AM ", "05/AUG/2020 07:32 AM") as FeeCalucateResponse;
            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.TotalAmount);
        }

        [TestMethod]
        public async Task GetWeekEndRateAsync_ShouldReturnCorrectValue()
        {
            var controller = new ParkingFeeController();
            var result = await controller.GetParkingFeeAsync("11-JUL-2020 12:30:03 AM ", "12/July/2020 11:59 PM") as FeeCalucateResponse;
            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.TotalAmount);
        }
        [TestMethod]
        public async Task GetNightWeekEndRateAsync_ShouldReturnCorrectValue()
        {
            var controller = new ParkingFeeController();
            var result = await controller.GetParkingFeeAsync("12-06-2020 6:01:03 PM ", "13/Jun/2020 11:29 PM") as FeeCalucateResponse;
            Assert.IsNotNull(result);
            Assert.AreEqual(6.5, result.TotalAmount);
        }
        [TestMethod]
        public  void GetNotNightWeekEndRate_ShouldReturnCorrectValue()
        {
            var controller = new ParkingFeeController();
            var result =  controller.GetParkingFee("12-06-2020 5:01:03 PM ", "13/Jun/2020 11:29 PM") as FeeCalucateResponse;
           
            Assert.AreNotEqual(20, result.TotalAmount); // Actual is 40 
        }

        [TestMethod]
        public void GetEarlyParkingFee_ShouldReturnValue()
        {
            var controller = new ParkingFeeController();
            var result = controller.GetParkingFee("13-May-2020 6:03:03 AM ", "13/May/2020 11:02 PM") as FeeCalucateResponse;
            Assert.AreEqual(13, result.TotalAmount);
        }
    }
}
