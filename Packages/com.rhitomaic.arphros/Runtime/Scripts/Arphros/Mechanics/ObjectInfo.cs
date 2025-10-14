using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using ArphrosFramework.Data;
// using ArphrosFramework.Scripting;

namespace ArphrosFramework {
    // TODO: Add scripting support again
    public class ObjectInfo : MonoBehaviour, InstanceableObject {
        public int instanceId;
        public int GetId() => instanceId;
        public void SetId(int id) => instanceId = id;

        /*private MoonSharpObject _scriptObject;
        public MoonSharpObject msObject
        {
            get
            {
                _scriptObject ??= new MoonSharpObject(this);
                return _scriptObject;
            }

            set => _scriptObject = value;
        }*/

        public ObjectType type = ObjectType.Model;
        public SpaceType spaceType = SpaceType.World;

        public ObstacleType obstacleType = ObstacleType.None;
        public VisibilityType visibility = VisibilityType.Shown;

        public ObjectSerializer serializer;
        public Animatable animatable;

        public List<int> groupId = new();

        public SpriteRenderer _spriteRenderer;
        public MeshRenderer _renderer;
        public TextMeshPro _text;
        public Light _light;

        public LTDescr positionTween;
        public LTDescr rotationTween;
        public LTDescr scaleTween;
        public LTDescr colorTween;

        public ObjectLevel state = ObjectLevel.Project;
        public bool canCollide = true;
        public bool isUnmodifiable;
        /// <summary>
        /// This object won't be reset on stop if made true while modifying at pause
        /// </summary>
        public bool isModified;

        public Vector3 defaultPosition;
        public Vector3 defaultEulerAngles;
        public Vector3 defaultScale;
        public VisibilityType defaultVisibility = VisibilityType.Shown;
        public Transform defaultParent;

        private ReflectionedObj _cache;
        [SerializeField]
        private ObjectData data;
        private bool _materialCloned;

        public void Initialize() {
            FixReferences();
            serializer = serializer ? serializer : GetComponent<ObjectSerializer>();

            switch (state) {
                case ObjectLevel.BuiltIn:
                case ObjectLevel.Project:
                    if (!isUnmodifiable)
                        LevelManager.Register(instanceId, this, true);
                    break;
                case ObjectLevel.Default:
                case ObjectLevel.New:
                    LevelManager.Register(-1, this);
                    state = ObjectLevel.Project;
                    break;
            }
        }

        public void ApplyDefault(bool save = true) {
            if (defaultParent) transform.SetParent(defaultParent);
            transform.localPosition = defaultPosition;
            transform.localEulerAngles = defaultEulerAngles;
            transform.localScale = defaultScale;
            SetVisibility(defaultVisibility);

            if (save)
                _cache = new ReflectionedObj(gameObject);
        }

        public void SaveAsDefault(bool save = false)
        {
            defaultParent = transform.parent;
            defaultPosition = transform.localPosition;
            defaultEulerAngles = transform.localEulerAngles;
            defaultScale = transform.localScale;
            defaultVisibility = visibility;

            if (save)
                _cache = new ReflectionedObj(gameObject);
        }

        /// <summary>
        /// Using System.Reflection to reset the object to its default state, might be wonky
        /// TODO: Considering to change this to a more manual way for optimization purposes
        /// </summary>
        public void ResetObject()
        {
            if (isModified && !gameObject.CompareTag("Player"))
            {
                isModified = false;
                return;
            }

            _cache?.Apply(gameObject);
            ApplyDefault(false);
        }

        /// <summary>
        /// This might be called when an object is Registered using <see cref="InstanceManager.Register(int, T, bool)"/> with override turned on
        /// </summary>
        public void OnIdRedirected(int previousId, int newId) => serializer.OnIdRedirected(previousId, newId);

