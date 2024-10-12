using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Bot : MonoBehaviour
{
    [SerializeField] private List<Int32> _toCheckIndices;
    [SerializeField] private Int32 _lastIndex;
    [SerializeField] private Boolean _isChecking;
    private Boolean isFirstStartUp = true;
    
    private void Start()
    {        
        GameProcess.Instance._restartButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            StartCoroutine(PlaceShips());
            _toCheckIndices = new List<int>();
            _lastIndex = -1;
        });
        StartCoroutine(PlaceShips());
        StartCoroutine(AddListenerToButtons());
    }

    private IEnumerator AddListenerToButtons()
    {
        yield return new WaitUntil(() => FieldController.Instance._enemyButtons.Count != 0);
        
        
        
        foreach (var button in FieldController.Instance._enemyButtons)
        {
            button.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(MakeStep()));
        }
    }

    private IEnumerator PlaceShips()
    {
        yield return new WaitUntil(() => GameProcess.Instance._isPlayerShipsPlaced);
        
        FieldController.Instance.Random(false);
        GameProcess.Instance.PlaceShips();
    }

    private IEnumerator MakeStep()
    {
        while (GameProcess.Instance._turn && !GameProcess.Instance._gameEnded)
        {
            if (_toCheckIndices.Count != 0)
            {
                foreach (var currentIndex in _toCheckIndices)
                {
                    
                    if (currentIndex < 0 || currentIndex >= FieldController.Instance._playerField.Count)
                    {
                        break;
                    }
                    
                    var button = FieldController.Instance._playerButtons[currentIndex];

                    if (!button.GetComponent<ButtonController>()._wasClicked) yield return new WaitForSeconds(1);
                    
                    button.GetComponent<ButtonController>().OnClick(true, 0, true, true);
                    
                    if (FieldController.Instance._playerField[currentIndex] == 2)
                    {
                        _isChecking = false;
                        _toCheckIndices.Clear();
                        
                        var offset = currentIndex - _lastIndex;
                        
                        var toCheckIndex = currentIndex + offset;
                        
                        if (toCheckIndex >= 0 && toCheckIndex < FieldController.Instance._playerField.Count)
                        {
                            _toCheckIndices.Add(toCheckIndex);
                        }
                        else if (currentIndex / 10 != toCheckIndex / 10)
                        {
                            offset = _lastIndex - currentIndex;
                            var iterationIndex = currentIndex;
        
                            while (FieldController.Instance._playerField[iterationIndex] == 2)
                            {
                                iterationIndex += offset;
                            }

                            _toCheckIndices.Add(iterationIndex);
                        }
                        else
                        {
                            offset = _lastIndex - currentIndex;
                            var iterationIndex = currentIndex;
        
                            while (FieldController.Instance._playerField[iterationIndex] == 2)
                            {
                                iterationIndex += offset;
                            }
                            
                            _toCheckIndices.Add(iterationIndex);
                            _lastIndex = currentIndex + offset;
                            break;
                        }
                        
                        
                        _lastIndex = currentIndex;
                        break;
                    }

                    if (!_isChecking)
                    {
                        var offset = _lastIndex - currentIndex;
                        var iterationIndex = currentIndex + offset;
                        var aliveCount = 0;

                        while (iterationIndex >= 0 && iterationIndex < FieldController.Instance._playerField.Count &&
                               FieldController.Instance._playerField[iterationIndex] != 0)
                        {
                            aliveCount += FieldController.Instance._playerField[iterationIndex] == 1 ? 1 : 0;
                            
                            if (aliveCount != 0)
                            {
                                _toCheckIndices.Clear();
                                _lastIndex = iterationIndex + offset;
                                _toCheckIndices.Add(iterationIndex);
                                break;
                            }
                            iterationIndex += offset;
                            
                            if (iterationIndex < 0 || iterationIndex >= FieldController.Instance._playerField.Count)
                            {
                                break;
                            } 
                        }
                        
                        _isChecking = true;
                    }

                    _toCheckIndices.Remove(currentIndex);
                    break;                    
                } 
            }
            else
            {
                var randomIndex = Random.Range(0, FieldController.Instance._playerField.Count);
                var button = FieldController.Instance._playerButtons[randomIndex];

                if (button.GetComponent<ButtonController>()._wasClicked) continue;

                yield return new WaitForSeconds(1);
                button.GetComponent<ButtonController>().OnClick(true, 0, true, true);

                if (FieldController.Instance._playerField[randomIndex] == 2)
                {
                    _isChecking = true;
                    _lastIndex = randomIndex;
                        
                    Int32 row = randomIndex / 10;

                    var offsets = new[] { -10, 1, 10, -1 };

                    foreach (var offset in offsets)
                    {
                        Int32 neighborIndex = randomIndex + offset;
                        Int32 neighborRow = neighborIndex / 10;

                        Boolean isInsideField =
                            neighborIndex >= 0 && neighborIndex < FieldController.Instance._playerField.Count;
                        Boolean isSameRow = (offset != 1 && offset != -1) || row == neighborRow;

                        if (isInsideField && isSameRow)
                        {
                            _toCheckIndices.Add(neighborIndex);
                        }
                    }
                }
            }
        }
    }
}