﻿using LangLang.Controller;
using LangLang.Domain.Model;
using System;
using System.Windows;

namespace LangLang.View.Student
{
    /// <summary>
    /// Interaction logic for StudentForm.xaml
    /// </summary>
    public partial class StudentForm : Window
    {
        int studentId;
        StudentsController studentController;

        public StudentForm(int studentId)
        {
            InitializeComponent();
            this.studentId = studentId;
            this.studentController = Injector.CreateInstance<StudentsController>();

            SetWelcomeHeading();
            SetStudentInformations();
            DisplayAcceptedCourseRequest();
            SetActiveCourse();
        }
 
        private void SetWelcomeHeading()
        {
            Domain.Model.Student student = studentController.GetStudentById(studentId);
            welcomeTextBlock.Text = welcomeTextBlock.Text + " " + student.FirstName;
        }
        private void SetStudentInformations()
        {
            PenaltyPointController penaltyPointController = new PenaltyPointController();
            completedCoursesTextBlock.Text = studentController.GetCompletedCourseNumber(studentId).ToString();
            passedExamsTextBlock.Text = studentController.GetPassedExamsNumber(studentId).ToString();
            penaltyPoinsTextBlcok.Text = penaltyPointController.GetPenaltyPointsByStudentId(studentId).Count.ToString();
        }

        private void SetActiveCourse()
        {
            Course? activeCourse = studentController.GetActiveCourse(studentId);
            if (activeCourse != null)
            {
                activeCourseTextBlock.Text = GetCourseName(activeCourse);

                if (studentController.IsQuitCourseMailSent(studentId, activeCourse.Id) ||
                    (DateTime.Now - activeCourse.StartDate).TotalDays < 7)
                {
                    dropOutButton.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                activeCourseTextBlock.Text = "/";
                dropOutButton.Visibility = Visibility.Collapsed;

                int currentlyCompletedCourseId = studentController.IsSomeCourseCompleted(studentId);
                if (currentlyCompletedCourseId >= 0)
                {
                    DisplayCompletedCourseForm(currentlyCompletedCourseId);
                }
            }
        }
        private void DisplayCompletedCourseForm(int courseId)
        {
            GradeTeacher gradeTeacherForm = new GradeTeacher(studentId, courseId);
            gradeTeacherForm.Show();
        }

        private void DisplayAcceptedCourseRequest()
        {
            if(studentController.IsEnterCourseRequestAccepted(studentId))
            {
                Course activeCourse = studentController.GetActiveCourse(studentId);
                AcceptedEnterCourseRequestForm acceptedCourseForm = new AcceptedEnterCourseRequestForm(GetCourseName(activeCourse));
                acceptedCourseForm.Show();
            }
        }
        private void DropOutFromCourseBoutton_Click(object sender, RoutedEventArgs e)
        {
            CancelCourseEnrollmentForm cancelCourseEnrollmentForm = new CancelCourseEnrollmentForm(studentId,studentController.GetStudentById(studentId).ActiveCourseId);
            cancelCourseEnrollmentForm.Owner = this;
            cancelCourseEnrollmentForm.WindowClosed += CancelCourseEnrollmentForm_WindowClosed;
            cancelCourseEnrollmentForm.ShowDialog();
            
        }

        private void AvailableCourses_Click(object sender, RoutedEventArgs e)
        {
            CoursesView coursesView = new CoursesView(studentId, 0);
            coursesView.Owner = this;
            coursesView.ShowDialog();
        }
        private void RegisteredCourses_Click(object sender, RoutedEventArgs e)
        {
            CoursesView coursesView = new CoursesView(studentId, 1);
            coursesView.Owner = this;
            coursesView.ShowDialog();
        }
        private void CompletedCourses_Click(object sender, RoutedEventArgs e)
        {
            CoursesView coursesView = new CoursesView(studentId, 2);
            coursesView.Owner = this;
            coursesView.ShowDialog();
        }
        private void PassedCourses_Click(object sender, RoutedEventArgs e)
        {
            CoursesView coursesView = new CoursesView(studentId, 3);
            coursesView.Owner = this;
            coursesView.ShowDialog();
        }
        private void ExamTerms_Click(object sender, RoutedEventArgs e)
        {
            ExamTermsPage examTermsPage = new ExamTermsPage(studentId);
            examTermsPage.Show();
        }

        private void UpdateAccount_Click(object sender, RoutedEventArgs e)
        {
            Domain.Model.Student student = studentController.GetStudentById(studentId);
            if (student.ActiveCourseId != -1 || student.RegisteredExamsIds.Count != 0)
            {
                MessageBox.Show("The student cannot change the data.");
            }
            else
            {
                UpdateForm updateDataForm = new UpdateForm(studentId, studentController);
                updateDataForm.Show();
                updateDataForm.Activate();
            }
        }
        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            studentController.Delete(studentId);
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
        private string GetCourseName(Course course)
        {
            return course.Language.ToString() + " " + course.Level.ToString();
        }
        private void CancelCourseEnrollmentForm_WindowClosed(object sender, System.EventArgs e)
        {
            SetActiveCourse();
        }
        private void Mail_Click(object sender, RoutedEventArgs e)
        {
            StudentMail studentMail = new StudentMail(studentController.GetStudentById(studentId));
            studentMail.Show();
        }
    }
}
