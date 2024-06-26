using Newtonsoft.Json;
using PersonaBackend.Interfaces;

namespace PersonaBackend.Models.HandOfZeus
{
    public class StartSimulationRequest
    {
        /// <example>1000</example>

        public int NumberOfPersonas { get; set; } = 1000;

        /// <example>00|01|01</example>
        public string StartDate { get; set; } = "00|01|01";
    }
}