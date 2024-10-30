using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Models
{
    /// <summary>
    /// Korisnik sustava koji pripada bendu.
    /// </summary>
    public class User
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Ime korisnika.
        /// </summary>
        [Required]
        public required string FirstName { get; set; }

        /// <summary>
        /// Prezime korisnika.
        /// </summary>
        [Required]
        public required string LastName { get; set; }

        /// <summary>
        /// ID benda kojem korisnik pripada.
        /// </summary>
        public required int BandId { get; set; }

        /// <summary>
        /// Bend kojem korisnik pripada.
        /// </summary>
        public Band? Band { get; set; }
    }
}
