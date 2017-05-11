// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Testing;
using Xunit;

namespace Microsoft.AspNetCore.Http
{
    public class PathStringTests
    {
        [Fact]
        public void CtorThrows_IfPathDoesNotHaveLeadingSlash()
        {
            // Act and Assert
            ExceptionAssert.ThrowsArgument(() => new PathString("hello"), "value", "The path in 'value' must start with '/'.");
        }

        [Fact]
        public void Equals_EmptyPathStringAndDefaultPathString()
        {
            // Act and Assert
            Assert.Equal(PathString.Empty, default(PathString));
            Assert.Equal(default(PathString), PathString.Empty);
            Assert.True(PathString.Empty == default(PathString));
            Assert.True(default(PathString) == PathString.Empty);
            Assert.True(PathString.Empty.Equals(default(PathString)));
            Assert.True(default(PathString).Equals(PathString.Empty));
        }

        [Fact]
        public void NotEquals_DefaultPathStringAndNonNullPathString()
        {
            // Arrange
            var pathString = new PathString("/hello");

            // Act and Assert
            Assert.NotEqual(pathString, default(PathString));
        }

        [Fact]
        public void NotEquals_EmptyPathStringAndNonNullPathString()
        {
            // Arrange
            var pathString = new PathString("/hello");

            // Act and Assert
            Assert.NotEqual(pathString, PathString.Empty);
        }

