using LangLang.Controller;
using LangLang.DTO;
using LangLang.Domain.Model.Enums;
using LangLang.Domain.Model;
using LangLang.Observer;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Windows;
using LangLang.View.Teacher;
using System.Windows.Controls;
using LangLang.Domain.Model.Reports;
using LangLang.Domain.IUtility;
using LangLang.Domain.Utility;

namespace LangLang.View.Director
{
    public partial class DirectorPage : Window, IObserver
    {
        public ObservableCollection<TeacherDTO>? Teachers { get; set; }
        public ObservableCollection<CourseDTO> CoursesDirector { get; set; }

        public class ViewModel
        {
            public ObservableCollection<TeacherDTO> Teachers { get; set; }
            public ObservableCollection<CourseDTO> Courses { get; set; }
            public ObservableCollection<CourseDTO> CoursesDirector { get; set; }
            public ObservableCollection<ExamTermDTO> ExamTermsDirector { get; set; }
            public ObservableCollection<ExamTermDTO> GradedExamTerms { get; set; }

            public ViewModel()
            {
                Teachers = new ObservableCollection<TeacherDTO>();
                Courses = new ObservableCollection<CourseDTO>();
                CoursesDirector = new ObservableCollection<CourseDTO>();
                ExamTermsDirector = new ObservableCollection<ExamTermDTO>();
                GradedExamTerms = new ObservableCollection<ExamTermDTO>();
            }
        }
        private readonly int directorId;
        private readonly DirectorController _directorController;
        private readonly TeacherController _teacherController;
        private readonly ReportController _reportController;
        private readonly ExamTermController _examTermController;
        private readonly CourseController _courseController;
        private readonly ExamTermGradeController _examTermGradeController;
        private readonly StudentsController _studentController;

        Domain.Model.Director director;

        public TeacherDTO? SelectedTeacher { get; set; }
        public CourseDTO SelectedCourse { get; set; }
        public CourseDTO SelectedCourseDirector { get; set; }
        public ExamTermDTO SelectedExamTerm { get; set; }
        public ViewModel TableViewModel { get; set; }

        private bool isSearchButtonClicked = false;
        private int selectedTabIndex = 0;
        private int currentTeacherPage = 1;
        private string teacherSortCriteria;
        private ISortStrategy sortStrategy = new SortByFirstName();

        public DirectorPage(int directorId)
        {
            InitializeComponent();
            this.directorId = directorId;
            _directorController = Injector.CreateInstance<DirectorController>();
            _teacherController = Injector.CreateInstance<TeacherController>();
            _reportController = Injector.CreateInstance<ReportController>();
            _examTermController = Injector.CreateInstance<ExamTermController>();
            _courseController = Injector.CreateInstance<CourseController>();
            _studentController = Injector.CreateInstance<StudentsController>();
            _examTermGradeController = Injector.CreateInstance<ExamTermGradeController>();

            TableViewModel = new ViewModel();
            DataContext = this;
            _directorController.Subscribe(this);
            this.director = _directorController.GetDirector();
            Domain.Model.Director director = _directorController.GetDirector();
            firstAndLastName.Text = director.FirstName + " " + director.LastName;

            languageComboBox.ItemsSource = Enum.GetValues(typeof(Language));
            levelComboBox.ItemsSource = Enum.GetValues(typeof(LanguageLevel));

            Update();
            UpdateCourses();
            UpdatePagination();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.Show();
            this.Close();
        }

