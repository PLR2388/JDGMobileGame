using System;

namespace JDG.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for deck configuration.
    /// Used to transfer deck data to the presentation layer.
    /// </summary>
    [Serializable]
    public class DeckDTO
    {
        public string Name { get; set; }
        public Guid[] CardIds { get; set; }
        public int CardCount { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
    }
}
