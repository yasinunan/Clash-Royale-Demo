using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UOP1.Helper
{
    public class UIDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private UnitData data;
        [SerializeField] private RectTransform UIDragElement;
        [SerializeField] RectTransform Canvas;

        private Vector2 mOriginalLocalPointerPosition;
        private Vector3 mOriginalPanelLocalPosition;
        private Vector2 mOriginalPosition;

        //===============================================================================================
        void Start()
        {
            mOriginalPosition = UIDragElement.localPosition;
        }

        //===============================================================================================

        public void OnBeginDrag(PointerEventData data)
        {
            mOriginalPanelLocalPosition = UIDragElement.localPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
              Canvas,
              data.position,
              data.pressEventCamera,
              out mOriginalLocalPointerPosition);
        }

        //===============================================================================================

        public void OnDrag(PointerEventData data)
        {
            Vector2 localPointerPosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
              Canvas,
              data.position,
              data.pressEventCamera,
              out localPointerPosition))
            {
                Vector3 offsetToOriginal =
                  localPointerPosition -
                  mOriginalLocalPointerPosition;
                UIDragElement.localPosition =
                  mOriginalPanelLocalPosition +
                  offsetToOriginal;
            }
        }

        //===============================================================================================

        public void OnEndDrag(PointerEventData eventData)
        {
            StartCoroutine(
              Coroutine_MoveUIElement(
                UIDragElement,
                mOriginalPosition,
                0.5f));

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(
              Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                Vector3 worldPoint = hit.point;

                //Debug.Log(worldPoint);
                CreateObject(worldPoint);
            }
        }

        //===============================================================================================

        public IEnumerator Coroutine_MoveUIElement(RectTransform r, Vector2 targetPosition, float duration = 0.1f)
        {
            float elapsedTime = 0;
            Vector2 startingPos = r.localPosition;
            while (elapsedTime < duration)
            {
                r.localPosition =
                  Vector2.Lerp(
                    startingPos,
                    targetPosition,
                    (elapsedTime / duration));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            r.localPosition = targetPosition;
        }


        //===============================================================================================

        public void CreateObject(Vector3 position)
        {
            switch (data.characterType)
            {
                case UnitData.CharacterType.MELEE:
                    if (ManaBarHelper.Instance.CanDragAndDrop(data.manaCost))
                    {
                        UpdateManaBar(data.manaCost);
                        PlayerUnitSpawner.Instance.SpawnGroundUnit(position);
                    }
                    break;
                case UnitData.CharacterType.RANGE:
                    if (ManaBarHelper.Instance.CanDragAndDrop(data.manaCost))
                    {
                        UpdateManaBar(data.manaCost);
                        PlayerUnitSpawner.Instance.SpawnLongRangeUnit(position);
                    }
                    break;
                case UnitData.CharacterType.AERIAL:
                    if (ManaBarHelper.Instance.CanDragAndDrop(data.manaCost))
                    {
                        UpdateManaBar(data.manaCost);
                        PlayerUnitSpawner.Instance.SpawnAerialUnit(position);
                    }
                    break;
                case UnitData.CharacterType.SUPPORT:
                    if (ManaBarHelper.Instance.CanDragAndDrop(data.manaCost))
                    {
                        UpdateManaBar(data.manaCost);
                        PlayerUnitSpawner.Instance.SpawnSupportUnit(position);
                    }

                    break;

            }

        }

        //===============================================================================================

        private void UpdateManaBar(float manaAmount)
        {
            ManaBarHelper.Instance.ManaChanged(manaAmount);
        }
    }
}
