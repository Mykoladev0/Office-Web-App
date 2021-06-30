using CoreDAL.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
namespace CoreDAL_Tests
{
    public class Utility_Tests
    {
        public Utility_Tests()
        {
            
        }

        [Fact(DisplayName = "Valid Email Addresses should return true")]
        public void BasicEmailValid()
        {
            string email = "test@test.com";
            bool result = Validators.IsValidEmail(email);
            Assert.True(result);
        }
        [Fact(DisplayName = "Email without domain should return false")]
        public void NoEmailDomain()
        {
            string email = "test@test.";
            bool result = Validators.IsValidEmail(email);
            Assert.False(result);
        }

        [Fact(DisplayName = "Email with extended domain should return true")]
        public void BadEmailDomain()
        {
            string email = "test@test.ax.ed.cs.phl";
            bool result = Validators.IsValidEmail(email);
            Assert.True(result);
        }
        [Fact(DisplayName = "Email without @ should return false")]
        public void NoATEmail()
        {
            string email = "test.com";
            bool result = Validators.IsValidEmail(email);
            Assert.False(result);
        }
    }

}
