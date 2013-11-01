using Core.Parser;

namespace Core.Engine
{
    public class WarriorStartInfo
    {
        public Warrior Warrior;
        public int LoadAddress;

        public WarriorStartInfo(Warrior warrior, int loadAddress)
        {
            Warrior = warrior;
            LoadAddress = loadAddress;
        }

    }
}