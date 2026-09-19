using System;
using Gameplay.Brewing;
using Gameplay.Interaction;
using Gameplay.Inventory;
using Gameplay.Items;
using Gameplay.Minigame;
using Gameplay.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using static EventsProvider;

namespace Gameplay.Presentation
{
    public class GameHud : MonoBehaviour
    {
        private EventManager _eventManager;
        private GraphicRaycaster _raycaster;
        private Font _font;
        private Text _hearts;
        private Text _heldItem;
        private Text _hint;
        private Text _cauldron;
        private Text _status;
        private Text _recipe;
        private GameObject _cabinetPanel;
        private Image[] _cabinetIcons;
        private GameObject _qtePanel;
        private Text _qteTitle;
        private Text _qteTimer;
        private Transform _qtePath1;
        private Transform _qtePath2;
        private GameObject _finishButton;

        public void Build(EventManager eventManager)
        {
            _eventManager = eventManager;
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (_font == null)
                _font = Font.CreateDynamicFontFromOSFont("Arial", 32);

            if (FindFirstObjectByType<EventSystem>() == null)
            {
                var eventSystem = new GameObject("EventSystem");
                eventSystem.transform.SetParent(transform, false);
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<InputSystemUIInputModule>();
            }

            var canvasObject = new GameObject("Canvas");
            canvasObject.transform.SetParent(transform, false);
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
            canvas.sortingOrder = 10;
            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.dynamicPixelsPerUnit = 4f;
            _raycaster = canvasObject.AddComponent<GraphicRaycaster>();

            _hearts = CreateLabel(canvasObject.transform, "Hearts", new Vector2(20, -16), 50, TextAnchor.UpperLeft);
            _heldItem = CreateLabel(canvasObject.transform, "Held", new Vector2(20, -48), 30, TextAnchor.UpperLeft);
            _recipe = CreateLabel(canvasObject.transform, "Recipe", new Vector2(20, -80), 30, TextAnchor.UpperLeft);
            _cauldron = CreateLabel(canvasObject.transform, "Cauldron", new Vector2(20, -140), 30, TextAnchor.UpperLeft);
            _hint = CreateLabel(canvasObject.transform, "Hint", new Vector2(0, 18), 20, TextAnchor.LowerCenter);
            _hint.rectTransform.anchorMin = new Vector2(0.5f, 0f);
            _hint.rectTransform.anchorMax = new Vector2(0.5f, 0f);
            _hint.rectTransform.pivot = new Vector2(0.5f, 0f);
            _hint.rectTransform.sizeDelta = new Vector2(980, 40);
            _status = CreateLabel(canvasObject.transform, "Status", new Vector2(0, -16), 20, TextAnchor.UpperCenter);
            _status.rectTransform.anchorMin = new Vector2(0.5f, 1f);
            _status.rectTransform.anchorMax = new Vector2(0.5f, 1f);
            _status.rectTransform.pivot = new Vector2(0.5f, 1f);
            _status.rectTransform.sizeDelta = new Vector2(800, 40);
            _status.text = string.Empty;

            _finishButton = CreateButton(
                canvasObject.transform,
                "Завершить зелье",
                new Vector2(-20, 18),
                new Vector2(180, 36),
                () => _eventManager.Publish(new FinishPotionRequestedEvent()));
            var finishRect = _finishButton.GetComponent<RectTransform>();
            finishRect.anchorMin = new Vector2(1f, 0f);
            finishRect.anchorMax = new Vector2(1f, 0f);
            finishRect.pivot = new Vector2(1f, 0f);
            _finishButton.SetActive(false);

            BuildCabinet(canvasObject.transform);
            BuildQte(canvasObject.transform);
        }

        public void Refresh(
            PlayerHealth health,
            HandInventory hands,
            Cauldron cauldron,
            InteractableType? nearby,
            bool cabinetOpen,
            bool qteActive)
        {
            _hearts.text = "Сердца: " + new string('♥', health.Hearts) + new string('♡', PlayerHealth.MaxHearts - health.Hearts);
            _heldItem.text = "В руках: " + FormatItem(hands.HeldItem);
            _cauldron.text = FormatCauldron(cauldron);
            _finishButton.SetActive(nearby == InteractableType.Cauldron && !cabinetOpen && !qteActive && cauldron.Contents.Count > 0);

            if (health.IsDead)
                _hint.text = "Маг без сил. R — начать заново";
            else if (qteActive)
                _hint.text = "Введите одну из двух последовательностей. WASD или стрелки. Esc — отмена";
            else if (cabinetOpen)
                _hint.text = "ЛКМ — взять или положить предмет. Esc — закрыть сундук";
            else if (nearby == InteractableType.Cabinet)
                _hint.text = "E — открыть сундук";
            else if (nearby == InteractableType.Cauldron)
                _hint.text = hands.HeldItem is Ingredient
                    ? "E — обработать ингредиент. F — завершить зелье"
                    : "F — завершить зелье";
            else if (nearby == InteractableType.Table)
                _hint.text = "E — поставить или взять зелье со стола";
            else
                _hint.text = "WASD / стрелки — ходьба. Esc — пауза";
        }

