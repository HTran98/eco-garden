using EcoGarden.Config;

namespace EcoGarden.Board
{
    public sealed class ProducerRuntime
    {
        public ProducerDefinition Definition { get; }
        public float NextReadyTime { get; private set; }

        public ProducerRuntime(ProducerDefinition definition)
        {
            Definition = definition;
        }

        public bool IsReady(float currentTime)
        {
            return currentTime >= NextReadyTime;
        }

        public void StartCooldown(float currentTime)
        {
            NextReadyTime = currentTime + (Definition != null ? Definition.CooldownSeconds : 0f);
        }
    }
}
