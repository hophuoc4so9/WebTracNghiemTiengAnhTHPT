﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTracNghiemTiengAnhTHPT.Models
{
    public class ExamViewModel
    {
        public int TotalQuestions { get; set; }
        public int EasyQuestions { get; set; }
        public int HardQuestions { get; set; }
        public int ExamDuration { get; set; }
    }
}