        /// <summary>
        /// Serialized data of the object to be saved in the level file
        /// </summary>
        /// <returns>The serialized object data</returns>
        public ObjectData GetObject()
        {
            data = new()
            {
                id = instanceId,
                type = type,
                spaceType = spaceType,
                name = name,
                position = transform.localPosition,
                eulerAngles = transform.localEulerAngles,
                scale = transform.localScale,
                groupId = groupId,
                obstacleType = obstacleType,
                visibility = visibility
            };

            if (animatable)
                data.animatable = animatable.Serialize();

            if (transform.parent)
            {
                var parentInfo = transform.parent.GetInfo();
                if (parentInfo)
                    if (parentInfo.instanceId != 1 && !parentInfo.isUnmodifiable)
                        data.parentId = parentInfo.instanceId;
            }

            if (serializer)
                data.customData = serializer.Serialize();

            data.canCollide = canCollide;

            return data;
        }

        /// <summary>
        /// Receiving the serialized data to finally be applied onto the object
        /// </summary>
        /// <param name="objData">The serialized data</param>
        public void FromObject(ObjectData objData)
        {
            data = objData;
            type = objData.type;
            if (state != ObjectLevel.BuiltIn)
                instanceId = objData.id;

            SetSpaceType(objData.spaceType);
            name = objData.name;

            transform.localPosition = objData.position;
            defaultPosition = objData.position;

            transform.localEulerAngles = objData.eulerAngles;
            defaultEulerAngles = objData.eulerAngles;

            transform.localScale = objData.scale;
            defaultScale = objData.scale;

            groupId = objData.groupId;

            if (state != ObjectLevel.BuiltIn)
                state = ObjectLevel.Project;
            Initialize();

            switch (type)
            {
                case ObjectType.Player:
                    serializer = GetComponent<PlayerMovement>();
                    gameObject.layer = LevelManager.Instance.playerLayer;
                    break;
                case ObjectType.MainCamera:
                    serializer = GetComponent<CameraSerializer>();
                    gameObject.layer = LevelManager.Instance.passthroughLayer;
                    break;
                case ObjectType.Light:
                    serializer = gameObject.AddOrGetComponent<LightSerializer>();
                    gameObject.tag = "Light";
                    gameObject.layer = LevelManager.Instance.passthroughLayer;
                    break;
                case ObjectType.Primitive:
                    serializer = gameObject.AddOrGetComponent<PrimitiveSerializer>();
                    break;
                case ObjectType.Model:
                    serializer = gameObject.AddOrGetComponent<ModelSerializer>();
                    break;
                case ObjectType.Trigger:
                    serializer = gameObject.AddOrGetComponent<Trigger>();
                    gameObject.tag = "Trigger";
                    gameObject.layer = LevelManager.Instance.triggerLayer;
                    break;
                case ObjectType.Road:
                    serializer = gameObject.AddOrGetComponent<Road>();
                    gameObject.AddOrGetComponent<BoxCollider>();
                    gameObject.layer = LevelManager.Instance.passthroughLayer;
                    break;
                case ObjectType.Sprite:
                    var spriteSer = gameObject.AddOrGetComponent<SpriteSerializer>();
                    var spriteRend = gameObject.AddOrGetComponent<SpriteRenderer>();
                    spriteSer.renderer = spriteRend;
                    serializer = spriteSer;
                    break;
                case ObjectType.Empty:
                    serializer = gameObject.AddOrGetComponent<EmptySerializer>();
                    break;
                case ObjectType.Text:
                    var texSerializer = gameObject.AddOrGetComponent<TextSerializer>();
                    serializer = texSerializer;
                    gameObject.AddOrGetComponent<MeshRenderer>();
                    texSerializer.renderer = gameObject.AddOrGetComponent<TextMeshPro>();
                    gameObject.AddOrGetComponent<BoxCollider>().isTrigger = true;
                    gameObject.layer = LevelManager.Instance.passthroughLayer;
                    break;
                case ObjectType.StartPos:
                    var startPos = gameObject.AddOrGetComponent<StartPositionSerializer>();
                    gameObject.AddOrGetComponent<BoxCollider>().isTrigger = true;
                    serializer = startPos;
                    gameObject.tag = "StartPosition";
                    gameObject.layer = LevelManager.Instance.passthroughLayer;
                    break;
            }

            serializer.info = this;
            serializer.Deserialize(objData.customData);

            if (!string.IsNullOrWhiteSpace(objData.animatable))
            {
                animatable = gameObject.AddComponent<Animatable>();
                animatable.info = this;
                animatable.Deserialize(objData.animatable);
            }

            switch (type)
            {
                case ObjectType.Primitive:
                case ObjectType.Model:
                case ObjectType.Road:
                    SetObstacleType(objData.obstacleType);
                    SetCollideMode(objData.canCollide);
                    break;
                case ObjectType.Empty:
                    SetObstacleType(objData.obstacleType);
                    break;
            }

            SetupColorAction();

            SetVisibility(objData.visibility);
            LevelManager.afterLoad += () =>
            {
                if (objData.parentId is <= -1 or 1) return;

                var parent = LevelManager.GetObject(objData.parentId);
                if (!parent) return;
                transform.SetParent(parent.transform, false);
                defaultParent = parent.transform;
            };
        }

