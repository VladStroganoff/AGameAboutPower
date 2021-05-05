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
        Debug.Log($"{gameObject.name} set to {Type}");
        _dropArea.DropCondition.Add(new IsWearableCondition(Type));
    }

    public override void Populate()
    {
        base.Populate();
        _dressControl.AddWear(RuntimeItem);
    }

}
