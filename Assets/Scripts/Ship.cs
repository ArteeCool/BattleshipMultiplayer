using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ship : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] public Boolean _rotated;
    [SerializeField] public Boolean _haveInvalidPlacement;
    [SerializeField] public Boolean _isEnemyShip;
    [SerializeField] public Boolean _wasPlaced;
    
    [SerializeField] private Single _distanceToStick;

    [SerializeField] public Vector2 _offset;

    [SerializeField] public Int16 _deckCount;
        
    private Vector2 _lastPosition;
    private Vector2 _dragOffset;
    
    private Camera _camera;

    private RectTransform _rTransform;
    
    private const Single DragMultiplier = 1f;
    
    private void Start()
    {
        _camera = Camera.main;
        _rTransform = GetComponent<RectTransform>();
        _lastPosition = _rTransform.anchoredPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (eventData.dragging) return;
        
        if (_rotated)
        {        
            _rotated = false;
            _rTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            _rotated = true;
            _rTransform.rotation = Quaternion.Euler(0f, 0f, 90f);
        }
        
        FieldController.Instance.FillFields(!_isEnemyShip);
        FieldController.Instance.CheckCollisions(!_isEnemyShip);
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        
        _dragOffset = (Vector2)_rTransform.position - mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            var position = Vector2.MoveTowards(_rTransform.position, mousePosition + _dragOffset, DragMultiplier);
            _rTransform.position = new Vector3(position.x, position.y, 0);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        TryPlace();
    }

    public Boolean TryPlace()
    {
        Boolean wasPlaced = false;
        foreach (var button in FieldController.Instance._playerButtons)
        {
            if (Vector2.Distance(_rTransform.anchoredPosition , button.GetComponent<RectTransform>().anchoredPosition + _offset) <= _distanceToStick)
            {
                _rTransform.anchoredPosition = button.GetComponent<RectTransform>().anchoredPosition + _offset;
                _lastPosition = _rTransform.anchoredPosition;
                _wasPlaced = true;
                wasPlaced = true;
                break;
            }
        }

        if (!wasPlaced)
            _rTransform.anchoredPosition = _lastPosition;
        
        FieldController.Instance.FillFields(!_isEnemyShip);
        FieldController.Instance.CheckCollisions(!_isEnemyShip);
        return true;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
    }
}