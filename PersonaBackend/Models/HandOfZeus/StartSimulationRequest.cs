using Newtonsoft.Json;
using PersonaBackend.Interfaces;

namespace PersonaBackend.Models.HandOfZeus
{
    public class StartSimulationRequest
    {
        /// <example>1000</example>

        public int NumberOfPersonas { get; set; } = 1000;

        public DateTime StartDate { get; set; }
    }
}