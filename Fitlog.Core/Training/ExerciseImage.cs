using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Training
{
    public class ExerciseImage
    {
        public Guid Id { get; set; }
        public Guid ExerciseId { get; set; }
        public string Type { get; set; }
    }
    public class ExerciseImageDetails : ExerciseImage
    {
        public byte[] Data { get; set; }
    }
}
