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

    public override void OnItemDropped(ItemView draggable, bool isLoot)
    {
        base.OnItemDropped(draggable, isLoot);
        draggable.CanDrag = false;
        ItemView oldView = _currentView;
        _currentView = draggable;
        _dressControl.SendSwapRequest(draggable.RuntimeItem, oldView.RuntimeItem);
    }

}
