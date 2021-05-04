using Zenject;
public class WearableSlot : ItemSlot
{
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
        _dropArea.DropCondition.Add(new IsWearableCondition());
    }

    public override void Populate()
    {
        base.Populate();
        _dressControl.AddWear(RuntimeItem);
    }

}
