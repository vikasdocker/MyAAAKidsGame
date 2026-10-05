namespace Dab.Runtime.Abilities
{
    public static class SignatureMarkCatalog
    {
        public static bool TryGetAbility(
            SignatureMarkId markId,
            out CreatureAbilityId abilityId)
        {
            switch (markId)
            {
                case SignatureMarkId.HornSwirl:
                    abilityId = CreatureAbilityId.PlayfulCharge;
                    return true;
                default:
                    abilityId = CreatureAbilityId.None;
                    return false;
            }
        }
    }
}
