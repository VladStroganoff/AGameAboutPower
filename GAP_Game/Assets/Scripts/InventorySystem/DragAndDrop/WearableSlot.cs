using UnityEngine;
public class WearableSlot : ItemSlot
{
    public BodyPart Type;
    DressController _dressControl;

    protected override void Awake()
    {
        base.Awake();
        _dropArea.DropCondition.Add(new IsWearableCondition(Type));
    }
    
    public override void Populate(float ItemStandardSize)
    {
        base.Populate(ItemStandardSize);
        _dressControl.AddLoadedWear(RuntimeItem);
    }

    public override void PopulateSlot(PlayerSpawned playerSpanwed)
    {
        base.PopulateSlot(playerSpanwed);
        _dressControl = playerSpanwed.player.GetComponent<DressController>();
    }

    public override void OnItemDropped(ItemView draggable)
    {
        if (_currentView != null) 
        {
            _currentView.transform.position = draggable.currentParent.position;
            _currentView.transform.SetParent(draggable.currentParent);
            _currentView.RuntimeItem.Item.Slot = draggable.currentParent.gameObject.name;
        }
        base.OnItemDropped(draggable);
        ItemView oldView = _currentView;
        _currentView = draggable;
        _dressControl.SendSwapRequest(draggable.RuntimeItem, oldView.RuntimeItem);
    }

}
