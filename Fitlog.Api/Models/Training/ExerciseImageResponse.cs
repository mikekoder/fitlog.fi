using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Api.Models.Training
{
    public class ExerciseImageResponse
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
    }
}
