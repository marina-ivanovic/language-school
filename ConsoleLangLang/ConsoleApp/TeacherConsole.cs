﻿using ConsoleLangLang.ConsoleApp;
using LangLang.Controller;
using LangLang.Domain.Model;
using System;

namespace LangLang.ConsoleApp
{
    public class TeacherConsole
    {
        private static Teacher teacher;
        private static DirectorController directorController = Injector.CreateInstance<DirectorController>();
        public static void Display(string teacherEmail)
        {
            teacher = directorController.GetTeacherByEmail(teacherEmail);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Choose an operation:\n" +
                                  "\t1) CRUD operations\n" +
                                  "\tx) Exit");

                string operation = Console.ReadLine().ToLower();

                switch (operation)
                {
                    case "1":
                        CRUDConsole.Display(teacher);
                        break;
                    case "x":
                        return;
                    default:
                        Console.WriteLine("Invalid operation.");
                        break;
                }
            }
        }

    }
}
