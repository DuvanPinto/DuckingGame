using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IngameEquations
{
    private static float COMMON_PERCENT = 50;
    private static float RARE_PERCENT = 30;
    private static float EPIC_PERCENT = 10;
    private static float MYTHIC_PERCENT = 5;
    private static float EXOTIC_PERCENT = 4;
    private static float LEGENDARY_PERCENT = 1;

    public static (int, PartRarity) GetIndexAndRarity()
    {
        var probability = Random.Range(0, 101);

        if (probability <= LEGENDARY_PERCENT)
            return (0, PartRarity.LEGENDARY);

        if (probability > LEGENDARY_PERCENT && probability <= EXOTIC_PERCENT)
            return (0, PartRarity.EXOTIC);

        if (probability > EXOTIC_PERCENT && probability <= MYTHIC_PERCENT)
            return (0, PartRarity.MYTHIC);

        if (probability > MYTHIC_PERCENT && probability <= EPIC_PERCENT)
            return (0, PartRarity.EPIC);

        if (probability > EPIC_PERCENT && probability <= RARE_PERCENT)
            return (0, PartRarity.RARE);

        return (0, PartRarity.COMMON);
    }
}
