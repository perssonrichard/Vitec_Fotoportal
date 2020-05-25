using System;
using Photobox.Helpers;
using Xunit;

namespace Photobox.Tests {
    public class PasswordHelperTests {
        private PasswordHelper passwordHelper;

        public PasswordHelperTests () {
            passwordHelper = new PasswordHelper ();
        }

        [Theory]
        [InlineData ("password")]
        [InlineData ("12312138645689")]
        [InlineData ("#¤%&()/(&%¤¤#")]
        [InlineData ("ThisIsALittleLongerPasswordWithBothLettersSymbolsAndNumbers123%¤%%#¤120")]
        public void HashPasswordShouldNotReturnSameValue (string value) {
            var actual = passwordHelper.HashPassword (value);
            Assert.NotEqual (value, actual);
        }

        [Fact]
        public void HashPasswordShouldThrowExceptionOnSpace () {
            var input = "Password with spaces";

            Assert.Throws<ArgumentException> (() => passwordHelper.HashPassword (input));
        }

        [Fact]
        public void HashPasswordShouldThrowExceptionOnNull () {
            Assert.Throws<ArgumentNullException> (() => passwordHelper.HashPassword (null));
        }

        [Fact]
        public void HashPasswordShouldThrowExceptionOnEmpty () {
            Assert.Throws<ArgumentNullException> (() => passwordHelper.HashPassword (""));
        }

        [Fact]
        public void HashPasswordShouldThrowExceptionOnWhitespace () {
            Assert.Throws<ArgumentNullException> (() => passwordHelper.HashPassword (" "));
        }
    }
}