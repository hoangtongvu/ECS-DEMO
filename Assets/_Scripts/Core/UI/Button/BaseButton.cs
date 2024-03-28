using Core;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseButton : SaiMonoBehaviour
{
    [Header("Base Button")]
    [SerializeField] protected Button button;

    protected virtual void Start() => this.AddOnClickEvent();


    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadButton();
    }

    protected virtual void LoadButton() => this.button = GetComponent<Button>();


    protected virtual void AddOnClickEvent() => this.button.onClick.AddListener(this.OnClick);


    protected abstract void OnClick();



}
