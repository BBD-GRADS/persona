using Newtonsoft.Json;
using PersonaBackend.Interfaces;

namespace PersonaBackend.Models.HandOfZeus
{
    public class StartSimulationRequest
    {
        public string action { get; set; }
        public DateTime startTime { get; set; }
    }
}