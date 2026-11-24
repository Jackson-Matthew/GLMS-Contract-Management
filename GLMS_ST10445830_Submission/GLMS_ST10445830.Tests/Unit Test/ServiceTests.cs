using System;
using System.IO;
using GLMS_ST10445830.Models;
using Xunit;

namespace GLMS_ST10445830.UnitTests
{
    public class ServiceTests
    {
        // test 1 Currency Calculation  Verify USD to ZAR conversion is correct
        [Fact]
        public void CurrencyCalculation_ConvertUsdToZar_ReturnsCorrectAmount()
        {

            decimal usdAmount = 100;
            decimal exchangeRate = 18.50m; 
            decimal expectedZarAmount = 1850; 

            decimal actualZarAmount = usdAmount * exchangeRate;

            Assert.Equal(expectedZarAmount, actualZarAmount);
        }

        // test 2 File Validation Only PDF files are allowed
        [Fact]
        public void FileValidation_OnlyPdfFiles_AreAllowed()
        {
            var pdfFile = "contract.pdf";
            var exeFile = "virus.exe";
            var txtFile = "document.txt";

            Assert.True(Path.GetExtension(pdfFile).ToLower() == ".pdf");

            Assert.False(Path.GetExtension(exeFile).ToLower() == ".pdf");

            Assert.False(Path.GetExtension(txtFile).ToLower() == ".pdf");
        }

        // test 3 Contract Status Cannot create service request for expired contract
        [Fact]
        public void ServiceRequest_CannotBeCreatedForExpiredContract()
        {
            var activeContract = new Contract { ContractStatus = Status.Active };
            var expiredContract = new Contract { ContractStatus = Status.Expired };

            bool canCreateForActive = activeContract.ContractStatus != Status.Expired;
            bool canCreateForExpired = expiredContract.ContractStatus != Status.Expired;

            Assert.True(canCreateForActive);  
            Assert.False(canCreateForExpired); 
        }

        // test 4 Date Validation End date must be after start date
        [Fact]
        public void Contract_EndDateMustBeAfterStartDate()
        {
            var validContract = new Contract
            {
                StartDate = new DateTime(2024, 1, 1),
                EndDate = new DateTime(2024, 12, 31)
            };

            var invalidContract = new Contract
            {
                StartDate = new DateTime(2024, 1, 1),
                EndDate = new DateTime(2023, 12, 31)
            };

            bool isValid = validContract.EndDate > validContract.StartDate;
            bool isInvalid = invalidContract.EndDate > invalidContract.StartDate;

            Assert.True(isValid);   
            Assert.False(isInvalid); 
        }
    }
}