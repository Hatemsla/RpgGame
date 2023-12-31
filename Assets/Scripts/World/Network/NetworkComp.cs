using Fusion;

namespace World.Network
{
    public struct NetworkComp
    {
        [Networked]
        public int token { get; set; }
    }
}