using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Models
{
    /// <summary>
    /// Rezervacija prostorije od strane benda.
    /// </summary>
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// ID benda koji je napravio rezervaciju.
        /// </summary>
        public required int BandId { get; set; }

        /// <summary>
        /// Bend koji je napravio rezervaciju.
        /// </summary>
        public Band? Band { get; set; }

        /// <summary>
        /// ID prostorije koja je rezervirana.
        /// </summary>
        public required int RoomId { get; set; }

        /// <summary>
        /// Prostorija koja je rezervirana.
        /// </summary>
        public Room? Room { get; set; }

        /// <summary>
        /// Datum i vrijeme početka rezervacije.
        /// </summary>
        public required DateTime StartTime { get; set; }

        /// <summary>
        /// Datum i vrijeme završetka rezervacije.
        /// </summary>
        public required DateTime EndTime { get; set; }
    }
}
