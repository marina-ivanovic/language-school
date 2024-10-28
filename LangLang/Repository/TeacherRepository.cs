﻿using System.Collections.Generic;
using System.Linq;
using LangLang.Domain.Model;
using LangLang.Observer;
using LangLang.Storage;
using LangLang.Domain.IRepository;

namespace LangLang.Repository
{
    public class TeacherRepository : Subject, ITeacherRepository
    {
        private readonly List<Course> _courses;
        private readonly Storage<Course> _courseStorage;
        private readonly List<ExamTerm> _examTerms;
        private readonly Storage<ExamTerm> _examTermsStorage;
        private readonly List<Mail> _mails;
        private readonly Storage<Mail> _mailsStorage;

        public TeacherRepository()
        {
            _courseStorage = new Storage<Course>("course.csv");
            _courses = _courseStorage.Load();
            _examTermsStorage = new Storage<ExamTerm>("exam.csv");
            _examTerms = _examTermsStorage.Load();
            _mailsStorage = new Storage<Mail>("mails.csv");
            _mails = _mailsStorage.Load();
        }
        private Course? GetCourseById(int id)
        {
            return _courses.Find(v => v.Id == id);
        }
        public List<Course> GetAllCourses()
        {
            return _courses;
        }

        public List<ExamTerm> GetAllExamTerms()
        {
            return _examTerms;
        }
        
    }
}
