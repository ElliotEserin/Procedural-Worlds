using System;

public static class NameGenerator
{
    static string[] villageAdjectives1 = new string[]
    {
            "Sun",
            "Moon",
            "Distant",
            "Settling",
            "River",
            "Ocean",
            "Silk",
            "Quake",
            "Lake",
            "Quiet",
            "Bustling",
            "Eternal"
    };

    static string[] villageAdjectives2 = new string[]
{
            "Edge",
            "Set",
            "Rise",
            "Bank",
            "Crossroad",
            "Peace",
            "Petal",
            "Metal",
            "Rock",
            "Farm",
            "Mine",
            "song"
};

    public static string GenerateVillageName(int seed, Village.VillageRadius radius)
    {
        System.Random rand = new System.Random(seed);

        string villageType;
        string villageAdjective1 = villageAdjectives1[rand.Next(0, villageAdjectives1.Length - 1)];
        string villageAdjective2 = villageAdjectives2[rand.Next(0, villageAdjectives2.Length - 1)];

        switch (radius)
        {
            case Village.VillageRadius.Large:
                villageType = "City";
                break;
            case Village.VillageRadius.Medium:
                villageType = "Town";
                break;
            default:
            case Village.VillageRadius.Small:
                villageType = "Village";
                break;
        }

        int layout = rand.Next(0, 2);

        if(layout == 0)
            return villageAdjective1 + " " + villageType;
        else if (layout == 1)
            return villageAdjective1 + villageAdjective2.ToLower();
        else
            return villageAdjective1 + villageAdjective2.ToLower() + " " + villageType;

    }
}
