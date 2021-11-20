using AFE_FitnessBackend.Models.Entities;
using System;
using System.Linq;
using static BCrypt.Net.BCrypt;

namespace AFE_FitnessBackend.Data
{
    public static class DbUtilities
    {
        public const int BcryptWorkfactor = 10;

        internal static void SeedData(ApplicationDbContext context)
        {
            try { 
            context.Database.EnsureCreated();
            if (!context.Users.Any())
            { 
                SeedTrainers(context);
                SeedClients(context);
            }
            if (!context.WorkoutPrograms.Any())
                SeedWorkoutPrograms(context);
            if (!context.Exercises.Any())
                SeedExercises(context);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error during seed of database: " + ex.Message);
            }
        }

        static void SeedTrainers(ApplicationDbContext context)
        {
            context.Users.AddRange(
                new User
                {
                    Email = "boss@fitness.moon",
                    FirstName = "Manager",
                    LastName = "The Boss",
                    AccountType = Models.Enums.Role.Manager,
                    PwHash = HashPassword("asdfQWER", BcryptWorkfactor)
                },
                // Personal trainers
                new User
                {
                Email = "m@fit",
                    FirstName = "Superman",
                    LastName = "Mars",
                    AccountType = Models.Enums.Role.PersonalTrainer,
                    PwHash = HashPassword("aQ", BcryptWorkfactor)
                },
                new User
                {
                    Email = "w@fit",
                    FirstName = "Superwoman",
                    LastName = "Venus",
                    AccountType = Models.Enums.Role.PersonalTrainer,
                    PwHash = HashPassword("aZ", BcryptWorkfactor)
                });
            context.SaveChanges();
        }

        static void SeedClients(ApplicationDbContext context)
        {
            context.Users.AddRange(
                new User
                {
                    Email = "c1@fit",
                    FirstName = "John",
                    LastName = "Doe",
                    AccountType = Models.Enums.Role.Client,
                    PwHash = HashPassword("aA", BcryptWorkfactor),
                    PersonalTrainerId = 2
                },
                new User
                {
                    Email = "c2@fit",
                    FirstName = "Jane",
                    LastName = "Doe",
                    AccountType = Models.Enums.Role.Client,
                    PwHash = HashPassword("aA", BcryptWorkfactor),
                    PersonalTrainerId = 3
                });
            context.SaveChanges();
        }

        private static void SeedWorkoutPrograms(ApplicationDbContext context)
        {
            context.WorkoutPrograms.AddRange(
                new WorkoutProgram
                {
                    ClientId = 4,
                    PersonalTrainerId = 2,
                    Name = "Easy",
                    Description = "Starter program"
                },
                new WorkoutProgram
                {
                    ClientId = 5,
                    PersonalTrainerId = 3,
                    Name = "Not so Easy",
                    Description = "Just another program"
                }
                );
            context.SaveChanges();
        }

        private static void SeedExercises(ApplicationDbContext context)
        {
            context.Exercises.AddRange(
                new Exercise
                {
                    Name = "Push ups",
                    Description = "Place your hands on the floor with legs straight out behind you resting on your toes. Bend your arms and slowly …",
                    Sets = 2,
                    Repetitions = 5,
                    PersonalTrainerId = 2,
                    WorkoutProgramId = 1
                },
                new Exercise
                {
                    Name = "Push ups",
                    Description = "Place your hands on the floor with legs straight out behind you resting on your toes. Bend your arms and slowly …",
                    Sets = 2,
                    Repetitions = 10,
                    PersonalTrainerId = 3,
                    WorkoutProgramId = 2
                },
                new Exercise
                {
                    Name = "Plank",
                    Description = "Place your elbows on the floor shoulder- width apart with legs stretched out behind you so only your elbows and toes are in contact with the ground. Use your abdominal muscles to keep …",
                    Repetitions = 0,
                    Time = "30 seconds",
                    PersonalTrainerId = 2,
                    WorkoutProgramId = 1
                },
                new Exercise
                {
                    Name = "Squat",
                    Description = "Stand with your feet spread shoulder-width apart. Lower your body as far as you can by pushing your hips back and bending your knees. Pause, and then slowly push yourself back to the starting position.",
                    Sets = 3,
                    Repetitions = 15,
                    PersonalTrainerId = 3,
                    WorkoutProgramId = 2
                }
                );
            context.SaveChanges();
        }

    }
}

