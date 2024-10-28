﻿using LangLang.Controller;
using System;
using System.Collections.Generic;
using LangLang.Domain.IRepository;
using LangLang.Repository;

namespace LangLang.Domain.Model
{
    public static class Injector
    {
        private static Dictionary<Type, Lazy<object>> _implementations = new Dictionary<Type, Lazy<object>>
        {
            { typeof(IStudentRepository), new Lazy<object>(() => new StudentRepository()) },
            { typeof(StudentsController), new Lazy<object>(() => new StudentsController()) },
            { typeof(ICourseRepository), new Lazy<object>(() => new CourseRepository()) },
            { typeof(CourseController), new Lazy<object>(() => new CourseController()) },
            { typeof(IExamTermRepository), new Lazy<object>(() => new ExamTermRepository()) },
            { typeof(ExamTermController), new Lazy<object>(() => new ExamTermController()) },
            { typeof(ITeacherRepository), new Lazy<object>(() => new TeacherRepository()) },
            { typeof(TeacherController), new Lazy<object>(() => new TeacherController()) },
            { typeof(IDirectorRepository), new Lazy<object>(() => new DirectorRepository()) },
            { typeof(DirectorController), new Lazy<object>(() => new DirectorController()) },
            { typeof(IMailRepository), new Lazy<object>(() => new MailRepository()) },
            { typeof(MailController), new Lazy<object>(() => new MailController()) },
            { typeof(ICourseGradeRepository), new Lazy<object>(() => new CourseGradeRepository()) },
            { typeof(CourseGradeController), new Lazy<object>(() => new CourseGradeController()) },
            { typeof(IExamTermGradeRepository), new Lazy<object>(() => new ExamTermGradeRepository()) },
            { typeof(ExamTermGradeController), new Lazy<object>(() => new ExamTermGradeController()) },
            { typeof(IStudentGradeRepository), new Lazy<object>(() => new StudentGradeRepository()) },
        };

        static Injector()
        {
            Data.AppDbContext appDbContext = new Data.AppDbContext();   
            _implementations.Add(typeof(IPenaltyPointRepository), new Lazy<object>(() => new PenaltyPointRepository()));
            _implementations.Add(typeof(PenaltyPointController), new Lazy<object>(() => new PenaltyPointController()));
            _implementations.Add(typeof(ReportController), new Lazy<object>(() => new ReportController()));
            _implementations.Add(typeof(IExamTermDbRepository), new Lazy<object>(() => new ExamTermDbRepository(appDbContext))); //
            _implementations.Add(typeof(ICourseDbRepository), new Lazy<object>(() => new CourseDbRepository(appDbContext)));     //
            _implementations.Add(typeof(IDirectorDbRepository), new Lazy<object>(() => new DirectorDbRepository(appDbContext)));

        }

        public static T CreateInstance<T>()
        {
            Type type = typeof(T);

            if (_implementations.ContainsKey(type))
            {
                return (T)_implementations[type].Value;
            }

            throw new ArgumentException($"No implementation found for type {type}");
        }
    }
}
