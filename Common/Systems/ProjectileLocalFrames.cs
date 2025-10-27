using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SignatureEquipmentDeluxe.Common.Systems
{
    public class ProjectileLocalFrames
    {
        public ProjectileDefinition Projectile;

        [DefaultValue(-2)]
        [Range(-2, int.MaxValue)]
        public int LocalImmunityFrames;
    }
}
