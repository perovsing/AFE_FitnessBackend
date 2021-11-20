using AFE_FitnessBackend.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AFE_FitnessBackend.Models.Dtos
{
    public class User
    {
        public long UserId { get; set; }
        [MaxLength(64)]
        public string FirstName { get; set; }
        [MaxLength(32)]
        public string LastName { get; set; }
        [MaxLength(254)]
        public string Email { get; set; }
        [MaxLength(60)]
        public string Password { get; set; }
        public long? PersonalTrainerId { get; set; }
        public string AccountType { get; set; }

        // Methods for converting
        public static User FromEntityUser(Entities.User eUser)
        {
            var user = new User();
            user.AccountType = eUser.AccountType.ToString();
            user.Email = eUser.Email;
            user.FirstName = eUser.FirstName;
            user.LastName = eUser.LastName;
            user.PersonalTrainerId = eUser.PersonalTrainerId;
            user.UserId = eUser.UserId;

            return user;
        }

        public Entities.User ToEntityUser()
        {
            var eUser = new Entities.User();
            eUser.AccountType = (Role)Enum.Parse(typeof(Role), AccountType);
            eUser.Email = Email;
            eUser.FirstName = FirstName;
            eUser.LastName = LastName;
            eUser.PersonalTrainerId = PersonalTrainerId;
            eUser.UserId = UserId;

            return eUser;
        }
    }
}
