namespace Progression.Common.Configs
{
    /// <summary>
    /// Weapon level cap modes
    /// </summary>
    public enum WeaponLevelCapMode
    {
        /// <summary>
        /// Weapon level cap follows world level
        /// </summary>
        WorldLevel,

        /// <summary>
        /// Weapon level cap is independent and fixed
        /// </summary>
        Independent,

        /// <summary>
        /// No weapon level cap
        /// </summary>
        Unlimited,
    }
}
