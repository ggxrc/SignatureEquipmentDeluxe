namespace SignatureEquipmentDeluxe.Common.Systems
{
    public struct ProjectileStatsData
    {
        public int LifeTime { get; set; }
        public float StartVelocity { get; set; }
        public float Size { get; set; }
        public float MinionSlotsReduce { get; set; }
        public int Penetrate { get; set; }

        public ProjectileStatsData()
        {
            LifeTime = 0;
            StartVelocity = 1;
            Size = 1;
            MinionSlotsReduce = 1;
            Penetrate = 0;
        }
    }
}
