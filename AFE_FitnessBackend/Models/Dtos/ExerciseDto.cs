using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AFE_FitnessBackend.Models.Dtos
{
    public class ExerciseDto
    {
        [MaxLength(64)]
        public string Name { get; set; }
        [MaxLength(4096)]
        public string Description { get; set; }
        public int? Sets { get; set; }
        public int? Repetitions { get; set; }
        [MaxLength(32)]
        public string Time { get; set; }

        // Methods for converting
        public static ExerciseDto FromEntityExercise(Entities.Exercise eExercise)
        {
            var exercise = new ExerciseDto();
            exercise.Description = eExercise.Description;
            exercise.Name = eExercise.Name;
            exercise.Sets = eExercise.Sets;
            exercise.Repetitions = eExercise.Repetitions;
            exercise.Time = eExercise.Time;
            
            return exercise;
        }

        public Entities.Exercise ToEntityExercise()
        {
            var eExercise = new Entities.Exercise();
            eExercise.Description = Description;
            eExercise.Name = Name;
            eExercise.Sets = Sets;
            eExercise.Repetitions = Repetitions;
            eExercise.Time = Time;

            return eExercise;
        }
    }
}
