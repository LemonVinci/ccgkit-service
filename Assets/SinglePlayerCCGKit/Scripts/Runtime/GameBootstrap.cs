// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;

namespace CCGKit
{
    /// <summary>
    /// Bootstraps the game scene: player character (visual), deck, dish, hand UI, and turn loop. No combat or enemies.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
#pragma warning disable 649
        [Header("Configuration")]
        [SerializeField]
        private PlayableCharacterConfiguration playerConfig;
        [SerializeField]
        private AssetReference characterTemplate;

        [Header("Systems")]
        [SerializeField]
        private DeckDrawingSystem deckDrawingSystem;
        [SerializeField]
        private HandPresentationSystem handPresentationSystem;
        [SerializeField]
        private ManaResetSystem manaResetSystem;
        [SerializeField]
        private TurnManagementSystem turnManagementSystem;
        [SerializeField]
        private CardWithArrowSelectionSystem cardWithArrowSelectionSystem;

        [Header("Character pivots")]
        [SerializeField]
        public Transform playerPivot;

        [Header("UI")]
        [SerializeField]
        private ManaWidget manaWidget;
        [SerializeField]
        private DeckWidget deckWidget;
        [SerializeField]
        private DiscardPileWidget discardPileWidget;
        [SerializeField]
        private EndTurnButton endTurnButton;
        [SerializeField]
        private SpriteRenderer background;

        [Header("Pools")]
        [SerializeField]
        private ObjectPool cardPool;
#pragma warning restore 649

        private Camera mainCamera;
        private List<CardTemplate> playerDeck = new List<CardTemplate>();
        private GameObject player;
        private GameInfo gameInfo;
        private readonly string saveDataPrefKey = "save";
        private int numAssetsToLoad;
        private int numAssetsLoaded;

        private void Start()
        {
            mainCamera = Camera.main;
            cardPool.Initialize();

            Addressables.InitializeAsync().Completed += op =>
            {
                CreatePlayer(characterTemplate);
                numAssetsToLoad = 1;

                gameInfo = FindFirstObjectByType<GameInfo>();
                if (gameInfo != null)
                {
                    var dish = gameInfo.Encounter != null ? gameInfo.Encounter.Dish : null;
                    var dishSystem = FindFirstObjectByType<DishSystem>();
                    if (dishSystem != null)
                    {
                        if (gameInfo.HasBattleTestConfig)
                        {
                            dishSystem.ApplyBattleTestSettings(gameInfo.BattleTest);
                            var stationDishes = BattleTestDishResolver.ResolveStations(gameInfo.BattleTest, dish);
                            dishSystem.InitializeStations(stationDishes);
                        }
                        else
                        {
                            dishSystem.InitializeWithDish(dish);
                        }
                    }

                    if (gameInfo.Encounter != null && gameInfo.Encounter.Background != null)
                        background.sprite = gameInfo.Encounter.Background;
                }
            };
        }

        private void CreatePlayer(AssetReference templateRef)
        {
            var handle = Addressables.LoadAssetAsync<HeroTemplate>(templateRef);
            handle.Completed += op =>
            {
                var template = op.Result;
                player = Instantiate(template.Prefab, playerPivot);
                Assert.IsNotNull(player);
                player.SetActive(false);
                if (playerPivot != null)
                    playerPivot.gameObject.SetActive(false);

                var hp = playerConfig.Hp;
                var mana = playerConfig.Mana;
                var shield = playerConfig.Shield;
                hp.Value = template.Hp;
                mana.Value = template.Mana;
                shield.Value = 0;
                manaResetSystem.SetDefaultMana(template.Mana);

                var sessionInfo = FindFirstObjectByType<GameInfo>();
                var useStartingDeck = sessionInfo != null && sessionInfo.UseStartingDeck;
                if (sessionInfo != null)
                    sessionInfo.UseStartingDeck = false;

                hp.Value = template.Hp;
                shield.Value = 0;
                BuildPlayerDeck(template, sessionInfo, useStartingDeck);

                var gi = FindFirstObjectByType<GameInfo>();
                if (gi != null)
                {
                    gi.SaveData.Hp = hp.Value;
                    gi.SaveData.Shield = shield.Value;
                    gi.SaveData.Deck.Clear();
                    foreach (var card in playerDeck)
                        gi.SaveData.Deck.Add(card.Id);
                }

                manaWidget.Initialize(mana);

                var obj = player.GetComponent<CharacterObject>();
                obj.Template = template;
                obj.Character = new RuntimeCharacter
                {
                    Hp = hp,
                    Shield = shield,
                    Mana = mana,
                    Status = playerConfig.Status,
                    MaxHp = template.Hp
                };
                obj.Character.Status.Value.Clear();

                numAssetsLoaded++;
                InitializeGame();
            };
        }

