using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PressYourLuck.Models
{
    public class Player
    {
        [Required(ErrorMessage = "please enter a name.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "please enter total coins.")]
        [Range(1.00, 10000.00, ErrorMessage = "Total coins should be between 1.00 and 10000.00.")]
        public double TotalCoins { get; set; }
    }
}
