using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManagerSelection : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject playerCardPrefab;
    [SerializeField] private GameObject playerLobbyPrefab;

    [Header("Buttons")]
    [SerializeField] private Button _addPlayer;
    [SerializeField] private Button _removePlayer;
    [SerializeField] private Button _startGame;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _selectionQuantity;

    [Header("Panels")]
    [SerializeField] private Transform _playerSelectionPanel;
    [SerializeField] private Transform _playerCardsPanel;
    [SerializeField] private Transform _playerAssignPanel;

    [Header("Misc")]
    [SerializeField] private Transform _playerCardView;
    [SerializeField] private float gameObjectViewRange;
    private int playersInGame;

    private void Start()
    {
        // Initialize Canvas
        _playerSelectionPanel.gameObject.SetActive(true);
        _playerCardsPanel.gameObject.SetActive(false);
        _playerAssignPanel.gameObject.SetActive(false);
    }




}
