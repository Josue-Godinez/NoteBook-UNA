﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UNA.NoteBook
{
    public class User
    {
        public string NameUser
        {
            get;
            set;
        }

        public string PasswordUser
        {
            get;
            set;
        }

        public Notebook.Book Book
        {
            get => default;
            set
            {
            }
        }
    }
}