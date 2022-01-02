using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aurum.Core.UnitTests
{
    public class HtmlTokenReplacerTests
    {
        [Fact]
        public void TokenReplacer_ShouldThrow_IfInputStreamCannotRead()
        {
            // Arrange
            var replacer = new HtmlTokenReplacer();
            var outputStream = new MemoryStream();
            var inputStream = new WriteOnlyStream();

            // Act
            var action = () => replacer.ReplaceTokens(inputStream, outputStream, new Dictionary<string, string>());

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void TokenReplacer_ShouldThrow_IfOutputStreamCannotWrite()
        {
            // Arrange
            var replacer = new HtmlTokenReplacer();
            var outputStream = new MemoryStream(Array.Empty<byte>(), false);
            var inputStream = new MemoryStream();

            // Act
            var action = () => replacer.ReplaceTokens(inputStream, outputStream, new Dictionary<string, string>());

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [Theory]
        [ClassData(typeof(HtmlInvalidTestMarkups))]
        public void TokenReplacer_ShouldThrow_IfStreamDoesNotContainHtmlMarkup(string html)
        {
            // Arrange
            var replacer = new HtmlTokenReplacer();
            var outputStream = new MemoryStream();
            var inputStream = new MemoryStream();
            var streamWriter = new StreamWriter(inputStream);
            streamWriter.Write(html);
            streamWriter.Flush();
            inputStream.Seek(0, SeekOrigin.Begin);

            // Act
            var action = () => replacer.ReplaceTokens(inputStream, outputStream, new Dictionary<string, string>());

            // Assert
            action.Should().Throw<Exception>();
        }

        [Fact]
        public void TokenReplacer_ShouldNotReplace_IfTokensAreEmpty()
        {
            // Arrange
            var replacer = new HtmlTokenReplacer();
            var outputStream = new MemoryStream();
            var inputStream = new MemoryStream();
            var streamWriter = new StreamWriter(inputStream);
            streamWriter.Write("<html><body>{{Token}}</body></html>");
            streamWriter.Flush();
            inputStream.Seek(0, SeekOrigin.Begin);

            // Act
            replacer.ReplaceTokens(inputStream, outputStream, new Dictionary<string,string>());

            // Assert
            var streamReader = new StreamReader(outputStream);
            var result = streamReader.ReadToEnd();

            result.Should().Be("<html><body>{{Token}}</body></html>");
        }

        [Fact]
        public void TokenReplacer_ShouldReplaceTokens()
        {
            // Arrange
            var replacer = new HtmlTokenReplacer();
            var outputStream = new MemoryStream();
            var inputStream = new MemoryStream();
            var streamWriter = new StreamWriter(inputStream);
            streamWriter.Write("<html><body>{{Token}}</body></html>");
            streamWriter.Flush();
            inputStream.Seek(0, SeekOrigin.Begin);

            // Act
            replacer.ReplaceTokens(inputStream, outputStream, new Dictionary<string, string>
            {
                {"{{Token}}", "Result!"}
            });

            // Assert
            var streamReader = new StreamReader(outputStream);
            var result = streamReader.ReadToEnd();

            result.Should().Be("<html><body>Result!</body></html>");
        }

        [Fact]
        public void TokenReplacer_ShouldReplaceTokensContaining()
        {
            // Arrange
            var replacer = new HtmlTokenReplacer();
            var outputStream = new MemoryStream();
            var inputStream = new MemoryStream();
            var streamWriter = new StreamWriter(inputStream);
            streamWriter.Write("<html><body>Outcome is {{Token}}</body></html>");
            streamWriter.Flush();
            inputStream.Seek(0, SeekOrigin.Begin);

            // Act
            replacer.ReplaceTokens(inputStream, outputStream, new Dictionary<string, string>
            {
                {"{{Token}}", "success!"}
            });


            // Assert
            var streamReader = new StreamReader(outputStream);
            var result = streamReader.ReadToEnd();

            result.Should().Be("<html><body>Outcome is success!</body></html>");
        }


        private class WriteOnlyStream : Stream
        {
            public override bool CanRead => false;

            public override bool CanSeek => true;

            public override bool CanWrite => true;

            public override long Length => 0L;

            public override long Position { get; set; }

            public override void Flush()
            {
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return 0;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return 0L;
            }

            public override void SetLength(long value)
            {
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
            }
        }
    }
}
