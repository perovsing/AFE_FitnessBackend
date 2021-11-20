using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AFE_FitnessBackend.Models.Entities
{
    public class WorkoutProgram
    {
        public long WorkoutProgramId { get; set; }
        [MaxLength(64)]
        public string Name { get; set; }
        [MaxLength(4096)]
        public string Description { get; set; }
        public ICollection<Exercise> Exercises { get; set; }
        public long PersonalTrainerId { get; set; }
        public long? ClientId { get; set; }

    }
}
