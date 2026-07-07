using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13
{
    public class JsonHelper
    {
        private readonly JsonSerializerOptions _options;

        public JsonHelper()
        {
            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new CustomDateConverter() }
            };
        }

        public string Serialize(Student student)
        {
            return JsonSerializer.Serialize(student, _options);
        }

        public Student Deserialize(string json)
        {
            var student = JsonSerializer.Deserialize<Student>(json, _options);
            if (student == null)
            {
                throw new JsonException("Не удалось десериализовать JSON в объект Student.");
            }
            Validate(student);
            return student;
        }

        public void SaveToFile(Student student, string filePath)
        {
            string json = Serialize(student);
            File.WriteAllText(filePath, json);
        }

        public Student LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл {filePath} не найден.");

            string json = File.ReadAllText(filePath);
            return Deserialize(json);
        }

        private void Validate(Student student)
        {
            var context = new ValidationContext(student, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            
            bool isValid = Validator.TryValidateObject(student, context, results, validateAllProperties: true);

            if (student.Grades != null)
            {
                foreach (var grade in student.Grades)
                {
                    var gradeContext = new ValidationContext(grade, serviceProvider: null, items: null);
                    Validator.TryValidateObject(grade, gradeContext, results, validateAllProperties: true);
                }
            }

            if (results.Count > 0)
            {
                var errors = string.Join("; ", results.ConvertAll(r => r.ErrorMessage));
                throw new ValidationException($"Ошибка валидации данных: {errors}");
            }
        }
    }
}