namespace CircleOfLife.Build.UI
{
    public class LevelUpUIData
    {
        public BuildBase Target;
        public int Material;
        public int Need;
    }

    public class LevelUpResponse
    {
        public BuildBase.LevelUpDirection Direction;
        public bool Confirm;
    }
}
