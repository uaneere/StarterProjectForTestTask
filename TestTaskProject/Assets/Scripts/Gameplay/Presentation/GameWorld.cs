using Gameplay.Brewing;
using Gameplay.Data;
using Gameplay.Interaction;
using Gameplay.Inventory;
using Gameplay.Items;
using Gameplay.Minigame;
using Gameplay.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using static EventsProvider;

namespace Gameplay.Presentation
{
    public class GameWorld : MonoBehaviour
    {
        private const float QteTime = 8f;
        private const float InteractRadius = 2.2f;

        private GameContent _content;
        private HandInventory _hands;
        private Cabinet _cabinet;
        private Cauldron _cauldron;
        private IngredientProcessor _processor;
        private PlayerHealth _health;
        private EventManager _eventManager;

        private MageController _mage;
        private GameHud _hud;
        private Transform _cabinetPoint;
        private Transform _cauldronPoint;
        private Transform _tablePoint;
        private SpriteRenderer _tableItemView;
        private Item _tableItem;

        private bool _cabinetOpen;
        private QteSession _qte;
        private Ingredient _qteIngredient;
        private float _statusUntil;
        private InteractableType? _nearby;
        private bool _paused;

        [Inject]
        public void Construct(
            GameContent content,
            HandInventory hands,
            Cabinet cabinet,
            Cauldron cauldron,
            IngredientProcessor processor,
            PlayerHealth health,
            EventManager eventManager)
        {
            _content = content;
            _hands = hands;
            _cabinet = cabinet;
            _cauldron = cauldron;
            _processor = processor;
            _health = health;
            BindEvents(eventManager);
        }

        private void BindEvents(EventManager eventManager)
        {
            UnbindEvents();
            _eventManager = eventManager;
            _eventManager.Subscribe<GamePauseChangedEvent>(OnPauseChanged);
            _eventManager.Subscribe<CabinetSlotClickedEvent>(OnCabinetSlotClicked);
            _eventManager.Subscribe<FinishPotionRequestedEvent>(OnFinishPotionRequested);
        }

        private void UnbindEvents()
        {
            if (_eventManager == null)
                return;

            _eventManager.Unsubscribe<GamePauseChangedEvent>(OnPauseChanged);
            _eventManager.Unsubscribe<CabinetSlotClickedEvent>(OnCabinetSlotClicked);
            _eventManager.Unsubscribe<FinishPotionRequestedEvent>(OnFinishPotionRequested);
        }

        private void OnDestroy()
        {
            UnbindEvents();
            Time.timeScale = 1f;
        }

        private void Start()
        {
            SetupCamera();
            BuildRoom();
            _cabinet.Fill(_content.CreateCabinetStock());
            _content.RerollRecipe();

            var hudObject = new GameObject("HUD");
            hudObject.transform.SetParent(transform, false);
            _hud = hudObject.AddComponent<GameHud>();
            _hud.Build(_eventManager);
            _hud.SetRecipeHint(_content.FormatCurrentRecipe());
        }

        private void Update()
        {
            if (_paused)
            {
                if (WasPressed(Key.Escape))
                    _eventManager.Publish(new CloseScreenEvent());
                return;
            }

            if (_health.IsDead)
            {
                _mage.CanMove = false;
                RefreshHud();
                if (WasPressed(Key.R))
                    Restart();
                return;
            }

            if (Time.time >= _statusUntil)
                _hud.SetStatus(string.Empty);

            if (_qte != null)
            {
                _mage.CanMove = false;
                UpdateQte();
                RefreshHud();
                return;
            }

            if (_cabinetOpen)
            {
                _mage.CanMove = false;
                if (WasPressed(Key.Escape))
                    CloseCabinet();
                RefreshHud();
                return;
            }

            _mage.CanMove = true;
            _nearby = FindNearby();

            if (WasPressed(Key.Escape))
            {
                _eventManager.Publish(new OpenScreenEvent(ScreenIds.Pause));
                return;
            }

            if (WasPressed(Key.E))
                Interact();

            if (WasPressed(Key.F) && _nearby == InteractableType.Cauldron)
                TryFinishPotion();

            RefreshHud();
        }

        private void Interact()
        {
            if (_nearby == InteractableType.Cabinet)
            {
                _cabinetOpen = true;
                _hud.SetCabinetOpen(true);
                _hud.RefreshCabinet(_cabinet);
                return;
            }

            if (_nearby == InteractableType.Table)
            {
                UseTable();
                return;
            }

            if (_nearby == InteractableType.Cauldron)
                TryStartQte();
        }

        private void UseTable()
        {
            if (_tableItem == null)
            {
                if (_hands.HeldItem is not Potion)
                {
                    ShowStatus("На стол можно поставить только готовое зелье");
                    return;
                }

                _tableItem = _hands.Remove();
                RefreshTableView();
                ShowStatus($"{_tableItem.Name} стоит на столе");
                return;
            }

            if (!_hands.IsEmpty)
            {
                ShowStatus("Руки заняты");
                return;
            }

            _hands.TryTake(_tableItem);
            _tableItem = null;
            RefreshTableView();
            ShowStatus("Зелье взято со стола");
        }

