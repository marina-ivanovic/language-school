﻿using LangLang.Domain.IUtility;
using LangLang.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Domain.Utility
{
    public class SortByDatetime : ISortStrategy
    {
        public IEnumerable<ExamTerm> Sort(IEnumerable<ExamTerm> exams)
        {
            return exams.OrderBy(x => x.ExamTime);
        }
        public IEnumerable<Course> Sort(IEnumerable<Course> exams)
        {
            return exams.OrderBy(x => x.StartDate);
        }
        public IEnumerable<Teacher> Sort(IEnumerable<Teacher> exams)
        {
            return exams.OrderBy(x => x.StartedWork);
        }
    }
}
