using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArphrosFramework.Data {
    [Serializable]
    public class ObjectData {
        public int id;
        public int parentId = -1;

        public ObjectType type = ObjectType.Model;
        public SpaceType spaceType = SpaceType.World;

        public string name = "Object";
        public bool canCollide = true;

        public Vector3 position;
        public Vector3 eulerAngles;
        public Vector3 scale = Vector3.one;

        public List<int> groupId = new();

        public ObstacleType obstacleType = ObstacleType.None;
        public VisibilityType visibility = VisibilityType.Shown;

        public string animatable;
        public AnimatableData animatableData;
        public string customData;
    }
}