        private void RefreshTableView()
        {
            if (_tableItemView == null)
                return;

            _tableItemView.enabled = _tableItem != null && _tableItem.Icon != null;
            if (_tableItem != null)
                _tableItemView.sprite = _tableItem.Icon;
        }

        private void TryStartQte()
        {
            if (_hands.HeldItem is not Ingredient ingredient)
            {
                ShowStatus("В руках нет ингредиента");
                return;
            }

            _qteIngredient = ingredient;
            _qte = new QteSession(ingredient.QtePath1, ingredient.QtePath2, QteTime);
            _hud.SetQteVisible(true);
            _hud.RefreshQte(ingredient, _qte);
        }

        private void UpdateQte()
        {
            if (WasPressed(Key.Escape))
            {
                CompleteQte(QteResult.Failed);
                return;
            }

            var input = ReadArrow();
            if (input.HasValue)
            {
                var submitted = _qte.Submit(input.Value);
                if (submitted.HasValue)
                {
                    CompleteQte(submitted.Value);
                    return;
                }
            }

            var timedOut = _qte.Tick(Time.deltaTime);
            if (timedOut.HasValue)
            {
                CompleteQte(timedOut.Value);
                return;
            }

            _hud.RefreshQte(_qteIngredient, _qte);
        }

        private void CompleteQte(QteResult result)
        {
            var processed = _processor.Process(_qteIngredient, result);
            _hands.Remove();
            _cauldron.AddIngredient(processed);

            var reason = result == QteResult.Failed ? "провал, случайный эффект" : "успех";
            ShowStatus($"{processed.Ingredient.Name} → {processed.Effect} ({reason})");

            _qte = null;
            _qteIngredient = null;
            _hud.SetQteVisible(false);
        }

        private void TryFinishPotion()
        {
            if (_nearby != InteractableType.Cauldron)
                return;

            if (_cauldron.Contents.Count == 0)
            {
                ShowStatus("Котёл пуст");
                return;
            }

            if (!_hands.IsEmpty)
            {
                ShowStatus("Освободите руки, чтобы взять зелье");
                return;
            }

            if (_cauldron.TryFinish(_content.Recipes, out var potion))
            {
                _hands.TryTake(potion);
                ShowStatus($"Готово: {potion.Name} [{potion.FormatEffects()}]");
                return;
            }

            _health.Damage();
            ShowStatus(_health.IsDead
                ? "Зелье взорвалось. Маг падает"
                : "Неверная последовательность — взрыв! -1 сердце");
        }

        private void OnCabinetSlotClicked(CabinetSlotClickedEvent slotEvent)
        {
            OnCabinetSlotClicked(slotEvent.Index);
        }

        private void OnFinishPotionRequested(FinishPotionRequestedEvent _)
        {
            TryFinishPotion();
        }

        private void OnPauseChanged(GamePauseChangedEvent pauseEvent)
        {
            _paused = pauseEvent.IsPaused;
            Time.timeScale = pauseEvent.IsPaused ? 0f : 1f;
            if (_mage != null)
                _mage.CanMove = !pauseEvent.IsPaused;
            if (_hud != null)
                _hud.SetWorldUiInteractable(!pauseEvent.IsPaused);
        }

        private void OnCabinetSlotClicked(int index)
        {
            if (_cabinet.GetSlot(index) != null)
            {
                if (_cabinet.TryTake(index, _hands))
                    _hud.RefreshCabinet(_cabinet);
                else
                    ShowStatus("Руки заняты");
                return;
            }

            if (_cabinet.TryStoreFromHands(index, _hands))
                _hud.RefreshCabinet(_cabinet);
        }

        private void CloseCabinet()
        {
            _cabinetOpen = false;
            _hud.SetCabinetOpen(false);
        }

        private InteractableType? FindNearby()
        {
            if (Vector2.Distance(_mage.transform.position, _cabinetPoint.position) <= InteractRadius)
                return InteractableType.Cabinet;

            if (Vector2.Distance(_mage.transform.position, _cauldronPoint.position) <= InteractRadius)
                return InteractableType.Cauldron;

            if (_tablePoint != null &&
                Vector2.Distance(_mage.transform.position, _tablePoint.position) <= InteractRadius)
                return InteractableType.Table;

            return null;
        }

        private void RefreshHud()
        {
            _hud.Refresh(_health, _hands, _cauldron, _nearby, _cabinetOpen, _qte != null);
        }

        private void Restart()
        {
            _paused = false;
            Time.timeScale = 1f;
            _nearby = null;
            _health.Reset();
            _hands.Clear();
            _cauldron.Clear();
            _cabinet.Fill(_content.CreateCabinetStock());
            _content.RerollRecipe();
            _hud.SetRecipeHint(_content.FormatCurrentRecipe());
            _cabinetOpen = false;
            _qte = null;
            _qteIngredient = null;
            _hud.SetCabinetOpen(false);
            _hud.SetQteVisible(false);
            _tableItem = null;
            RefreshTableView();
            _mage.transform.position = Vector3.zero;
            if (_mage != null)
            {
                _mage.CanMove = true;
                _mage.StopMovement();
            }

            ShowStatus("Новая попытка");
        }

