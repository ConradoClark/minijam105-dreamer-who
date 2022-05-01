using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Generation;
using Licht.Interfaces.Generation;
using Licht.Unity.Pooling;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class RoomGenerator : MonoBehaviour
{
    public enum RoomType
    {
        PlatformGauntlet,
        DreamingCharacter,
        EnemyGauntlet,
        Boss,
    }

    public enum RoomDifficulty
    {
        Easy,
        Medium,
        Hard
    }

    public enum DesignIntention
    {
        //Repeat,
        Level,
        Climb,
        //Fall,
        Gap,
    }

    public enum RoomThemes
    {

    }

    public class EnumDice<T> : IWeighted<float> where T : Enum
    {
        public float Weight { get; private set; }
        public T Value { get; private set; }

        public static EnumDice<T>[] GenerateFromEnum()
        {
            return Enum.GetValues(typeof(T)).OfType<T>().Select(d => new EnumDice<T>
            {
                Weight = 1,
                Value = d
            }).ToArray();
        }
    }

    public class GenericDice<T> : IWeighted<float>
    {
        public T Value { get; private set; }
        public float Weight { get; private set; }

        public static GenericDice<T> FromObject(T obj, float weight = 1f)
        {
            return new GenericDice<T>
            {
                Value = obj,
                Weight = weight
            };
        }
    }

    public string Seed;

    public Camera MainCamera;

    public PlatformPool[] Platforms;
    public Sprite[] PlatformSprites;
    public PrefabPool[] Enemies;

    public Vector2 PointZero;
    public Vector2 PositionMultiplier;
    public int MaximumXDistance;
    public int MaximumYDistance;
    public float BonusXDistancePerY;

    private Vector2 _currentPosition;
    private DefaultGenerator _rng;
    private float[] _platformColors;

    public class DefaultGenerator : IGenerator<int, float>
    {
        private readonly System.Random _random;
        public int Seed { get; set; }

        public float Generate()
        {
            return (float)_random.NextDouble();
        }

        public int GenerateRange(int minInclusive, int maxInclusive)
        {
            return _random.Next(minInclusive, maxInclusive + 1);
        }

        public DefaultGenerator(string seed)
        {
            Seed = seed.GetHashCode();
            _random = new System.Random(Seed.GetHashCode());
        }
    }

    void OnEnable()
    {
        GenerateRoom();
    }

    public void GenerateRoom()
    {
        _rng = new DefaultGenerator(Seed);

        _platformColors = Enumerable.Range(0, 5).Select(_ => _rng.Generate() * 255f).ToArray();

        // set the background color
        MainCamera.backgroundColor = Color.HSVToRGB(_rng.Generate(), 0.25f, 0.4f + _rng.Generate() * 0.4f);

        // Always create a platform at point zero (hero spawn)
        // (I gotta create a platform also at the end, in case you enter the room from the room to the right)

        var platformSelection = new WeightedDice<PlatformPool>(Platforms, _rng);
        var pool = platformSelection.Generate();

        if (pool.TryGetPlatform(out var platform))
        {
            platform.SpriteRenderer.material.SetFloat("_Hue", _platformColors.First());
            platform.transform.position = PointZero;
            _currentPosition = new Vector2(platform.Width, platform.Height - 1);
        }

        var intentions = EnumDice<DesignIntention>.GenerateFromEnum();

        int attempts = 0;
        while (_currentPosition.x < 16)
        {
            var intentionSelection = new WeightedDice<EnumDice<DesignIntention>>(intentions, _rng);
            var result = intentionSelection.Generate();
            GenerateSection(result.Value);
            attempts++;
            if (attempts > 32) break;
        }
    }

    private void GenerateSection(DesignIntention intention)
    {
        switch (intention)
        {
            case DesignIntention.Level:
                GenerateLeveledSection();
                break;
            case DesignIntention.Climb:
                GenerateClimbSection();
                break;
            case DesignIntention.Gap:
                GenerateGapSection();
                break;
        }
    }

    private void GenerateGapSection()
    {
        var platformSelection = new WeightedDice<PlatformPool>(Platforms, _rng);
        var pool = platformSelection.Generate();

        _currentPosition += new Vector2(_rng.GenerateRange(1, MaximumXDistance), 0);

        if (pool.TryGetPlatform(out var platform))
        {
            platform.SpriteRenderer.material.SetFloat("_Hue", _platformColors.First());
            PutPlatformInPosition(platform);
        }
    }


    private void GenerateClimbSection()
    {
        var platformSelection = new WeightedDice<PlatformPool>(Platforms, _rng);
        var pool = platformSelection.Generate();

        _currentPosition += new Vector2(0, _rng.GenerateRange(1, MaximumYDistance));

        if (pool.TryGetPlatform(out var platform))
        {
            platform.SpriteRenderer.material.SetFloat("_Hue", _platformColors.First());
            PutPlatformInPosition(platform);
        }
    }

    private void GenerateLeveledSection()
    {
        var platformSelection = new WeightedDice<PlatformPool>(Platforms, _rng);
        var pool = platformSelection.Generate();

        if (pool.TryGetPlatform(out var platform))
        {
            platform.SpriteRenderer.material.SetFloat("_Hue", _platformColors.First());
            PutPlatformInPosition(platform);
        }
    }

    private void PutPlatformInPosition(Platform platform)
    {
        platform.transform.position = PointZero + _currentPosition * PositionMultiplier;
        _currentPosition += new Vector2( platform.Width, platform.Height - 1);
    }

}