        /// <summary>
        /// Use this instead of manually changing <see cref="spaceType"/>
        /// </summary>
        public void SetSpaceType(SpaceType targetType)
        {
            if (spaceType == targetType) return;

            spaceType = targetType;

            transform.SetParent(
                spaceType == SpaceType.Screen
                    ? LevelManager.Instance.screenTransform
                    : LevelManager.Instance.environmentTransform, false);
        }

        public void AdjustParentType()
        {
            // var previousSpaceType = spaceType;

            var parent = transform.parent;
            if (!parent) return;

            var pInfo = parent.GetInfo();
            if (pInfo)
                spaceType = pInfo.spaceType;
            else
                spaceType = parent == LevelManager.Instance.screenTransform ? SpaceType.Screen : SpaceType.World;

            // TODO: Decide if we want to keep this behavior
            /*if (previousSpaceType != spaceType)
                References.Inspector.UpdateValues();*/
        }

        public void ChangeParent(Transform parent)
        {
            transform.SetParent(parent);
        }

        /// <summary>
        /// A way to change the collidable state of the object.
        /// This will not work if the object is an obstacle, since it will always have to be collidable, use Float or Wall instead
        /// </summary>
        public void SetCollideMode(bool to)
        {
            if (obstacleType != ObstacleType.None) return;

            canCollide = to;
            var currentCollider = GetComponent<Collider>();
            if (currentCollider)
            {
                CarefullySetIsTrigger(currentCollider, !to);
                gameObject.layer = to ? LevelManager.Instance.normalLayer : LevelManager.Instance.passthroughLayer;
            }
            serializer.OnCollidableChanged(to);
        }

        /// <summary>
        /// Use this instead of manually changing <see cref="obstacleType"/>.
        /// This will also change the tag, layer, collider and collidable state of the object
        /// </summary>
        public void SetObstacleType(ObstacleType targetType)
        {
            obstacleType = targetType;
            switch (targetType)
            {
                case ObstacleType.None:
                    tag = "Untagged";
                    var currentCollider = GetComponent<Collider>();
                    CarefullySetIsTrigger(currentCollider, false);
                    if (currentCollider) currentCollider.isTrigger = false;
                    gameObject.layer = LevelManager.Instance.normalLayer;
                    SetCollideMode(true);
                    break;
                case ObstacleType.Wall:
                    tag = "Obstacle-Wall";
                    currentCollider = GetComponent<Collider>();
                    CarefullySetIsTrigger(currentCollider, false);
                    gameObject.layer = LevelManager.Instance.normalLayer;
                    SetCollideMode(true);
                    break;
                case ObstacleType.PassThrough:
                    tag = "Obstacle-Float";
                    currentCollider = GetComponent<Collider>();
                    CarefullySetIsTrigger(currentCollider, true);
                    gameObject.layer = LevelManager.Instance.floatingObstacleLayer;
                    SetCollideMode(false);
                    break;
                default:
                    tag = "Obstacle-Water";
                    currentCollider = GetComponent<Collider>();
                    CarefullySetIsTrigger(currentCollider, true);
                    gameObject.layer = LevelManager.Instance.floatingObstacleLayer;
                    SetCollideMode(false);
                    break;
            }
        }

