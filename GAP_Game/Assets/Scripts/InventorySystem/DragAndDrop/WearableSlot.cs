using UnityEngine;
using Zenject;
public class WearableSlot : ItemSlot
{
    public BodyPart Type;
    DressController _dressControl;

    [Inject]
    public void Inject(SignalBus bus)
    {
        bus.Subscribe<PlayerDresserSpawned>(GetDresser);
    }

    public void GetDresser(PlayerDresserSpawned dresser)
    {
        _dressControl = dresser.DressController;
    }

    protected override void Awake()
    {
        base.Awake();
        _dropArea.DropCondition.Add(new IsWearableCondition(Type));
    }
    
    public override void Populate(float ItemStandardSize)
    {
        base.Populate(ItemStandardSize);
        _dressControl.AddWear(RuntimeItem);
    }

    public override void OnItemDropped(ItemView draggable)
    {
        if (_currentView != null) 
        {
            _currentView.transform.position = draggable.currentParent.position;
            _currentView.transform.SetParent(draggable.currentParent);
        }
        base.OnItemDropped(draggable);
        _currentView = draggable;
        _dressControl.SwapWear(draggable.RuntimeItem);
    }

}
