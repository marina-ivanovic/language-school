﻿using System.Windows;
using System.Windows.Input;
using LangLang.View.Director;
using LangLang.Controller;
using LangLang.Domain.Model;
using LangLang.View.Teacher;
using LangLang.View.Student;
using LangLang.Data;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LangLang
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private StudentsController studentController { get; set; }
        private DirectorController directorController { get; set; }
        private CourseController courseController { get; set; }
        private ExamTermController examTermController { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            directorController = Injector.CreateInstance<DirectorController>();
            studentController = Injector.CreateInstance<StudentsController>();
            courseController = Injector.CreateInstance<CourseController>(); 
            examTermController = Injector.CreateInstance<ExamTermController>();

            SetPlaceholders();
            
            //InitializeTeacherTable();
            //InitializeCourseTable();
            //InitializeExamTermTable();
        }
        void InitializeTeacherTable()
        {
            List<Teacher> teachers = directorController.GetAllTeachers();
            using (var dbContext = new AppDbContext())
            {
                foreach (Teacher teacher in teachers)
                {
                    dbContext.Teachers.Add(teacher);
                }
                dbContext.SaveChanges();
            }
        }
        void InitializeCourseTable()
        {
            List<Course> courses = courseController.GetAllCourses();
            using (var dbContext = new AppDbContext())
            {
                foreach (Course course in courses)
                {
                    dbContext.Courses.Add(course);
                }
                dbContext.SaveChanges();
            }
        }
        void InitializeExamTermTable()
        {
            List<ExamTerm> examTerms = examTermController.GetAllExamTerms();
            using (var dbContext = new AppDbContext())
             {
                foreach (ExamTerm examTerm in examTerms)
                {
                    dbContext.ExamTerms.Add(examTerm);
                }
                dbContext.SaveChanges();         
             }
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            /*string email = "ivana@gmail.com";
            string password = "ivana123";*/
            string email = Email.Text;
            string password = Password.Password;

            if (HasStudentLoggedIn(email, password) || HasTeacherLoggedIn(email, password) || HasDirectorLoggedIn(email, password))
            {
                this.Close();
                return;
            }
            
             MessageBox.Show("User does not exist.");
        } 

        private bool HasStudentLoggedIn(string email, string password)
        {
            foreach (Student student in studentController.GetAllStudents())
            {
                if (student.Email == email && student.Password == password)
                {
                    if (student.ActiveCourseId != -10)
                    {
                        studentController.ProcessPenaltyPoints();
                        StudentForm welcomePage = new StudentForm(student.Id);
                        welcomePage.Show();
                    }
                    else
                    {
                        MessageBox.Show("Your account has been deactivated.");
                    }

                    return true;
                }
            }
            return false;
        }

        private bool HasTeacherLoggedIn(string email, string password)
        {
            foreach (Teacher teacher in directorController.GetAllTeachers())
            {
                if (teacher.Email == email && teacher.Password == password)
                {
                    TeacherPage teacherPage = new TeacherPage(teacher.Id);
                    teacherPage.Show();
                    return true;
                }
            }
            return false;
        }

        private bool HasDirectorLoggedIn(string email, string password)
        {
            Director director = directorController.GetDirector();

            if (director.Email == email && director.Password == password)
            {
                DirectorPage directorPage = new DirectorPage(director.Id);
                directorPage.Show();
                return true;
            }
            return false;
        }

        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            View.Student.RegistrationForm registrationForm = new View.Student.RegistrationForm(studentController);
            registrationForm.Show();
        }

        private void SetPlaceholders()
        {
            EmailPlaceholder.Visibility = Visibility.Visible;
            PasswordPlaceholder.Visibility = Visibility.Visible;

            Email.GotFocus += EmailTextBox_GotFocus;
            Password.GotFocus += PasswordBox_GotFocus;

            Email.LostFocus += EmailTextBox_LostFocus;
            Password.LostFocus += PasswordBox_LostFocus;

            EmailPlaceholder.MouseDown += Placeholder_MouseDown;
            PasswordPlaceholder.MouseDown += Placeholder_MouseDown;
        }

        private void EmailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            EmailPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void EmailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Email.Text))
                EmailPlaceholder.Visibility = Visibility.Visible;
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Password.Password))
                PasswordPlaceholder.Visibility = Visibility.Visible;
        }

        private void Placeholder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == EmailPlaceholder)
            {
                EmailPlaceholder.Visibility = Visibility.Collapsed;
                Email.Focus();
            }
            else if (sender == PasswordPlaceholder)
            {
                PasswordPlaceholder.Visibility = Visibility.Collapsed;
                Password.Focus();
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PasswordPlaceholder != null)
                PasswordPlaceholder.Visibility = string.IsNullOrEmpty(Password.Password) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}

