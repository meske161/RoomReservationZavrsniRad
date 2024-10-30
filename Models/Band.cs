using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RoomReservation.Models
{
    /// <summary>
    /// Bend koji ima članove (korisnike).
    /// </summary>
    public class Band
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Naziv benda.
        /// </summary>
        [Required]
        public required string Name { get; set; }

        /// <summary>
        /// Članovi benda.
        /// </summary>
        [JsonIgnore]
        public List<User>? Users { get; set; } = new List<User>();
    }
}
