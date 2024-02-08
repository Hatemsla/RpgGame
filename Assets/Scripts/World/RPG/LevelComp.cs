namespace World.RPG
{
    public struct LevelComp
    {
        public int Level;
        public float Experience;
        public float ExperienceToNextLevel;
        public float AwardExperienceDiv;

        public int LevelScore;
        public int SpentLevelScore;
        
        public int Strength;
        public int Dexterity;
        public int Constitution;
        public int Intelligence;
        public int Charisma;
        public int Luck;
        
        public int PreviousStrength;
        public int PreviousDexterity;
        public int PreviousConstitution;
        public int PreviousIntelligence;
        public int PreviousCharisma;
        public int PreviousLuck;

        public float PAtk;
        public float MAtk;
        public float Spd;
        public float MaxHp;
        public float MaxSt;
        public float MaxSp;
        
        public float PreviousPAtk;
        public float PreviousMAtk;
        public float PreviousSpd;
        public float PreviousMaxHp;
        public float PreviousMaxSt;
        public float PreviousMaxSp;
    }
}