        public void Update()
        {
            try
            {
                SetTeachers();
                SetCourses();
                SetExamTerms();
                SetDirectorExamTerms();
                UpdatePagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void SetTeachers()
        {
            TableViewModel.Teachers.Clear();
            var teachers = _directorController.GetAllTeachers();

            if (teachers != null)
            {
                foreach (Domain.Model.Teacher teacher in teachers)
                    TableViewModel.Teachers.Add(new TeacherDTO(teacher));
            }
        }

        private void SetCourses()
        {
            TableViewModel.CoursesDirector.Clear();
            var coursesId = director.CoursesId;
            var courses = _teacherController.GetAllCourses();
            if (coursesId != null)
            {
                foreach (Course course in courses)
                {
                    if (coursesId.Contains(course.Id))
                    {
                        CourseDTO courseViewModel = new CourseDTO(course);
                        Domain.Model.Teacher? courseTeacher = _directorController.GetTeacherByCourse(course.Id);
                        if (courseTeacher != null)
                            courseViewModel.HasTeacher = true;
                        else
                            courseViewModel.HasTeacher = false;
                        TableViewModel.CoursesDirector.Add(courseViewModel);
                        foreach (int examTermId in director.ExamsId)
                            TableViewModel.ExamTermsDirector.Add(new ExamTermDTO(_examTermController.GetById(examTermId)));
                    }
                }
            }
        }
        private void SetDirectorExamTerms()
        {
            TableViewModel.ExamTermsDirector.Clear();
            var examsId = director.ExamsId;
            var exams = _examTermController.GetAllExamTerms();

            if (examsId != null)
            {
                foreach (ExamTerm exam in exams)
                    if (examsId.Contains(exam.ExamID))
                        TableViewModel.ExamTermsDirector.Add(new ExamTermDTO(exam));
            }
        }

        private void SetExamTerms()
        {
            TableViewModel.GradedExamTerms.Clear();
            var exams = _examTermController.GetAllExamTerms();

            foreach (ExamTerm exam in exams)
            {
                if (HasExamTermBeenGraded(exam))
                    TableViewModel.GradedExamTerms.Add(new ExamTermDTO(exam));
            }
        }

        public bool HasExamTermBeenGraded(ExamTerm examTerm)
        {
            var grades = _examTermGradeController.GetExamTermGradeByExam(examTerm.ExamID);
            var examTermStudents = _studentController.GetAllStudentsForExamTerm(examTerm.ExamID);

            if (grades.Count == 0)
                return false;

            if (examTermStudents.Count != grades.Count)
                return false;

            return true;
        }

        public void UpdateSearch()
        {
            try
            {
                TableViewModel.Teachers.Clear();
                List<Domain.Model.Teacher> teachers = GetFilteredTeachers();

                if (teachers != null)
                    foreach (Domain.Model.Teacher teacher in teachers)
                        TableViewModel.Teachers.Add(new TeacherDTO(teacher));
                else
                    MessageBox.Show("No courses found.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void CreateTeacher_Click(object sender, RoutedEventArgs e)
        {
            CreateTeacherFrom createTeacherFrom = new CreateTeacherFrom();
            createTeacherFrom.Show();
            Update();
        }

        private void UpdateTeacher_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTeacher == null)
                MessageBox.Show("Please choose a teacher to update!");
            else
            {
                UpdateTeacherForm updateTeacherForm = new UpdateTeacherForm(SelectedTeacher.Id);
                updateTeacherForm.Show();
                updateTeacherForm.Activate();
                Update();
            }
        }

        private void DeleteTeacher_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTeacher == null)
                MessageBox.Show("Please choose a teacher to delete!");
            else
            {
                int id = SelectedTeacher.Id;
                var activeCoursesWithoutTeacher = _directorController.GetActiveCourses(SelectedTeacher.ToTeacher());

                if (activeCoursesWithoutTeacher!=null)
                {
                    foreach (var course in activeCoursesWithoutTeacher)
                    {
                        ChooseTeacherView choseTeacherView = new ChooseTeacherView(course, id);
                        choseTeacherView.Show();
                        choseTeacherView.Activate();
                    }
                }
                _directorController.Delete(SelectedTeacher.ToTeacher());
                Update();
            }
        }

        private void SearchTeachers_Click(object sender, RoutedEventArgs e)
        {
            UpdateSearch();
            UpdatePagination();
            isSearchButtonClicked = true;
        }

        private void ResetTeachers_Click(object sender, EventArgs e)
        {
            isSearchButtonClicked = false;
            Update();
            ResetSearchElements();
            UpdatePagination();
        }

        private void ResetSearchElements()
        {
            languageComboBox.SelectedItem = null;
            levelComboBox.SelectedItem = null;
            startedWorkDatePicker.SelectedDate = null;
        }

        private List<Domain.Model.Teacher> GetFilteredTeachers()
        {
            Language selectedLanguage = Domain.Model.Enums.Language.NULL;
            LanguageLevel selectedLevel = LanguageLevel.NULL;
            DateTime selectedStartDate = DateTime.MinValue;

            if (languageComboBox.SelectedItem != null)
                selectedLanguage = (Language)languageComboBox.SelectedItem;

            if (levelComboBox.SelectedItem != null)
                selectedLevel = (LanguageLevel)levelComboBox.SelectedItem;

            if (startedWorkDatePicker.SelectedDate.HasValue)
                selectedStartDate = (DateTime)startedWorkDatePicker.SelectedDate;

            return GetDisplayTeachers(selectedLanguage, selectedLevel, selectedStartDate);
        }

        private List<Domain.Model.Teacher> GetDisplayTeachers(Language selectedLanguage, LanguageLevel selectedLevel, DateTime selectedStartDate)
        {
            List<Domain.Model.Teacher> finalTeachers = new();

            if (isSearchButtonClicked)
            {
                List<Domain.Model.Teacher> allFilteredTeachers = _directorController.FindTeachersByCriteria(selectedLanguage, selectedLevel, selectedStartDate);

                foreach (Domain.Model.Teacher teacher in allFilteredTeachers)
                    finalTeachers.Add(teacher);
            }
            else
            {
                foreach (Domain.Model.Teacher teacher in _directorController.GetAllTeachers())
                    finalTeachers.Add(teacher);
            }
            return finalTeachers;
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            UpdateCourses();
            isSearchButtonClicked = true;
        }
        public void UpdateCourses()
        {
            try
            {
                TableViewModel.Courses.Clear();
                var courses = _courseController.GetCoursesForTopStudentMails();

                if (courses != null)
                    foreach (Course course in courses)
                        TableViewModel.Courses.Add(new CourseDTO(course));
                else
                    MessageBox.Show("No courses found.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void ViewCourseButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCourse == null)
                MessageBox.Show("Please choose a course to view!");
            else
            {
                CourseView courseView = new CourseView(SelectedCourse.Id, _directorController.GetDirector());
                courseView.Show();
                UpdateCourses();
            }
        }
        private void CreateCourse_Click(object sender, RoutedEventArgs e)
        {
            CreateCourseForm courseForm = new CreateCourseForm(-1);
            courseForm.Show();
        }
        private void CreateExam_Click(object sender, RoutedEventArgs e)
        {
            CreateExamForm examForm = new CreateExamForm(-1);
            examForm.Show();
        }

        private void AssignTeacherCourse_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCourseDirector == null)
                MessageBox.Show("Please choose a course to assign teacher!");
            else
            {
                Domain.Model.Teacher? courseTeacher = _directorController.GetTeacherByCourse(SelectedCourseDirector.Id);
                if (courseTeacher != null)
                    MessageBox.Show("This course already has a teacher!");
                else 
                { 
                    AssignTeacher();
                    SetCourses();
                }
            }
        }
        private void AssignTeacher()
        {
            int teacherCourseId = _directorController.FindMostAppropriateTeacher(SelectedCourseDirector.ToCourse());
            if (teacherCourseId != -1)
            {
                Domain.Model.Teacher teacher = _directorController.GetById(teacherCourseId);
                SelectedCourseDirector.SetTeacher(teacher, SelectedCourseDirector.ToCourse());
                teacher.CoursesId.Add(SelectedCourseDirector.Id);
                _directorController.Update(teacher);
                MessageBox.Show($"{teacher.FirstName} {teacher.LastName}", "Teacher who was chosen");
            }
            else
            {
                SelectedCourseDirector.HasTeacher = false;
                MessageBox.Show("There is no available teacher for that course");
            }
        }
        private void SendReport_Click(object sender, RoutedEventArgs e)
        {
            IReportGenerator reportGenerator = null;
            if (ReportOneRadioButton.IsChecked == true)
            {
                reportGenerator = new FirstReportGenerator();
            }
            else if (ReportTwoRadioButton.IsChecked == true)
            {
                reportGenerator = new SecondReportGenerator();
            }
            else if (ReportThreeRadioButton.IsChecked == true)
            {
                reportGenerator = new ThirdReportGenerator();
            }
            else if (ReportFourRadioButton.IsChecked == true)
            {
                reportGenerator = new FourthReportGenerator();
            }

            if (reportGenerator != null)
                _reportController.GenerateReport(reportGenerator); 
            else
                MessageBox.Show("Please select the report you want to send.");
        }

        public void ViewExam_Click(object sender, EventArgs e)
        {
            if (SelectedExamTerm == null)
                MessageBox.Show("Please choose an exam term to view!");
            else
            {
                ExamTerm? examTerm = _teacherController.GetExamTermById(SelectedExamTerm.ExamID);
                Domain.Model.Teacher? teacher = _directorController.GetTeacherByExamTerm(examTerm.ExamID);
                ExamTermView examTermView = new ExamTermView(examTerm, teacher, this);
                examTermView.Owner = this;
                this.Visibility = Visibility.Collapsed;
                examTermView.Show();
            }
        }

        public void SearchDirectorCourse_Click(object sender, EventArgs e) {}
        public void ResetDirectorCourse_Click(object sender, EventArgs e) {}
        public void SearchDirectorExams_Click(object sender, EventArgs e) {}
        public void ResetExam_Click(object sender, EventArgs e) {}

        private void TeacherNextPage_Click(object sender, RoutedEventArgs e)
        {
            currentTeacherPage++;
            TeacherPreviousButton.IsEnabled = true;
            UpdatePagination();
        }

        private void TeacherPreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentTeacherPage > 1)
            {
                currentTeacherPage--;
                TeacherNextButton.IsEnabled = true;
                UpdatePagination();
            }
            else if (currentTeacherPage == 1)
               TeacherPreviousButton.IsEnabled = false;
        }

