namespace ArphrosFramework {
    public enum SpaceType {
        World,
        Screen
    }

    public enum ObstacleType {
        None,
        Wall,
        PassThrough,
        Water
    }

    public enum VisibilityType {
        Shown,
        Hidden,
        Gone
    }

    public enum ObjectType {
        Primitive,
        Model,
        Sprite,
        Light,
        Trigger,
        Road,
        Particle,
        Player,
        MainCamera,
        Empty,
        Text,
        Tail,
        StartPos,
        Unspecified = 256
    }

    public enum TailMode
    {
        Clear,
        Disable,
        Enable
    }
    
    public enum RoadRandomOptions {
        Move,
        Rotate,
        Both
    }
}