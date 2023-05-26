namespace JS.CharacterSystem.Creation
{
    [System.Serializable]
    public class CreaturePresetData
    {
        public string presetName;

        public int raceID;
        public int classID;
        public int domainID;
        public int gender;
        public int age;
        public bool isUndead;
        public int[] attributeValues;
    }
}