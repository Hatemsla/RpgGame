using World.SaveGame;

namespace World.LoadGame
{
    public struct LoadDataEventComp
    {
        public bool IsLoadData;
        public PlayerSaveData PlayerSaveData;
        public ChestSaveDatas ChestSaveDatas;
        public TraderSaveDatas TraderSaveDatas;
    }
}