﻿using BirthdayCelebrations.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayCelebrations.Models
{
    public class Citizen : IIdentifiable, IBirthable
    {
        public Citizen(string id, string name, int age, string birthdate)
        {
            this.Id = id;
            this.Name = name;
            this.Age = age;
            this.Birthdate = birthdate;
        }

        public string Id { get; private set; }

        public int Age { get; private set; }
        public string Name { get ; set; }
        public string Birthdate { get ; set ; }
    }
}
