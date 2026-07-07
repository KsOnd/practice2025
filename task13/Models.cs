using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace task13
{
    public class Subject
    {
        [Required(ErrorMessage = "Название предмета не может быть пустым.")]
        public string Name { get; set; } = default!;

        [Range(0, 100, ErrorMessage = "Оценка должна быть в диапазоне от 0 до 100.")]
        public int Grade { get; set; }
    }

    public class Student
    {
        [Required(ErrorMessage = "Имя обязательно для заполнения.")]
        public string FirstName { get; set; } = default!;

        [Required(ErrorMessage = "Фамилия обязательна для заполнения.")]
        public string LastName { get; set; } = default!;

        public DateTime BirthDate { get; set; }

        public List<Subject>? Grades { get; set; }
    }
}