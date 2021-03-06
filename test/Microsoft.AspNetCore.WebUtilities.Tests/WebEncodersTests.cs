// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Xunit;

namespace Microsoft.AspNetCore.WebUtilities
{
    public class WebEncodersTests
    {
        [Theory]
        [InlineData("", 1, 0)]
        [InlineData("", 0, 1)]
        [InlineData("0123456789", 9, 2)]
        [InlineData("0123456789", Int32.MaxValue, 2)]
        [InlineData("0123456789", 9, -1)]
        public void Base64UrlDecode_BadOffsets(string input, int offset, int count)
        {
            // Act & assert
            Assert.ThrowsAny<ArgumentException>(() =>
            {
                var retVal = WebEncoders.Base64UrlDecode(input, offset, count);
            });
        }

        [Theory]
        [InlineData(0, 1, 0)]
        [InlineData(0, 0, 1)]
        [InlineData(10, 9, 2)]
        [InlineData(10, Int32.MaxValue, 2)]
        [InlineData(10, 9, -1)]
        public void Base64UrlEncode_BadOffsets(int inputLength, int offset, int count)
        {
            // Arrange
            byte[] input = new byte[inputLength];

            // Act & assert
            Assert.ThrowsAny<ArgumentException>(() =>
            {
                var retVal = WebEncoders.Base64UrlEncode(input, offset, count);
            });
        }
    }
}