        private void BuildPlayerDeck(HeroTemplate template, GameInfo sessionInfo, bool useStartingDeck)
        {
            playerDeck.Clear();
            var test = sessionInfo != null && sessionInfo.HasBattleTestConfig ? sessionInfo.BattleTest : null;

            if (test != null)
            {
                switch (test.DeckMode)
                {
                    case DeckTestMode.SavedDeck:
                        if (!TryLoadDeckFromSave(template))
                            FillDeckFromStartingLibrary(template);
                        break;
                    default:
                        FillDeckFromStartingLibrary(template);
                        break;
                }

                ApplyDeckTestFilters(test);
                return;
            }

            if (useStartingDeck)
            {
                FillDeckFromStartingLibrary(template);
                return;
            }

            if (PlayerPrefs.HasKey(saveDataPrefKey))
            {
                var json = PlayerPrefs.GetString(saveDataPrefKey);
                var saveData = JsonUtility.FromJson<SaveData>(json);
                if (!TryLoadDeckFromSaveData(template, saveData))
                    FillDeckFromStartingLibrary(template);
                return;
            }

            FillDeckFromStartingLibrary(template);
        }

        private bool TryLoadDeckFromSave(HeroTemplate template)
        {
            if (!PlayerPrefs.HasKey(saveDataPrefKey))
                return false;
            var json = PlayerPrefs.GetString(saveDataPrefKey);
            var saveData = JsonUtility.FromJson<SaveData>(json);
            return TryLoadDeckFromSaveData(template, saveData);
        }

        private bool TryLoadDeckFromSaveData(HeroTemplate template, SaveData saveData)
        {
            playerDeck.Clear();
            if (saveData?.Deck == null)
                return false;

            foreach (var id in saveData.Deck)
            {
                var card = template.StartingDeck.Entries.Find(x => x.Card != null && x.Card.Id == id);
                if (card == null && template.RewardDeck != null)
                    card = template.RewardDeck.Entries.Find(x => x.Card != null && x.Card.Id == id);
                if (card != null)
                    playerDeck.Add(card.Card);
            }

            if (playerDeck.Count == 0 || playerDeck.Count != saveData.Deck.Count)
                return false;

            return true;
        }

        private void ApplyDeckTestFilters(BattleTestSettings test)
        {
            if (playerDeck.Count == 0)
                return;

            switch (test.DeckMode)
            {
                case DeckTestMode.SweetIngredientsOnly:
                    playerDeck.RemoveAll(c => c == null || c.Suit != IngredientSuit.Sweet);
                    break;
                case DeckTestMode.HighValueOnly:
                    playerDeck.RemoveAll(c => c == null || c.Value < 4);
                    break;
                case DeckTestMode.ShuffledSubset:
                {
                    playerDeck.Shuffle();
                    var take = Mathf.Clamp(test.ShuffledSubsetSize, 1, playerDeck.Count);
                    var subset = playerDeck.GetRange(0, take);
                    playerDeck.Clear();
                    playerDeck.AddRange(subset);
                    break;
                }
            }
        }

        private void FillDeckFromStartingLibrary(HeroTemplate template)
        {
            playerDeck.Clear();
            if (template.StartingDeck == null)
                return;

            foreach (var entry in template.StartingDeck.Entries)
            {
                if (entry == null || entry.Card == null)
                    continue;
                for (var i = 0; i < entry.NumCopies; i++)
                    playerDeck.Add(entry.Card);
            }
        }

        private void InitializeGame()
        {
            if (numAssetsLoaded != numAssetsToLoad)
                return;

            deckDrawingSystem.Initialize(deckWidget, discardPileWidget);
            var deckSize = deckDrawingSystem.LoadDeck(playerDeck);
            deckDrawingSystem.ShuffleDeck();

            handPresentationSystem.Initialize(cardPool, deckWidget, discardPileWidget);

            var playerCharacter = player.GetComponent<CharacterObject>();
            var noEnemies = new List<CharacterObject>(0);
            cardWithArrowSelectionSystem.Initialize(playerCharacter, noEnemies);
            turnManagementSystem.BeginGame();
        }
    }
}