        private void ShowStatus(string text)
        {
            _hud.SetStatus(text);
            _statusUntil = Time.time + 3.5f;
        }

        private void SetupCamera()
        {
            var camera = Camera.main;
            if (camera == null)
                return;

            camera.orthographic = true;
            camera.orthographicSize = 3.5f;
            camera.transform.position = new Vector3(0f, 0f, -10f);
            camera.backgroundColor = new Color(0.12f, 0.11f, 0.14f);
            camera.clearFlags = CameraClearFlags.SolidColor;
        }

        private void BuildRoom()
        {
            var camera = Camera.main;
            var background = CreateProp("Background", Vector3.zero, VisualCatalog.BackgroundRoom, 12f, -2, false, false);
            if (camera != null && background.GetComponent<SpriteRenderer>().sprite != null)
                FitToCamera(background.GetComponent<SpriteRenderer>(), camera);

            CreateInvisibleWall("WallTop", new Vector2(0f, 5.4f), new Vector2(18f, 5f));
            CreateInvisibleWall("WallBottom", new Vector2(0f, -5.4f), new Vector2(18f, 4.5f));
            CreateInvisibleWall("WallLeft", new Vector2(-9.2f, 0f), new Vector2(6f, 11f));
            CreateInvisibleWall("WallRight", new Vector2(9.2f, 0f), new Vector2(6f, 11f));

            var chest = CreateProp("Chest", new Vector3(0f, 3.6f, 0f), VisualCatalog.Chest, 2.2f, 2, true, true);
            chest.AddComponent<Interactable>().Type = InteractableType.Cabinet;
            _cabinetPoint = chest.transform;

            var cauldron = CreateProp("Cauldron", new Vector3(6.1f, 0.1f, 0f), VisualCatalog.Cauldron, 2.4f, 2, true, true);
            cauldron.AddComponent<Interactable>().Type = InteractableType.Cauldron;
            var burn = cauldron.AddComponent<SpriteAnimator>();
            burn.SetFrames(VisualCatalog.CauldronBurnFrames, 0.7f);
            _cauldronPoint = cauldron.transform;

            CreateProp("Bed", new Vector3(-6.1f, 0.2f, 0f), VisualCatalog.Bed, 2.6f, 2, true, true);

            var table = CreateProp("Table", new Vector3(0f, -3.5f, 0f), VisualCatalog.Table, 2.1f, 2, true, true);
            table.AddComponent<Interactable>().Type = InteractableType.Table;
            _tablePoint = table.transform;
            var itemView = new GameObject("TableItem");
            itemView.transform.SetParent(table.transform, false);
            itemView.transform.localPosition = new Vector3(0f, 0.35f, 0f);
            itemView.transform.localScale = Vector3.one * 0.45f;
            _tableItemView = itemView.AddComponent<SpriteRenderer>();
            _tableItemView.sortingOrder = 3;
            _tableItemView.enabled = false;

            var mage = CreateProp("Mage", Vector3.zero, SpriteLoader.Load("MCstand", new Color(0.45f, 0.35f, 0.72f)), 1.7f, 8, true, false);
            var body = mage.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _mage = mage.AddComponent<MageController>();
            mage.AddComponent<MageWalkAnimator>();
        }

        private static GameObject CreateProp(
            string name,
            Vector3 position,
            Sprite sprite,
            float worldHeight,
            int order,
            bool collider,
            bool isTrigger)
        {
            var go = new GameObject(name);
            go.transform.position = position;
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = order;
            if (sprite != null && sprite.bounds.size.y > 0.001f)
                go.transform.localScale = Vector3.one * (worldHeight / sprite.bounds.size.y);

            if (collider)
            {
                var box = go.AddComponent<BoxCollider2D>();
                box.isTrigger = isTrigger;
            }

            return go;
        }

        private static void FitToCamera(SpriteRenderer renderer, Camera camera)
        {
            var sprite = renderer.sprite;
            if (sprite == null)
                return;

            var height = camera.orthographicSize * 2f;
            var width = height * camera.aspect;
            var size = sprite.bounds.size;
            renderer.transform.localScale = new Vector3(width / size.x, height / size.y, 1f);
        }

        private static void CreateInvisibleWall(string name, Vector2 position, Vector2 size)
        {
            var go = new GameObject(name);
            go.transform.position = position;
            var box = go.AddComponent<BoxCollider2D>();
            box.size = size;
            var body = go.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Static;
        }

        private static bool WasPressed(Key key)
        {
            var keyboard = Keyboard.current;
            return keyboard != null && keyboard[key].wasPressedThisFrame;
        }

        private static Arrow? ReadArrow()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null)
                return null;

            if (keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame)
                return Arrow.Up;
            if (keyboard.sKey.wasPressedThisFrame || keyboard.downArrowKey.wasPressedThisFrame)
                return Arrow.Down;
            if (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame)
                return Arrow.Left;
            if (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame)
                return Arrow.Right;

            return null;
        }
    }
}