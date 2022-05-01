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
        Fall,
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
    public EnemyPool[] Enemies;

    public Vector2 PointZero;
    public Vector2 PositionMultiplier;
    public int MaximumXDistance;
    public int MaximumYDistance;
    public float BonusXDistancePerY;

    private Vector2Int _currentPosition;
    private DefaultGenerator _rng;
    private float[] _platformColors;

    private Vector2Int _levelSize = new Vector2Int(15, 10);

    private readonly Vector3 _enemyOffset = new Vector3(.5f, .75f);

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
            _currentPosition = new Vector2Int(platform.Width, platform.Height - 1);
        }

        var intentions = EnumDice<DesignIntention>.GenerateFromEnum();

        int attempts = 0;
        while (_currentPosition.x < _levelSize.x)
        {
            var intentionSelection = new WeightedDice<EnumDice<DesignIntention>>(PickPossibleIntentions(intentions), _rng);
            var result = intentionSelection.Generate();
            GenerateSection(result.Value);
            attempts++;
            if (attempts > 32) break;
        }
    }

    private EnumDice<DesignIntention>[] PickPossibleIntentions(EnumDice<DesignIntention>[] intentions)
    {
        return intentions.Where(intention =>
            {
                return intention.Value switch
                {
                    DesignIntention.Gap => _currentPosition.x < _levelSize.x - 2,
                    DesignIntention.Level => true,
                    DesignIntention.Climb => _currentPosition.y < _levelSize.y,
                    DesignIntention.Fall => _currentPosition.y > 0,
                    _ => false
                };
            }
        ).ToArray();
    }

    private PlatformPool[] PickPossiblePlatforms(PlatformPool[] platforms)
    {
        return platforms.Where(platformDefinition =>
            platformDefinition.Platform.Width <= _levelSize.x - _currentPosition.x
        ).ToArray();
    }

    private EnemyPool[] PickPossibleEnemies(EnemyPool[] enemies, Platform platform)
    {
        return enemies.Where(enemyDefinition => platform.AllowsEnemies.Intersect(enemyDefinition.Enemy.Tags).Count() ==
                                                enemyDefinition.Enemy.Tags.Length).ToArray();
    }

    private void GenerateSection(DesignIntention intention)
    {
        var currentPosition = _currentPosition;
        Platform platform = null;
        switch (intention)
        {
            case DesignIntention.Level:
                platform = GenerateLeveledSection();
                break;
            case DesignIntention.Climb:
                platform = GenerateClimbSection();
                break;
            case DesignIntention.Fall:
                platform = GenerateFallSection();
                break;
            case DesignIntention.Gap:
                platform = GenerateGapSection();
                break;
        }

        if (platform == null) return;

        GenerateEnemies(currentPosition, platform);

    }

    private void GenerateEnemies(Vector2Int position, Platform platform)
    {
        if (platform.AllowsEnemies.Length == 0) return;

        // % of enemies based on difficulty?

        if (_rng.Generate() < 0.75f) return;

        var enemySelection = new WeightedDice<EnemyPool>(PickPossibleEnemies(Enemies, platform), _rng);
        var pool = enemySelection.Generate();

        if (pool.TryGetEnemy(out var enemy))
        {
            enemy.transform.position = platform.transform.position + _enemyOffset;
        }
    }

    private bool TryGeneratePlatform(out Platform platform)
    {
        var platformSelection = new WeightedDice<PlatformPool>(PickPossiblePlatforms(Platforms), _rng);
        var pool = platformSelection.Generate();
        
        if (pool.TryGetPlatform(out platform))
        {
            platform.SpriteRenderer.material.SetFloat("_Hue", _platformColors.First());
            return true;
        }

        platform = null;
        return false;
    }

    private Platform GenerateGapSection()
    {
        if (!TryGeneratePlatform(out var platform)) return null;
        _currentPosition += new Vector2Int(_rng.GenerateRange(1, MaximumXDistance), 0);
        PutPlatformInPosition(platform);
        return platform;
    }
    private Platform GenerateClimbSection()
    {
        if (!TryGeneratePlatform(out var platform)) return null;
        _currentPosition += new Vector2Int(0, _rng.GenerateRange(1, Mathf.Min(_levelSize.y - _currentPosition.y, MaximumYDistance)));
        PutPlatformInPosition(platform);
        return platform;
    }

    private Platform GenerateFallSection()
    {
        if (!TryGeneratePlatform(out var platform)) return null;
        _currentPosition -= new Vector2Int(0, _rng.GenerateRange(1, Mathf.Min(_currentPosition.y, MaximumYDistance)));
        PutPlatformInPosition(platform);
        return platform;
    }

    private Platform GenerateLeveledSection()
    {
        if (!TryGeneratePlatform(out var platform)) return null;
        PutPlatformInPosition(platform);
        return platform;
    }

    private void PutPlatformInPosition(Platform platform)
    {
        platform.transform.position = PointZero + _currentPosition * PositionMultiplier;
        _currentPosition += new Vector2Int( platform.Width, platform.Height - 1);
    }

}
