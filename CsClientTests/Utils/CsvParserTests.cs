using CsClient.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CsClientTests.Utils
{
    /// <summary>
    /// Tests the <see cref="CsvParser"/> functions work as expected.
    /// </summary>
    [TestClass]
    public class CsvParserTests : BaseTest
    {
        /// <summary>
        /// Tests that the csv parser correctly parses. <see cref="CsvParser.GetStringValueWithHeader(string, string[])"/>.
        /// </summary>
        [TestMethod]
        public void TestCsvParserStringFromHeader_Exists_ReturnsValue()
        {
            // Arrange
            string header1 = "header";
            string header2 = "any";
            string header3 = "ok";
            string text = $"{header1},{header2},{header3}";

            // Act
            CsvParser csvParser = new CsvParser(text);
            string[] parts = text.Split(','); 
            string value1 = csvParser.GetStringValueWithHeader(header1, parts);
            string value2 = csvParser.GetStringValueWithHeader(header2, parts);
            string value3 = csvParser.GetStringValueWithHeader(header3, parts);

            // Assertions
            Assert.AreEqual(header1, value1);
            Assert.AreEqual(header2, value2);
            Assert.AreEqual(header3, value3);
        }

        /// <summary>
        /// Tests that the csv parser correctly parses. <see cref="CsvParser.GetStringValueWithHeader(string, string[])"/>.
        /// </summary>
        [TestMethod]
        public void TestCsvParserStringFromHeader_Missing_ReturnsValue()
        {
            // Arrange
            string header1 = "header";
            string header2Missing = "missing";
            string text = $"{header1}";

            // Act
            CsvParser csvParser = new CsvParser(text);
            string[] parts = text.Split(',');
            string value = csvParser.GetStringValueWithHeader(header2Missing, parts);

            // Assertions
            Assert.IsNull(value);
        }

        /// <summary>
        /// Tests that the csv <see cref="CsvParser.GetCsvParts(string)"/> does get the csv parts correctly in normal format.
        /// </summary>
        [TestMethod]
        public void TestCsvParserGetParts_ReturnsParts()
        {
            // Arrange
            string part1 = "any";
            string part2 = "more";
            string part3 = "extra";
            string part4 = "";
            string csv = $"{part1},{part2},{part3},{part4}";

            // Act
            CsvParser csvParser = new CsvParser("");
            string[] parts = csvParser.GetCsvParts(csv);

            // Assertions
            Assert.AreEqual(3, parts.Length);
            Assert.AreEqual(part1, parts[0]);
            Assert.AreEqual(part2, parts[1]);
            Assert.AreEqual(part3, parts[2]);
            Assert.AreEqual(part4, parts[3]);
        }
    }
}
