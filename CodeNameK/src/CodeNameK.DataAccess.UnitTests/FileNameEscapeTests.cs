using Xunit;

namespace CodeNameK.DataAccess.UnitTests
{
    public class FileNameEscapeTests
    {
        [Theory]
        [InlineData("abc%99", "abc%002599")]
        [InlineData("abc\\cde", "abc%005Ccde")]
        public void ShouldEscapeSpecialCharacter(string category, string expectedFolderName)
        {
            FileNameEscaper target = new FileNameEscaper();
            string actual = target.Escape(category);

            Assert.Equal(expectedFolderName, actual);
        }


        [Theory]
        [InlineData("abc%002599", "abc%99")]
        [InlineData("abc%005Ccde", "abc\\cde")]
        public void ShouldUnescapeSpecialCharacter(string folderName, string expectedCategory)
        {
            FileNameEscaper target = new FileNameEscaper();
            string actual = target.Unescape(folderName);

            Assert.Equal(expectedCategory, actual);
        }
    }
}