        [Fact]
        public void HashCode_CheckNullAndEmptyHaveSameHashcodes()
        {
            Assert.Equal(PathString.Empty.GetHashCode(), default(PathString).GetHashCode());
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        public void AddPathString_HandlesNullAndEmptyStrings(string appString, string concatString)
        {
            // Arrange
            var appPath = new PathString(appString);
            var concatPath = new PathString(concatString);

            // Act
            var result = appPath.Add(concatPath);

            // Assert
            Assert.False(result.HasValue);
        }

        [Theory]
        [InlineData("", "/", "/")]
        [InlineData("/", null, "/")]
        [InlineData("/", "", "/")]
        [InlineData("/", "/test", "/test")]
        [InlineData("/myapp/", "/test/bar", "/myapp/test/bar")]
        [InlineData("/myapp/", "/test/bar/", "/myapp/test/bar/")]
        public void AddPathString_HandlesLeadingAndTrailingSlashes(string appString, string concatString, string expected)
        {
            // Arrange
            var appPath = new PathString(appString);
            var concatPath = new PathString(concatString);

            // Act
            var result = appPath.Add(concatPath);

            // Assert
            Assert.Equal(expected, result.Value);
        }

        [Fact]
        public void ImplicitStringConverters_WorksWithAdd()
        {
            var scheme = "http";
            var host = new HostString("localhost:80");
            var pathBase = new PathString("/base");
            var path = new PathString("/path");
            var query = new QueryString("?query");
            var fragment = new FragmentString("#frag");

            var result = scheme + "://" + host + pathBase + path + query + fragment;
            Assert.Equal("http://localhost:80/base/path?query#frag", result);

            result = pathBase + path + query + fragment;
            Assert.Equal("/base/path?query#frag", result);

            result = path + "text";
            Assert.Equal("/pathtext", result);
        }

        [Theory]
        [InlineData("/test/path", "/TEST", true)]
        [InlineData("/test/path", "/TEST/pa", false)]
        [InlineData("/TEST/PATH", "/test", true)]
        [InlineData("/TEST/path", "/test/pa", false)]
        [InlineData("/test/PATH/path/TEST", "/TEST/path/PATH", true)]
        public void StartsWithSegments_DoesACaseInsensitiveMatch(string sourcePath, string testPath, bool expectedResult)
        {
            var source = new PathString(sourcePath);
            var test = new PathString(testPath);

            var result = source.StartsWithSegments(test);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("/test/path", "/TEST", true)]
        [InlineData("/test/path", "/TEST/pa", false)]
        [InlineData("/TEST/PATH", "/test", true)]
        [InlineData("/TEST/path", "/test/pa", false)]
        [InlineData("/test/PATH/path/TEST", "/TEST/path/PATH", true)]
        public void StartsWithSegmentsWithRemainder_DoesACaseInsensitiveMatch(string sourcePath, string testPath, bool expectedResult)
        {
            var source = new PathString(sourcePath);
            var test = new PathString(testPath);

            PathString remaining;
            var result = source.StartsWithSegments(test, out remaining);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("/test/path", "/TEST", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("/test/path", "/TEST", StringComparison.Ordinal, false)]
        [InlineData("/test/path", "/TEST/pa", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("/test/path", "/TEST/pa", StringComparison.Ordinal, false)]
        [InlineData("/TEST/PATH", "/test", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("/TEST/PATH", "/test", StringComparison.Ordinal, false)]
        [InlineData("/TEST/path", "/test/pa", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("/TEST/path", "/test/pa", StringComparison.Ordinal, false)]
        [InlineData("/test/PATH/path/TEST", "/TEST/path/PATH", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("/test/PATH/path/TEST", "/TEST/path/PATH", StringComparison.Ordinal, false)]
        public void StartsWithSegments_DoesMatchUsingSpecifiedComparison(string sourcePath, string testPath, StringComparison comparison, bool expectedResult)
        {
            var source = new PathString(sourcePath);
            var test = new PathString(testPath);

            var result = source.StartsWithSegments(test, comparison);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("/test/path", "/TEST", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("/test/path", "/TEST", StringComparison.Ordinal, false)]
        [InlineData("/test/path", "/TEST/pa", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("/test/path", "/TEST/pa", StringComparison.Ordinal, false)]
        [InlineData("/TEST/PATH", "/test", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("/TEST/PATH", "/test", StringComparison.Ordinal, false)]
        [InlineData("/TEST/path", "/test/pa", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("/TEST/path", "/test/pa", StringComparison.Ordinal, false)]
        [InlineData("/test/PATH/path/TEST", "/TEST/path/PATH", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("/test/PATH/path/TEST", "/TEST/path/PATH", StringComparison.Ordinal, false)]
        public void StartsWithSegmentsWithRemainder_DoesMatchUsingSpecifiedComparison(string sourcePath, string testPath, StringComparison comparison, bool expectedResult)
        {
            var source = new PathString(sourcePath);
            var test = new PathString(testPath);

            PathString remaining;
            var result = source.StartsWithSegments(test, comparison, out remaining);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("unreserved", "/abc123.-_~", "/abc123.-_~")]
        [InlineData("colon", "/:", "/:")]
        [InlineData("at", "/@", "/@")]
        [InlineData("sub-delims", "/!$&'()*+,;=", "/!$&'()*+,;=")]
        [InlineData("reserved", "/?#[]", "/%3F%23%5B%5D")]
        [InlineData("pct-encoding", "/单行道", "/%E5%8D%95%E8%A1%8C%E9%81%93")]
        [InlineData("mixed1", "/index/单行道=(x*y)[abc]", "/index/%E5%8D%95%E8%A1%8C%E9%81%93=(x*y)%5Babc%5D")]
        [InlineData("mixed2", "/index/单行道=(x*y)[abc]_", "/index/%E5%8D%95%E8%A1%8C%E9%81%93=(x*y)%5Babc%5D_")]
        [InlineData("encoded", "/http%3a%2f%2f[foo]%3A5000/", "/http%3a%2f%2f%5Bfoo%5D%3A5000/")]
        [InlineData("encoded", "/http%3a%2f%2f[foo]%3A5000/%", "/http%3a%2f%2f%5Bfoo%5D%3A5000/%25")]
        [InlineData("encoded", "/http%3a%2f%2f[foo]%3A5000/%2", "/http%3a%2f%2f%5Bfoo%5D%3A5000/%252")]
        [InlineData("encoded", "/http%3a%2f%2f[foo]%3A5000/%2F", "/http%3a%2f%2f%5Bfoo%5D%3A5000/%2F")]
        public void ToUriComponentEscapeCorrectly(string category, string input, string expected)
        {
            var path = new PathString(input);

            Assert.Equal(expected, path.ToUriComponent());
        }
    }
}
