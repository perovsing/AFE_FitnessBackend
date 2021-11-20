using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AFE_FitnessBackend.Models.Entities
{
    public class Exercise
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public long ExerciseId { get; set; }
        [MaxLength(64)]
        public string Name { get; set; }
        [MaxLength(4096)]
        public string Description { get; set; }
        public int? Sets { get; set; }
        public int? Repetitions { get; set; }
        [MaxLength(32)]
        public string Time { get; set; }
        /// <summary>
        /// FK
        /// </summary>
        public long? WorkoutProgramId { get; set; } 
        /// <summary>
        /// FK to personal trainer
        /// </summary>
        public long? PersonalTrainerId { get; set; } 
    }
}