        public void SetWorldUiInteractable(bool interactable)
        {
            if (_raycaster != null)
                _raycaster.enabled = interactable;
        }

        public void SetRecipeHint(string text)
        {
            _recipe.text = text;
        }

        public void SetStatus(string text)
        {
            _status.text = text;
        }

        public void SetCabinetOpen(bool open)
        {
            _cabinetPanel.SetActive(open);
        }

        public void RefreshCabinet(Cabinet cabinet)
        {
            for (var i = 0; i < Cabinet.SlotCount; i++)
            {
                var item = cabinet.GetSlot(i);
                var icon = _cabinetIcons[i];
                icon.enabled = item != null && item.Icon != null;
                icon.sprite = item != null ? item.Icon : null;
                var label = icon.transform.parent.GetComponentInChildren<Text>();
                if (label != null)
                    label.text = item == null ? string.Empty : item.Name;
            }
        }

        public void SetQteVisible(bool visible)
        {
            if (!visible)
            {
                ClearChildren(_qtePath1);
                ClearChildren(_qtePath2);
            }

            _qtePanel.SetActive(visible);
        }

        public void RefreshQte(Ingredient ingredient, QteSession session)
        {
            _qteTitle.text = $"{ingredient.Name}: 1) {ingredient.Effect1}    2) {ingredient.Effect2}";
            FillPath(_qtePath1, session.Path1, session.Index1, session.Path1Alive);
            FillPath(_qtePath2, session.Path2, session.Index2, session.Path2Alive);
            _qteTimer.text = $"Время: {Mathf.CeilToInt(session.RemainingTime)}";
        }

        private void BuildCabinet(Transform parent)
        {
            _cabinetPanel = new GameObject("Cabinet");
            _cabinetPanel.transform.SetParent(parent, false);
            var background = _cabinetPanel.AddComponent<Image>();
            background.sprite = VisualCatalog.BackgroundChest;
            background.preserveAspect = false;
            var panelRect = _cabinetPanel.GetComponent<RectTransform>();
            panelRect.anchorMin = panelRect.anchorMax = panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(1100, 680);

            _cabinetIcons = new Image[Cabinet.SlotCount];
            const float cell = 88f;
            const float gap = 10f;
            var gridWidth = Cabinet.Columns * cell + (Cabinet.Columns - 1) * gap;
            var gridHeight = Cabinet.Rows * cell + (Cabinet.Rows - 1) * gap;
            var originX = -gridWidth / 2f + cell / 2f;
            var originY = gridHeight / 2f - cell / 2f - 20f;

            for (var i = 0; i < Cabinet.SlotCount; i++)
            {
                var index = i;
                var column = i % Cabinet.Columns;
                var row = i / Cabinet.Columns;
                var button = CreateButton(
                    _cabinetPanel.transform,
                    string.Empty,
                    new Vector2(originX + column * (cell + gap), originY - row * (cell + gap)),
                    new Vector2(cell, cell),
                    () => _eventManager.Publish(new CabinetSlotClickedEvent(index)));
                var rect = button.GetComponent<RectTransform>();
                rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);

                var iconGo = new GameObject("Icon");
                iconGo.transform.SetParent(button.transform, false);
                var icon = iconGo.AddComponent<Image>();
                icon.preserveAspect = true;
                icon.raycastTarget = false;
                var iconRect = icon.rectTransform;
                iconRect.anchorMin = new Vector2(0.12f, 0.28f);
                iconRect.anchorMax = new Vector2(0.88f, 0.92f);
                iconRect.offsetMin = iconRect.offsetMax = Vector2.zero;
                _cabinetIcons[i] = icon;

                var text = button.GetComponentInChildren<Text>();
                text.fontSize = 11;
                text.alignment = TextAnchor.LowerCenter;
            }

            _cabinetPanel.SetActive(false);
        }

        private void BuildQte(Transform parent)
        {
            _qtePanel = new GameObject("Qte");
            _qtePanel.transform.SetParent(parent, false);
            var panelImage = _qtePanel.AddComponent<Image>();
            panelImage.color = new Color(0f, 0f, 0f, 0.25f);
            var panelRect = _qtePanel.GetComponent<RectTransform>();
            panelRect.anchorMin = panelRect.anchorMax = panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(980, 420);

            _qteTitle = CreateLabel(_qtePanel.transform, "QteTitle", new Vector2(0, -12), 20, TextAnchor.UpperCenter);
            _qteTitle.rectTransform.anchorMin = new Vector2(0.5f, 1f);
            _qteTitle.rectTransform.anchorMax = new Vector2(0.5f, 1f);
            _qteTitle.rectTransform.pivot = new Vector2(0.5f, 1f);
            _qteTitle.rectTransform.sizeDelta = new Vector2(900, 40);

            _qtePath1 = CreateQteLane(_qtePanel.transform, new Vector2(0f, 50f));
            _qtePath2 = CreateQteLane(_qtePanel.transform, new Vector2(0f, -90f));

            _qteTimer = CreateLabel(_qtePanel.transform, "QteTimer", new Vector2(0, 16), 20, TextAnchor.LowerCenter);
            _qteTimer.rectTransform.anchorMin = new Vector2(0.5f, 0f);
            _qteTimer.rectTransform.anchorMax = new Vector2(0.5f, 0f);
            _qteTimer.rectTransform.pivot = new Vector2(0.5f, 0f);
            _qteTimer.rectTransform.sizeDelta = new Vector2(400, 36);
            _qtePanel.SetActive(false);
        }

