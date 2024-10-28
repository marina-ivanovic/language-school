﻿using LangLang.Domain.Model.Enums;
using System;
using LangLang.Storage.Serialization;

namespace LangLang.Domain.Model
{
    public abstract class Person : ISerializable
    {
        protected int id;
        protected string firstName;
        protected string lastName;
        protected Gender gender;
        protected DateTime dateOfBirth;
        protected string phoneNumber;
        protected string email;
        protected string password;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public Gender Gender
        {
            get { return gender; }
            set { gender = value; }
        }

        public DateTime DateOfBirth
        {
            get { return dateOfBirth; }
            set { dateOfBirth = value; }
        }

        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        protected Person() 
        {
            this.firstName = "";
            this.lastName = "";
            this.phoneNumber = "";
            this.email = "";
            this.password = "";
        }
        protected Person(int id, string firstName, string lastName, Gender gender, DateTime dateOfBirth, string phoneNumber, string email, string password)
        {
            this.id = id;
            this.firstName = firstName;
            this.lastName = lastName;
            this.gender = gender;
            this.dateOfBirth = dateOfBirth;
            this.phoneNumber = phoneNumber;
            this.email = email;
            this.password = password;
        }
        protected Person(string firstName, string lastName, Gender gender, DateTime dateOfBirth, string phoneNumber, string email, string password)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.gender = gender;
            this.dateOfBirth = dateOfBirth;
            this.phoneNumber = phoneNumber;
            this.email = email;
            this.password = password;
        }

        public override string ToString()
        {
            return $"{firstName} {lastName}";
        }
        public abstract string[] ToCSV();
        public abstract void FromCSV(string[] values);
    }
}
