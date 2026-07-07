using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Xunit;
using task13;

namespace task13tests
{
    public class SerializationTests
    {
        private readonly JsonHelper _jsonHelper;

        public SerializationTests()
        {
            _jsonHelper = new JsonHelper();
        }

        [Fact]
        public void Serialize_ShouldIgnoreNullAndFormatDateCorrectly()
        {
            var student = new Student
            {
                FirstName = "Maksim",
                LastName = "Vlasov",
                BirthDate = new DateTime(2005, 12, 12),
                Grades = null
            };

            string json = _jsonHelper.Serialize(student);

            // Assert
            Assert.DoesNotContain("Grades", json);
            Assert.Contains("2005-12-12", json);
        }

        [Fact]
        public void Deserialize_ShouldThrowValidationException_WhenDataIsInvalid()
        {
            string invalidJson = @"{
                ""LastName"": ""Vlasov"",
                ""BirthDate"": ""2005-12-12"",
                ""Grades"": [ { ""Name"": ""C#"", ""Grade"": 150 } ]
            }";

            var ex = Assert.Throws<ValidationException>(() => _jsonHelper.Deserialize(invalidJson));
            Assert.Contains("Имя обязательно для заполнения", ex.Message);
            Assert.Contains("в диапазоне от 0 до 100", ex.Message);
        }

        [Fact]
        public void SaveAndLoad_ShouldWorkCorrectly()
        {
            var student = new Student
            {
                FirstName = "Maksim",
                LastName = "Vlasov",
                BirthDate = new DateTime(2005, 12, 12),
                Grades = new List<Subject>
                {
                    new Subject { Name = "C# Programming", Grade = 95 }
                }
            };
            string filePath = "test_student.json";

            try
            {
                _jsonHelper.SaveToFile(student, filePath);
                var loadedStudent = _jsonHelper.LoadFromFile(filePath);

                Assert.Equal("Maksim", loadedStudent.FirstName);
                Assert.Single(loadedStudent.Grades);
                Assert.Equal(95, loadedStudent.Grades[0].Grade);
            }
            finally
            {
                if (File.Exists(filePath)) File.Delete(filePath);
            }
        }
    }
}