        private static Transform CreateQteLane(Transform parent, Vector2 position)
        {
            var lane = new GameObject("Lane");
            lane.transform.SetParent(parent, false);
            var image = lane.AddComponent<Image>();
            image.sprite = VisualCatalog.BackgroundQte;
            image.preserveAspect = false;
            var rect = lane.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(860, 110);
            return lane.transform;
        }

        private static void ClearChildren(Transform parent)
        {
            if (parent == null)
                return;

            for (var i = parent.childCount - 1; i >= 0; i--)
            {
                var child = parent.GetChild(i).gameObject;
                if (Application.isPlaying)
                    Destroy(child);
                else
                    DestroyImmediate(child);
            }
        }

        private void FillPath(Transform lane, ArrowSequence sequence, int index, bool alive)
        {
            if (sequence == null)
                return;

            if (lane.childCount != sequence.Arrows.Length)
            {
                for (var i = lane.childCount - 1; i >= 0; i--)
                    Destroy(lane.GetChild(i).gameObject);

                var count = sequence.Arrows.Length;
                var width = 72f;
                var start = -((count - 1) * width) / 2f;
                for (var i = 0; i < count; i++)
                {
                    var arrow = new GameObject("Arrow");
                    arrow.transform.SetParent(lane, false);
                    var image = arrow.AddComponent<Image>();
                    image.sprite = VisualCatalog.ArrowRight;
                    image.preserveAspect = true;
                    image.raycastTarget = false;
                    arrow.transform.localRotation = Quaternion.Euler(0f, 0f, Rotation(sequence.Arrows[i]));
                    var rect = image.rectTransform;
                    rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
                    rect.sizeDelta = new Vector2(64, 64);
                    rect.anchoredPosition = new Vector2(start + i * width, 0f);
                }
            }

            for (var i = 0; i < sequence.Arrows.Length && i < lane.childCount; i++)
            {
                var image = lane.GetChild(i).GetComponent<Image>();
                if (image == null)
                    continue;

                image.color = !alive
                    ? new Color(0.45f, 0.45f, 0.45f, 1f)
                    : i < index
                        ? new Color(0.45f, 1f, 0.45f, 1f)
                        : i == index
                            ? new Color(1f, 0.9f, 0.35f, 1f)
                            : Color.white;
            }
        }

        private static float Rotation(Arrow arrow)
        {
            switch (arrow)
            {
                case Arrow.Up: return 90f;
                case Arrow.Left: return 180f;
                case Arrow.Down: return -90f;
                default: return 0f;
            }
        }

        private Text CreateLabel(Transform parent, string name, Vector2 anchored, int size, TextAnchor align)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var text = go.AddComponent<Text>();
            text.font = _font;
            text.fontSize = size;
            text.color = Color.white;
            var outline = go.AddComponent<Outline>();
            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(2, -2);
            text.alignment = align;
            text.supportRichText = true;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            var rect = text.rectTransform;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = anchored;
            rect.sizeDelta = new Vector2(560, 56);
            return text;
        }

        private GameObject CreateButton(Transform parent, string label, Vector2 pos, Vector2 size, Action click)
        {
            var go = new GameObject("Button");
            go.transform.SetParent(parent, false);
            var image = go.AddComponent<Image>();
            image.color = new Color(0.25f, 0.22f, 0.18f, 0.55f);
            var button = go.AddComponent<Button>();
            button.onClick.AddListener(() => click());
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;

            var textGo = new GameObject("Text");
            textGo.transform.SetParent(go.transform, false);
            var text = textGo.AddComponent<Text>();
            text.font = _font;
            text.text = label;
            text.alignment = TextAnchor.LowerCenter;
            text.color = Color.white;
            text.fontSize = 16;
            text.raycastTarget = false;
            var textRect = text.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(4, 2);
            textRect.offsetMax = new Vector2(-4, -2);
            return go;
        }

        private static string FormatItem(Item item)
        {
            switch (item)
            {
                case null:
                    return "пусто";
                case Ingredient ingredient:
                    return $"{ingredient.Name} ({ingredient.Effect1} / {ingredient.Effect2})";
                case Potion potion:
                    return $"{potion.Name} [{potion.FormatEffects()}]";
                default:
                    return item.Name;
            }
        }

        private static string FormatCauldron(Cauldron cauldron)
        {
            if (cauldron.Contents.Count == 0)
                return "Котёл: пуст";

            var parts = new string[cauldron.Contents.Count];
            for (var i = 0; i < cauldron.Contents.Count; i++)
            {
                var processed = cauldron.Contents[i];
                parts[i] = $"{i + 1}. {processed.Ingredient.Name}+{processed.Effect}";
            }

            return "Котёл:\n" + string.Join("\n", parts);
        }
    }
}