        /// <summary>
        /// Use this instead of manually changing <see cref="visibility"/>.
        /// This will change the active state of the object or the enabled state of renderers based on the serializers
        /// </summary>
        public void SetVisibility(VisibilityType targetType)
        {
            if (state == ObjectLevel.BuiltIn)
            {
                if (gameObject.CompareTag("Player"))
                {
                    serializer.OnVisibilityChange(targetType);
                    return;
                }

                visibility = VisibilityType.Shown;
                return;
            }

            visibility = targetType;
            switch (targetType)
            {
                case VisibilityType.Shown:
                case VisibilityType.Hidden:
                    gameObject.SetActive(true);
                    break;
                case VisibilityType.Gone:
                    gameObject.SetActive(false);
                    break;
            }
            serializer.OnVisibilityChange(targetType);
        }

        private void OnCollisionEnter(Collision collision) {
            if (collision.gameObject.CompareTag("Player")) {
                if (obstacleType == ObstacleType.Water) {
                    var meshCollider = GetComponent<MeshCollider>();
                    if (meshCollider) {
                        meshCollider.convex = true;
                        meshCollider.isTrigger = true;

                        // HACK: Very weird workaround for PhysX support with non-convex isTrigger collision
                        // collision.gameObject.GetComponent<Rigidbody>().linearVelocity = LevelManager.Instance.player.previousVelocity;
                    }
                }
            }
        }

        /// <summary>
        /// HACK: A workaround for MeshCollider.isTrigger not being supported in PhysX
        /// </summary>
        private void CarefullySetIsTrigger(Collider col, bool to)
        {
            if (!col) return;

            if (col is MeshCollider)
            {
                //Debug.LogWarning("The level creator has set an obstacle type of water on a non-primitive object, the support for this is still experimental and may be buggy", gameObject);
            }
            else
            {
                col.isTrigger = to;
            }
        }

        /// <summary>
        /// HACK: Why do we do this? Can we just detect the type of the object or is something mischevious happening here?
        /// </summary>
        private void FixReferences()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _renderer = GetComponent<MeshRenderer>();
            _light = GetComponent<Light>();
            _text = GetComponent<TextMeshPro>();
        }

        private Func<Color> _getColor;
        private Action<Color> _setColor;
        public void SetupColorAction() {
            FixReferences();
            switch (type) {
                case ObjectType.Light:
                    _getColor = () => _light.color;
                    _setColor = val => _light.color = val;
                    break;
                case ObjectType.Primitive:
                case ObjectType.Model:
                case ObjectType.Road:
                case ObjectType.Player:
                    _getColor = () => _renderer.sharedMaterial.color;
                    _setColor = val => _renderer.sharedMaterial.color = val;
                    break;
                case ObjectType.Text:
                    _getColor = () => _text.color;
                    _setColor = val => _text.color = val;
                    break;
                case ObjectType.Sprite:
                    _getColor = () => _spriteRenderer.color;
                    _setColor = val => _spriteRenderer.color = val;
                    break;
            }
        }

        public void CloneMaterial() {
            switch (type) {
                case ObjectType.Primitive:
                case ObjectType.Model:
                case ObjectType.Road:
                case ObjectType.Player:
                    _renderer.sharedMaterial = new(_renderer.sharedMaterial);
                    break;
            }
        }

        public Color GetColor() {
            if (_getColor == null)
                return Color.white;
            else
                return _getColor();
        }

        public void SetColor(Color to) {
            _setColor?.Invoke(to);
        }

        public void KillTween(LTDescr descr) {
            if (descr != null)
                LeanTween.cancel(descr.uniqueId);
        }

        public void AvoidColorLeak() {
            if (_materialCloned) return;

            CloneMaterial();
            _materialCloned = true;
        }
    }

    public enum ObjectLevel {
        BuiltIn,
        Project,
        New,
        Default
    }
}