        private void UpdatePagination()
        {
            if (currentTeacherPage == 1)
                TeacherPreviousButton.IsEnabled = false;
            TeacherPageNumberTextBlock.Text = $"{currentTeacherPage}";

            try
            {
                TableViewModel.Teachers.Clear();
                var filteredTeachers= GetFilteredTeachers();
                 //List<Domain.Model.Teacher> teachers = _directorController.GetAllTeachers(currentTeacherPage, 4, teacherSortCriteria, filteredTeachers);
                //List<Domain.Model.Teacher> newTeachers = _directorController.GetAllTeachers(currentTeacherPage + 1, 4, teacherSortCriteria, filteredTeachers);

                List<Domain.Model.Teacher> teachers = _directorController.GetAllTeachers(currentTeacherPage, 4, sortStrategy, filteredTeachers);
                List<Domain.Model.Teacher> newTeachers = _directorController.GetAllTeachers(currentTeacherPage + 1, 4, sortStrategy, filteredTeachers);

                if (newTeachers.Count == 0)
                    TeacherNextButton.IsEnabled = false;
                else
                    TeacherNextButton.IsEnabled = true;

                if (filteredTeachers != null)
                {
                    foreach (Domain.Model.Teacher teacher in teachers)
                        TableViewModel.Teachers.Add(new TeacherDTO(teacher));
                }
                else
                    MessageBox.Show("No exam terms found.");
           }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void TeacherSortCriteriaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (teacherSortCriteriaComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedContent = selectedItem.Content.ToString();
                switch (selectedContent)
                {
                    case "FirstName":
                        teacherSortCriteria = "FirstName";
                        sortStrategy = new SortByFirstName();
                        break;
                    case "LastName":
                        teacherSortCriteria = "LastName";
                        sortStrategy = new SortByLastName();
                        break;
                    case "StartedWork":
                        teacherSortCriteria = "StartedWork";
                        sortStrategy = new SortByDatetime();
                        break;
                }
                UpdatePagination();
            }
        }
    }
}
